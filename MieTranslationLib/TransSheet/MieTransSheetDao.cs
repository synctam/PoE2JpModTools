namespace MieTranslationLib.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper;
    using CsvHelper.Configuration;
    using MieCommon;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Data.Chatter;
    using MieTranslationLib.Data.Conversations;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.Quests;
    using MieTranslationLib.MieUtils;
    using MieTranslationLib.Product;
    using NLog;

    public class MieTransSheetDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LoadFromCsv(MieTransSheetInfo sheetInfo, string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    //// 区切り文字
                    csv.Configuration.Delimiter = ",";
                    //// ヘッダーの有無
                    csv.Configuration.HasHeaderRecord = true;
                    //// CSVファイルに合ったマッピングルールを登録
                    csv.Configuration.RegisterClassMap<CsvReadMapper>();
                    //// データを読み出し
                    var records = csv.GetRecords<CsvReadRecord>();

                    foreach (var record in records)
                    {
                        var entry = new MieTransSheetEntry(
                            record.GetFileID(),
                            record.GetID(),
                            record.DefaultText,
                            string.Empty);
                        entry.DefaultTranslationText = record.DefaultTranslationText;
                        entry.ReferenceID = record.GetReferenceID();
                        entry.SpeakerName = record.SpeakerName;
                        entry.SortKey = record.GetSortKey();
                        entry.MachineTranslation = record.MachineTranslation;

                        sheetInfo.AddEntry(entry);
                    }
                }
            }
        }

        public static MieTransSheetInfo LoadFromCsvForMaintenance(
            MieTransSheetInfo sheetInfo, string path, bool isSimple, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    //// 区切り文字
                    csv.Configuration.Delimiter = ",";
                    //// ヘッダーの有無
                    csv.Configuration.HasHeaderRecord = true;
                    //// CSVファイルに合ったマッピングルールを登録
                    if (isSimple)
                    {
                        csv.Configuration.RegisterClassMap<CsvMapperForSimple>();
                    }
                    else
                    {
                        csv.Configuration.RegisterClassMap<CsvMapperForMaintenance>();
                    }

                    //// データを読み出し
                    var records = csv.GetRecords<MieTransSheetEntry>();

                    foreach (var record in records)
                    {
                        sheetInfo.AddEntry(record);
                    }
                }
            }

            return sheetInfo;
        }

        public static void SaveToDiffCsv(List<DiffUodateResult> updateDiffResult, string path, bool isOld, bool useTag)
        {
            var enc = new UTF8Encoding(false);
            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isOld)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForDiffUodateResultOld>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForDiffUodateResultNew>();
                }

                writer.WriteHeader<DiffUodateResult>();
                writer.NextRecord();

                foreach (var diffEntry in updateDiffResult)
                {
                    if (useTag)
                    {
                        diffEntry.TextOld = ChangeLineBreakToTag(diffEntry.TextOld);
                        diffEntry.TextNew = ChangeLineBreakToTag(diffEntry.TextNew);
                    }

                    writer.WriteRecord(diffEntry);
                    writer.NextRecord();
                }
            }
        }

        public static void SaveToCsvForMC(MieTransSheetInfo sheetInfo, string path)
        {
            //// UTF-8 BOM なし
            var enc = new UTF8Encoding(false);

            var folder = Path.GetDirectoryName(Path.GetFullPath(path));
            MieCommonUtils.SafeCreateDirectory(folder);

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                writer.Configuration.RegisterClassMap<CsvMapperForMC>();

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                foreach (var sheetFile in sheetInfo.Items.Values)
                {
                    foreach (var sheetEntry in sheetFile.Items.Values)
                    {
                        if (string.IsNullOrWhiteSpace(sheetEntry.DefaultTranslationText))
                        {
                            //// 日本語訳が存在しない。新規追加分など。
                            continue;
                        }

                        MieTransSheetEntry entry = new MieTransSheetEntry(
                            sheetEntry.FileID,
                            sheetEntry.ID,
                            sheetEntry.DefaultText,
                            sheetEntry.FemaleText);
                        entry.DefaultTranslationText = sheetEntry.DefaultTranslationText;

                        writer.WriteRecord(entry);
                        writer.NextRecord();
                    }
                }
            }
        }

        public static void SaveToCsv(
            MieTransSheetInfo sheetInfoOld,
            MieTransSheetInfo sheetInfoNew,
            string path,
            bool isMaintenance = false,
            Encoding enc = null)
        {
            if (enc == null)
            {
                //// UTF-8 BOM なし
                enc = new UTF8Encoding(false);
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isMaintenance)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForMaintenance2>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapper>();
                }

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                foreach (var sheetFileNew in sheetInfoNew.Items.Values)
                {
                    var fileOld = sheetInfoOld.GetFile(sheetFileNew.FileID);
                    foreach (var sheetEntryNew in sheetFileNew.Items.Values)
                    {
                        MieTransSheetEntry entry = new MieTransSheetEntry(
                            sheetEntryNew.FileID,
                            sheetEntryNew.ID,
                            sheetEntryNew.DefaultText,
                            sheetEntryNew.FemaleText);

                        entry.DefaultTranslationText = sheetEntryNew.DefaultTranslationText;
                        entry.Depth = sheetEntryNew.Depth;
                        entry.DisplayDepth = sheetEntryNew.DisplayDepth;
                        entry.Gender = sheetEntryNew.Gender;
                        entry.LanguageType = sheetEntryNew.LanguageType;
                        entry.ListenerName = sheetEntryNew.ListenerName;
                        entry.NodeType = sheetEntryNew.NodeType;
                        entry.ProductLine = sheetEntryNew.ProductLine;
                        entry.RaceName = sheetEntryNew.RaceName;
                        entry.ReferenceID = sheetEntryNew.ReferenceID;

                        var mtype = string.Empty;
                        var entryOld = fileOld.GetEntry(sheetEntryNew.ID);
                        var oldMT = string.Empty;
                        if (!string.IsNullOrWhiteSpace(entryOld.MachineTranslation))
                        {
                            oldMT = entryOld.MachineTranslation;
                        }

                        switch (sheetEntryNew.MaintenanceType)
                        {
                            case MieTransSheetEntry.NMaintenanceType.None:
                                entry.MachineTranslation = oldMT;
                                break;
                            case MieTransSheetEntry.NMaintenanceType.Added:
                                entry.MachineTranslation = sheetEntryNew.MachineTranslation;
                                mtype = "added";
                                break;
                            case MieTransSheetEntry.NMaintenanceType.Updated:
                                entry.MachineTranslation = oldMT;
                                mtype = "updated";
                                break;
                            case MieTransSheetEntry.NMaintenanceType.Deleted:
                                mtype = "deleted";
                                break;
                            default:
                                logger.Error($"Unknown MaintenanceType.");
                                throw new Exception($"Unknown MaintenanceType.");
                        }

                        entry.Scene = mtype;

                        if (sheetFileNew.IsDeleted)
                        {
                            entry.Scene = "deleted";
                        }

                        entry.SortKey = sheetEntryNew.SortKey;
                        if (string.IsNullOrWhiteSpace(entryOld.DefaultTranslationText))
                        {
                            //// 未翻訳の場合は原文を転記する。
                            var searchedText = sheetInfoOld.GetSearchText(entry.DefaultText, entry.LanguageType);
                            if (searchedText == null)
                            {
                                entry.DefaultTranslationText = entry.DefaultText;
                            }
                            else
                            {
                                entry.DefaultTranslationText = searchedText;
                            }
                        }
                        else
                        {
                            //// 翻訳済みの場合は、旧翻訳シートの翻訳文を転記する。
                            entry.DefaultTranslationText = entryOld.DefaultTranslationText;
                        }

                        if (string.IsNullOrWhiteSpace(entryOld.SpeakerName))
                        {
                            entry.SpeakerName = sheetEntryNew.SpeakerName;
                        }
                        else
                        {
                            entry.SpeakerName = entryOld.SpeakerName;
                        }

                        writer.WriteRecord(entry);
                        writer.NextRecord();
                    }
                }
            }
        }

        public static void SaveConversationsToCsv(
            MieProduct.NProductLine productLine,
            MieLanguageInfo languageInfo,
            MieConversationNodeInfo convDesign,
            MieCharacterAttributeFile characterAttributeFile,
            MieFileList fileList,
            string path,
            MieTransSheetOldFile poe1Sheet,
            bool isCompare = false,
            bool forcePrint = false,
            Encoding enc = null)
        {
            //// ToDo: 言語区分別に正しく翻訳シートが作成できるようにする。
            if (enc == null)
            {
                //// UTF-8 BOM なし
                enc = new UTF8Encoding(false);
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isCompare)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForCompare>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapper>();
                }

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                //// 処理済み FileID 格納用ハッシュ
                HashSet<long> hashFileID = new HashSet<long>();
                //// 付加情報ファイルが存在する項目を処理する。
                int hit = PutConversationDesignFile(
                    productLine,
                    MieProduct.NLanguageType.Conversations,
                    languageInfo,
                    convDesign,
                    characterAttributeFile,
                    fileList,
                    poe1Sheet,
                    forcePrint,
                    writer,
                    hashFileID);

                //// 付加情報ファイルが存在しない項目を処理する(ゾンビファイル)。
                hit += PutLangOnlyFile(
                    productLine,
                    MieProduct.NLanguageType.Conversations,
                    "ZombieFileNode",
                    languageInfo,
                    fileList,
                    poe1Sheet,
                    forcePrint,
                    writer,
                    hashFileID);

                /***************************************************************************************/

                Console.WriteLine($"PoE1 hit:{hit}");
            }
        }

        public static void SaveGameToCsv(
            MieProduct.NProductLine productLine,
            MieLanguageInfo languageInfo,
            MieFileList fileList,
            string path,
            MieTransSheetOldFile poe1Sheet = null,
            bool isCompare = false,
            Encoding enc = null)
        {
            if (enc == null)
            {
                //// UTF-8 BOM なし
                enc = new UTF8Encoding(false);
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isCompare)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForCompare>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapper>();
                }

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                //// 処理済み FileID 格納用ハッシュ
                HashSet<long> hashFileID = new HashSet<long>();

                var hit = PutLangOnlyFile(
                    productLine,
                    MieProduct.NLanguageType.Game,
                    string.Empty,
                    languageInfo,
                    fileList,
                    poe1Sheet,
                    false,
                    writer,
                    hashFileID);

                var all = languageInfo.GetEntryCount(
                    MieProduct.NProductLine.Vanilla,
                    MieProduct.NLanguageType.Game,
                    true);

                Console.WriteLine($"hit({hit}/{all}) {((double)hit / all) * 100} %");
            }
        }

        public static void SaveQuestsToCsv(
            MieProduct.NProductLine productLine,
            MieLanguageInfo languageInfo,
            MieQuestsNodeInfo questsDesign,
            MieFileList fileList,
            string path,
            MieTransSheetOldFile poe1Sheet,
            bool isCompare = false,
            bool forcePrint = false,
            Encoding enc = null)
        {
            if (enc == null)
            {
                //// UTF-8 BOM なし
                enc = new UTF8Encoding(false);
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isCompare)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForCompare>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapper>();
                }

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                //// 処理済み FileID 格納用ハッシュ
                HashSet<long> hashFileID = new HashSet<long>();
                int hit = PutQuestsDesignFile(productLine, languageInfo, questsDesign, fileList, poe1Sheet, forcePrint, writer, hashFileID);

                //// 付加情報ファイルが存在しない項目を処理する(ゾンビファイル)。
                hit += PutLangOnlyFile(productLine, MieProduct.NLanguageType.Quests, "ZombieFileNode", languageInfo, fileList, poe1Sheet, forcePrint, writer, hashFileID);

                Console.WriteLine($"PoE1 hit:{hit}");
            }
        }

        public static void SaveChatterToCsv(
            MieProduct.NProductLine productLine,
            MieLanguageInfo languageInfo,
            MieChatterNodeInfo chatterDesign,
            MieFileList fileList,
            string path,
            MieTransSheetOldFile poe1Sheet,
            bool isCompare = false,
            bool forcePrint = false,
            Encoding enc = null)
        {
            if (enc == null)
            {
                //// UTF-8 BOM なし
                enc = new UTF8Encoding(false);
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                if (isCompare)
                {
                    writer.Configuration.RegisterClassMap<CsvMapperForCompare>();
                }
                else
                {
                    writer.Configuration.RegisterClassMap<CsvMapper>();
                }

                writer.WriteHeader<MieTransSheetEntry>();
                writer.NextRecord();

                //// 処理済み FileID 格納用ハッシュ
                HashSet<long> hashFileID = new HashSet<long>();
                int hit = PutChatterDesignFile(productLine, languageInfo, chatterDesign, fileList, poe1Sheet, forcePrint, writer, hashFileID);

                //// 付加情報ファイルが存在しない項目を処理する(ゾンビファイル)。
                hit += PutLangOnlyFile(productLine, MieProduct.NLanguageType.Chatter, "ZombieFileNode", languageInfo, fileList, poe1Sheet, forcePrint, writer, hashFileID);

                Console.WriteLine($"PoE1 hit:{hit}");
            }
        }

        private static int PutConversationDesignFile(
            MieProduct.NProductLine productLine,
            MieProduct.NLanguageType languageType,
            MieLanguageInfo languageInfo,
            MieConversationNodeInfo design,
            MieCharacterAttributeFile characterAttributeFile,
            MieFileList fileList,
            MieTransSheetOldFile poe1Sheet,
            bool forcePrint,
            CsvWriter writer,
            HashSet<long> hashFileID)
        {
            //// PoE1の翻訳情報を流用できた項目数
            int hit = 0;
            foreach (var file in design.Files.Values)
            {
                var fileEntry = fileList.GetFileEntry(file.FileCode);
                if (fileEntry.LanguageType != languageType)
                {
                    continue;
                }

                //// 言語ファイルを取得。
                var languageFile = languageInfo.GetFile(file.FileCode);
                if (languageFile == null)
                {
                    continue;
                }

                hashFileID.Add(file.FileCode);
                //// 処理済み NodeID 格納用ハッシュ
                HashSet<int> hashIDs = new HashSet<int>();
                var sortedFlatList = file.GetSortedFlatNodes(false);

                int sortKey = 0;

                //// 付加情報に存在する項目を処理する。
                foreach (var designEntry in sortedFlatList)
                {
                    //// 言語エントリーを取得。
                    var languageEntry = languageFile.GetEntry(designEntry.NodeID);

                    if (languageEntry == null)
                    {
                        //// 言語エントリーがない場合はスキップ。
                        continue;
                    }

                    if ((languageEntry.ProductLine & productLine) != languageEntry.ProductLine)
                    {
                        //// ProductLine が不一致の場合はスキップ。
                        continue;
                    }

                    if (!forcePrint && string.IsNullOrWhiteSpace(languageEntry.DefaultText))
                    {
                        //// 原文が空文の場合はスキップ。ただし、強制印刷の場合はスキップしない。
                        continue;
                    }

                    hashIDs.Add(designEntry.NodeID);
                    var data = new MieTransSheetEntry();

                    data.DefaultText = languageEntry.DefaultText;
                    data.FemaleText = languageEntry.FemaleText;
                    data.LanguageType = file.LanguageType;
                    data.ProductLine = languageEntry.ProductLine;
                    data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);

                    data.ID = designEntry.NodeID;
                    data.Scene = fileEntry.FileID;
                    data.FileID = fileEntry.FileID;
                    data.SortKey = sortKey;

                    data.Depth = designEntry.Depth;
                    data.DisplayDepth = data.GetDisplayDepth();

                    data.NodeType = designEntry.NodeType.ToString();

                    //// 話者/種族情報所得
                    SetSpeakerAndRace(data, characterAttributeFile, designEntry.SpeakerGuid, designEntry.ListenerGuid, designEntry.NodeType);

                    //// PoE1翻訳データを反映する。
                    if (poe1Sheet != null)
                    {
                        data.DefaultTranslationText = poe1Sheet.GetTransData(data.DefaultText);
                        if (!string.IsNullOrWhiteSpace(data.DefaultTranslationText))
                        {
                            hit++;
                        }
                    }

                    writer.WriteRecord(data);
                    writer.NextRecord();

                    sortKey++;
                }

                //// 付加情報に存在しない項目を処理する(ゾンビノード)。
                languageFile.Items
                    .Where(languageEntry => (languageEntry.ProductLine & productLine) == languageEntry.ProductLine)
                    .Where(languageEntry => forcePrint || !string.IsNullOrWhiteSpace(languageEntry.DefaultText))
                    .Where(languageEntry => !hashIDs.Contains(languageEntry.ID))
                    .ToList()
                    .ForEach(languageEntry =>
                    {
                        //// PoE1翻訳データを反映する。
                        var translatedDefaultText = string.Empty;
                        if (poe1Sheet != null)
                        {
                            translatedDefaultText = poe1Sheet.GetTransData(languageEntry.DefaultText);
                            if (!string.IsNullOrWhiteSpace(translatedDefaultText))
                            {
                                hit++;
                            }
                        }

                        var data = new MieTransSheetEntry(fileEntry.FileID, languageEntry.ID, languageEntry.DefaultText, languageEntry.FemaleText);
                        data.DefaultTranslationText = translatedDefaultText;
                        data.LanguageType = languageType;
                        data.ProductLine = languageEntry.ProductLine;
                        data.Scene = fileEntry.FileID;
                        data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);
                        data.SortKey = sortKey;
                        data.Depth = -999;

                        MieConversationNodeEntry convNodeEntry = file.GetNode(languageEntry.ID);
                        if (convNodeEntry == null)
                        {
                            data.ListenerName = string.Empty;
                            data.SpeakerName = string.Empty;
                            data.NodeType = "ZombieNode";
                        }
                        else
                        {
                            //// 話者/種族情報取得
                            SetSpeakerAndRace(data, characterAttributeFile, convNodeEntry.SpeakerGuid, convNodeEntry.ListenerGuid, convNodeEntry.NodeType);

                            data.NodeType = convNodeEntry.NodeType.ToString();
                        }

                        writer.WriteRecord(data);
                        writer.NextRecord();

                        sortKey++;
                    });
            }

            return hit;
        }

        /// <summary>
        /// 話者/種族情報を取得しセットする。
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="characterAttributeFile">characterAttributeFile</param>
        /// <param name="speakerGuid">speakerGuid</param>
        /// <param name="listenerGuid">listenerGuid</param>
        /// <param name="nodeType">nodeType</param>
        private static void SetSpeakerAndRace(MieTransSheetEntry data, MieCharacterAttributeFile characterAttributeFile, Guid speakerGuid, Guid listenerGuid, MieConversationNodeEntry.NNodeType nodeType)
        {
            //// 話者/種族情報取得
            var speakerAttrEntry = characterAttributeFile.GetCharacterAttributeEntryBySpeakerID(speakerGuid);
            if (speakerAttrEntry == null)
            {
                data.SpeakerName = speakerGuid.ToString();
            }
            else
            {
                data.SpeakerName = speakerAttrEntry.Name;
                data.Gender = speakerAttrEntry.Gender;
                if (speakerAttrEntry.RaceID != null)
                {
                    var raceAttr = characterAttributeFile.GetRaceAttributeEntry(speakerAttrEntry.RaceID);
                    if (raceAttr != null)
                    {
                        data.RaceName = raceAttr.Name;
                    }
                }
            }

            var listenerAttrEntry = characterAttributeFile.GetCharacterAttributeEntryBySpeakerID(listenerGuid);
            if (listenerAttrEntry == null)
            {
                data.ListenerName = listenerGuid.ToString();
            }
            else
            {
                data.ListenerName = listenerAttrEntry.Name;
            }

            if (nodeType == MieConversationNodeEntry.NNodeType.PlayerResponseNode)
            {
                data.SpeakerName = $"Player";
            }

            if (data.SpeakerName == "00000000-0000-0000-0000-000000000000")
            {
                data.SpeakerName = $"(Player)";
            }

            if (data.ListenerName == "00000000-0000-0000-0000-000000000000")
            {
                data.ListenerName = $"(Player)";
            }
        }

        private static int PutLangOnlyFile(
            MieProduct.NProductLine productLine,
            MieProduct.NLanguageType languageType,
            string nodeTypeText,
            MieLanguageInfo languageInfo,
            MieFileList fileList,
            MieTransSheetOldFile poe1Sheet,
            bool forcePrint,
            CsvWriter writer,
            HashSet<long> hashFileID)
        {
            int hit = 0;
            languageInfo.GetFiles(MieProduct.NProductLine.ALL).Values
                //// すでに出力済みのものは除外する
                .Where(x => !hashFileID.Contains(x.FileCode))
                //// 言語区分が一致したものを選択
                .Where(x => fileList.GetFileEntry(x.FileCode).LanguageType == languageType)
                .ToList()
                .ForEach(file =>
                {
                    //// FileListからFileEntryを取得。
                    var fileID = fileList.GetFileEntry(file.FileCode).FileID;
                    int sortKey = 0;

                    //// forcePrint の時は全て出力する。
                    //// !forcePrint の時は DefaultText が空欄以外を出力する。
                    file.Items
                    .Where(entry => forcePrint || !string.IsNullOrWhiteSpace(entry.DefaultText))
                    .Where(entry => (entry.ProductLine & productLine) == entry.ProductLine)
                    .ToList()
                    .ForEach(entry =>
                    {
                        //// PoE1翻訳データを反映する。
                        var translatedDefaultText = string.Empty;
                        if (poe1Sheet != null)
                        {
                            translatedDefaultText = poe1Sheet.GetTransData(entry.DefaultText);
                            if (!string.IsNullOrWhiteSpace(translatedDefaultText))
                            {
                                hit++;
                            }
                        }

                        var fileListEntry = fileList.GetFileEntry(file.FileCode);
                        OutputSheetEntry(writer, fileID, sortKey, fileListEntry.LanguageType, entry, translatedDefaultText, nodeTypeText);
                        sortKey++;
                    });
                });

            return hit;
        }

        private static int PutChatterDesignFile(
            MieProduct.NProductLine productLine,
            MieLanguageInfo languageInfo,
            MieChatterNodeInfo chatterDesign,
            MieFileList fileList,
            MieTransSheetOldFile poe1Sheet,
            bool forcePrint,
            CsvWriter writer,
            HashSet<long> hashFileID)
        {
            //// PoE1の翻訳情報を流用できた項目数
            int hit = 0;
            foreach (var file in chatterDesign.Files.Values)
            {
                var fileID = fileList.GetFileEntry(file.FileCode).FileID;

                hashFileID.Add(file.FileCode);

                //// 処理済み NodeID 格納用ハッシュ
                HashSet<int> hashIDs = new HashSet<int>();
                var sortedFlatList = file.GetSortedFlatNodes(false);

                int sortKey = 0;

                //// 言語ファイルを取得。
                var languageFile = languageInfo.GetFile(file.FileCode);
                if (languageFile == null)
                {
                    continue;
                }

                //// 付加情報を含む項目を処理する。
                foreach (var designEntry in sortedFlatList)
                {
                    hashIDs.Add(designEntry.NodeID);

                    //// 言語エントリーを取得。
                    var languageEntry = languageFile.GetEntry(designEntry.NodeID);

                    var data = new MieTransSheetEntry();

                    if (languageEntry == null)
                    {
                        continue;
                    }

                    if ((languageEntry.ProductLine & productLine) != languageEntry.ProductLine)
                    {
                        continue;
                    }

                    data.DefaultText = languageEntry.DefaultText;
                    data.FemaleText = languageEntry.FemaleText;
                    data.LanguageType = file.LanguageType;
                    data.ProductLine = languageEntry.ProductLine;
                    data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);

                    data.ID = designEntry.NodeID;
                    data.Scene = fileID;
                    data.FileID = fileID;
                    data.SortKey = sortKey;

                    data.Depth = designEntry.Depth;
                    data.NodeType = designEntry.NodeType.ToString();

                    //// PoE1翻訳データを反映する。
                    if (poe1Sheet != null)
                    {
                        //// ChatterはPoE1には無いので反映は不要。
                    }

                    writer.WriteRecord(data);
                    writer.NextRecord();

                    sortKey++;
                }

                //// 付加情報に存在しない項目を処理する(ゾンビノード)。
                languageFile.Items
                    .Where(languageEntry => (languageEntry.ProductLine & productLine) == languageEntry.ProductLine)
                    .Where(languageEntry => forcePrint || !string.IsNullOrWhiteSpace(languageEntry.DefaultText))
                    .Where(languageEntry => !hashIDs.Contains(languageEntry.ID))
                    .ToList()
                    .ForEach(languageEntry =>
                    {
                        var data = new MieTransSheetEntry();

                        data.FileID = fileID;
                        data.ID = languageEntry.ID;
                        data.DefaultText = languageEntry.DefaultText;
                        data.FemaleText = languageEntry.FemaleText;
                        data.LanguageType = file.LanguageType;
                        data.ProductLine = languageEntry.ProductLine;
                        data.Scene = fileID;
                        data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);
                        data.SortKey = sortKey;

                        data.NodeType = "ZombieNode";

                        data.Depth = -999;

                        //// PoE1翻訳データを反映する。
                        if (poe1Sheet != null)
                        {
                        }

                        writer.WriteRecord(data);
                        writer.NextRecord();

                        sortKey++;
                    });
            }

            return hit;
        }

        private static int PutQuestsDesignFile(MieProduct.NProductLine productLine, MieLanguageInfo languageInfo, MieQuestsNodeInfo questsDesign, MieFileList fileList, MieTransSheetOldFile poe1Sheet, bool forcePrint, CsvWriter writer, HashSet<long> hashFileID)
        {
            //// PoE1の翻訳情報を流用できた項目数
            int hit = 0;
            foreach (var file in questsDesign.Files.Values)
            {
                var fileID = fileList.GetFileEntry(file.FileCode).FileID;

                hashFileID.Add(file.FileCode);

                //// 処理済み NodeID 格納用ハッシュ
                HashSet<int> hashIDs = new HashSet<int>();
                var sortedFlatList = file.GetSortedFlatNodes(false);

                int sortKey = 0;

                //// 言語ファイルを取得。
                var languageFile = languageInfo.GetFile(file.FileCode);
                if (languageFile == null)
                {
                    continue;
                }

                //// 付加情報を含む項目を処理する。
                foreach (var designEntry in sortedFlatList)
                {
                    hashIDs.Add(designEntry.NodeID);

                    //// 言語エントリーを取得。
                    var languageEntry = languageFile.GetEntry(designEntry.NodeID);

                    var data = new MieTransSheetEntry();

                    if (languageEntry == null)
                    {
                        continue;
                    }

                    if ((languageEntry.ProductLine & productLine) != languageEntry.ProductLine)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(languageEntry.DefaultText))
                    {
                        continue;
                    }

                    data.DefaultText = languageEntry.DefaultText;
                    data.FemaleText = languageEntry.FemaleText;
                    data.LanguageType = file.LanguageType;
                    data.ProductLine = languageEntry.ProductLine;
                    data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);

                    data.ID = designEntry.NodeID;
                    data.Scene = fileID;
                    data.FileID = fileID;
                    data.SortKey = sortKey;

                    data.Depth = designEntry.Depth;

                    data.NodeType = designEntry.NodeType.ToString();

                    //// クエストツリーまたは状態を反映
                    if (designEntry.NodeType == MieQuestsNodeEntry.NNodeType.EndStateNode)
                    {
                        //// クエスト終了ノード(EndStatus)を表示
                        data.DisplayDepth = $"EndState[{data.ID}]";
                    }
                    else
                    {
                        //// ツリーを表示
                        data.DisplayDepth = data.GetDisplayDepth();
                    }

                    //// PoE1翻訳データを反映する。
                    if (poe1Sheet != null)
                    {
                        data.DefaultTranslationText = poe1Sheet.GetTransData(data.DefaultText);
                        if (!string.IsNullOrWhiteSpace(data.DefaultTranslationText))
                        {
                            hit++;
                        }
                    }

                    writer.WriteRecord(data);
                    writer.NextRecord();

                    sortKey++;
                }

                //// 付加情報に存在しない項目を処理する(ゾンビノード)。
                languageFile.Items
                    .Where(languageEntry => (languageEntry.ProductLine & productLine) == languageEntry.ProductLine)
                    .Where(languageEntry => forcePrint || !string.IsNullOrWhiteSpace(languageEntry.DefaultText))
                    .Where(languageEntry => !hashIDs.Contains(languageEntry.ID))
                    .ToList()
                    .ForEach(languageEntry =>
                    {
                        var data = new MieTransSheetEntry();

                        data.FileID = fileID;
                        data.ID = languageEntry.ID;
                        data.DefaultText = languageEntry.DefaultText;
                        data.FemaleText = languageEntry.FemaleText;
                        data.LanguageType = file.LanguageType;
                        data.ProductLine = languageEntry.ProductLine;
                        data.Scene = fileID;
                        data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);
                        data.SortKey = sortKey;

                        data.NodeType = "ZombieNode";

                        data.Depth = -999;

                        //// PoE1翻訳データを反映する。
                        if (poe1Sheet != null)
                        {
                            data.DefaultTranslationText = poe1Sheet.GetTransData(data.DefaultText);
                            if (!string.IsNullOrWhiteSpace(data.DefaultTranslationText))
                            {
                                hit++;
                            }
                        }

                        writer.WriteRecord(data);
                        writer.NextRecord();

                        sortKey++;
                    });
            }

            return hit;
        }

        private static void OutputSheetEntry(
            CsvWriter writer,
            string fileID,
            int sortKey,
            MieProduct.NLanguageType languageType,
            MieLanguageEntry languageEntry,
            string defaultTranslationText,
            string nodeTypeText)
        {
            var data = new MieTransSheetEntry();

            data.FileID = fileID;
            data.ID = languageEntry.ID;
            data.DefaultText = languageEntry.DefaultText;
            data.FemaleText = languageEntry.FemaleText;
            data.DefaultTranslationText = defaultTranslationText;
            data.LanguageType = languageType;
            data.ProductLine = languageEntry.ProductLine;
            data.Scene = fileID;
            data.ReferenceID = MieHashTools.ComputeHashIds(languageEntry.ReferenceID);
            data.SortKey = sortKey;

            data.NodeType = nodeTypeText;

            data.Depth = -999;
            data.ListenerName = string.Empty;
            data.SpeakerName = string.Empty;

            writer.WriteRecord(data);
            writer.NextRecord();
        }

        private static string ChangeLineBreakToTag(string text)
        {
            StringBuilder buff = new StringBuilder(text);
            buff.Replace("\r\n", "<CRLF>");
            buff.Replace("\r", "<CR>");
            buff.Replace("\n", "<LF>");

            return buff.ToString();
        }

        /// <summary>
        /// レコード：翻訳シート読み込み用。
        /// </summary>
        public class CsvReadRecord
        {
            public string FileID { get; set; }

            public string FileIDExt { get; set; }

            public string SpeakerName { get; set; }

            public string DefaultText { get; set; }

            public string MachineTranslation { get; set; }

            public int ID { get; set; }

            public string DefaultTranslationText { get; set; }

            public string ReferenceID { get; set; }

            public int SortKey { get; set; } = 0;

            public string ProductLine { get; set; }

            public string GetFileID()
            {
                var array = this.FileIDExt.Split(':');
                return array[0];
            }

            public int GetID()
            {
                var array = this.FileIDExt.Split(':');
                var strID = array[2];
                int id = 0;
                if (!int.TryParse(strID, out id))
                {
                    throw new Exception($"Invalid ID. ID({strID})");
                }

                return id;
            }

            public int GetSortKey()
            {
                var array = this.FileIDExt.Split(':');
                var strSortKey = array[1];
                int sortKey = 0;
                if (!int.TryParse(strSortKey, out sortKey))
                {
                    throw new Exception($"Invalid ID. ID({strSortKey})");
                }

                return sortKey;
            }

            public string GetReferenceID()
            {
                var array = this.FileIDExt.Split(':');
                return array[3];
            }
        }

        /// <summary>
        /// 格納ルール ：翻訳シート読み込み用。
        /// </summary>
        public class CsvReadMapper : ClassMap<CsvReadRecord>
        {
            public CsvReadMapper()
            {
                this.Map(x => x.FileIDExt).Name("[[FileIDExt]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：翻訳用
        /// </summary>
        public class CsvMapper : ClassMap<MieTransSheetEntry>
        {
            public CsvMapper()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.LanguageType).Name("[[LanguageType]]");
                this.Map(x => x.Scene).Name("[[Scene]]");

                this.Map(x => x.DisplayDepth).Name("[[Tree]]");
                this.Map(x => x.GenderText).Name("[[Gender]]");
                this.Map(x => x.RaceName).Name("[[Race]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.ListenerName).Name("[[ListenerName]]");

                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");

                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.SortKey).Name("[[SortKey]]");
                this.Map(x => x.ProductLine).Name("[[ProductLine]]");
                this.Map(x => x.NodeType).Name("[[NodeType]]");
                this.Map(x => x.FileIDExt).Name("[[FileIDExt]]");

                ////this.Map(x => x.FemaleText).Name("[[FemaleText]]");
                ////this.Map(x => x.FemaleTranslationText).Name("[[FemaleTranslationText]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：比較用
        /// </summary>
        public class CsvMapperForCompare : ClassMap<MieTransSheetEntry>
        {
            public CsvMapperForCompare()
            {
                // 出力時の列の順番は指定した順となる。
                ////this.Map(x => x.LanguageType).Name("[[LanguageType]]");
                ////this.Map(x => x.Scene).Name("[[Scene]]");

                ////this.Map(x => x.DisplayDepth).Name("[[Tree]]");
                ////this.Map(x => x.GenderText).Name("[[Gender]]");
                ////this.Map(x => x.RaceName).Name("[[Race]]");
                ////this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                ////this.Map(x => x.ListenerName).Name("[[ListenerName]]");

                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                ////this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                ////this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");

                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                ////this.Map(x => x.SortKey).Name("[[SortKey]]");
                ////this.Map(x => x.ProductLine).Name("[[ProductLine]]");
                ////this.Map(x => x.NodeType).Name("[[NodeType]]");

                ////this.Map(x => x.FemaleText).Name("[[FemaleText]]");
                ////this.Map(x => x.FemaleTranslationText).Name("[[FemaleTranslationText]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：翻訳用
        /// </summary>
        public class CsvMapperForMaintenance : ClassMap<MieTransSheetEntry>
        {
            public CsvMapperForMaintenance()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.LanguageType).Name("[[LanguageType]]");
                this.Map(x => x.Scene).Name("[[Scene]]");
                ////this.Map(x => x.DisplayDepth).Name("[[Tree]]");
                ////this.Map(x => x.Gender).Name("[[Gender]]");
                this.Map(x => x.RaceName).Name("[[Race]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.ListenerName).Name("[[ListenerName]]");
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.SortKey).Name("[[SortKey]]");
                this.Map(x => x.ProductLine).Name("[[ProductLine]]");
                this.Map(x => x.NodeType).Name("[[NodeType]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：シンプル
        /// </summary>
        public class CsvMapperForSimple : ClassMap<MieTransSheetEntry>
        {
            public CsvMapperForSimple()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[FileIDExt]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：差分 old
        /// </summary>
        public class CsvMapperForDiffUodateResultOld : ClassMap<DiffUodateResult>
        {
            public CsvMapperForDiffUodateResultOld()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.TextOld).Name("[[TextOld]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：差分 new
        /// </summary>
        public class CsvMapperForDiffUodateResultNew : ClassMap<DiffUodateResult>
        {
            public CsvMapperForDiffUodateResultNew()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.ID).Name("[[ID]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.TextNew).Name("[[TextNew]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：MC用
        /// </summary>
        public class CsvMapperForMC : ClassMap<MieTransSheetEntry>
        {
            public CsvMapperForMC()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[FileID]]");
                this.Map(x => x.Scene).Name("[[Scene]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.ID).Name("[[ID]]");
            }
        }

        /// <summary>
        /// 翻訳シート出力用マッピング定義：メンテナンス用
        /// </summary>
        public class CsvMapperForMaintenance2 : ClassMap<MieTransSheetEntry>
        {
            public CsvMapperForMaintenance2()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileIDExt).Name("[[FileIDExt]]");
                this.Map(x => x.SpeakerName).Name("[[SpeakerName]]");
                this.Map(x => x.DefaultText).Name("[[DefaultText]]");
                this.Map(x => x.DefaultTranslationText).Name("[[DefaultTranslationText]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
                this.Map(x => x.Scene).Name("[[変更区分]]");
            }
        }

        public class DiffUodateResult
        {
            public string FileID { get; set; } = string.Empty;

            public int ID { get; set; } = 0;

            public string TextOld { get; set; } = string.Empty;

            public string TextNew { get; set; } = string.Empty;

            public string ReferenceID { get; set; } = string.Empty;
        }
    }
}
