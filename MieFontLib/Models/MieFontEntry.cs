namespace MieFontLib
{
    using System.IO;
    using System.Text;

    public class MieFontEntry
    {
        private MieFontEntry() { }

        public int CharacterID { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float OffsetX { get; set; }

        public float OffsetY { get; set; }

        public float AdvanceX { get; set; }

        public float Scale { get; set; }

        public static MieFontEntry Read(BinaryReader br)
        {
            MieFontEntry result = new MieFontEntry();
            result.CharacterID = br.ReadInt32();
            result.PosX = br.ReadSingle();
            result.PosY = br.ReadSingle();
            result.Width = br.ReadSingle();
            result.Height = br.ReadSingle();
            result.OffsetX = br.ReadSingle();
            result.OffsetY = br.ReadSingle();
            result.AdvanceX = br.ReadSingle();
            result.Scale = br.ReadSingle();

            return result;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.CharacterID);
            writer.Write(this.PosX);
            writer.Write(this.PosY);
            writer.Write(this.Width);
            writer.Write(this.Height);
            writer.Write(this.OffsetX);
            writer.Write(this.OffsetY);
            writer.Write(this.AdvanceX);
            writer.Write(this.Scale);
        }

        public override string ToString()
        {
            var tab2 = "\t\t\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"{tab2}CharacterID = {this.CharacterID}");
            buff.AppendLine($"{tab2}PosX = {this.PosX}");
            buff.AppendLine($"{tab2}PosY = {this.PosY}");
            buff.AppendLine($"{tab2}Width = {this.Width}");
            buff.AppendLine($"{tab2}Height = {this.Height}");
            buff.AppendLine($"{tab2}OffsetX = {this.OffsetX}");
            buff.AppendLine($"{tab2}OffsetY = {this.OffsetY}");
            buff.AppendLine($"{tab2}AdvanceX = {this.AdvanceX}");
            buff.AppendLine($"{tab2}Scale = {this.Scale}");

            return buff.ToString();
        }
    }
}
