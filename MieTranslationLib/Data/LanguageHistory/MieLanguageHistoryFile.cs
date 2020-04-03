namespace MieTranslationLib.Data.LanguageHistory
{
    using System.Collections.Generic;
    using System.Linq;
    using MieTranslationLib.Data.Language;
    using NLog;

    public class MieLanguageHistoryFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 履歴辞書。
        /// キーはID。
        /// ToDo: この辞書は使っているのか？
        /// </summary>
        private readonly IDictionary<int, MieLanguageHistoryEntry> items = new Dictionary<int, MieLanguageHistoryEntry>();

        /// <summary>
        /// 言語履歴アーカイバー：未保存の言語エントリーを保有。
        /// </summary>
        private MieLanguageHistoryArchiber languageArchiber = new MieLanguageHistoryArchiber();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileID">FileID</param>
        public MieLanguageHistoryFile(string fileID)
        {
            this.FileID = fileID;
        }

        /// <summary>
        /// 履歴の有無を返す。
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (this.items.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// FileID
        /// </summary>
        public string FileID { get; }

        /// <summary>
        /// 登録されている履歴の件数を返す。
        /// </summary>
        /// <returns>履歴件数</returns>
        public int Count()
        {
            return this.items.Count();
        }

        /// <summary>
        /// 履歴に言語エントリーを追加する。
        /// </summary>
        /// <param name="entry">言語エントリー</param>
        public void Archibe(MieLanguageEntry entry)
        {
            //// ファイル履歴からエントリー履歴を取り出す。
            var history = this.items[entry.ID];
            //// エントリー履歴に追加する。
            history.AddEntry(entry);
            //// エントリー履歴を
            this.languageArchiber.ArchibeHistory(history);
        }

        /// <summary>
        /// 指定したIDの最新の履歴を返す。存在しない場合はNULLを返す。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>指定したIDの最新の履歴</returns>
        public MieLanguageEntry GetFirstEntry(int id)
        {
            return null;
        }
    }
}
