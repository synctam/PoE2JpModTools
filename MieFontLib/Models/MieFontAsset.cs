namespace MieFontLib.Models
{
    using System.IO;
    using System.Text;

    public class MieFontAsset
    {
        public MieFontAsset(int fileID, long pathID)
        {
            this.FileID = fileID;
            this.PathID = pathID;
        }

        public int FileID { get; private set; } = 0;

        public long PathID { get; private set; } = 0;

        public static MieFontAsset Read(BinaryReader br)
        {
            var fileID = br.ReadInt32();
            var pathID = br.ReadInt64();
            var fontAsset = new MieFontAsset(fileID, pathID);

            return fontAsset;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.FileID);
            bw.Write(this.PathID);
        }

        public override string ToString()
        {
            var tab = "\t\t\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"{tab}FileID: {this.FileID}, PathID: {this.PathID}");

            return buff.ToString();
        }
    }
}