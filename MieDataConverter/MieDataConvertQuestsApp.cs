namespace MieDataConverter
{
    using System;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.Quests;
    using NLog;

    /// <summary>
    /// クエスト/言語情報
    /// </summary>
    [Obsolete("使用禁止")]
    public class MieDataConvertQuestsApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieDataConvertQuestsApp(MieLanguageInfo languageInfo)
        {
            this.LanguageInfo = languageInfo;
        }

        private MieDataConvertQuestsApp() { }

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; private set; } = null;

        /// <summary>
        /// 付加情報
        /// </summary>
        public MieQuestsNodeInfo QuestsNodeInfo { get; private set; }
    }
}
