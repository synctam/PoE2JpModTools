namespace MieDataConverter
{
    using System;
    using System.IO;
    using System.Text;
    using MieDbLib.SystemDB;
    using MieOELib.Data.Chatter;
    using MieOELib.Data.Conversations;
    using MieOELib.Data.Quests;
    using MieOELib.Data.Speakers;
    using MieOELib.Data.StringTable;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Data.Conversations;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.Quests;
    using MieTranslationLib.Product;
    using NLog;

    /// <summary>
    /// 会話/言語情報
    /// </summary>
    public class MieDataConvertConversationApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; private set; } = null;

        /// <summary>
        /// 付加情報(会話)
        /// </summary>
        public MieConversationNodeInfo ConversationInfo { get; private set; } = null;

        /// <summary>
        /// 付加情報(クエスト)
        /// </summary>
        public MieQuestsNodeInfo QuestsNodeInfo { get; private set; } = null;

        /// <summary>
        /// キャラクター属性ファイル
        /// </summary>
        public MieCharacterAttributeFile CharacterAttributeFile { get; private set; } = null;

        public void InitConversations(MieSystemDB systemDb)
        {
            MieTableCharacterAttributesDao.ClearTable(systemDb);
            MieTableRaceAttributesDao.ClearTable(systemDb);
            MieTableSpeakerAttributesDao.ClearTable(systemDb);

            MieTableConversationNodeLinksDao.ClearTable(systemDb);
            MieTableConversationEntriesDao.ClearTable(systemDb);
        }

        public void InitFileList(MieSystemDB systemDb)
        {
            MieTableFileListDao.ClearTable(systemDb);
        }

        public void InitQuests(MieSystemDB systemDb)
        {
            MieTableQuestsNodeLinksDao.ClearTable(systemDb);
            MieTableQuestsEntriesDao.ClearTable(systemDb);

            MieTableLanguageDao.ClearTable(systemDb);
        }

        public void InitChatter(MieSystemDB systemDb)
        {
            MieTableChatterNodeLinksDao.ClearTable(systemDb);
            MieTableChatterEntriesDao.ClearTable(systemDb);

            MieTableLanguageDao.ClearTable(systemDb);
        }

        public void InitLanguage(MieSystemDB systemDb)
        {
            MieTableLanguageDao.ClearTable(systemDb);
        }

        /// <summary>
        /// FileListのDB化。
        /// </summary>
        /// <param name="systemDb">データベース接続情報</param>
        /// <param name="fileList">FileList</param>
        public void ConvertFileList(MieSystemDB systemDb, MieFileList fileList)
        {
            var oldFileList = MieTableFileListDao.LoadFromSystemDB(systemDb);
            MieFileList fileListAdd = fileList.GetAdd(oldFileList);
            MieFileList fileListUpdate = fileList.GetUpdate(oldFileList);

            MieTableFileListDao.SaveToSystemDB(systemDb, fileListAdd, MieTableFileListDao.NUpdateMode.Add);
            MieTableFileListDao.SaveToSystemDB(systemDb, fileListUpdate, MieTableFileListDao.NUpdateMode.Update);
        }

        /// <summary>
        /// 言語DBの初期化とDB化。
        /// </summary>
        /// <param name="systemDb">データベース接続情報</param>
        /// <param name="langPath">言語情報フォルダーのパス</param>
        /// <param name="productLine">製品区分</param>
        /// <param name="languageType">言語区分</param>
        /// <param name="fileList">FileList</param>
        public void ConvertLanguage(MieSystemDB systemDb, string langPath, MieProduct.NProductLine productLine, MieProduct.NLanguageType languageType, MieFileList fileList)
        {
            if (!Directory.Exists(langPath))
            {
                var msg = $"Directory not found({langPath}).";
                if (productLine == MieProduct.NProductLine.Vanilla)
                {
                    logger.Error(msg);
                    throw new DirectoryNotFoundException(msg);
                }
                else
                {
                    logger.Warn(msg);
                    Console.WriteLine(msg);

                    return;
                }
            }

            //// 話者情報の読み込みとFileListの作成。
            var langInfo = MieStringTableDao.LoadFromFolder(langPath, productLine, languageType, fileList);
            if (this.LanguageInfo == null)
            {
                this.LanguageInfo = langInfo;
            }
            else
            {
                foreach (var langFile in langInfo.Items.Values)
                {
                    this.LanguageInfo.AddFile(langFile, true);
                }
            }
        }

        /// <summary>
        /// キャラクター情報、種族情報の初期化とDB化。
        /// </summary>
        /// <param name="systemDb">データベース接続情報</param>
        /// <param name="charAttrPath">キャラクター情報ファイルのパス</param>
        public void ConvertCharcterAttributes(MieSystemDB systemDb, string charAttrPath)
        {
            //// JSONファイルからキャラクター情報を読み込み、CharAttrとRaceAttrを作成する。
            this.CharacterAttributeFile = MieConversationsDesignDao.LoadCharacterAttribute(charAttrPath);

            //// キャラクター情報(CharAttr)をDBに保存する。
            MieTableCharacterAttributesDao.SaveToSysyemDB(systemDb, this.CharacterAttributeFile);

            //// 種族情報(RaceAttr)をDBに保存する。
            MieTableRaceAttributesDao.SaveToSysyemDB(systemDb, this.CharacterAttributeFile);
        }

        /// <summary>
        /// 話者情報のDB化。
        /// </summary>
        /// <param name="systemDb">データベース接続情報</param>
        /// <param name="speakerAttrPath">話者情報</param>
        public void ConvertSpeakerAttributes(MieSystemDB systemDb, string speakerAttrPath)
        {
            //// JSONファイルからSpeaker情報を読み込む
            MieOESpeakersDao.AppendSpeakerAttribute(this.CharacterAttributeFile, speakerAttrPath);
            //// Speaker情報をDBに書き込む
            MieTableSpeakerAttributesDao.SaveToSysyemDB(systemDb, this.CharacterAttributeFile);
        }

        /// <summary>
        /// 付加情報(会話)のDB化
        /// </summary>
        /// <param name="systemDb">データベース接続情報</param>
        /// <param name="conversationPath">会話情報フォルダーのパス</param>
        /// <param name="fileList">FileList</param>
        public void ConvertConversations(MieSystemDB systemDb, string conversationPath, MieFileList fileList)
        {
            if (this.LanguageInfo == null)
            {
                var msg = $"LanguageInfo が未設定です。ConvertLanguage()で言語情報を先に作成してください。";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            //// 付加情報(会話)の取得
            var convNodeInfo = MieConversationsDesignDao.LoadFromFolder(conversationPath, fileList);

            //// NodeLink情報をDBに格納する。
            MieTableConversationNodeLinksDao.SaveToDB(systemDb, convNodeInfo);

            //// 会話情報をDBに格納する。
            MieTableConversationEntriesDao.SaveToDB(systemDb, convNodeInfo);
        }

        public void ConvertQuests(MieSystemDB systemDb, string questsPath, MieFileList fileList)
        {
            if (this.LanguageInfo == null)
            {
                var msg = $"LanguageInfo が未設定です。ConvertLanguage()で言語情報を先に作成してください。";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            var questsNodeInfo = MieQuestsDesignDao.LoadFromFolder(questsPath, fileList);
            //// NodeLink情報をDBに格納する。
            MieTableQuestsNodeLinksDao.SaveToDB(systemDb, questsNodeInfo);
            //// 付加情報(クエスト)の取得
            MieTableQuestsEntriesDao.SaveToDB(systemDb, questsNodeInfo);
        }

        public void ConvertChatter(MieSystemDB systemDb, string chatterPath, MieFileList fileList)
        {
            if (this.LanguageInfo == null)
            {
                var msg = $"LanguageInfo が未設定です。ConvertLanguage()で言語情報を先に作成してください。";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            var chatterNodeInfo = MieChatterDesignDao.LoadFromFolder(chatterPath, fileList);
            //// NodeLink情報をDBに格納する。
            MieTableChatterNodeLinksDao.SaveToDB(systemDb, chatterNodeInfo);
            //// 付加情報(チャッター)の取得
            MieTableChatterEntriesDao.SaveToDB(systemDb, chatterNodeInfo);
        }

        /// <summary>
        /// 製品区分、言語区分ごとの件数を書式化して返す。
        /// </summary>
        /// <returns>書式化した件数</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            buff.AppendLine("Vanilla:");
            buff.AppendLine($"\tConv  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Conversations),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Conversations),6:#,##0})");
            buff.AppendLine($"\tGame  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Game),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Game),6:#,##0})");
            buff.AppendLine($"\tQuests=" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Quests),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Quests),6:#,##0})");
            buff.AppendLine($"\tChatter=" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Chatter),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.Vanilla, MieProduct.NLanguageType.Chatter),6:#,##0})");

            buff.AppendLine("DLC1:");
            buff.AppendLine($"\tConv  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Conversations),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Conversations),6:#,##0})");
            buff.AppendLine($"\tGame  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Game),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Game),6:#,##0})");
            buff.AppendLine($"\tQuests=" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Quests),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC1, MieProduct.NLanguageType.Quests),6:#,##0})");

            buff.AppendLine("DLC2:");
            buff.AppendLine($"\tConv  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Conversations),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Conversations),6:#,##0})");
            buff.AppendLine($"\tGame  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Game),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Game),6:#,##0})");
            buff.AppendLine($"\tQuests=" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Quests),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC2, MieProduct.NLanguageType.Quests),6:#,##0})");

            buff.AppendLine("DLC3:");
            buff.AppendLine($"\tConv  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Conversations),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Conversations),6:#,##0})");
            buff.AppendLine($"\tGame  =" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Game),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Game),6:#,##0})");
            buff.AppendLine($"\tQuests=" +
                $"Files(  {this.LanguageInfo.GetFileCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Quests),6:#,##0}) " +
                $"Entries({this.LanguageInfo.GetEntryCount(MieProduct.NProductLine.DLC3, MieProduct.NLanguageType.Quests),6:#,##0})");

            return buff.ToString();
        }
    }
}
