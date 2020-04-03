namespace MieTranslationLib.MieUtils
{
    public class MieStringUtils
    {
        /// <summary>
        /// FileIDを正規化する。
        /// ・小文字化
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <returns>正規化したFileID</returns>
        public static string NormalizedFileID(string fileID)
        {
            var result = fileID.ToLower();

            return result;
        }
    }
}
