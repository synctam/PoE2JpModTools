namespace MieFontLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class MieFontKerningTable
    {
        private MieFontKerningTable() { }

        public List<MieFontKerningPairBase> Items { get; } = new List<MieFontKerningPairBase>();

        public static MieFontKerningTable Read(BinaryReader br, MieFont.NFormatType type)
        {
            var kerningTable = new MieFontKerningTable();

            var count = br.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                switch (type)
                {
                    case MieFont.NFormatType.Unknown:
                        throw new NotImplementedException();
                    case MieFont.NFormatType.Type1:
                        throw new NotImplementedException();
                    case MieFont.NFormatType.Type2:
                        var kerningPair2 = new MieFontKerningPairType2();
                        kerningPair2.Read(br);
                        kerningTable.Items.Add(kerningPair2);

                        break;
                    case MieFont.NFormatType.Type3:
                        throw new NotImplementedException();
                    case MieFont.NFormatType.Type4:
                        throw new NotImplementedException();
                    case MieFont.NFormatType.Type5:
                        throw new NotImplementedException();
                    case MieFont.NFormatType.PoE2:
                        var kerningPair1 = new MieFontKerningPairType1();
                        kerningPair1.Read(br);
                        kerningTable.Items.Add(kerningPair1);

                        break;
                    default:
                        throw new Exception($"Unknown Format type({type})");
                }
            }

            return kerningTable;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.Items.Count);
            foreach (var entry in this.Items)
            {
                entry.Write(bw);
            }
        }

        public override string ToString()
        {
            var tab = "\t";
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"KerningTable");
            buff.AppendLine($"{tab}KerningPairs");
            buff.AppendLine($"{tab}{tab}size = {this.Items.Count}");
            foreach (var entry in this.Items)
            {
                buff.Append(entry.ToString());
            }

            return buff.ToString();
        }
    }
}
