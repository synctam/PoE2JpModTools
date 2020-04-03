namespace MieOELib.Data.Chatter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MieOELib.FileUtils;
    using MieTranslationLib.Data.Chatter;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.MieUtils;
    using Newtonsoft.Json;
    using NLog;

    public class MieChatterDesignDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 指定したフォルダー内のチャッターファイルを読み込みチャッター情報を返す。
        /// </summary>
        /// <param name="folderPath">フォルダーのパス</param>
        /// <param name="fileList">FileList</param>
        /// <returns>チャッター情報</returns>
        public static MieChatterNodeInfo LoadFromFolder(string folderPath, MieFileList fileList)
        {
            MieChatterNodeInfo mieChatterInfo = null;

            if (!Directory.Exists(folderPath))
            {
                var msg = $"Error: Folder not exists. Folder({folderPath})";
                logger.Error(msg);
                throw new Exception(msg);
            }

            string[] files = Directory.GetFiles(folderPath, "*.chatterbundle", SearchOption.AllDirectories);
            files
                .Where(file => Path.GetExtension(file).ToLower() == ".chatterbundle")
                .ToList()
                .ForEach(x =>
                {
                    mieChatterInfo = LoadFromJson(x, fileList);
                });

            return mieChatterInfo;
        }

        /// <summary>
        /// チャッター付加情報ファイルを読み込み、チャッターノード情報を返す。
        /// チャッター付加情報は１ファイルの中に複数ファイルの情報が格納されている。
        /// </summary>
        /// <param name="path">チャッター付加情報ファイルのパス</param>
        /// <param name="fileList">ファイルリストのパス</param>
        /// <returns>チャッターノード情報</returns>
        public static MieChatterNodeInfo LoadFromJson(string path, MieFileList fileList)
        {
            MieChatterNodeInfo mieChatterInfo = new MieChatterNodeInfo();

            string ctext = LoadJson(path);
            ctext = ctext.Replace("$type", "MieChatterNodeDataTypeTag");
            CreateChatterInfo(ctext, mieChatterInfo, fileList);

            return mieChatterInfo;
        }

        private static void CreateChatterInfo(string ctext, MieChatterNodeInfo mieChatterInfo, MieFileList fileList)
        {
            //// デシリアライズ
            var oeChatter = JsonConvert.DeserializeObject<MieOEChatter>(ctext);

            foreach (var chatterFile in oeChatter.ChatterFiles)
            {
                var fileID = chatterFile.Filename.Replace(".chatter", string.Empty);
                //// FileIDを統一形式に変換する。
                fileID = MieFileUtils.ConvertFileIDToCommon(fileID);
                fileID = MieStringUtils.NormalizedFileID(fileID);

                MieChatterNodeFile chatterNodeFile = null;

                var fileCode = fileList.GetHashByFileID(fileID);
                if (fileCode == 0)
                {
                    //// 言語情報から生成されたFileListに含まれないものは無視する。
                    continue;
                }
                else
                {
                    chatterNodeFile = new MieChatterNodeFile(fileCode);
                    mieChatterInfo.AddFile(chatterNodeFile);
                }

                foreach (var node in chatterFile.Nodes)
                {
                    var dataType = node.MieChatterNodeDataTypeTag;

                    var nodeID = node.NodeID;
                    var isQuestionNode = node.IsQuestionNode;
                    bool isRootNode = node.NodeID == 0 ? true : false;

                    //// チャッターエントリー
                    MieChatterNodeEntry.NNodeType nodeType = MieChatterNodeEntry.NNodeType.Unknown;
                    MieChatterNodeEntry nodeEntry = new MieChatterNodeEntry(nodeType, nodeID, isRootNode);
                    chatterNodeFile.AddFlatNodeEntry(nodeEntry);

                    //// Link情報の登録
                    foreach (var link in node.Links)
                    {
                        MieChatterLink mieChatterLink = new MieChatterLink(link.FromNodeID, link.ToNodeID);
                        chatterNodeFile.AddLinkEntry(mieChatterLink);
                    }
                }
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
        private static string GetGameDataTypeTag(string text)
        {
            var tag = text
                .Replace(", Assembly-CSharp", string.Empty)
                .Replace(", OEIFormats", string.Empty);

            return tag;
        }
    }
}
