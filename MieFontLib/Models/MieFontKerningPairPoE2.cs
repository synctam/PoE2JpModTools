namespace MieFontLib.Models
{
    using System.IO;
    using System.Text;

    public class MieFontKerningPairPoE2 : MieFontKerningPairBase
    {
        public int AsciiLeft { get; private set; } = 0;

        public int AsciiRight { get; private set; } = 0;

        public float XadvanceOffset { get; private set; } = 0.0f;

        public override MieFontKerningPairBase Read(BinaryReader br)
        {
            var kerningPair = new MieFontKerningPairPoE2();

            kerningPair.AsciiLeft = br.ReadInt32();
            kerningPair.AsciiRight = br.ReadInt32();
            kerningPair.XadvanceOffset = br.ReadSingle();

            return kerningPair;
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(this.AsciiLeft);
            bw.Write(this.AsciiRight);
            bw.Write(this.XadvanceOffset);
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"KerningPair");
            buff.AppendLine($"{tab}AsciiLeft = {this.AsciiLeft}");
            buff.AppendLine($"{tab}AsciiRight = {this.AsciiRight}");
            buff.AppendLine($"{tab}XadvanceOffset = {this.XadvanceOffset}");

            return buff.ToString();
        }
    }
}
