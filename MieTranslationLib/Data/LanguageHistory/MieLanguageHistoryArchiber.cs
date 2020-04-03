namespace MieTranslationLib.Data.LanguageHistory
{
    using System.Collections.Generic;
    using NLog;

    /// <summary>
    /// 言語履歴アーカイバー：未保存の履歴を保有。保存が完了するとキューは空になる。
    /// </summary>
    public class MieLanguageHistoryArchiber
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// アーカイブキュー：未保存の履歴を保有。
        /// </summary>
        private Queue<MieLanguageHistoryEntry> archibe = new Queue<MieLanguageHistoryEntry>();

        /// <summary>
        /// 履歴エントリーをアーカイブする。
        /// </summary>
        /// <param name="historyEntry">履歴エントリー</param>
        public void ArchibeHistory(MieLanguageHistoryEntry historyEntry)
        {
            this.archibe.Enqueue(historyEntry);
        }

        /// <summary>
        /// 履歴エントリーをアーカイブから取り出す。
        /// </summary>
        /// <returns>履歴</returns>
        public MieLanguageHistoryEntry Dequeue()
        {
            return this.archibe.Dequeue();
        }
    }
}
