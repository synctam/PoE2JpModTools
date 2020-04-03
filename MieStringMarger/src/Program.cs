namespace MieStringMarger
{
    using MieTranslationLib.TransSheet;
    using MonoOptions;
    using S5mDebugTools;

    public class Program
    {
        /// <summary>
        /// 翻訳シートと言語DBから統合版日本語化MODを作成する。
        /// </summary>
        /// <param name="args">args</param>
        /// <returns>終了コード</returns>
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

            CreateMod(opt.Arges);

            TDebugUtils.Pause();
            return 0;
        }

        private static void CreateMod(TOptions.TArgs opt)
        {
            var transSheetInfo = new MieTransSheetInfo();

            //// 翻訳シート（会話）の読み込み
            MieTransSheetDao.LoadFromCsv(
                transSheetInfo,
                opt.FileNameConvasation);

            //// 翻訳シート（システム）の読み込み
            MieTransSheetDao.LoadFromCsv(
                transSheetInfo,
                opt.FileNameSystem);

            //// 翻訳シート（chatter）の読み込み
            MieTransSheetDao.LoadFromCsv(
                transSheetInfo,
                opt.FileNameChatter);

            //// DBから言語情報を取得する。
            var stringMarger = new MieStringMargeUtils(
                opt.FileNameSystemDB);

            //// 統合版日本語化MODを作成する。
            {
                //// ID付き
                var useReferenceID = true;
                var useMT = false;
                stringMarger.SaveToFolder(
                    opt.FolderNameOut,
                    transSheetInfo,
                    true,
                    true,
                    true,
                    true,
                    useMT,
                    useReferenceID);

                stringMarger.SaveLanguageConf(opt.FolderNameOut, useReferenceID);
            }

            {
                //// IDなし
                var useReferenceID = false;
                var useMT = false;
                stringMarger.SaveToFolder(
                    opt.FolderNameOut,
                    transSheetInfo,
                    true,
                    true,
                    true,
                    true,
                    useMT,
                    useReferenceID);

                stringMarger.SaveLanguageConf(opt.FolderNameOut, useReferenceID);
            }

            stringMarger.SaveToCsvForMC(transSheetInfo, @"MC\0PoE2_MC用翻訳シート.csv");
        }
    }
}
