namespace MieFontConverter
{
    using System.IO;
    using MieFontLib;
    using MonoOptions;
    using S5mDebugTools;

    public class Program
    {
        private static int Main(string[] args)
        {
            TOptions opt = new TOptions(args);
            if (opt.IsError)
            {
                TDebugUtils.Pause();
                return 1;
            }

            if (opt.Arges.Help)
            {
                opt.ShowUsage();

                TDebugUtils.Pause();
                return 1;
            }

            MakeFont(opt.Arges);

            TDebugUtils.Pause();

            return 0;
        }

        private static void MakeFont(TOptions.TArgs opt)
        {
            MieFont fontEn = new MieFont();
            fontEn.Load(opt.FileNameSource, opt.EnumFormatType);

            //// 自作のTMPフォント。形式は Type2 に固定。
            MieFont fontJp = new MieFont();
            fontJp.Load(opt.FileNameTarget, MieFont.NFormatType.Type2);

            var newJp = MieFontConvert.Convert(
                fontEn,
                opt.EnumFormatType,
                fontJp,
                MieFont.NFormatType.Type2,
                opt.Ascender);

            var fileName = Path.GetFileName(opt.FileNameSource);
            var fullPath = Path.Combine(opt.FolderNameOutput, fileName);
            newJp.Save(fullPath, opt.EnumFormatType);

            if (opt.UseList)
            {
                var dumpPath = $"{fullPath}.txt";
                newJp.Dump(dumpPath, opt.IsDetail);
            }
        }
    }
}
