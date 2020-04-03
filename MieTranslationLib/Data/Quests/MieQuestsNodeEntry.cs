namespace MieTranslationLib.Data.Quests
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// クエストノードエントリー
    /// </summary>
    public class MieQuestsNodeEntry
    {
        public MieQuestsNodeEntry(
            NNodeType questNode,
            int nodeID,
            bool isRootNode)
        {
            this.NodeType = questNode;
            this.NodeID = nodeID;
            this.IsRootNode = isRootNode;
        }

        /// <summary>
        /// クエストノードタイプ
        /// </summary>
        public enum NNodeType
        {
            Unknown,
            QuestNode,
            GlobalQuestNode,
            ObjectiveNode,
            EndStateNode,
            BranchCompleteNode,
        }

        /// <summary>
        /// クエストノード属性
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

        public bool IsRootNode { get; }

        public int Depth { get; private set; } = 0;

        public NNodeAttribute Attribute { get; private set; } = NNodeAttribute.None;

        /// <summary>
        /// クエストノードの辞書(ツリー形式)。
        /// キーは NodeID。
        /// クエストツリーに存在せず、言語情報に存在する項目はルートノードとして登録する(ゾンビノード)
        /// </summary>
        private IDictionary<int, MieQuestsNodeEntry> Nodes { get; } =
            new SortedDictionary<int, MieQuestsNodeEntry>();

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
        public static MieQuestsNodeEntry CreateStopNode(int nodeID, int fromID)
        {
            MieQuestsNodeEntry node = new MieQuestsNodeEntry(
                NNodeType.Unknown,
                nodeID,
                false);

            node.Attribute = NNodeAttribute.StopNode;
            node.AddFromNodeID(fromID);

            return node;
        }

        /// <summary>
        /// クエストリンクエントリーを追加する。追加に成功した場合は true を返す。
        /// </summary>
        /// <param name="nodeEntry">クエストリンクエントリー</param>
        /// <returns>追加の成否</returns>
        public bool AddNodeEntry(MieQuestsNodeEntry nodeEntry)
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
        public MieQuestsNodeEntry GetNode(int nodeID)
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
        /// ソート済みフラットノードをリストに格納する。
        /// </summary>
        /// <param name="list">リスト</param>
        /// <param name="viewStopNode">Stop node の格納有無</param>
        public void GetSortedFlatNodes(IList<MieQuestsNodeEntry> list, bool viewStopNode)
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
                if (this.Attribute.HasFlag(MieQuestsNodeEntry.NNodeAttribute.StopNode))
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
                if (this.Attribute.HasFlag(MieQuestsNodeEntry.NNodeAttribute.StopNode))
                {
                    attributeText = "StopNode,";
                }

                if (this.Attribute.HasFlag(MieQuestsNodeEntry.NNodeAttribute.MissingNode))
                {
                    attributeText += "MissingNode,";
                }

                if (this.Attribute.HasFlag(MieQuestsNodeEntry.NNodeAttribute.None))
                {
                    attributeText += "None,";
                }

                attributeText = attributeText.TrimEnd(',');
                buff.Append($" Attribute({attributeText})");
            }

            /**********************************************************************************/

            buff.AppendLine();

            return buff.ToString();
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"NodeID({this.NodeID}) Childrens({this.Nodes.Count}) Depth({this.Depth}) FromNodesID({this.FromNodesID})");

            return buff.ToString();
        }
    }
}
