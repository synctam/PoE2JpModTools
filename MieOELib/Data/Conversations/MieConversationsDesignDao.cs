namespace MieOELib.Data.Conversations
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MieOELib.Data.Characters;
    using MieOELib.FileUtils;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Data.Conversations;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.MieUtils;
    using Newtonsoft.Json;
    using NLog;

    public class MieConversationsDesignDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// JSON形式のキャラクター属性ファイルを読み込みキャラクター属性ファイルを返す。
        /// [非OE DLL]
        /// </summary>
        /// <param name="path">キャラクター属性ファイルのパス</param>
        /// <returns>キャラクター属性ファイル</returns>
        public static MieCharacterAttributeFile LoadCharacterAttribute(string path)
        {
            MieCharacterAttributeFile charAttributeFile = new MieCharacterAttributeFile();

            string ctext = LoadJson(path);
            ctext = ctext.Replace("$type", "MieGameDataTypeTag");

            CreateAttributeEntry(ctext, charAttributeFile);

            return charAttributeFile;
        }

        /// <summary>
        /// 指定したフォルダー内の会話ファイルを読み込み会話情報を返す。
        /// </summary>
        /// <param name="folderPath">フォルダーのパス</param>
        /// <param name="fileList">FileLIst</param>
        /// <returns>会話情報</returns>
        public static MieConversationNodeInfo LoadFromFolder(string folderPath, MieFileList fileList)
        {
            MieConversationNodeInfo mieConversationInfo = new MieConversationNodeInfo();

            if (!Directory.Exists(folderPath))
            {
                var msg = $"Error: Folder not exists. Folder({folderPath})";
                logger.Error(msg);
                throw new Exception(msg);
            }

            string[] files = Directory.GetFiles(folderPath, "*.conversationbundle", SearchOption.AllDirectories);
            files
                .Where(file => Path.GetExtension(file).ToLower() == ".conversationbundle")
                .ToList()
                .ForEach(x =>
                {
                    var convFile = LoadFromJson(x, fileList);
                    if (convFile != null)
                    {
                        mieConversationInfo.AddFile(convFile);
                    }
                });

            return mieConversationInfo;
        }

        /// <summary>
        /// JSONテキストからキャラクター属性を作成する。
        /// </summary>
        /// <param name="jtext">JSONテキスト</param>
        /// <param name="charAttributeFile">キャラクター属性ファイル</param>
        private static void CreateAttributeEntry(string jtext, MieCharacterAttributeFile charAttributeFile)
        {
            //// デシリアライズ
            var oeCharacters = JsonConvert.DeserializeObject<MieOECharacters>(jtext);
            foreach (var gameDataObject in oeCharacters.GameDataObjects)
            {
                var tag = GetGameDataTypeTag(gameDataObject.MieGameDataTypeTag);
                switch (tag)
                {
                    case "CharacterStatsGameData":
                        var component = gameDataObject.Components[0];
                        //// キャラクター属性
                        MieCharacterAttributeEntry characterAttrEntry = new MieCharacterAttributeEntry(
                            new Guid(gameDataObject.ID),
                            gameDataObject.DebugName,
                            GetGenderType(component.Gender),
                            new Guid(component.SpeakerID),
                            new Guid(component.RaceID));
                        charAttributeFile.AddCharacterAttributeEntry(characterAttrEntry);
                        break;
                    case "RaceGameData":
                        //// 種族属性
                        MieRaceAttributeEntry entry = new MieRaceAttributeEntry(
                            new Guid(gameDataObject.ID),
                            gameDataObject.DebugName);
                        charAttributeFile.AddRaceEntry(entry);
                        break;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// テキスト形式のGenderをGenderタイプに変換して返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>Genderタイプ</returns>
        private static MieCharacterAttributeFile.NGender GetGenderType(string text)
        {
            switch (text.ToLower())
            {
                case "male":
                    return MieCharacterAttributeFile.NGender.Male;
                case "female":
                    return MieCharacterAttributeFile.NGender.Female;
                case "neuter":
                    return MieCharacterAttributeFile.NGender.Neuter;
                default:
                    var msg = $"Unknown Gender text({text}).";
                    throw new InvalidEnumArgumentException(msg);
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
        /// JSON形式の会話ファイルを読み込み、会話ファイルを返す。
        /// [非OE DLL]
        /// </summary>
        /// <param name="path">会話ファイルのパス</param>
        /// <param name="fileList">FileLIst</param>
        /// <returns>会話ファイル</returns>
        private static MieConversationNodeFile LoadFromJson(string path, MieFileList fileList)
        {
            MieConversationNodeFile mieConversationsFile = null;

            var ctext = LoadJson(path);
            ctext = ctext.Replace("$type", "MieFlowChartsDataTypeTag");
            var oeConversations = JsonConvert.DeserializeObject<MieOEConversations>(ctext);

            foreach (var conv in oeConversations.Conversations)
            {
                var fileID = conv.Filename.Replace(".conversation", string.Empty);
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
                    mieConversationsFile = new MieConversationNodeFile(fileCode);
                }

                //// ノード情報作成
                foreach (var node in conv.Nodes)
                {
                    var nodeType = GetFlowChartsDataTypeTag(node.MieFlowChartsDataTypeTag);

                    MieConversationNodeEntry conversationNode = null;

                    Guid speakerGuid;
                    if (node.SpeakerGuid == null)
                    {
                        speakerGuid = default(Guid);
                    }
                    else
                    {
                        speakerGuid = new Guid(node.SpeakerGuid);
                    }

                    Guid listenerGuid;
                    if (node.ListenerGuid == null)
                    {
                        listenerGuid = default(Guid);
                    }
                    else
                    {
                        listenerGuid = new Guid(node.ListenerGuid);
                    }

                    //// Root node は NodeIDで判断する。
                    conversationNode = new MieConversationNodeEntry(
                        nodeType,
                        node.NodeID,
                        speakerGuid,
                        listenerGuid,
                        node.NodeID == 0 ? true : false,
                        node.IsQuestionNode);

                    //// リンク情報作成
                    foreach (var link in node.Links)
                    {
                        MieConversationLink mieConversationLinks = new MieConversationLink(link.FromNodeID, link.ToNodeID);
                        mieConversationsFile.AddLinkEntry(mieConversationLinks);
                    }

                    mieConversationsFile.AddFlatNodeEntry(conversationNode);
                }

                //// キャラクターマップ情報作成
                foreach (var map in conv.CharacterMappings)
                {
                    MieCharacterMapEntry mapEntry = new MieCharacterMapEntry(new Guid(map.Guid));
                    mieConversationsFile.AddCharacterMapEntry(mapEntry);
                }
            }

            if (mieConversationsFile != null)
            {
                mieConversationsFile.BuildLink();
                mieConversationsFile.UpdateDepth();
            }

            return mieConversationsFile;
        }

        /// <summary>
        /// DataTypeTagからDataTypeを返す。
        /// </summary>
        /// <param name="text">DataTypeTag</param>
        /// <returns>DataType</returns>
        private static string GetGameDataTypeTag(string text)
        {
            var tag = text.Replace("Game.GameData.", string.Empty).Replace(", Assembly-CSharp", string.Empty);

            return tag;
        }

        /// <summary>
        /// DataTypeTagからDataTypeを返す。
        /// </summary>
        /// <param name="text">DataTypeTag</param>
        /// <returns>DataType</returns>
        private static MieConversationNodeEntry.NNodeType GetFlowChartsDataTypeTag(string text)
        {
            var tag = text
                .Replace("OEIFormats.FlowCharts.Conversations.", string.Empty)
                .Replace("OEIFormats.FlowCharts.", string.Empty)
                .Replace(", OEIFormats", string.Empty);

            switch (tag.ToLower())
            {
                case "banknode":
                    return MieConversationNodeEntry.NNodeType.BankNode;
                case "playerresponsenode":
                    return MieConversationNodeEntry.NNodeType.PlayerResponseNode;
                case "scriptnode":
                    return MieConversationNodeEntry.NNodeType.ScriptNode;
                case "talknode":
                    return MieConversationNodeEntry.NNodeType.TalkNode;
                case "triggerconversationnode":
                    return MieConversationNodeEntry.NNodeType.TriggerConversationNode;
                case "unknown":
                    return MieConversationNodeEntry.NNodeType.Unknown;
                default:
                    var msg = $"Unknown FlowCharts data type({tag}).";
                    throw new InvalidEnumArgumentException(msg);
            }
        }
    }
}
