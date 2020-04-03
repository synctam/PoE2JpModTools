namespace MieFontLib.Models
{
    using System.IO;
    using System.Text;

    public class MieFontKerningPairType2 : MieFontKerningPairBase
    {
        public int FirstGlyph { get; set; } = 0;

        public GlyphValueRecord FirstGlyphAjustments { get; } = new GlyphValueRecord();

        public int SecondGlyph { get; set; } = 0;

        public GlyphValueRecord SecondGlyphAdjustments { get; } = new GlyphValueRecord();

        public float XOffset { get; set; } = 0.0f;

        public override MieFontKerningPairBase Read(BinaryReader br)
        {
            var kerningPair = new MieFontKerningPairType2();

            kerningPair.FirstGlyph = br.ReadInt32();
            kerningPair.FirstGlyphAjustments.Read(br);
            kerningPair.SecondGlyph = br.ReadInt32();
            kerningPair.SecondGlyphAdjustments.Read(br);

            kerningPair.XOffset = br.ReadSingle();

            return kerningPair;
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"KerningPair");

            buff.AppendLine($"{tab}FirstGlyph = {this.FirstGlyph}");
            buff.Append($"{this.FirstGlyphAjustments.ToString()}");
            buff.AppendLine($"{tab}FirstGlyph = {this.SecondGlyph}");
            buff.Append($"{this.SecondGlyphAdjustments.ToString()}");
            buff.AppendLine($"{tab}XOffset = {this.XOffset}");

            return buff.ToString();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(this.FirstGlyph);
            this.FirstGlyphAjustments.Write(bw);
            bw.Write(this.SecondGlyph);
            this.SecondGlyphAdjustments.Write(bw);

            bw.Write(this.XOffset);
        }

        public class GlyphValueRecord
        {
            public float XPlacement { get; set; } = 0.0f;

            public float YPlacement { get; set; } = 0.0f;

            public float XAdvance { get; set; } = 0.0f;

            public float YAdvance { get; set; } = 0.0f;

            public void Read(BinaryReader br)
            {
                this.XPlacement = br.ReadSingle();
                this.YPlacement = br.ReadSingle();
                this.XAdvance = br.ReadSingle();
                this.YAdvance = br.ReadSingle();
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(this.XPlacement);
                bw.Write(this.YPlacement);
                bw.Write(this.XAdvance);
                bw.Write(this.YAdvance);
            }
        }
    }
}
