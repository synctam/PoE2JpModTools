namespace MieFontLib.Models
{
    using System.IO;
    using System.Text;

    public class MieFontCreationSetting
    {
        private MieFontCreationSetting() { }

        public string FontSourcePath { get; private set; } = string.Empty;

        public int FontSizeMode { get; private set; }

        public int FontSize { get; private set; }

        public int FontPadding { get; private set; }

        public int FontPackingMode { get; private set; }

        public int FontAtlasWidth { get; private set; }

        public int FontAtlasHeight { get; private set; }

        public int FontCharacterSet { get; private set; }

        public int FontStyle { get; private set; }

        public float FontStyleModifier { get; private set; }

        public int FontRenderMode { get; private set; }

        public bool FontKerning { get; private set; }

        public static MieFontCreationSetting Read(BinaryReader br)
        {
            var setting = new MieFontCreationSetting();

            setting.FontSourcePath = MieFont.ReadString(br);
            setting.FontSizeMode = br.ReadInt32();
            setting.FontSize = br.ReadInt32();
            setting.FontPadding = br.ReadInt32();
            setting.FontPackingMode = br.ReadInt32();
            setting.FontAtlasWidth = br.ReadInt32();
            setting.FontAtlasHeight = br.ReadInt32();
            setting.FontCharacterSet = br.ReadInt32();
            setting.FontStyle = br.ReadInt32();
            setting.FontStyleModifier = br.ReadSingle();
            setting.FontRenderMode = br.ReadInt32();
            setting.FontKerning = br.ReadBoolean();
            MieFont.ReadPadding(br);

            return setting;
        }

        public void Write(BinaryWriter writer)
        {
            MieFont.WriteString(writer, this.FontSourcePath);
            writer.Write(this.FontSizeMode);
            writer.Write(this.FontSize);
            writer.Write(this.FontPadding);
            writer.Write(this.FontPackingMode);
            writer.Write(this.FontAtlasWidth);
            writer.Write(this.FontAtlasHeight);
            writer.Write(this.FontCharacterSet);
            writer.Write(this.FontStyle);
            writer.Write(this.FontStyleModifier);
            writer.Write(this.FontRenderMode);
            writer.Write(this.FontKerning);
            MieFont.WritePadding(writer);
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FontCreationSetting");
            buff.AppendLine($"{tab}FontSourcePath = {this.FontSourcePath}");
            buff.AppendLine($"{tab}FontSizeMode = {this.FontSizeMode}");
            buff.AppendLine($"{tab}FontSize = {this.FontSize}");
            buff.AppendLine($"{tab}FontPadding = {this.FontPadding}");
            buff.AppendLine($"{tab}FontPackingMode = {this.FontPackingMode}");
            buff.AppendLine($"{tab}FontAtlasWidth = {this.FontAtlasWidth}");
            buff.AppendLine($"{tab}FontAtlasHeight = {this.FontAtlasHeight}");
            buff.AppendLine($"{tab}FontCharacterSet = {this.FontCharacterSet}");
            buff.AppendLine($"{tab}FontStyle = {this.FontStyle}");
            buff.AppendLine($"{tab}FontStyleModifier = {this.FontStyleModifier}");
            buff.AppendLine($"{tab}FontRenderMode = {this.FontRenderMode}");
            buff.AppendLine($"{tab}FontKerning = {this.FontKerning}");

            return buff.ToString();
        }
    }
}
