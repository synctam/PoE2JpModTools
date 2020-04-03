namespace MieTranslationLib.Data.FileList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MieTranslationLib.Product;
    using NLog;

    public class MieFileList
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ファイル一覧。
        /// キーはFileCode。
        /// </summary>
        public IDictionary<long, MieFileEntry> Items { get; } = new SortedDictionary<long, MieFileEntry>();

        /// <summary>
        /// 指定されたFileListを追加する。
        /// </summary>
        /// <param name="fileList">FileList</param>
        public void AddFile(MieFileList fileList)
        {
            foreach (var fileEntry in fileList.Items.Values)
            {
                if (this.Items.ContainsKey(fileEntry.FileCode))
                {
                    var msg = $"Duplicate FileCode({fileEntry.FileCode}). FileID({fileEntry.FileID})";
                    logger.Fatal(msg);
                    throw new Exception(msg);
                }
                else
                {
                    this.Items.Add(fileEntry.FileCode, fileEntry);
                }
            }
        }

        /// <summary>
        /// FileIDとHashを登録する。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <param name="fileCode">FileCode</param>
        /// <param name="languageType">言語区分</param>
        public void AddEntryByFileIdAndFileCode(string fileID, long fileCode, MieProduct.NLanguageType languageType)
        {
            MieFileEntry mieFileEntry = new MieFileEntry(fileID, fileCode, languageType);

            if (this.Items.ContainsKey(mieFileEntry.FileCode))
            {
                var msg = $"Duplicate fileID({fileCode}). Hash({mieFileEntry.FileCode})";
                logger.Warn(msg);
            }
            else
            {
                this.Items.Add(mieFileEntry.FileCode, mieFileEntry);
            }
        }

        /// <summary>
        /// FileEntryを追加する。
        /// </summary>
        /// <param name="fileEntry">FileEntry</param>
        public void AddEntry(MieFileEntry fileEntry)
        {
            if (this.Items.ContainsKey(fileEntry.FileCode))
            {
                var msg = $"Duplicate fileCode({fileEntry.FileCode})";
                logger.Fatal(msg);
                throw new Exception(msg);
            }
            else
            {
                this.Items.Add(fileEntry.FileCode, fileEntry);
            }
        }

        public MieFileEntry GetFileEntry(long fileCode)
        {
            if (this.Items.ContainsKey(fileCode))
            {
                var fileID = this.Items[fileCode];

                return fileID;
            }
            else
            {
                return null;
            }
        }

        public long GetHashByFileID(string fileID)
        {
            foreach (var fileIdPair in this.Items)
            {
                var hash = fileIdPair.Key;
                var fileEntry = fileIdPair.Value;
                if (fileEntry.FileID == fileID)
                {
                    return hash;
                }
            }

            return 0;
        }

        public MieFileList GetAdd(MieFileList oldFileList)
        {
            MieFileList result = new MieFileList();
            foreach (var currentEntry in this.Items.Values)
            {
                var oldEntry = oldFileList.GetFileEntry(currentEntry.FileCode);
                if (oldEntry == null)
                {
                    //// 追加
                    result.AddEntry(currentEntry);
                }
            }

            return result;
        }

        public MieFileList GetUpdate(MieFileList oldFileList)
        {
            MieFileList result = new MieFileList();
            foreach (var currentEntry in this.Items.Values)
            {
                var oldEntry = oldFileList.GetFileEntry(currentEntry.FileCode);
                if (oldEntry != null && oldEntry.LanguageType != currentEntry.LanguageType)
                {
                    //// 更新
                    result.AddEntry(currentEntry);
                }
            }

            return result;
        }

        public MieFileList GetDelete(MieFileList oldFileList)
        {
            MieFileList result = new MieFileList();
            foreach (var entry in oldFileList.Items.Values)
            {
                if (this.GetFileEntry(entry.FileCode) == null)
                {
                    //// 削除
                    result.AddEntry(entry);
                }
            }

            return result;
        }

        public string GetFileID(long fileCode)
        {
            if (this.Items.ContainsKey(fileCode))
            {
                return this.Items[fileCode].FileID;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            this.Items.Values
                .OrderBy(x => x.FileID)
                .ToList()
                .ForEach(x => buff.Append(x.ToString()));

            return buff.ToString();
        }
    }
}
