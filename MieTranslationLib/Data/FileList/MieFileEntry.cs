namespace MieTranslationLib.Data.FileList
{
    using System;
    using System.Text;
    using MieTranslationLib.Product;
    using NLog;

    public class MieFileEntry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ。
        /// 新規作成用
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <param name="fileCode">FileCode</param>
        /// <param name="languageType">LanguageType</param>
        public MieFileEntry(string fileID, long fileCode, MieProduct.NLanguageType languageType)
        {
            this.FileCode = fileCode;
            this.FileID = fileID;
            this.LanguageType = languageType;
            this.UpdateAt = DateTime.UtcNow;
        }

        /// <summary>
        /// コンストラクタ
        /// 既存データの復元用
        /// </summary>
        /// <param name="fileCode">FileCode</param>
        /// <param name="fileID">FileID</param>
        /// <param name="languageType">言語区分</param>
        /// <param name="createdAt">作成日</param>
        /// <param name="updatedAt">更新日</param>
        public MieFileEntry(long fileCode, string fileID, MieProduct.NLanguageType languageType, long updatedAt)
        {
            this.FileCode = fileCode;
            this.FileID = fileID;
            this.LanguageType = languageType;
            this.UpdateAt = new DateTime(updatedAt);
        }

        private MieFileEntry() { }

        public long FileCode { get; } = 0;

        public string FileID { get; } = string.Empty;

        public string DisplayFileID
        {
            get
            {
                return this.FileID;
            }
        }

        public MieProduct.NLanguageType LanguageType { get; } = MieProduct.NLanguageType.Conversations;

        public DateTime UpdateAt { get; set; }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FileCode({this.FileCode}) FileID({this.FileID}) Updated({this.UpdateAt.ToLocalTime()})");

            return buff.ToString();
        }
    }
}
