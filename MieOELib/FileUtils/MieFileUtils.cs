namespace MieOELib.FileUtils
{
    using System.Text;
    using Org.BouncyCastle.Crypto.Macs;
    using Org.BouncyCastle.Crypto.Parameters;

    public class MieFileUtils
    {
        /// <summary>
        /// テキストのハッシュを64ビット値で返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>テキストのハッシュ(64ビット)</returns>
        public static long ComputeSipHash(string text)
        {
            //// 16バイトの鍵を生成する。
            byte[] key = Encoding.ASCII.GetBytes("dWV9#LeZGa8eu7Hx");

            SipHash mac = new SipHash();
            mac.Init(new KeyParameter(key));

            //// テキストのハッシュを算出する。
            byte[] arrayOfText = Encoding.UTF8.GetBytes(text);
            mac.BlockUpdate(arrayOfText, 0, arrayOfText.Length);
            var result = mac.DoFinal();

            return result;
        }

        /// <summary>
        /// 付加情報と言語情報では FileID の形式が異なる。
        /// 付加情報の FileID を言語情報の書式に合わせて変換したものを返す。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <returns>変換後のFileID</returns>
        public static string ConvertFileIDToCommon(string fileID)
        {
            fileID = fileID.Replace("/", "\\");

            return fileID;
        }

        public static string ConvertFileIDToDesign(string fileID)
        {
            fileID = fileID.Replace("\\", "/");

            return fileID;
        }

        public static string ConvertFileIDToLanguage(string fileID)
        {
            //// 言語情報の場合は変換の必要はない。

            return fileID;
        }
    }
}
