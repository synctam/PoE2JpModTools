namespace MieTranslationLib.Data.Conversations
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 会話ノードエントリー
    /// </summary>
    public class MieConversationNodeEntry
    {
        public MieConversationNodeEntry(
            NNodeType nodeType,
            int nodeID,
            Guid speakerGuid,
            Guid listenerGuid,
            bool isRootNode,
            bool isQuestionNode)
        {
            this.NodeType = nodeType;
            this.NodeID = nodeID;
            this.SpeakerGuid = speakerGuid;
            this.ListenerGuid = listenerGuid;
            this.IsRootNode = isRootNode;
            this.IsQuestionNode = isQuestionNode;
        }

        /// <summary>
        /// 会話ノードタイプ
        /// </summary>
        public enum NNodeType
        {
            Unknown,
            TalkNode,
            BankNode,
            ScriptNode,
            PlayerResponseNode,
            TriggerConversationNode,
        }

        /// <summary>
        /// 会話ノード属性
        /// </summary>
        public enum NNodeAttribute
        {
            None = 1 << 0,

            /// <summary>
            /// 付加情報にあり、言語情報にないノード。
            /// </summary>
            MissingNode = 1 << 1,

            /// <summary>
            /// 循環リンク防止のための番兵
            /// </summary>
            StopNode = 1 << 2,
        }

        public NNodeType NodeType { get; } = NNodeType.Unknown;

        public int NodeID { get; }

        public Guid SpeakerGuid { get; }

        public Guid ListenerGuid { get; }

        public bool IsRootNode { get; } = false;

        public bool IsQuestionNode { get; } = false;

        public int Depth { get; private set; } = 0;

        public NNodeAttribute Attribute { get; private set; } = NNodeAttribute.None;

        /// <summary>
        /// 会話ノードの辞書(ツリー形式)。
        /// キーは NodeID。
        /// 会話ノードの内ルートノードのみを保有する。
        /// 会話ツリーに存在せず、言語情報に存在する項目はルートノードとして登録する(ゾンビノード)
        /// </summary>
        public IDictionary<int, MieConversationNodeEntry> Nodes { get; } =
            new SortedDictionary<int, MieConversationNodeEntry>();

        /// <summary>
        /// リンク元 NodeID のリスト
        /// </summary>
        private HashSet<int> FromNodesID { get; } = new HashSet<int>();

        /// <summary>
        /// 番兵用ノードを作成する。
        /// 番兵なのでToNodeは存在させない。
        /// </summary>
        /// <param name="nodeID">番兵用ノードのNodeID</param>
        /// <param name="fromID">番兵用ノードのToID</param>
        /// <returns>番兵用ノード</returns>
        public static MieConversationNodeEntry CreateStopNode(int nodeID, int fromID)
        {
            MieConversationNodeEntry node = new MieConversationNodeEntry(
                NNodeType.Unknown,
                nodeID,
                default(Guid),
                default(Guid),
                false,
                false);

            //// 番兵を設定
            node.Attribute = NNodeAttribute.StopNode;
            node.AddFromNodeID(fromID);

            return node;
        }

        /// <summary>
        /// 会話リンクエントリーを追加する。追加に成功した場合は true を返す。
        /// </summary>
        /// <param name="nodeEntry">会話リンクエントリー</param>
        /// <returns>追加の成否</returns>
        public bool AddNodeEntry(MieConversationNodeEntry nodeEntry)
        {
            if (this.Nodes.ContainsKey(nodeEntry.NodeID))
            {
                //// このノードはすでに登録済みのためスキップ
                return false;
            }
            else
            {
                this.Nodes.Add(nodeEntry.NodeID, nodeEntry);
                return true;
            }
        }

        /// <summary>
        /// 属性を設定する。
        /// </summary>
        /// <param name="attribute">属性値</param>
        public void AddAttribute(NNodeAttribute attribute)
        {
            this.Attribute |= attribute;
        }

        /// <summary>
        /// このエントリーに FromNodeID を追加する。
        /// </summary>
        /// <param name="fromNodeID">FromNodeID</param>
        public void AddFromNodeID(int fromNodeID)
        {
            this.FromNodesID.Add(fromNodeID);
        }

        /// <summary>
        /// 指定したNodeIDのノードを返す。
        /// </summary>
        /// <param name="nodeID">NodeID</param>
        /// <returns>ノード</returns>
        public MieConversationNodeEntry GetNode(int nodeID)
        {
            if (this.Nodes.ContainsKey(nodeID))
            {
                var node = this.Nodes[nodeID];
                return node;
            }
            else
            {
                return null;
            }
        }

        public void UpdateDepthRecursive(int depth)
        {
            foreach (var node in this.Nodes.Values)
            {
                node.UpdateDepth(depth);
                node.UpdateDepthRecursive(depth + 1);
            }
        }

        public void UpdateDepth(int depth)
        {
            this.Depth = depth;
        }

        /// <summary>
        /// 再帰的にリンク情報をテキスト化して返す。
        /// </summary>
        /// <param name="viewStopNode">Stop node の表示有無</param>
        /// <param name="tabCount">タブ数</param>
        /// <returns>テキスト化したリンク情報</returns>
        public string ToLinkStringRecursive(bool viewStopNode, int tabCount)
        {
            StringBuilder buff = new StringBuilder();

            foreach (var node in this.Nodes.Values)
            {
                buff.Append(node.ToLinkString(viewStopNode, tabCount));
                buff.Append(node.ToLinkStringRecursive(viewStopNode, tabCount + 1));
            }

            return buff.ToString();
        }

        /// <summary>
        /// このエントリーのリンク情報をテキスト化して返す。
        /// </summary>
        /// <param name="viewStopNode">Stop node の表示有無</param>
        /// <param name="tabCount">タブ数</param>
        /// <returns>テキスト化したリンク情報</returns>
        public string ToLinkString(bool viewStopNode, int tabCount)
        {
            StringBuilder buff = new StringBuilder();

            if (viewStopNode)
            {
                //// StopNodeは表示する。
                //// 処理継続
            }
            else
            {
                if (this.Attribute.HasFlag(MieConversationNodeEntry.NNodeAttribute.StopNode))
                {
                    //// StopNodeは表示しない。
                    return string.Empty;
                }
                else
                {
                    //// 通常のノードは表示する。
                }
            }

            var tab = new string('\t', tabCount);

            buff.Append($"{tab}");
            buff.Append($" NodeID({this.NodeID})");
            buff.Append($" NodeType({this.NodeType})");

            /**********************************************************************************/
            {
                string attributeText = string.Empty;
                if (this.Attribute.HasFlag(MieConversationNodeEntry.NNodeAttribute.StopNode))
                {
                    attributeText = "StopNode,";
                }

                if (this.Attribute.HasFlag(MieConversationNodeEntry.NNodeAttribute.MissingNode))
                {
                    attributeText += "MissingNode,";
                }

                if (this.Attribute.HasFlag(MieConversationNodeEntry.NNodeAttribute.None))
                {
                    attributeText += "None,";
                }

                attributeText = attributeText.TrimEnd(',');
                buff.Append($" Attribute({attributeText})");
            }

            /**********************************************************************************/

            var errMsg = string.Empty;
            if (tabCount != this.Depth)
            {
                errMsg = "***";
            }

            buff.Append($"TabCount({tabCount}) Depth({this.Depth}) {errMsg}");

            /**********************************************************************************/

            if (this.IsRootNode)
            {
                buff.Append($" RootNode({this.IsRootNode})");
            }

            if (this.IsQuestionNode)
            {
                buff.Append($" QuestionNode({this.IsQuestionNode})");
            }

            buff.AppendLine();

            return buff.ToString();
        }

        /// <summary>
        /// ノードを再帰的に辿り、ツリー形式のデータをフラットな形式に変換しリストに格納する。
        /// </summary>
        /// <param name="list">リスト</param>
        /// <param name="viewStopNode">Stop node の格納有無</param>
        public void GetSortedFlatNodes(IList<MieConversationNodeEntry> list, bool viewStopNode)
        {
            //// Stop node を表示する。
            //// 階層を辿りながら処理する。
            foreach (var node in this.Nodes.Values)
            {
                if (node.Attribute.HasFlag(NNodeAttribute.StopNode))
                {
                    if (viewStopNode)
                    {
                        list.Add(node);
                    }
                }
                else
                {
                    list.Add(node);
                }

                node.GetSortedFlatNodes(list, viewStopNode);
            }
        }
    }
}
