namespace MieFontLib
{
    using System.IO;
    using System.Text;

    public class MieFontInfo
    {
        private readonly MieFont.NFormatType formatType = MieFont.NFormatType.Unknown;

        public MieFontInfo(MieFont.NFormatType formatType)
        {
            this.formatType = formatType;
        }

        public string Name { get; set; }

        public float PointSize { get; set; }

        public float PointScale { get; set; }

        public int CharacterCount { get; set; }

        public float LineHeight { get; set; }

        public float Baseline { get; set; }

        public float Ascender { get; set; }

        public float CapHeight { get; set; }

        public float Descender { get; set; }

        public float CenterLine { get; set; }

        public float SuperScriptOffset { get; set; }

        public float SubScriptOffset { get; set; }

        public float SubSize { get; set; }

        /// <summary>
        /// 自作フォントにのみ存在する
        /// </summary>
        public float StrikeThrough { get; set; }

        /// <summary>
        /// 自作フォントにのみ存在する
        /// </summary>
        public float StrikeThroughThickness { get; set; }

        public float UnderlineUnderline { get; set; }

        public float UnderlineUnderlineThickness { get; set; }

        public float TabWidth { get; set; }

        public float Padding { get; set; }

        public float AtlasWidth { get; set; }

        public float AtlasHeight { get; set; }

        public void Read(BinaryReader reader, MieFont.NFormatType formatType)
        {
            this.Name = MieFont.ReadString(reader);
            this.PointSize = reader.ReadSingle();
            this.PointScale = reader.ReadSingle();
            this.CharacterCount = reader.ReadInt32();
            this.LineHeight = reader.ReadSingle();
            this.Baseline = reader.ReadSingle();
            this.Ascender = reader.ReadSingle();
            this.CapHeight = reader.ReadSingle();
            this.Descender = reader.ReadSingle();
            this.CenterLine = reader.ReadSingle();
            this.SuperScriptOffset = reader.ReadSingle();
            this.SubScriptOffset = reader.ReadSingle();
            this.SubSize = reader.ReadSingle();
            this.UnderlineUnderline = reader.ReadSingle();
            this.UnderlineUnderlineThickness = reader.ReadSingle();

            if (formatType != MieFont.NFormatType.PoE2)
            {
                //// オリジナルと自作フォントのレイアウトが会わない！
                //// 自作分はここに次のに項目が追加されている。
                this.StrikeThrough = reader.ReadSingle();
                this.StrikeThroughThickness = reader.ReadSingle();
            }

            this.TabWidth = reader.ReadSingle();
            this.Padding = reader.ReadSingle();
            this.AtlasWidth = reader.ReadSingle();
            this.AtlasHeight = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer, MieFont.NFormatType formatType)
        {
            MieFont.WriteString(writer, this.Name);

            writer.Write(this.PointSize);
            writer.Write(this.PointScale);
            writer.Write(this.CharacterCount);
            writer.Write(this.LineHeight);
            writer.Write(this.Baseline);
            writer.Write(this.Ascender);
            writer.Write(this.CapHeight);
            writer.Write(this.Descender);
            writer.Write(this.CenterLine);
            writer.Write(this.SuperScriptOffset);
            writer.Write(this.SubScriptOffset);
            writer.Write(this.SubSize);
            writer.Write(this.UnderlineUnderline);
            writer.Write(this.UnderlineUnderlineThickness);

            if (formatType != MieFont.NFormatType.PoE2)
            {
                //// オリジナルと自作フォントのレイアウトが会わない！
                //// 自作分はここに次のに項目が追加されている。
                writer.Write(this.StrikeThrough);
                writer.Write(this.StrikeThroughThickness);
            }

            writer.Write(this.TabWidth);
            writer.Write(this.Padding);
            writer.Write(this.AtlasWidth);
            writer.Write(this.AtlasHeight);
        }

