namespace MieFontLib
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class MieFontEntries
    {
        private MieFontEntries() { }

        public int Count { get; private set; }

        public List<MieFontEntry> Items { get; } = new List<MieFontEntry>();

        public static MieFontEntries Read(BinaryReader br)
        {
            MieFontEntries result = new MieFontEntries();

            result.Count = br.ReadInt32();
            for (int i = 0; i < result.Count; i++)
            {
                MieFontEntry entry = MieFontEntry.Read(br);
                result.Items.Add(entry);
            }

            return result;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.Count);
            foreach (var entry in this.Items)
            {
                entry.Write(writer);
            }
        }

        public string ToString(bool isDetail)
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"glyphInfoList");
            buff.AppendLine($"{tab}size = {this.Items.Count}");
            int count = 0;
            bool printed = false;
            foreach (var entry in this.Items)
            {
                if (isDetail)
                {
                    buff.AppendLine($"{tab}{tab}[{count}]");
                    buff.Append(entry.ToString());
                }
                else
                {
                    if (count < 5)
                    {
                        buff.AppendLine($"{tab}{tab}[{count}]");
                        buff.Append(entry.ToString());
                    }
                    else if (count > this.Items.Count - 5)
                    {
                        if (!printed)
                        {
                            //// 詳細表示の区切りを表示する
                            buff.AppendLine();
                            buff.AppendLine("\t\t...");
                            buff.AppendLine();
                            printed = true;
                        }

                        buff.AppendLine($"{tab}{tab}[{count}]");
                        buff.Append(entry.ToString());
                    }
                }

                count++;
            }

            return buff.ToString();
        }
    }
}
