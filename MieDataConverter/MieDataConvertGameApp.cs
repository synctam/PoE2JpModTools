namespace MieDataConverter
{
    using System;
    using MieTranslationLib.Data.Language;
    using NLog;

    [Obsolete("使用禁止")]
    public class MieDataConvertGameApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieDataConvertGameApp(MieLanguageInfo languageInfo)
        {
            this.LanguageInfo = languageInfo;
        }

        private MieDataConvertGameApp() { }

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; } = null;
    }
}
