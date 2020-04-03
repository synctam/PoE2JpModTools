namespace MieOELib.Data.StringTable
{
    using System;
    using System.IO;
    using System.Linq;
    using MieCommon;
    using MieOELib.FileUtils;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.LanguageHistory;
    using MieTranslationLib.MieUtils;
    using MieTranslationLib.Product;
    using MieTranslationLib.TransSheet;
    using NLog;
    using OEIFormats.Strings;

    public class MieStringTableDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void MargeFromFolder(MieLanguageInfo mieLanguageInfo, string folderPath, MieProduct.NProductLine productLine, MieProduct.NLanguageType languageType)
        {
            if (!Directory.Exists(folderPath))
            {
                var msg = $"Warning: Folder not exists. Folder({folderPath})";
                logger.Warn(msg);
                Console.WriteLine(msg);
                return;
            }

            string[] files = Directory.GetFiles(folderPath, "*.stringtable", SearchOption.AllDirectories);
            files
                .Where(file => Path.GetExtension(file).ToLower() == ".stringtable")
                .ToList()
                .ForEach(x =>
                {
                    var fileID = string.Empty;
                    var languageFile = LoadFromXml(x, productLine, out fileID);
                    mieLanguageInfo.AddFile(languageFile, true);
                });
        }

        public static MieLanguageInfo LoadFromFolder(
            string folderPath,
            MieProduct.NProductLine productLine,
            MieProduct.NLanguageType languageType,
            MieFileList fileList)
        {
            var mieLanguageInfo = new MieLanguageInfo();

            string[] files = Directory.GetFiles(folderPath, "*.stringtable", SearchOption.AllDirectories);
            files
                .Where(file => Path.GetExtension(file).ToLower() == ".stringtable")
                .ToList()
                .ForEach(x =>
                {
                    var fileID = string.Empty;
                    var languageFile = LoadFromXml(x, productLine, out fileID);
                    //// 言語情報の登録
                    mieLanguageInfo.AddFile(languageFile, false);
                    //// FileListの作成
                    fileList.AddEntryByFileIdAndFileCode(fileID, languageFile.FileCode, languageType);
                });

            return mieLanguageInfo;
        }

        public static string GetFileIdFromXML(string path)
        {
            //// StringTableの読み込み
            var stringTableFile = StringTableFile.Load(path);
            var fileID = MieFileUtils.ConvertFileIDToCommon(stringTableFile.Name);

            return fileID;
        }

        /// <summary>
        /// StringTableを読み込み、言語ファイルを返す。
        /// 言語ファイルにファイル履歴の枠組みを作成し設定する。
        /// </summary>
        /// <param name="path">StringTableファイルのパス</param>
        /// <param name="productLine">製品区分</param>
        /// <param name="fileID">FileID</param>
        /// <returns>言語ファイル</returns>
        public static MieLanguageFile LoadFromXml(string path, MieProduct.NProductLine productLine, out string fileID)
        {
            //// StringTableの読み込み
            var stringTableFile = StringTableFile.Load(path);
            //// ファイル履歴の作成
            MieLanguageHistoryFile mieLanguageHistoryFile = new MieLanguageHistoryFile(stringTableFile.Name);

            //// FileIDを統一形式に変換する。
            fileID = MieFileUtils.ConvertFileIDToCommon(stringTableFile.Name);
            var commonFileID = MieStringUtils.NormalizedFileID(fileID);
            var fileCode = MieHashTools.ComputeFileID(commonFileID);

            var mieLanguageFile = new MieLanguageFile(fileCode);
            stringTableFile
                .Entries
                .ForEach(entry =>
                {
                    var referenceID = MieTranslationLib.MieUtils.MieHashTools.ComputeReferenceID(stringTableFile.Name, entry.ID);
                    var mieLanguageEntry = new MieLanguageEntry(entry.ID, entry.DefaultText, entry.FemaleText, productLine, referenceID);
                    mieLanguageFile.AddEntry(mieLanguageEntry);
                });

            return mieLanguageFile;
        }

        public static void CreateXml(
            string fileID,
            MieLanguageFile langFile,
            string jpPath,
            MieTransSheetFile transSheetFile,
            bool useMT,
            bool useReferenceID)
        {
            var stringTableFile = new StringTableFile(fileID);
            foreach (var tableEntry in langFile.Items)
            {
                MieTransSheetEntry transSheetEntry = null;
                if (transSheetFile != null)
                {
                    transSheetEntry = transSheetFile.GetEntry(tableEntry.ID);
                }

                var translatedText = string.Empty;
                if (transSheetEntry == null)
                {
                    //// 翻訳シートエントリーがないので、ReferenceIDを個別に算出。
                    //// no | useRef | en | result
                    //// 0  |    o   |  o | ref付きen
                    //// 1  |    o   |  x | en
                    //// 2  |    x   |  o | en
                    //// 3  |    x   |  x | en
                    if (useReferenceID && !string.IsNullOrEmpty(tableEntry.DefaultText)) //// 0
                    {
                        //// 翻訳シートにエントリーが存在しない。
                        logger.Error($"TransSheetEntry not found. Product({tableEntry.ProductLine.ToString()}) FileID({fileID}), ID({tableEntry.ID})");

                        //// ReferenceIDを算出。
                        long referenceID = MieHashTools.ComputeReferenceID(stringTableFile.Name, tableEntry.ID);
                        //// ReferenceIDからhash文字列を生成
                        var strReferenceID = $"{MieHashTools.ComputeHashIds(referenceID)}:";

                        translatedText = $"{strReferenceID}{tableEntry.DefaultText}";
                    }
                    else
                    {
                        translatedText = tableEntry.DefaultText;
                    }
                }
                else
                {
                    //// 翻訳
                    translatedText = transSheetEntry.Translate(tableEntry.DefaultText, useMT, useReferenceID);
                }

                if (string.IsNullOrWhiteSpace(tableEntry.FemaleText))
                {
                    //// 女性の台詞なしの場合。
                    var entry = new StringTableFile.Entry(tableEntry.ID, translatedText);
                    stringTableFile.Entries.Add(entry);
                }
                else
                {
                    //// 女性の台詞ありの場合。
                    //// 男性と同じ台詞を割り当てる。
                    var entry = new StringTableFile.Entry(tableEntry.ID, translatedText, translatedText);
                    stringTableFile.Entries.Add(entry);
                }
            }

            Console.WriteLine(fileID);
            SaveToXml(fileID, stringTableFile, jpPath);
        }

        public static void SaveToXml(
            string fileID,
            StringTableFile stringTableFile,
            string jpFolderPath,
            bool useReferenceID = false)
        {
            var path = Path.Combine(jpFolderPath, fileID + ".stringtable");
            var folder = Path.GetDirectoryName(path);
            MieCommonUtils.SafeCreateDirectory(folder);

            StringTableFile.Save(stringTableFile, path, StringTableFile.ExportType.Xml);
        }

        /// <summary>
        /// 日本語化とXMLファイルの保存。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <param name="enFolderPath">原文のXMLファイルのパス</param>
        /// <param name="jpFolderPath">日本語版のXMLファイルのパス</param>
        /// <param name="transSheetFile">翻訳シートファイル</param>
        /// <param name="useReferenceID">ReferenceIDの有無</param>
        public static void SaveToXml(
        string fileID,
        string enFolderPath,
        string jpFolderPath,
        MieTransSheetFile transSheetFile,
        bool useReferenceID = false)
        {
            //// StringTableの読み込み
            var enPath = Path.Combine(enFolderPath, fileID + ".stringtable");
            {
                //// Bugfix: stringtable内のNameパラメーターに誤りがあるため、修正する。
                enPath = enPath.Replace("bounty encounters", "bounty_encounters");
            }

            var stringTableFile = StringTableFile.Load(enPath);
            //// ファイル履歴の作成
            MieLanguageHistoryFile mieLanguageHistoryFile = new MieLanguageHistoryFile(stringTableFile.Name);

            //// FileIDを統一形式に変換する。
            var commonFileID = MieStringUtils.NormalizedFileID(fileID);
            var fileCode = MieHashTools.ComputeFileID(commonFileID);

            if (transSheetFile == null)
            {
                //// 翻訳シートがない場合は転記しない。
            }
            else
            {
                foreach (var entry in stringTableFile.Entries)
                {
                    MieTransSheetEntry sheetEntry = transSheetFile.GetEntry(entry.ID);

                    if (sheetEntry == null || string.IsNullOrWhiteSpace(entry.DefaultText))
                    {
                        //// 原文の翻訳テキストが存在しない場合は転記しない。
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(sheetEntry.DefaultTranslationText))
                        {
                            //// 翻訳テキストが存在しない場合は転記しない。
                        }
                        else
                        {
                            entry.DefaultText = sheetEntry.DefaultTranslationText;
                            if (!string.IsNullOrWhiteSpace(entry.FemaleText))
                            {
                                //// 女性版テキストが存在する場合は標準テキストを転記する。
                                entry.FemaleText = sheetEntry.DefaultTranslationText;
                            }
                        }
                    }

                    if (sheetEntry != null)
                    {
                        if (sheetEntry.FileID.StartsWith(@"game"))
                        {
                            if (useReferenceID)
                            {
                                entry.DefaultText = $"{sheetEntry.ReferenceID}:{entry.DefaultText}";
                            }
                        }
                    }
                }
            }

            var jpPath = Path.Combine(jpFolderPath, fileID + ".stringtable");
            var jpFolder = Path.GetDirectoryName(jpPath);
            MieCommonUtils.SafeCreateDirectory(jpFolder);
            StringTableFile.Save(stringTableFile, jpPath, StringTableFile.ExportType.Xml);
        }
    }
}
