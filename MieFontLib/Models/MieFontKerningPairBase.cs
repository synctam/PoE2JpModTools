namespace MieFontLib.Models
{
    using System.IO;

    public abstract class MieFontKerningPairBase
    {
        public abstract MieFontKerningPairBase Read(BinaryReader br);

        public abstract void Write(BinaryWriter bw);

        public abstract override string ToString();
    }
}
