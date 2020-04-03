namespace MieFontLib.Models
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class MieFontFallbackFontAssets
    {
        private MieFontFallbackFontAssets() { }

        public List<MieFontAsset> Items { get; } = new List<MieFontAsset>();

        public static MieFontFallbackFontAssets Read(BinaryReader br)
        {
            var fallbackFontAssets = new MieFontFallbackFontAssets();

            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var fontAsset = MieFontAsset.Read(br);
                fallbackFontAssets.Items.Add(fontAsset);
            }

            return fallbackFontAssets;
        }

        public void Write(BinaryWriter bw)
        {
            int count = this.Items.Count;
            bw.Write(count);
            foreach (var entry in this.Items)
            {
                entry.Write(bw);
            }
        }

        public override string ToString()
        {
            var tab = "\t\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FallbackFontAssets");
            buff.AppendLine($"{tab}size = {this.Items.Count}");
            int count = 0;
            foreach (var entry in this.Items)
            {
                buff.AppendLine($"{tab}[{count}]");
                buff.Append(entry.ToString());
                count++;
            }

            return buff.ToString();
        }
    }
}
