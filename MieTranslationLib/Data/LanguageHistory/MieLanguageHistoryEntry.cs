namespace MieTranslationLib.Data.LanguageHistory
{
    using System;
    using System.Collections.Generic;
    using MieTranslationLib.Data.Language;
    using NLog;

    /// <summary>
    /// 言語エントリーの履歴エントリー
    /// </summary>
    public class MieLanguageHistoryEntry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<DateTime, MieLanguageEntry> items = new SortedDictionary<DateTime, MieLanguageEntry>();

        public void AddEntry(MieLanguageEntry entry)
        {
            DateTime updateat = DateTime.UtcNow;
            this.items.Add(updateat, entry);
        }
    }
}
