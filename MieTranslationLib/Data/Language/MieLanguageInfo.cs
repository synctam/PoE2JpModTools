namespace MieTranslationLib.Data.Language
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MieTranslationLib.Product;
    using NLog;

    /// <summary>
    /// 言語情報
    /// </summary>
    public class MieLanguageInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 言語ファイルの辞書。キーはFileCode。
        /// FileIDはStringTable内のNameタグに記載されている内容。
        /// </summary>
        public IDictionary<long, MieLanguageFile> Items { get; } = new Dictionary<long, MieLanguageFile>();

        /// <summary>
        /// ファイル数
        /// </summary>
        public int FileCount { get { return this.Items.Count; } }

        /// <summary>
        /// 言語ファイルを追加する。
        /// すでにFileが存在する場合は言語ファイル内の言語エントリーを追加する。
        /// </summary>
        /// <param name="mieLanguageFile">言語ファイル</param>
        /// <param name="isMarge">マージの有無</param>
        public void AddFile(MieLanguageFile mieLanguageFile, bool isMarge)
        {
            if (this.Items.ContainsKey(mieLanguageFile.FileCode))
            {
                //// すでに登録済みの場合は言語エントリーを追加する。
                var currentFile = this.Items[mieLanguageFile.FileCode];
                currentFile.AddFile(mieLanguageFile, isMarge);
            }
            else
            {
                this.Items.Add(mieLanguageFile.FileCode, mieLanguageFile);
            }
        }

        /// <summary>
        /// 指定したFileIDの言語ファイルを返す。
        /// </summary>
        /// <param name="fileCode">FileCode</param>
        /// <returns>言語ファイル</returns>
        public MieLanguageFile GetFile(long fileCode)
        {
            if (this.Items.ContainsKey(fileCode))
            {
                var languageFile = this.Items[fileCode];
                return languageFile;
            }
            else
            {
                var msg = $"File not found. FileID({fileCode})";
                logger.Error(msg);
                return null;
            }
        }

        /// <summary>
        /// 指定した製品区分のファイル数を返す。
        /// </summary>
        /// <param name="nProductLine">製品区分</param>
        /// <param name="nLanguageType">言語区分</param>
        /// <returns>ファイル数</returns>
        public int GetFileCount(MieProduct.NProductLine nProductLine, MieProduct.NLanguageType nLanguageType)
        {
            int fileCount = 0;
            foreach (var file in this.Items.Values)
            {
                if (file.GetEntryCount(nProductLine, nLanguageType) > 0)
                {
                    fileCount++;
                }
            }

            return fileCount;
        }

        public int GetEntryCount(MieProduct.NProductLine productLine, MieProduct.NLanguageType languageType, bool ommitEmpty)
        {
            var total = 0;
            this.Items.Values
                .ToList()
                .ForEach(x =>
                {
                    total += x.GetEntryCount(productLine, languageType, ommitEmpty);
                });

            return total;
        }

        /// <summary>
        /// 指定した製品区分の言語エントリー数を返す。
        /// </summary>
        /// <param name="nProductLine">製品区分</param>
        /// <param name="nLanguageType">言語区分</param>
        /// <returns>言語エントリー数</returns>
        public int GetEntryCount(MieProduct.NProductLine nProductLine, MieProduct.NLanguageType nLanguageType)
        {
            var count = this.Items
                .SelectMany((x) => x.Value.Items)
                .Count((x) => x.ProductLine.HasFlag(nProductLine));

            return count;
        }

        public IDictionary<long, MieLanguageFile> GetFiles(MieProduct.NProductLine productLine)
        {
            if (productLine == MieProduct.NProductLine.ALL)
            {
                return this.Items;
            }
            else
            {
                var result = this.Items.Values
                    .Where(x => (x.ProductLine & productLine) == productLine)
                    .ToDictionary(x => x.FileCode);
                return result;
            }
        }

        public string ToString(bool isShortText)
        {
            StringBuilder buff = new StringBuilder();
            this.Items.Values
                .OrderBy((x) => x.FileCode)
                .ToList()
                .ForEach((x) =>
                 {
                     buff.Append($"{x.ToString(isShortText)}");
                 });

            return buff.ToString();
        }

        /// <summary>
        /// 指定したFileCodeの言語ファイルを取り出し、言語エントリーを追加する。
        /// 言語ファイルが存在しない場合は、新たに作成後、言語エントリーを追加する。
        /// </summary>
        /// <param name="fileCode">FileCode</param>
        /// <param name="langEntry">言語エントリー</param>
        public void AddFileEntry(long fileCode, MieLanguageEntry langEntry)
        {
            if (this.Items.ContainsKey(fileCode))
            {
                //// 言語ファイルが存在する。
                var langFile = this.Items[fileCode];
                langFile.AddEntry(langEntry);
            }
            else
            {
                //// 言語ファイルが存在しない。
                MieLanguageFile mieLanguageFile = new MieLanguageFile(fileCode);
                mieLanguageFile.AddEntry(langEntry);
                this.Items.Add(mieLanguageFile.FileCode, mieLanguageFile);
            }
        }
    }
}
