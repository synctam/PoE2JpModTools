namespace MieTranslationLib.Facade
{
    using MieTranslationLib.Data.Language;
    using NLog;

    public class MieGameApp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieGameApp(MieLanguageInfo languageInfo)
        {
            this.LanguageInfo = languageInfo;
        }

        private MieGameApp() { }

        /// <summary>
        /// 言語情報
        /// </summary>
        public MieLanguageInfo LanguageInfo { get; } = null;
    }
}
