namespace MieTranslationLib.Data.Quests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MieTranslationLib.Product;
    using NLog;

    public class MieQuestsNodeFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileCode">FileID</param>
        public MieQuestsNodeFile(long fileCode)
        {
            this.FileCode = fileCode;
        }

        /// <summary>
        /// FileCode
        /// </summary>
        public long FileCode { get; } = 0;

        /// <summary>
        /// 言語区分
        /// </summary>
        public MieProduct.NLanguageType LanguageType { get; } = MieProduct.NLanguageType.Quests;

        /// <summary>
        /// クエストノードの辞書(フラット形式)。
        /// このファイルに存在する全てのノードを保有する辞書。
        /// 主にノード検索などの用途に使用する。
        /// </summary>
        public IDictionary<int, MieQuestsNodeEntry> FlatNodes { get; } =
            new Dictionary<int, MieQuestsNodeEntry>();

        /// <summary>
        /// ノードのリンクエントリーのリスト。
        /// </summary>
        public IList<MieQuestsLink> Links { get; } = new List<MieQuestsLink>();

        /// <summary>
        /// クエストノード(ツリー形式)。
        /// </summary>
        private MieQuestsNodeEntry NodeEntry { get; set; } = null;

        /// <summary>
        /// Nodeのリンクエントリーを追加する。
        /// </summary>
        /// <param name="link">リンクエントリー</param>
        public void AddLinkEntry(MieQuestsLink link)
        {
            this.Links.Add(link);
        }

        /// <summary>
        /// クエストエントリーを追加する
        /// </summary>
        /// <param name="nodeEntry">クエストエントリー</param>
        /// <returns>追加に成功した場合は true</returns>
        public bool AddFlatNodeEntry(MieQuestsNodeEntry nodeEntry)
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
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// クエストノードファイルのマージ
        /// </summary>
        /// <param name="questsNodeFile">会話ノードファイル</param>
        public void MargeFile(MieQuestsNodeFile questsNodeFile)
        {
            //// クエストノードの追加
            foreach (var newNode in questsNodeFile.FlatNodes.Values)
            {
                this.AddFlatNodeEntry(newNode);
            }
        }

        /// <summary>
        /// 指定したNodeIDのノードを返す。
        /// </summary>
        /// <param name="nodeID">NodeID</param>
        /// <returns>ノード</returns>
        public MieQuestsNodeEntry GetNode(int nodeID)
        {
            if (this.FlatNodes.ContainsKey(nodeID))
            {
                return this.FlatNodes[nodeID];
            }
            else
            {
                //// 言語ファイルにあり、クエストツリーに無い。
                var msg = $"NodeID not found. NodeID({nodeID})";
                logger.Error(msg);
                return null;
                throw new ArgumentOutOfRangeException("NodeID", msg);
            }
        }

        /// <summary>
        /// ノードのリンク情報を構築する。
        /// </summary>
        public void BuildLink()
        {
            HashSet<int> flatTo = new HashSet<int>();
            foreach (var link in this.Links)
            {
                var fromNode = this.GetNode(link.FromeNode);

                //// ルートノードを登録
                if (fromNode.IsRootNode)
                {
                    if (this.NodeEntry.GetNode(fromNode.NodeID) != null)
                    {
                        //// すでに登録済み：無視。
                    }
                    else
                    {
                        if (this.NodeEntry == null)
                        {
                            this.NodeEntry = fromNode;
                        }
                    }
                }

                //// 子ノードを登録
                var toNode = this.GetNode(link.ToNode);
                //// リンクの無限循環を防止するため、すでにリンク先が登録されていた場合は、リンクを追加しない。
                if (flatTo.Add(link.ToNode))
                {
                    //// 未登録
                    fromNode.AddNodeEntry(toNode);
                }
                else
                {
                    //// すでに登録済みの場合は番兵ノードを追加する。
                    var stopNode = MieQuestsNodeEntry.CreateStopNode(link.ToNode, link.FromeNode);
                    stopNode.AddFromNodeID(link.FromeNode);

                    fromNode.AddNodeEntry(stopNode);
                }

                toNode.AddFromNodeID(link.FromeNode);
            }
        }

        /// <summary>
        /// ソート済みフラットノードをリストを返す。
        /// </summary>
        /// <param name="viewStopNode">Stop node の格納有無</param>
        /// <returns>ソート済みフラットノードのリスト</returns>
        public IList<MieQuestsNodeEntry> GetSortedFlatNodes(bool viewStopNode)
        {
            IList<MieQuestsNodeEntry> list = new List<MieQuestsNodeEntry>();

            list.Add(this.NodeEntry);
            //// リンクを辿りlistにノード登録する。
            this.NodeEntry.GetSortedFlatNodes(list, viewStopNode);

            //// リンク切れのノードを登録する。
            foreach (var linklessNode in this.FlatNodes.Values)
            {
                if (list.Contains(linklessNode))
                {
                    //// 登録済み
                }
                else
                {
                    //// 未登録
                    list.Add(linklessNode);
                }
            }

            return list;
        }

        public string ToFlatNodeString()
        {
            StringBuilder buff = new StringBuilder();

            this.FlatNodes.Values
                .OrderBy(x => x.NodeID)
                .ToList()
                .ForEach(x =>
                {
                    x.ToString();
                });

            return buff.ToString();
        }

        public void UpdateDepth()
        {
            int depth = 0;
            this.NodeEntry.UpdateDepth(depth);
            this.NodeEntry.UpdateDepthRecursive(depth + 1);
        }

        /// <summary>
        /// リンク情報をテキスト化して返す。
        /// </summary>
        /// <param name="viewStopNode">Stop nodeを出力する。</param>
        /// <returns>テキスト化したリンク情報</returns>
        public string ToLinkString(bool viewStopNode)
        {
            StringBuilder buff = new StringBuilder();

            int tabCount = 0;
            buff.AppendLine($"{this.FileCode}");
            buff.Append(this.NodeEntry.ToLinkString(viewStopNode, tabCount));
            buff.Append(this.NodeEntry.ToLinkStringRecursive(viewStopNode, tabCount + 1));

            return buff.ToString();
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
