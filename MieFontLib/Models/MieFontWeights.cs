namespace MieFontLib
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class MieFontWeights
    {
        private MieFontWeights() { }

        public int NumOfFontWeights { get; private set; }

        public IList<MieFontWeightEntry> Items { get; } = new List<MieFontWeightEntry>();

        public static MieFontWeights Read(BinaryReader reader)
        {
            MieFontWeights result = new MieFontWeights();

            result.NumOfFontWeights = reader.ReadInt32();
            for (int i = 0; i < result.NumOfFontWeights; i++)
            {
                MieFontWeightEntry entry = MieFontWeightEntry.Read(reader);
                result.Items.Add(entry);
            }

            return result;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.NumOfFontWeights);
            foreach (var entry in this.Items)
            {
                entry.Write(writer);
            }
        }

        public void Convert(MieFontWeights en)
        {
            for (var i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].ItalicTypeFaceFileID = en.Items[i].ItalicTypeFaceFileID;
                this.Items[i].ItalicTypeFacePathID = en.Items[i].ItalicTypeFacePathID;
                this.Items[i].RegularTypeFaceFildID = en.Items[i].RegularTypeFaceFildID;
                this.Items[i].RegularTypeFacePathID = en.Items[i].RegularTypeFacePathID;
            }
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FontWeights");
            buff.AppendLine($"{tab}size = {this.Items.Count}");
            int count = 0;
            foreach (var entry in this.Items)
            {
                buff.AppendLine($"{tab}{tab}[{count}]");
                buff.Append(entry.ToString());
            }

            return buff.ToString();
        }
    }
}
