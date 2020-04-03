namespace MieTranslationLib.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NLog;

    /// <summary>
    /// 翻訳シートファイル
    /// </summary>
    public class MieTransSheetFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieTransSheetFile(string fileID)
        {
            this.FileID = fileID;
        }

        /// <summary>
        /// 翻訳シートエントリーの辞書。キーはID。
        /// </summary>
        public IDictionary<int, MieTransSheetEntry> Items { get; } = new SortedDictionary<int, MieTransSheetEntry>();

        public string FileID { get; } = string.Empty;

        public long FileCode { get; } = 0;

        public bool IsDeleted { get; set; } = false;

        public void MergeFile(MieTransSheetFile sheetFile)
        {
            foreach (var entry in sheetFile.Items.Values)
            {
                this.AddEntry(entry);
            }
        }

        public void AddEntry(MieTransSheetEntry mieTransSheetEntry)
        {
            if (this.Items.ContainsKey(mieTransSheetEntry.ID))
            {
                var msg = $"Duplicate trans sheet entry({mieTransSheetEntry.ID}) FileID({this.FileID})";
                logger.Error(msg);
                throw new Exception(msg);
            }
            else
            {
                this.Items.Add(mieTransSheetEntry.ID, mieTransSheetEntry);
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            foreach (var sheetEntry in this.Items.Values)
            {
                buff.Append(sheetEntry.ToString());
            }

            return buff.ToString();
        }

        public MieTransSheetEntry GetEntry(int id)
        {
            if (this.Items.ContainsKey(id))
            {
                return this.Items[id];
            }
            else
            {
                return null;
            }
        }
    }
}
