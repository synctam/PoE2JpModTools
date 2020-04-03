namespace MieTranslationLib.TransSheet
{
    using System.IO;
    using System.Text;
    using CsvHelper;
    using CsvHelper.Configuration;
    using NLog;

    public class MieTransSheetOldDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static MieTransSheetOldFile LoadFromOldCsv(string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    //// 区切り文字
                    csv.Configuration.Delimiter = ",";
                    //// ヘッダーの有無
                    csv.Configuration.HasHeaderRecord = true;
                    //// CSVファイルに合ったマッピングルールを登録
                    csv.Configuration.RegisterClassMap<CsvMapperPoE1>();
                    //// データを読み出し
                    var records = csv.GetRecords<MieTransSheetOldEntry>();

                    MieTransSheetOldFile sheet = new MieTransSheetOldFile();
                    foreach (var record in records)
                    {
                        //// PoE1はタグ<LF>付きである。
                        record.DefaultText = record.DefaultText.Trim();
                        record.DefaultTranslationText = record.DefaultTranslationText.Trim();
                        sheet.AddEntry(record);
                    }

                    return sheet;
                }
            }
        }

        /// <summary>
        /// 格納ルール
        /// </summary>
        public class CsvMapperPoE1 : ClassMap<MieTransSheetOldEntry>
        {
            public CsvMapperPoE1()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[日本語DefaultText]]");
                this.Map(x => x.FileID).Name("[[FileName]]");
                this.Map(x => x.ID).Name("[[ID]]");
            }
        }
    }
}
