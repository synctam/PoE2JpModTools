namespace MieFontLib
{
    using System.IO;
    using System.Text;

    public class MieFontHeader
    {
        private MieFontHeader() { }

        public int GameObjectFileID { get; private set; }

        public long GameObjectPathID { get; private set; }

        public bool GameObjectEnables { get; private set; }

        public int ScriptFileID { get; private set; }

        public long ScriptPathID { get; private set; }

        public string FileName { get; private set; }

        public int FileNameHashCode { get; private set; }

        public int MaterialFileID { get; private set; }

        public long MaterialPathID { get; private set; }

        public int MaterialHashCode { get; private set; }

        public int FontAssetType { get; private set; }

        public MieFontInfo FontInfo { get; set; } = null;

        public int AtlasFileID { get; set; }

        public long AtlasPathID { get; set; }

        /// <summary>
        /// Reade Header.
        /// </summary>
        /// <param name="reader">reader</param>
        /// <param name="formatType">TMPのフォーマットタイプ。</param>
        /// <returns>Header</returns>
        public static MieFontHeader Read(BinaryReader reader, MieFont.NFormatType formatType)
        {
            var result = new MieFontHeader();

            result.GameObjectFileID = reader.ReadInt32();
            result.GameObjectPathID = reader.ReadInt64();
            result.GameObjectEnables = reader.ReadBoolean();
            MieFont.ReadPadding(reader);

            result.ScriptFileID = reader.ReadInt32();
            result.ScriptPathID = reader.ReadInt64();

            result.FileName = MieFont.ReadString(reader);

            result.FileNameHashCode = reader.ReadInt32();
            result.MaterialFileID = reader.ReadInt32();
            result.MaterialPathID = reader.ReadInt64();
            result.MaterialHashCode = reader.ReadInt32();
            result.FontAssetType = reader.ReadInt32();

            result.FontInfo = new MieFontInfo(formatType);
            result.FontInfo.Read(reader, formatType);

            result.AtlasFileID = reader.ReadInt32();
            result.AtlasPathID = reader.ReadInt64();

            return result;
        }

        public void Write(BinaryWriter writer, MieFont.NFormatType formatType)
        {
            //// ToDo: ファイル名や内部名の長さを求める処理を追加。
            writer.Write(this.GameObjectFileID);
            writer.Write(this.GameObjectPathID);
            writer.Write(this.GameObjectEnables);
            MieFont.WritePadding(writer);

            writer.Write(this.ScriptFileID);
            writer.Write(this.ScriptPathID);

            MieFont.WriteString(writer, this.FileName);

            writer.Write(this.FileNameHashCode);
            writer.Write(this.MaterialFileID);
            writer.Write(this.MaterialPathID);
            writer.Write(this.MaterialHashCode);
            writer.Write(this.FontAssetType);

            this.FontInfo.Write(writer, formatType);

            writer.Write(this.AtlasFileID);
            writer.Write(this.AtlasPathID);
        }

        public void Convert(
            MieFontHeader jp,
            MieFont.NFormatType formatType,
            bool forceAdjustAscender)
        {
            ////this.GameObjectFileID = jp.GameObjectFileID;
            ////this.GameObjectPathID = jp.GameObjectPathID;
            ////this.GameObjectEnables = jp.GameObjectEnables;

            ////this.ScriptFileID = jp.ScriptFileID;
            ////this.ScriptPathID = jp.ScriptPathID;
            ////this.FileNameLength = jp.FileNameLength;
            ////this.FileName = jp.FileName;

            ////this.FileNameHashCode = jp.FileNameHashCode;
            ////this.MaterialFileID = jp.MaterialFileID;
            ////this.MaterialPathID = jp.MaterialPathID;
            ////this.MaterialHashCode = jp.MaterialHashCode;
            ////this.FontAssetType = jp.FontAssetType;
            ////this.LengthOfFontName = jp.LengthOfFontName;

            ////this.InternalFileName = jp.InternalFileName;

            this.FontInfo.Convert(jp.FontInfo, formatType, forceAdjustAscender);

            ////this.AtlasFileID = jp.AtlasFileID;
            ////this.AtlasPathID = jp.AtlasPathID;
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"m_GameObject");
            buff.AppendLine($"{tab}FileID = {this.GameObjectFileID}");
            buff.AppendLine($"{tab}PathID = {this.GameObjectPathID}");
            buff.AppendLine($"Enables = {this.GameObjectEnables}");
            buff.AppendLine($"m_Script");
            buff.AppendLine($"{tab}FileID = {this.ScriptFileID}");
            buff.AppendLine($"{tab}PathID = {this.ScriptPathID}");
            buff.AppendLine($"FileName = {this.FileName}");
            buff.AppendLine($"HashCode = {this.FileNameHashCode}");
            buff.AppendLine($"Material = FileID: {this.MaterialFileID} PathID: {this.MaterialPathID}");
            buff.AppendLine($"HashCode = {this.MaterialHashCode}");
            buff.AppendLine($"FontAssetType = {this.FontAssetType}");

            buff.Append(this.FontInfo.ToString());

            buff.AppendLine($"Atlas = FileID: {this.AtlasFileID} PathID: {this.AtlasPathID}");

            return buff.ToString();
        }
    }
}
