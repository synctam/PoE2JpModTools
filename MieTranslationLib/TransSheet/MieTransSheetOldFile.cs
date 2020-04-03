namespace MieTranslationLib.TransSheet
{
    using System;
    using System.Collections.Generic;

    public class MieTransSheetOldFile
    {
        public IDictionary<string, MieTransSheetOldEntry> Items { get; } = new SortedDictionary<string, MieTransSheetOldEntry>(StringComparer.CurrentCultureIgnoreCase);

        public void AddEntry(MieTransSheetOldEntry record)
        {
            var text = record.DefaultText
                .Replace("<CRLF>", string.Empty)
                .Replace("<CR>", string.Empty)
                .Replace("<LF>", string.Empty)
                .Replace(" ", string.Empty);

            if (this.Items.ContainsKey(text))
            {
                //// skip
            }
            else
            {
                this.Items.Add(text, record);
            }
        }

        /// <summary>
        /// 指定されたテキストがPoE1の翻訳シートに存在すれば返す。
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>翻訳されたテキスト</returns>
        public string GetTransData(string text)
        {
            var key = text.Trim()
                .Replace("\r\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(" ", string.Empty);

            if (this.Items.ContainsKey(key))
            {
                return this.Items[key].DefaultTranslationText;
            }

            return string.Empty;
        }
    }
}
