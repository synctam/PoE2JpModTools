namespace MieTranslationLib.Data.LanguageHistory
{
    using System;
    using System.Collections.Generic;
    using NLog;

    /// <summary>
    /// 言語エントリーの履歴情報
    /// </summary>
    public class MieLanguageHistoryInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 履歴ファイルの辞書。キーはFileID。
        /// </summary>
        private readonly IDictionary<string, MieLanguageHistoryFile> items = new SortedDictionary<string, MieLanguageHistoryFile>();

        /// <summary>
        /// 言語ファイルの履歴を返す。履歴がない場合は例外を発生させる。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <returns>履歴</returns>
        public MieLanguageHistoryFile GetFileHistory(string fileID)
        {
            if (this.items.ContainsKey(fileID))
            {
                return this.items[fileID];
            }
            else
            {
                var msg = $"History file not found. FileID({fileID})";
                logger.Trace(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// ファイル履歴を追加する。
        /// </summary>
        /// <param name="mieLanguageHistoryFile">ファイル履歴</param>
        public void AddFile(MieLanguageHistoryFile mieLanguageHistoryFile)
        {
            if (this.items.ContainsKey(mieLanguageHistoryFile.FileID))
            {
                var msg = $"Warning: File is already registered. FileID({mieLanguageHistoryFile.FileID})";
                logger.Warn(msg);
            }
            else
            {
                this.items.Add(mieLanguageHistoryFile.FileID, mieLanguageHistoryFile);
            }
        }
    }
}
