namespace MieTranslationLib.Data.Conversations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Product;
    using NLog;

    public class MieConversationNodeFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileCode">FileCode</param>
        /// <param name="languageType">LanguageType</param>
        public MieConversationNodeFile(long fileCode)
        {
            this.FileCode = fileCode;
        }

        /// <summary>
        /// FileID
        /// </summary>
        public long FileCode { get; private set; } = 0;

        /// <summary>
        /// 言語区分
        /// </summary>
        public MieProduct.NLanguageType LanguageType { get; } = MieProduct.NLanguageType.Conversations;

        /// <summary>
        /// キャラクターマップ
        /// </summary>
        public MieCharacterMapFile CharacterMapFile { get; } = new MieCharacterMapFile();

        /// <summary>
        /// ノードのリンクエントリーのリスト。
        /// </summary>
        public IList<MieConversationLink> Links { get; } = new List<MieConversationLink>();

        /// <summary>
        /// 会話ノードの辞書(フラット形式)。
        /// このファイルに存在する全てのノードを保有する辞書。
        /// 主にノード検索などの用途に使用する。
        /// キーは NodeID。
        /// </summary>
        public IDictionary<int, MieConversationNodeEntry> FlatNodes { get; } =
            new Dictionary<int, MieConversationNodeEntry>();

        /// <summary>
        /// 会話ノード(ツリー形式)。
        /// </summary>
        public MieConversationNodeEntry NodeEntry { get; private set; } = null;

        /// <summary>
        /// キャラクターマップにエントリーを追加する。
        /// </summary>
        /// <param name="entry">キャラクターマップエントリー</param>
        public void AddCharacterMapEntry(MieCharacterMapEntry entry)
        {
            this.CharacterMapFile.AddEntry(entry);
        }

        /// <summary>
        /// Nodeのリンクエントリーを追加する。
        /// </summary>
        /// <param name="link">リンクエントリー</param>
        public void AddLinkEntry(MieConversationLink link)
        {
            this.Links.Add(link);
        }

        /// <summary>
        /// 会話エントリーを追加する
        /// </summary>
        /// <param name="nodeEntry">会話リンクエントリー</param>
        /// <returns>追加に成功した場合は true</returns>
        public bool AddFlatNodeEntry(MieConversationNodeEntry nodeEntry)
        {
            if (this.FlatNodes.ContainsKey(nodeEntry.NodeID))
            {
                //// このノードはすでに登録済み。
                return false;
            }
            else
            {
                //// FlatNodeに追加する。
                this.FlatNodes.Add(nodeEntry.NodeID, nodeEntry);
                //// ルートノードを抽出
                if (nodeEntry.IsRootNode)
                {
                    if (this.NodeEntry == null)
                    {
                        this.NodeEntry = nodeEntry;
                    }
                    else
                    {
                        var msg = $"Duplicate root node({nodeEntry.NodeID}).";
                        logger.Error(msg);
                        throw new Exception(msg);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 会話ノードファイルのマージ
        /// </summary>
        /// <param name="convNodeFile">会話ノードファイル</param>
        public void MargeFile(MieConversationNodeFile convNodeFile)
        {
            //// 会話ノードの追加
            foreach (var newNode in convNodeFile.FlatNodes.Values)
            {
                this.AddFlatNodeEntry(newNode);
            }
        }

        /// <summary>
        /// 指定したNodeIDのノードを返す。
        /// </summary>
        /// <param name="nodeID">NodeID</param>
        /// <returns>ノード</returns>
        public MieConversationNodeEntry GetNode(int nodeID)
        {
            if (this.FlatNodes.ContainsKey(nodeID))
            {
                return this.FlatNodes[nodeID];
            }
            else
            {
                //// 言語ファイルにあり、会話ツリーに無い。
                var msg = $"NodeID not found. NodeID({nodeID})";
                logger.Error(msg);
                return null;
                throw new ArgumentOutOfRangeException("NodeID", msg);
            }
        }

        /// <summary>
        /// ノードのリンク情報を構築する。
        /// 1.各ノードに FronID と ToID を設定する。
        /// 2.登録済みリンクノードは無限循環防止のためStopNodeに差し替える。
        /// </summary>
        public void BuildLink()
        {
            HashSet<int> flatTo = new HashSet<int>();
            this.Links
                .OrderBy(link => link.FromeNode)
                .ThenBy(link => link.ToNode)
                .ToList()
                .ForEach(link =>
                {
                    var fromNode = this.GetNode(link.FromeNode);

                    var toNode = this.GetNode(link.ToNode);
                    //// 親ノードに子ノードを登録
                    if (flatTo.Add(link.ToNode))
                    {
                        //// 未登録の子ノードは追加する。
                        fromNode.AddNodeEntry(toNode);
                    }
                    else
                    {
                        //// 登録済みの子ノードは追加せず、StopNode(番兵ノード)に差し替える。
                        //// （リンクの無限循環を防止するため）
                        var stopNode = MieConversationNodeEntry.CreateStopNode(link.ToNode, link.FromeNode);
                        stopNode.AddFromNodeID(link.FromeNode);

                        fromNode.AddNodeEntry(stopNode);
                    }

                    //// 子ノードに親ノードを設定する。
                    toNode.AddFromNodeID(link.FromeNode);
                });
        }

        /// <summary>
        /// ソート済みフラットノードをリストを返す。
        /// </summary>
        /// <param name="viewStopNode">Stop node の格納有無</param>
        /// <returns>ソート済みフラットノードのリスト</returns>
        public IList<MieConversationNodeEntry> GetSortedFlatNodes(bool viewStopNode)
        {
            IList<MieConversationNodeEntry> list = new List<MieConversationNodeEntry>();

            list.Add(this.NodeEntry);
            this.NodeEntry.GetSortedFlatNodes(list, viewStopNode);

            return list;
        }

        /// <summary>
        /// 会話Nodeのリンク情報をテキスト化する。
        /// </summary>
        /// <param name="viewStopNode">StopNodeの表示有無</param>
        /// <returns>テキスト化した会話Nodeリンク情報</returns>
        public string ToLinkString(bool viewStopNode)
        {
            StringBuilder buff = new StringBuilder();

            int tabCount = 0;
            buff.AppendLine($"{this.FileCode}");
            buff.Append(this.NodeEntry.ToLinkString(viewStopNode, tabCount));
            buff.Append(this.NodeEntry.ToLinkStringRecursive(viewStopNode, tabCount + 1));

            return buff.ToString();
        }

        public void UpdateDepth()
        {
            int depth = 0;
            this.NodeEntry.UpdateDepth(depth);
            this.NodeEntry.UpdateDepthRecursive(depth + 1);
        }

        /// <summary>
        /// リンクの From/To 情報を返す。
        /// </summary>
        /// <returns>リンクの From/To 情報</returns>
        public string ToLinksFromToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"{this.FileCode}");
            //// FromNode>ToNode順にソート
            var sortedLinks = this.Links
                .OrderBy(x => x.FromeNode)
                .ThenBy(x => x.ToNode)
                .ToList();
            foreach (var link in sortedLinks)
            {
                buff.AppendLine($"\tFrom({link.FromeNode}) To({link.ToNode})");
            }

            return buff.ToString();
        }
    }
}
