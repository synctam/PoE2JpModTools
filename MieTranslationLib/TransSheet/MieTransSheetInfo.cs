namespace MieTranslationLib.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MieTranslationLib.Product;
    using NLog;
    using S5mCommon_1F1F6148_9E9B_4F66_AEB6_EB749A40E94E;

    /// <summary>
    /// 翻訳シート情報
    /// </summary>
    public class MieTransSheetInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IDictionary<string, MieTransSheetFile> Items { get; } = new SortedDictionary<string, MieTransSheetFile>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 重複テキスト翻訳文流用辞書（会話）。
        /// キーは、不要文字を除去した原文のテキスト
        /// </summary>
        public IDictionary<string, MieTransSheetEntry> SearchConvItems { get; } =
            new SortedDictionary<string, MieTransSheetEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 重複テキスト翻訳文流用辞書（ゲーム）。
        /// キーは、不要文字を除去した原文のテキスト
        /// </summary>
        public IDictionary<string, MieTransSheetEntry> SearchGameItems { get; } =
            new SortedDictionary<string, MieTransSheetEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 重複テキスト翻訳文流用辞書（クエスト）。
        /// キーは、不要文字を除去した原文のテキスト
        /// </summary>
        public IDictionary<string, MieTransSheetEntry> SearchQuestsItems { get; } =
            new SortedDictionary<string, MieTransSheetEntry>(StringComparer.OrdinalIgnoreCase);

        public string GetSearchText(string text, MieProduct.NLanguageType languageType)
        {
            var key = this.GetDiscardedText(text);
            switch (languageType)
            {
                case MieProduct.NLanguageType.Chatter:
                    throw new NotImplementedException();
                case MieProduct.NLanguageType.Conversations:
                    if (this.SearchConvItems.ContainsKey(key))
                    {
                        return this.SearchConvItems[key].DefaultTranslationText;
                    }
                    else if (this.SearchGameItems.ContainsKey(key))
                    {
                        return this.SearchGameItems[key].DefaultTranslationText;
                    }
                    else
                    {
                        return null;
                    }

                case MieProduct.NLanguageType.Game:
                    if (this.SearchConvItems.ContainsKey(key))
                    {
                        return this.SearchGameItems[key].DefaultTranslationText;
                    }
                    else
                    {
                        return null;
                    }

                case MieProduct.NLanguageType.Quests:
                    if (this.SearchQuestsItems.ContainsKey(key))
                    {
                        return this.SearchQuestsItems[key].DefaultTranslationText;
                    }
                    else if (this.SearchGameItems.ContainsKey(key))
                    {
                        return this.SearchGameItems[key].DefaultTranslationText;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new Exception($"Unknown LanguageType({languageType}).");
            }
        }

        public void MargeInfo(MieTransSheetInfo sourceInfo)
        {
            foreach (var sheetFile in sourceInfo.Items.Values)
            {
                this.AddFile(sheetFile);
            }
        }

        public void AddFile(MieTransSheetFile mieTransSheetFile)
        {
            if (this.Items.ContainsKey(mieTransSheetFile.FileID))
            {
                var transSheetFile = this.Items[mieTransSheetFile.FileID];
                transSheetFile.MergeFile(mieTransSheetFile);
            }
            else
            {
                this.Items.Add(mieTransSheetFile.FileID, mieTransSheetFile);
            }

            this.AddSearchFile(mieTransSheetFile);
        }

        public void AddEntry(MieTransSheetEntry entry)
        {
            if (this.Items.ContainsKey(entry.FileID))
            {
                var sheetFile = this.Items[entry.FileID];
                sheetFile.AddEntry(entry);
            }
            else
            {
                var mieTransSheetFile = new MieTransSheetFile(entry.FileID);
                this.Items.Add(mieTransSheetFile.FileID, mieTransSheetFile);
                mieTransSheetFile.AddEntry(entry);
            }

            this.AddSearchEntry(entry);
        }

        public MieTransSheetFile GetFile(string fileID)
        {
            if (this.Items.ContainsKey(fileID))
            {
                return this.Items[fileID];
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            foreach (var sheetFile in this.Items.Values)
            {
                buff.Append(sheetFile.ToString());
            }

            return buff.ToString();
        }

        private void AddSearchFile(MieTransSheetFile sheetFile)
        {
            foreach (var entry in sheetFile.Items.Values)
            {
                this.AddSearchEntry(entry);
            }
        }

        private void AddSearchEntry(MieTransSheetEntry mieTransSheetEntry)
        {
            var newEntry = mieTransSheetEntry;
            var numNew = JapaneseStringUtils.TJapaneseStringUtils.NumJapaneseChars(newEntry.DefaultTranslationText);
            if (numNew == 0)
            {
                return;
            }

            IDictionary<string, MieTransSheetEntry> items = null;
            switch (newEntry.LanguageType)
            {
                case Product.MieProduct.NLanguageType.Chatter:
                    break;
                case Product.MieProduct.NLanguageType.Conversations:
                    items = this.SearchConvItems;
                    break;
                case Product.MieProduct.NLanguageType.Game:
                    items = this.SearchGameItems;
                    break;
                case Product.MieProduct.NLanguageType.Quests:
                    items = this.SearchQuestsItems;
                    break;
                default:
                    throw new Exception($"Unknown LanguageType({newEntry.LanguageType}).");
            }

            var keyNew = this.GetDiscardedText(mieTransSheetEntry.DefaultText);
            if (items.ContainsKey(keyNew))
            {
                var oldEntry = items[keyNew];
                var keyOld = this.GetDiscardedText(oldEntry.DefaultText);
                var numOld = JapaneseStringUtils.TJapaneseStringUtils.NumJapaneseChars(oldEntry.DefaultTranslationText);

                if (numOld < numNew)
                {
                    //// 日本語の文字数が多いので入れ替え
                    items.Remove(keyOld);
                    items.Add(keyNew, mieTransSheetEntry);
                }
            }
            else
            {
                items.Add(keyNew, mieTransSheetEntry);
            }
        }

        private string GetDiscardedText(string text)
        {
            var buff = new StringBuilder(text);

            buff.Replace(" ", string.Empty);
            buff.Replace("\r\n", string.Empty);
            buff.Replace("\r", string.Empty);
            buff.Replace("\n", string.Empty);
            buff.Replace("\t", string.Empty);

            return buff.ToString();
        }
    }
}
