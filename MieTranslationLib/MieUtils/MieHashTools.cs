namespace MieTranslationLib.MieUtils
{
    using System;
    using System.Text;
    using Force.Crc32;

    public class MieHashTools
    {
        private const string Salt = "PoE2";
        private const string Alphabet = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>
        /// 小文字化したFileIDのCRC32を返す。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <returns>CRC32</returns>
        public static long ComputeFileID(string fileID)
        {
            var result = ComputeHashInt(MieStringUtils.NormalizedFileID(fileID));

            return result;
        }

        /// <summary>
        /// FileIDとNodeIDからReferenceIDを算出する。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <param name="nodeID">NodeID</param>
        /// <returns>ReferenceID</returns>
        public static long ComputeReferenceID(string fileID, int nodeID)
        {
            fileID = MieStringUtils.NormalizedFileID(fileID);
            var reference = $"{fileID}.{nodeID}";
            var result = ComputeHashInt(reference);

            return result;
        }

        /// <summary>
        /// CRC32のハッシュからからハッシュテキストを返す。
        /// </summary>
        /// <param name="hash">CRC32</param>
        /// <returns>ハッシュテキスト</returns>
        public static string ComputeHashIds(long hash)
        {
            //// hashidsでhashを算出する。
            HashidsNet.Hashids hashids = new HashidsNet.Hashids(Salt, 0, Alphabet);

            var result = hashids.EncodeLong(hash);

            return result;
        }

        /// <summary>
        /// テキストからHashを返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>Hash</returns>
        private static string ComputeHashX(string text)
        {
            string result = string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            //// テキストのCRC32を計算する。
            long crc = Crc32Algorithm.Compute(bytes);

            //// Encodeの入力は正の整数が必要なため絶対値を取る。
            crc = Math.Abs(crc);

            //// hashidsでhashを算出する。
            HashidsNet.Hashids hashids = new HashidsNet.Hashids(Salt, 0, Alphabet);

            result = hashids.EncodeLong(crc);

            return result;
        }

        private static long ComputeHashInt(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            //// テキストのCRC32を計算する。
            long crc = Crc32Algorithm.Compute(bytes);

            return crc;
        }
    }
}
