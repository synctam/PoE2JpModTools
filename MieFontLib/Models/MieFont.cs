namespace MieFontLib
{
    using System.IO;
    using System.Text;
    using MieCommon;

    public class MieFont
    {
        public enum NFormatType
        {
            Unknown = 0,
            Type1 = 1,
            Type2 = 2,
            Type3 = 3,
            Type4 = 4,
            Type5 = 5,
            PoE2 = 10000,
        }

        public MieFontHeader Header { get; set; }

        public MieFontEntries FontEntries { get; set; }

        public MieFontFooter Footer { get; set; }

        public static string ReadString(BinaryReader reader)
        {
            string result = string.Empty;

            var length = reader.ReadInt32();
            var stringArray = reader.ReadBytes(length);
            result = Encoding.UTF8.GetString(stringArray);
            //// Padding
            ReadPadding(reader);

            return result;
        }

        public static void WriteString(BinaryWriter writer, string text)
        {
            writer.Write(Encoding.UTF8.GetByteCount(text));
            byte[] arrayOfFileName = Encoding.UTF8.GetBytes(text);
            writer.Write(arrayOfFileName);
            //// Padding
            WritePadding(writer);
        }

        /// <summary>
        /// Padding. streamのポジションにより、ワード境界に合わせるため、空読みする。
        /// </summary>
        /// <param name="reader">stream</param>
        public static void ReadPadding(BinaryReader reader)
        {
            var pos = reader.BaseStream.Position;
            var mod = pos % 4;
            if (mod == 0)
            {
                return;
            }
            else
            {
                var paddingCount1 = (int)(4 - mod);
                var padding1 = reader.ReadBytes(paddingCount1);
            }
        }

        /// <summary>
        /// Padding. ワード境界に合わせるため、ワード境界までゼロを出力する。
        /// </summary>
        /// <param name="writer">stream</param>
        public static void WritePadding(BinaryWriter writer)
        {
            var pos = writer.BaseStream.Position;
            var mod = pos % 4;
            if (mod == 0)
            {
                return;
            }
            else
            {
                var paddingCount = (int)(4 - mod);
                for (var i = 0; i < paddingCount; i++)
                {
                    writer.Write((byte)0);
                }
            }
        }

        public void Load(string path, NFormatType formatType)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(path), Encoding.UTF8))
            {
                this.Header = MieFontHeader.Read(br, formatType);
                this.FontEntries = MieFontEntries.Read(br);
                this.Footer = MieFontFooter.Read(br, formatType);
            }
        }

        public void Save(string path, NFormatType formatType)
        {
            var folder = Path.GetDirectoryName(path);
            MieCommonUtils.SafeCreateDirectory(folder);
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path), Encoding.UTF8))
            {
                this.Header.Write(writer, formatType);
                this.FontEntries.Write(writer);
                this.Footer.Write(writer, formatType);
            }
        }

        public void Dump(string path, bool isDetail)
        {
            var dump = this.ToString(isDetail);
            File.WriteAllText(path, dump, Encoding.UTF8);
        }

        public string ToString(bool isDetail)
        {
            StringBuilder buff = new StringBuilder();

            buff.Append(this.Header.ToString());
            buff.Append(this.FontEntries.ToString(isDetail));
            buff.Append(this.Footer.ToString());

            return buff.ToString();
        }
    }
}
