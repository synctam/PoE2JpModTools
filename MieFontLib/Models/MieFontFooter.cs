namespace MieFontLib
{
    using System;
    using System.IO;
    using System.Text;
    using MieFontLib.Models;

    public class MieFontFooter
    {
        private MieFontFooter() { }

        public MieFontKerningTable KerningTable { get; set; } = null;

        public MieFontKerningPairBase KerningPairBase { get; set; } = null;

        public MieFontFallbackFontAssets FontFallbackFontAssets { get; set; } = null;

        #region original font data

        public int LigaturesFileID { get; private set; }

        public long LigaturesPathID { get; private set; }

        public int InumStartAt { get; private set; }

        public bool IsIlluminated { get; private set; }

        #endregion

        public int NumofFallbackFontAssetsSize { get; private set; }

        public MieFontCreationSetting FontCreationSetting { get; private set; } = null;

        public MieFontWeights FontWeights { get; private set; }

        public float NormalStyle { get; private set; }

        public float NormalSpaceingOffset { get; private set; }

        public float BoldStyle { get; private set; }

        public float BoldSpacing { get; private set; }

        public int ItalicStyle { get; private set; }

        public int TabSize { get; private set; }

        public static MieFontFooter Read(BinaryReader br, MieFont.NFormatType formatType)
        {
            MieFontFooter result = new MieFontFooter();

            switch (formatType)
            {
                case MieFont.NFormatType.Unknown:
                    break;
                case MieFont.NFormatType.Type1:
                    break;
                case MieFont.NFormatType.Type2:
                    //// 日本語データの構造
                    result.KerningTable = MieFontKerningTable.Read(br, MieFont.NFormatType.Type2);

                    result.KerningPairBase = new MieFontKerningPairType2();
                    result.KerningPairBase.Read(br);

                    result.FontFallbackFontAssets = MieFontFallbackFontAssets.Read(br);

                    break;
                case MieFont.NFormatType.Type3:
                    break;
                case MieFont.NFormatType.Type4:
                    break;
                case MieFont.NFormatType.Type5:
                    break;
                case MieFont.NFormatType.PoE2:
                    //// オリジナルのデータ構造
                    result.KerningTable = MieFontKerningTable.Read(br, MieFont.NFormatType.PoE2);

                    result.KerningPairBase = new MieFontKerningPairPoE2();
                    result.KerningPairBase.Read(br);

                    result.LigaturesFileID = br.ReadInt32();
                    result.LigaturesPathID = br.ReadInt64();

                    result.InumStartAt = br.ReadInt32();
                    result.IsIlluminated = br.ReadBoolean();
                    MieFont.ReadPadding(br);

                    result.FontFallbackFontAssets = MieFontFallbackFontAssets.Read(br);

                    break;
                default:
                    throw new Exception($"Unknown format type({formatType}).");
            }

            ////

            result.FontCreationSetting = MieFontCreationSetting.Read(br);

            ////

            result.FontWeights = MieFontWeights.Read(br);

            result.NormalStyle = br.ReadSingle();
            result.NormalSpaceingOffset = br.ReadSingle();
            result.BoldStyle = br.ReadSingle();
            result.BoldSpacing = br.ReadSingle();
            result.ItalicStyle = br.ReadInt32();
            result.TabSize = br.ReadInt32();

            return result;
        }

        public void Write(BinaryWriter writer, MieFont.NFormatType formatType)
        {
            this.KerningTable.Write(writer);

            switch (formatType)
            {
                case MieFont.NFormatType.Unknown:
                    break;
                case MieFont.NFormatType.Type1:
                    break;
                case MieFont.NFormatType.Type2:
                    //// 日本語データの構造
                    this.KerningPairBase.Write(writer);
                    this.FontFallbackFontAssets.Write(writer);

                    break;
                case MieFont.NFormatType.Type3:
                    break;
                case MieFont.NFormatType.Type4:
                    break;
                case MieFont.NFormatType.Type5:
                    break;
                case MieFont.NFormatType.PoE2:
                    //// オリジナルのデータ構造
                    this.KerningPairBase.Write(writer);

                    writer.Write(this.LigaturesFileID);
                    writer.Write(this.LigaturesPathID);

                    writer.Write(this.InumStartAt);
                    writer.Write(this.IsIlluminated);
                    MieFont.WritePadding(writer);

                    this.FontFallbackFontAssets.Write(writer);

                    break;
                default:
                    throw new Exception($"Unknown format type({formatType})");
            }

            this.FontCreationSetting.Write(writer);

            this.FontWeights.Write(writer);

            writer.Write(this.NormalStyle);
            writer.Write(this.NormalSpaceingOffset);
            writer.Write(this.BoldStyle);
            writer.Write(this.BoldSpacing);
            writer.Write(this.ItalicStyle);
            writer.Write(this.TabSize);
        }

        public void Convert(MieFontFooter jp, MieFont.NFormatType formatType)
        {
            switch (formatType)
            {
                case MieFont.NFormatType.Unknown:
                    throw new NotImplementedException();
                case MieFont.NFormatType.Type1:
                    throw new NotImplementedException();
                case MieFont.NFormatType.Type2:
                    //// 日本語データの構造
                    //// 元データをそのまま使用するため、処理なし。
                    break;
                case MieFont.NFormatType.Type3:
                    throw new NotImplementedException();
                case MieFont.NFormatType.Type4:
                    throw new NotImplementedException();
                case MieFont.NFormatType.Type5:
                    throw new NotImplementedException();
                case MieFont.NFormatType.PoE2:
                    //// オリジナルのデータ構造
                    throw new NotImplementedException();
                default:
                    throw new Exception($"Unknown format type({formatType})");
            }

            //// this.FontWeights.Convert(jp.FontWeights);

            this.NormalStyle = jp.NormalStyle;
            this.NormalSpaceingOffset = jp.NormalSpaceingOffset;
            this.BoldStyle = jp.BoldStyle;
            this.BoldSpacing = jp.BoldSpacing;
            this.ItalicStyle = jp.ItalicStyle;
            this.TabSize = jp.TabSize;
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.Append(this.KerningTable.ToString());
            buff.Append(this.KerningPairBase.ToString());

            buff.AppendLine($"ligatures = FileID: {this.LigaturesFileID}, PathID: {this.LigaturesPathID}");
            buff.AppendLine($"InumStartAt = {this.InumStartAt}");
            buff.AppendLine($"IsIlluminated = {this.IsIlluminated}");

            buff.Append(this.FontFallbackFontAssets.ToString());
            buff.Append(this.FontCreationSetting.ToString());
            buff.Append(this.FontWeights.ToString());

            buff.AppendLine($"NormalStyle = {this.NormalStyle}");
            buff.AppendLine($"NormalSpaceingOffset = {this.NormalSpaceingOffset}");
            buff.AppendLine($"BoldStyle = {this.BoldStyle}");
            buff.AppendLine($"BoldSpacing = {this.BoldSpacing}");
            buff.AppendLine($"ItalicStyle = {this.ItalicStyle}");
            buff.AppendLine($"TabSize = {this.TabSize}");

            return buff.ToString();
        }
    }
}
