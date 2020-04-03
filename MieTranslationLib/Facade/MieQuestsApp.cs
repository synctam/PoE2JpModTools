namespace MieTranslationLib.Facade
{
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.Quests;
    using NLog;

    /// <summary>
    /// クエスト/言語情報
    /// </summary>
    public class MieQuestsApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieQuestsApp(MieLanguageInfo languageInfo, MieQuestsNodeInfo design)
        {
            this.LanguageInfo = languageInfo;
            this.Design = design;
        }

        private MieQuestsApp() { }

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; private set; } = null;

        /// <summary>
        /// 付加情報
        /// </summary>
        public MieQuestsNodeInfo Design { get; private set; }
    }
}
