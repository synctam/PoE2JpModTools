namespace MieFontLib
{
    using System.IO;
    using System.Text;

    public class MieFontWeightEntry
    {
        private MieFontWeightEntry() { }

        public int RegularTypeFaceFildID { get; set; }

        public long RegularTypeFacePathID { get; set; }

        public int ItalicTypeFaceFileID { get; set; }

        public long ItalicTypeFacePathID { get; set; }

        public static MieFontWeightEntry Read(BinaryReader reader)
        {
            MieFontWeightEntry result = new MieFontWeightEntry();

            result.RegularTypeFaceFildID = reader.ReadInt32();
            result.RegularTypeFacePathID = reader.ReadInt64();
            result.ItalicTypeFaceFileID = reader.ReadInt32();
            result.ItalicTypeFacePathID = reader.ReadInt64();

            return result;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.RegularTypeFaceFildID);
            writer.Write(this.RegularTypeFacePathID);
            writer.Write(this.ItalicTypeFaceFileID);
            writer.Write(this.ItalicTypeFacePathID);
        }

        public override string ToString()
        {
            var tab = "\t\t\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"{tab}RegularTypeFace = FildID: {this.RegularTypeFaceFildID}, PathID: {this.RegularTypeFacePathID}");
            buff.AppendLine($"{tab}ItalicTypeFace = FildID: {this.ItalicTypeFaceFileID}, PathID: {this.ItalicTypeFacePathID}");

            return buff.ToString();
        }
    }
}
