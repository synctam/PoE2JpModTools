namespace MieTranslationLib.Data.Language
{
    public interface IMieLanguageFileAttribute
    {
        long FileCode { get; }

        /// <summary>
        /// 表示名。未設定の場合はFileIDを返す。
        /// </summary>
        string DisplayName { get; }

        string Comment { get; }

        void SetComment(string text);

        void SetDisplayName(string text);
    }
}