        public void Convert(
            MieFontInfo jpFontInfo,
            MieFont.NFormatType formatType,
            bool forceAdjustAscender)
        {
            ////this.Name = jpFontInfo.Name;
            this.PointSize = jpFontInfo.PointSize;
            this.PointScale = jpFontInfo.PointScale;
            this.CharacterCount = jpFontInfo.CharacterCount;
            this.LineHeight = jpFontInfo.LineHeight;

            //// フォント・ボディの高さ
            this.Baseline = jpFontInfo.Baseline;
            if (forceAdjustAscender)
            {
                //// Ascenderの値を強制的に調整する。
                //// Ascenderの値をPointSizeにする。
                this.Ascender = jpFontInfo.PointSize;
            }
            else
            {
                //// フォントの設定値をそのまま利用する。
                this.Ascender = jpFontInfo.Ascender;
            }

            this.CapHeight = jpFontInfo.CapHeight;
            this.Descender = jpFontInfo.Descender;
            this.CenterLine = jpFontInfo.CenterLine;
            this.SuperScriptOffset = jpFontInfo.SuperScriptOffset;
            this.SubScriptOffset = jpFontInfo.SubScriptOffset;
            this.SubSize = jpFontInfo.SubSize;
            if (formatType == MieFont.NFormatType.PoE2)
            {
                //// オリジナルと自作フォントのレイアウトが会わない！
                //// 自作分はここに次のに項目が追加されている。
                this.StrikeThrough = jpFontInfo.StrikeThrough;
                this.StrikeThroughThickness = jpFontInfo.StrikeThroughThickness;
            }

            this.UnderlineUnderline = jpFontInfo.UnderlineUnderline;
            this.UnderlineUnderlineThickness = jpFontInfo.UnderlineUnderlineThickness;
            this.TabWidth = jpFontInfo.TabWidth;
            this.Padding = jpFontInfo.Padding;
            this.AtlasWidth = jpFontInfo.AtlasWidth;
            this.AtlasHeight = jpFontInfo.AtlasHeight;
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FontInfo");
            buff.AppendLine($"{tab}Name = {this.Name}");
            buff.AppendLine($"{tab}PointSize = {this.PointSize}");
            buff.AppendLine($"{tab}PointScale = {this.PointScale}");
            buff.AppendLine($"{tab}CharacterCount = {this.CharacterCount}");
            buff.AppendLine($"{tab}LineHeight = {this.LineHeight}");
            buff.AppendLine($"{tab}Baseline = {this.Baseline}");
            buff.AppendLine($"{tab}Ascender = {this.Ascender}");
            buff.AppendLine($"{tab}CapHeight = {this.CapHeight}");
            buff.AppendLine($"{tab}Descender = {this.Descender}");
            buff.AppendLine($"{tab}CenterLine = {this.CenterLine}");
            buff.AppendLine($"{tab}SuperScriptOffset = {this.SuperScriptOffset}");
            buff.AppendLine($"{tab}SubScriptOffset = {this.SubScriptOffset}");
            buff.AppendLine($"{tab}SubSize = {this.SubSize}");

            if (this.formatType != MieFont.NFormatType.PoE2)
            {
                buff.AppendLine($"{tab}StrikeThrough = {this.StrikeThrough}");
                buff.AppendLine($"{tab}StrikeThroughThickness = {this.StrikeThroughThickness}");
            }

            buff.AppendLine($"{tab}UnderlineUnderline = {this.UnderlineUnderline}");
            buff.AppendLine($"{tab}UnderlineUnderlineThickness = {this.UnderlineUnderlineThickness}");
            buff.AppendLine($"{tab}TabWidth = {this.TabWidth}");
            buff.AppendLine($"{tab}Padding = {this.Padding}");
            buff.AppendLine($"{tab}AtlasWidth = {this.AtlasWidth}");
            buff.AppendLine($"{tab}AtlasHeight = {this.AtlasHeight}");

            return buff.ToString();
        }
    }
}
