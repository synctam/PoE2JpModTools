namespace MieTranslationLib.Facade
{
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Data.Conversations;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Product;
    using NLog;

    /// <summary>
    /// 会話/言語情報
    /// </summary>
    public class MieConversationApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieConversationApp(MieLanguageInfo languageInfo, MieConversationNodeInfo design, MieCharacterAttributeFile characterAttributeFile)
        {
            this.LanguageInfo = languageInfo;
            this.Design = design;
            this.CharacterAttributeFile = characterAttributeFile;
        }

        private MieConversationApp() { }

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; private set; } = null;

        /// <summary>
        /// 付加情報
        /// </summary>
        public MieConversationNodeInfo Design { get; private set; } = null;

        /// <summary>
        /// キャラクター属性ファイル
        /// </summary>
        public MieCharacterAttributeFile CharacterAttributeFile { get; } = null;

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
