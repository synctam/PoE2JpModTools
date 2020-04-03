namespace MieTranslationLib.Data.Language
{
    /// <summary>
    /// ファイル属性
    /// </summary>
    public class MieLanguageFileAttribute : IMieLanguageFileAttribute
    {
        /// <summary>
        /// 変更有無
        /// </summary>
        private bool isModified = false;
        private string displayName = string.Empty;
        private string comment = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileCode">FileID</param>
        /// <param name="displayName">DisplayName</param>
        public MieLanguageFileAttribute(long fileCode, string displayName)
        {
            this.FileCode = fileCode;
            this.displayName = displayName;
        }

        /// <summary>
        /// FileCode
        /// </summary>
        public long FileCode { get; } = 0;

        /// <summary>
        /// 表示名。未設定の場合はFileIDを返す。
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.DisplayName))
                {
                    return string.Empty;
                }
                else
                {
                    return this.displayName;
                }
            }
        }

        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get { return this.comment; } }

        /// <summary>
        /// コメントを設定する。
        /// </summary>
        /// <param name="comment">コメント</param>
        public void SetComment(string comment)
        {
            if (this.comment != comment)
            {
                this.comment = comment;
                this.isModified = true;
            }
        }

        /// <summary>
        /// 表示名の設定。
        /// </summary>
        /// <param name="displayName">表示名</param>
        public void SetDisplayName(string displayName)
        {
            if (this.displayName != displayName)
            {
                this.displayName = displayName;
                this.isModified = true;
            }
        }

        /// <summary>
        /// ファイル属性のセーブ
        /// </summary>
        public void Save()
        {
            if (this.isModified)
            {
                //// Todo: ファイル属性のセーブ処理
            }
        }
    }
}
