namespace MieDataConverter
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using MieCommon;
    using MieDbLib.SystemDB;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Product;
    using MonoOptions;
    using NLog;
    using S5mDebugTools;

    /// <summary>
    /// 言語および付加情報をDB化する
    /// </summary>
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static int Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en");

            Stopwatch sw = new Stopwatch();
            sw.Start();

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

            if (opt.Arges.IsInit)
            {
                InitDB(opt.Arges);
            }
            else
            {
                //// ToDo: テスト期間中は初期化を行う。
                //// InitDB(opt.Arges);

                OE2DB_No2(opt.Arges);
                DB2MieObj(opt.Arges);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            TDebugUtils.Pause();
            return 0;
        }

        private static void InitDB(TOptions.TArgs opt)
        {
            var systemDb = new MieSystemDB();

            var dbFolder = Path.GetDirectoryName(Path.GetFullPath(opt.FileNameSystemDB));
            MieCommonUtils.SafeCreateDirectory(dbFolder);

            systemDb.CreateSystemDB(opt.FileNameSystemDB, opt.SchemaPath, opt.IsReplace);
            systemDb.Open(opt.FileNameSystemDB);

            var convertConvApp = new MieDataConvertConversationApp();

            try
            {
                convertConvApp.InitConversations(systemDb);
                convertConvApp.InitFileList(systemDb);
                convertConvApp.InitLanguage(systemDb);
                convertConvApp.InitQuests(systemDb);
                convertConvApp.InitChatter(systemDb);
            }
            catch (Exception ex)
            {
                logger.Trace(ex.Message);
            }

            systemDb.CompactDatabase();
            systemDb.Close();
        }

        private static void OE2DB_No2(TOptions.TArgs opt)
        {
            MieSystemDB systemDb = new MieSystemDB();
            systemDb.Open(opt.FileNameSystemDB);
            MieDataConvertConversationApp convertConvApp = new MieDataConvertConversationApp();
            var productLine = MieProduct.GetProductLineFromText(opt.ProductLine);

            //// FileListの作成と言語情報のDB化。
            MieFileList fileList = new MieFileList();
            {
                var langPath = string.Empty;
                //// 会話情報の取り込み
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        //// チャッター情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\chatter");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Chatter, fileList);
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);
                        //// クエスト情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\quests");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Quests, fileList);

                        break;
                    case MieProduct.NProductLine.LaxA:
                        //// チャッター情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\chatter");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Chatter, fileList);
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxB:
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxC:
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxD:
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxE:

                        break;
                    case MieProduct.NProductLine.LaxF:
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxG:
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxH:
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.LaxI:
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);

                        break;
                    case MieProduct.NProductLine.DLC1:
                        //// チャッター情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\chatter");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Chatter, fileList);
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);
                        //// クエスト情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\quests");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Quests, fileList);

                        break;
                    case MieProduct.NProductLine.DLC2:
                        //// チャッター情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\chatter");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Chatter, fileList);
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);
                        //// クエスト情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\quests");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Quests, fileList);

                        break;
                    case MieProduct.NProductLine.DLC3:
                        //// チャッター情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\chatter");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Chatter, fileList);
                        //// 会話情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\conversations");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Conversations, fileList);
                        //// ゲーム情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\game");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Game, fileList);
                        //// クエスト情報の取り込み
                        langPath = Path.Combine(opt.FileNameLang, @"text\quests");
                        convertConvApp.ConvertLanguage(systemDb, langPath, productLine, MieProduct.NLanguageType.Quests, fileList);

                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }

                //// 言語ファイルのDB化。
                MieTableLanguageDao.SaveToSysyemDB(systemDb, convertConvApp.LanguageInfo);

                //// FileListのDB化。
                convertConvApp.ConvertFileList(systemDb, fileList);
            }

            //// キャラクター情報と種族情報のDB化。
            {
                var charAttrPath = string.Empty;
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxA:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxa_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxB:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxb_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxC:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxc_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxD:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxd_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxE:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxe_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxF:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxf_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxG:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxg_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxH:
                        //// キャラクター情報および種族情報なし
                        break;
                    case MieProduct.NProductLine.LaxI:
                        //// キャラクター情報および種族情報なし
                        break;
                    case MieProduct.NProductLine.DLC1:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax2_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.DLC2:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax1_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    case MieProduct.NProductLine.DLC3:
                        charAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax3_characters.gamedatabundle");
                        convertConvApp.ConvertCharcterAttributes(systemDb, charAttrPath);
                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }
            }

            //// Speaker情報のDB化。
            {
                var speakerAttrPath = string.Empty;
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxA:
                        break;
                    case MieProduct.NProductLine.LaxB:
                        break;
                    case MieProduct.NProductLine.LaxC:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxc_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxD:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxd_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxE:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\laxe_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.LaxF:
                        break;
                    case MieProduct.NProductLine.LaxG:
                        break;
                    case MieProduct.NProductLine.LaxH:
                        break;
                    case MieProduct.NProductLine.LaxI:
                        break;
                    case MieProduct.NProductLine.DLC1:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax2_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.DLC2:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax1_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    case MieProduct.NProductLine.DLC3:
                        speakerAttrPath = Path.Combine(opt.FileNameDesign, @"gamedata\lax3_speakers.gamedatabundle");
                        convertConvApp.ConvertSpeakerAttributes(systemDb, speakerAttrPath);
                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }
            }

            //// 会話情報のDB化
            {
                var convPath = string.Empty;
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    case MieProduct.NProductLine.LaxA:
                        break;
                    case MieProduct.NProductLine.LaxB:
                        break;
                    case MieProduct.NProductLine.LaxC:
                        break;
                    case MieProduct.NProductLine.LaxD:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    case MieProduct.NProductLine.LaxE:
                        break;
                    case MieProduct.NProductLine.LaxF:
                        break;
                    case MieProduct.NProductLine.LaxG:
                        break;
                    case MieProduct.NProductLine.LaxH:
                        break;
                    case MieProduct.NProductLine.LaxI:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC1:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC2:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC3:
                        convPath = Path.Combine(opt.FileNameDesign, @"conversations");
                        convertConvApp.ConvertConversations(systemDb, convPath, fileList);
                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }
            }

            //// ToDo:クエスト情報のDB化
            {
                var questsPath = string.Empty;
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        questsPath = Path.Combine(opt.FileNameDesign, @"quests");
                        convertConvApp.ConvertQuests(systemDb, questsPath, fileList);
                        break;
                    case MieProduct.NProductLine.LaxA:
                        break;
                    case MieProduct.NProductLine.LaxB:
                        break;
                    case MieProduct.NProductLine.LaxC:
                        break;
                    case MieProduct.NProductLine.LaxD:
                        break;
                    case MieProduct.NProductLine.LaxE:
                        break;
                    case MieProduct.NProductLine.LaxF:
                        break;
                    case MieProduct.NProductLine.LaxG:
                        break;
                    case MieProduct.NProductLine.LaxH:
                        break;
                    case MieProduct.NProductLine.LaxI:
                        break;
                    case MieProduct.NProductLine.DLC1:
                        questsPath = Path.Combine(opt.FileNameDesign, @"quests");
                        convertConvApp.ConvertQuests(systemDb, questsPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC2:
                        questsPath = Path.Combine(opt.FileNameDesign, @"quests");
                        convertConvApp.ConvertQuests(systemDb, questsPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC3:
                        questsPath = Path.Combine(opt.FileNameDesign, @"quests");
                        convertConvApp.ConvertQuests(systemDb, questsPath, fileList);
                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }
            }

            //// ToDo:チャッター情報のDB化
            {
                var chatterPath = string.Empty;
                switch (productLine)
                {
                    case MieProduct.NProductLine.Vanilla:
                        chatterPath = Path.Combine(opt.FileNameDesign, @"chatter");
                        convertConvApp.ConvertChatter(systemDb, chatterPath, fileList);
                        break;
                    case MieProduct.NProductLine.LaxA:
                        break;
                    case MieProduct.NProductLine.LaxB:
                        break;
                    case MieProduct.NProductLine.LaxC:
                        break;
                    case MieProduct.NProductLine.LaxD:
                        break;
                    case MieProduct.NProductLine.LaxE:
                        break;
                    case MieProduct.NProductLine.LaxF:
                        break;
                    case MieProduct.NProductLine.LaxG:
                        break;
                    case MieProduct.NProductLine.LaxH:
                        break;
                    case MieProduct.NProductLine.LaxI:
                        break;
                    case MieProduct.NProductLine.DLC1:
                        chatterPath = Path.Combine(opt.FileNameDesign, @"chatter");
                        convertConvApp.ConvertChatter(systemDb, chatterPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC2:
                        chatterPath = Path.Combine(opt.FileNameDesign, @"chatter");
                        convertConvApp.ConvertChatter(systemDb, chatterPath, fileList);
                        break;
                    case MieProduct.NProductLine.DLC3:
                        chatterPath = Path.Combine(opt.FileNameDesign, @"chatter");
                        convertConvApp.ConvertChatter(systemDb, chatterPath, fileList);
                        break;
                    default:
                        var msg = $"Unknown ProductLine({productLine}).";
                        throw new InvalidEnumArgumentException(msg);
                }
            }

            systemDb.CompactDatabase();
            systemDb.Close();
        }

        private static MieSystemDbApp DB2MieObj(TOptions.TArgs opt)
        {
            MieSystemDB systemDb = new MieSystemDB();
            systemDb.Open(opt.FileNameSystemDB);

            MieSystemDbApp sysApp = new MieSystemDbApp();
            sysApp.LoadFromDB(systemDb);

            //// 検証リスト出力
            sysApp.ToCharacterAttributeString(@"D_CheckList(CharacterAttribute).txt");
            sysApp.ToFileListString(@"D_CheckList(FileList).txt");
            sysApp.ToLanguageString(@"D_CheckList(Language).txt");
            ////sysApp.ToNodeStringString(@"D_CheckList(Node).txt");

            sysApp.ToConversationLinkString(@"D_CheckList(Conv)(LinkWithStopNode).txt", true);
            sysApp.ToConversationLinkString(@"D_CheckList(Conv)(LinkWithoutStopNode).txt", false);
            sysApp.ToConversationLinksFromToString(@"D_CheckList(Conv)(FromTo).txt");

            sysApp.ToRaceAttributeString(@"D_CheckList(Race).txt");
            sysApp.ToSpeakerAttributeString(@"D_CheckList(Speaker).txt");

            sysApp.ToQuestsLinksFromToString(@"D_CheckList(Quests)(FromTo).txt");
            sysApp.ToQuestsLinkString(@"D_CheckList(Quests)(LinkWithStopNode).txt", true);
            sysApp.ToQuestsLinkString(@"D_CheckList(Quests)(LinkWithoutStopNode).txt", false);

            sysApp.ToChatterLinksFromToString(@"D_CheckList(Chatter)(FromTo).txt");
            sysApp.ToChatterLinkString(@"D_CheckList(Chatter)(LinkWithStopNode).txt", true);
            sysApp.ToChatterLinkString(@"D_CheckList(Chatter)(LinkWithoutStopNode).txt", false);

            systemDb.Close();

            return sysApp;
        }
    }
}
