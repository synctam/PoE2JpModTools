namespace MieOELib.Data.Quests
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MieOELib.FileUtils;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Quests;
    using MieTranslationLib.MieUtils;
    using Newtonsoft.Json;
    using NLog;

    public class MieQuestsDesignDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 指定したフォルダー内のクエストファイルを読み込みクエスト情報を返す。
        /// </summary>
        /// <param name="folderPath">フォルダーのパス</param>
        /// <param name="fileList">FileList</param>
        /// <returns>クエスト情報</returns>
        public static MieQuestsNodeInfo LoadFromFolder(string folderPath, MieFileList fileList)
        {
            MieQuestsNodeInfo mieQuestsInfo = null;

            if (!Directory.Exists(folderPath))
            {
                var msg = $"Error: Folder not exists. Folder({folderPath})";
                logger.Error(msg);
                throw new Exception(msg);
            }

            string[] files = Directory.GetFiles(folderPath, "*.questbundle", SearchOption.AllDirectories);
            files
                .Where(file => Path.GetExtension(file).ToLower() == ".questbundle")
                .ToList()
                .ForEach(x =>
                {
                    var questsNodeInfo = LoadFromJson(x, fileList);
                    mieQuestsInfo = questsNodeInfo;
                });

            return mieQuestsInfo;
        }

        /// <summary>
        /// クエスト付加情報ファイルを読み込み、クエストノード情報を返す。
        /// クエスト付加情報は１ファイルの中に複数ファイルの情報が格納されている。
        /// </summary>
        /// <param name="path">クエスト付加情報ファイルのパス</param>
        /// <param name="fileList">FileList</param>
        /// <returns>クエストノード情報</returns>
        public static MieQuestsNodeInfo LoadFromJson(string path, MieFileList fileList)
        {
            MieQuestsNodeInfo mieQuestsNodeInfo = new MieQuestsNodeInfo();
            string jtext = LoadJson(path);
            jtext = jtext.Replace("$type", "MieQuestsNodeDataTypeTag");
            var oeQuests = JsonConvert.DeserializeObject<MieOEQuests>(jtext);

            foreach (var questFile in oeQuests.Quests)
            {
                var questTypeNumber = questFile.QuestType;

                //// QuestsFileの処理
                MieQuestsNodeFile mieQuestsNodeFile = null;

                var fileID = questFile.Filename.Replace(".quest", string.Empty);
                //// FileIDを統一形式に変換する。
                fileID = MieFileUtils.ConvertFileIDToCommon(fileID);
                fileID = MieStringUtils.NormalizedFileID(fileID);

                var fileCode = fileList.GetHashByFileID(fileID);
                if (fileCode == 0)
                {
                    //// 言語情報から生成されたFileListに含まれないものは無視する。
                    continue;
                }
                else
                {
                    mieQuestsNodeFile = new MieQuestsNodeFile(fileCode);
                    mieQuestsNodeInfo.AddFile(mieQuestsNodeFile);
                }

                //// QuestEntryの処理
                foreach (var node in questFile.Nodes)
                {
                    var nodeType = GetQuestNodeDataTypeTag(node.MieQuestsNodeDataTypeTag);
                    bool isRootNode = node.NodeID == 0 ? true : false;

                    var questsNode = new MieQuestsNodeEntry(
                        nodeType,
                        node.NodeID,
                        isRootNode);
                    mieQuestsNodeFile.AddFlatNodeEntry(questsNode);

                    //// EndStatusNodeを追加する。
                    AddEndStatusNode(mieQuestsNodeFile, node);

                    //// リンク情報作成
                    foreach (var link in node.Links)
                    {
                        MieQuestsLink mieQuestsLinks = new MieQuestsLink(link.FromNodeID, link.ToNodeID);
                        mieQuestsNodeFile.AddLinkEntry(mieQuestsLinks);
                    }
                }

                mieQuestsNodeFile.BuildLink();
                mieQuestsNodeFile.UpdateDepth();
            }

            return mieQuestsNodeInfo;
        }

        /// <summary>
        /// EndStatusNodeIDを追加する。
        /// </summary>
        /// <param name="mieQuestsNodeFile">QuestsNodeFile</param>
        /// <param name="node">Node</param>
        private static void AddEndStatusNode(MieQuestsNodeFile mieQuestsNodeFile, Node node)
        {
            if (node.EndStates == null)
            {
                return;
            }

            foreach (var endStatsuNode in node.EndStates)
            {
                var questsNode = new MieQuestsNodeEntry(
                            MieQuestsNodeEntry.NNodeType.EndStateNode,
                            endStatsuNode.DescriptionID,
                            false);
                mieQuestsNodeFile.AddFlatNodeEntry(questsNode);
            }
        }

        /// <summary>
        /// pathで指定したJSONファイルを読み込みテキスト形式JSONを返す。
        /// </summary>
        /// <param name="path">JSONファイルのパス</param>
        /// <returns>テキスト形式JSON</returns>
        private static string LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                var msg = $"JSON faile not found.";
                throw new FileNotFoundException(msg, path);
            }

            string jtext = File.ReadAllText(path, Encoding.UTF8);

            return jtext;
        }

        /// <summary>
        /// DataTypeTagからDataTypeを返す。
        /// </summary>
        /// <param name="text">DataTypeTag</param>
        /// <returns>DataType</returns>
        private static MieQuestsNodeEntry.NNodeType GetQuestNodeDataTypeTag(string text)
        {
            var tag = text
                .Replace("OEIFormats.FlowCharts.Quests.", string.Empty)
                .Replace(", OEIFormats", string.Empty);

            switch (tag.ToLower())
            {
                case "branchcompletenode":
                    return MieQuestsNodeEntry.NNodeType.BranchCompleteNode;
                case "endstatenode":
                    return MieQuestsNodeEntry.NNodeType.EndStateNode;
                case "globalquestnode":
                    return MieQuestsNodeEntry.NNodeType.GlobalQuestNode;
                case "objectivenode":
                    return MieQuestsNodeEntry.NNodeType.ObjectiveNode;
                case "questnode":
                    return MieQuestsNodeEntry.NNodeType.QuestNode;
                case "unknown":
                    return MieQuestsNodeEntry.NNodeType.Unknown;
                default:
                    var msg = $"Unknown FlowCharts data type({tag}).";
                    throw new InvalidEnumArgumentException(msg);
            }
        }
    }
}
