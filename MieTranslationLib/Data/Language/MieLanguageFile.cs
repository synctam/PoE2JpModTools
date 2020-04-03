namespace MieTranslationLib.Data.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MieTranslationLib.Data.LanguageHistory;
    using MieTranslationLib.Product;
    using NLog;

    /// <summary>
    /// 言語ファイル
    /// </summary>
    public class MieLanguageFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MieLanguageFileAttributeInfo mieLanguageFileAttributeInfo = null;
        private IDictionary<int, MieLanguageEntry> items = new Dictionary<int, MieLanguageEntry>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileCode">FileID</param>
        public MieLanguageFile(long fileCode)
        {
            this.FileCode = fileCode;
        }

        /// <summary>
        /// 言語エントリーの辞書。キーはID。
        /// 言語エントリーは更新せず、変更時は旧エントリーをアーカイブ後削除し、新たにエントリーを追加する。
        /// </summary>
        public IReadOnlyCollection<MieLanguageEntry> Items { get { return this.items.Values as IReadOnlyCollection<MieLanguageEntry>; } }

        /// <summary>
        /// 言語エントリーの履歴。キーは更新日。
        /// 製品のアップデートにより変更されたファイルの履歴を保有する。
        /// キーは協定世界時(UTC)。
        /// </summary>
        public MieLanguageHistoryFile Histories { get; }

        /// <summary>
        /// FileID: FileIDはStringTable内のNameタグに記載されている内容。
        /// 形式：Directory + FileName without extension.
        /// </summary>
        public long FileCode { get; } = 0;

        /// <summary>
        /// ファイル属性
        /// </summary>
        public IMieLanguageFileAttribute Attribute
        {
            get
            {
                return this.mieLanguageFileAttributeInfo.GetAttribute(this.FileCode);
            }
        }

        /// <summary>
        /// このファイル配下にある言語エントリーの製品区分を全て返す。
        /// </summary>
        public MieProduct.NProductLine ProductLine { get { return this.GetLanguageTypeText(); } }

        /// <summary>
        /// このファイル配下にある言語エントリーの製品区分をテキスト化して返す。
        /// テキストの形式は V/1/2/3 。V:Vanilla, 1:DLC1, ...
        /// 該当しない項目はブランクとなる。 V/1/ /3
        /// </summary>
        public string ProductLineText
        {
            get
            {
                char separator = '/';
                var productLineText = string.Empty;

                if (this.ProductLine.HasFlag(MieProduct.NProductLine.Vanilla))
                {
                    productLineText += $"V{separator}";
                }
                else
                {
                    productLineText += $"-{separator}";
                }

                if (this.ProductLine.HasFlag(MieProduct.NProductLine.DLC1))
                {
                    productLineText += $"1{separator}";
                }
                else
                {
                    productLineText += $"-{separator}";
                }

                if (this.ProductLine.HasFlag(MieProduct.NProductLine.DLC2))
                {
                    productLineText += $"2{separator}";
                }
                else
                {
                    productLineText += $"-{separator}";
                }

                if (this.ProductLine.HasFlag(MieProduct.NProductLine.DLC3))
                {
                    productLineText += $"3{separator}";
                }
                else
                {
                    productLineText += $"-{separator}";
                }

                return productLineText.TrimEnd(separator);
            }
        }

        /// <summary>
        /// 言語エントリーの追加し、その言語エントリーを返す。
        /// </summary>
        /// <param name="mieLanguageEntry">言語エントリー</param>
        /// <returns>追加した言語エントリー</returns>
        public MieLanguageEntry AddEntry(MieLanguageEntry mieLanguageEntry)
        {
            if (this.items.ContainsKey(mieLanguageEntry.ID))
            {
                var msg = $"Duplicate MieLanguageEntry key({mieLanguageEntry.ID}). FileID{this.FileCode}";
                logger.Error(msg);
                Console.WriteLine(msg);
                return null;
                ////throw new Exception(msg);
            }
            else
            {
                this.items.Add(mieLanguageEntry.ID, mieLanguageEntry);

                return mieLanguageEntry;
            }
        }

        /// <summary>
        /// 言語エントリーを更新し、更新後の言語エントリーを返す。
        /// 更新後の言語エントリーはアーカイブにも保管する。
        /// また、言語エントリーが存在しない場合は追加する。
        /// </summary>
        /// <param name="mieLanguageEntry">言語エントリー</param>
        /// <returns>更新後のエントリー</returns>
        public MieLanguageEntry UpdateEntry(MieLanguageEntry mieLanguageEntry)
        {
            if (this.items.ContainsKey(mieLanguageEntry.ID))
            {
                var oldEntry = this.items[mieLanguageEntry.ID];
                if ((oldEntry.DefaultText == mieLanguageEntry.DefaultText) && (oldEntry.FemaleText == mieLanguageEntry.FemaleText))
                {
                    //// 更新しようとしたがテキストに変更がなかった。
                    logger.Warn($"Update canceled. Text was not changed. FileID({this.FileCode}) ID({mieLanguageEntry.ID})");

                    return mieLanguageEntry;
                }
                else
                {
                    //// 更新時は旧言語エントリーの製品区分を反映するため、新たに言語エントリーを作り直す。
                    MieLanguageEntry newEntry = new MieLanguageEntry(
                        mieLanguageEntry.ID,
                        mieLanguageEntry.DefaultText,
                        mieLanguageEntry.FemaleText,
                        mieLanguageEntry.ProductLine | oldEntry.ProductLine, // 旧言語エントリーの製品区分を反映する。
                        mieLanguageEntry.ReferenceID,
                        mieLanguageEntry.UpdatedAt);
                    mieLanguageEntry = null;

                    this.RemoveEntry(oldEntry.ID);
                    this.AddEntry(newEntry);
                    //// 更新された言語エントリーをアーカイブする。
                    this.Histories.Archibe(oldEntry);
                    logger.Info($"marged. ID({newEntry.ID})");

                    return newEntry;
                }
            }
            else
            {
                this.AddEntry(mieLanguageEntry);

                return mieLanguageEntry;
            }
        }

        /// <summary>
        /// 指定したidの言語エントリーを返す。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>言語エントリー</returns>
        public MieLanguageEntry GetEntry(int id)
        {
            if (this.items.ContainsKey(id))
            {
                return this.items[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 言語ファイルを追加する。
        /// </summary>
        /// <param name="newFile">言語ファイル</param>
        /// <param name="isMarge">追加モード</param>
        public void AddFile(MieLanguageFile newFile, bool isMarge)
        {
            foreach (var newEntry in newFile.items.Values)
            {
                if (isMarge)
                {
                    //// マージモードの場合は追加または更新する。
                    this.UpdateEntry(newEntry);
                }
                else
                {
                    //// 通常モードの場合は追加する。
                    this.AddEntry(newEntry);
                }
            }
        }

        /// <summary>
        /// 指定した製品区分の言語エントリー数を返す。
        /// </summary>
        /// <param name="nProductLine">製品区分</param>
        /// <param name="nLanguageType">言語区分</param>
        /// <param name="ommitEmpty">空文をスキップする</param>
        /// <returns>言語エントリー数</returns>
        public int GetEntryCount(MieProduct.NProductLine nProductLine, MieProduct.NLanguageType nLanguageType, bool ommitEmpty = false)
        {
            var total = 0;
            if (ommitEmpty)
            {
                total = this.items.Values.Count(
                    (x) => (
                    x.ProductLine.HasFlag(nProductLine) &&
                    !string.IsNullOrWhiteSpace(x.DefaultText)));
            }
            else
            {
                total = this.items.Values.Count(
                    (x) => x.ProductLine.HasFlag(nProductLine));
            }

            return total;
        }

        /// <summary>
        /// 言語ヒストリーを保存する。
        /// </summary>
        public void Save()
        {
            //// ToDo: 言語エントリーの履歴を保存する。
            if (!this.Histories.IsEmpty)
            {
                //// 保存処理。
            }
        }

        public string ToString(bool isShortText)
        {
            StringBuilder buff = new StringBuilder();
            buff.AppendLine($"FileID({this.FileCode})");
            this.items.Values
                .OrderBy((x) => x.ID)
                .ToList()
                .ForEach((x) =>
                {
                    buff.Append(x.ToString(isShortText));
                });

            return buff.ToString();
        }

        /// <summary>
        /// 言語区分をテキスト化したものを返す。
        /// </summary>
        /// <returns>テキスト化した言語区分</returns>
        private MieProduct.NProductLine GetLanguageTypeText()
        {
            MieProduct.NProductLine nProductLine = MieProduct.NProductLine.Vanilla;

            foreach (var entry in this.items.Values)
            {
                if (entry.ProductLine.HasFlag(MieProduct.NProductLine.None))
                {
                    nProductLine |= MieProduct.NProductLine.None;
                }

                if (entry.ProductLine.HasFlag(MieProduct.NProductLine.Vanilla))
                {
                    nProductLine |= MieProduct.NProductLine.Vanilla;
                }

                if (entry.ProductLine.HasFlag(MieProduct.NProductLine.DLC1))
                {
                    nProductLine |= MieProduct.NProductLine.DLC1;
                }

                if (entry.ProductLine.HasFlag(MieProduct.NProductLine.DLC2))
                {
                    nProductLine |= MieProduct.NProductLine.DLC2;
                }

                if (entry.ProductLine.HasFlag(MieProduct.NProductLine.DLC3))
                {
                    nProductLine |= MieProduct.NProductLine.DLC3;
                }
            }

            return nProductLine;
        }

        /// <summary>
        /// 言語エントリーを削除する。
        /// </summary>
        /// <param name="id">削除する言語エントリーのID</param>
        /// <returns>削除の成否</returns>
        private bool RemoveEntry(int id)
        {
            var rc = this.items.Remove(id);

            return rc;
        }
    }
}
