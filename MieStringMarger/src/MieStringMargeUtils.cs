namespace MieStringMarger
{
    using System;
    using System.IO;
    using System.Text;
    using MieDbLib.SystemDB;
    using MieOELib.Data.StringTable;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Product;
    using MieTranslationLib.TransSheet;

    public class MieStringMargeUtils
    {
        private MieLanguageInfo languageInfo = null;
        private MieFileList fileList = null;

        /// <summary>
        /// 指定されたDBから言語情報を取得する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        public MieStringMargeUtils(string path)
        {
            var systemDb = new MieSystemDB();
            systemDb.Open(path);

            var sysApp = new MieSystemDbApp();
            sysApp.LoadFromDB(systemDb);
            this.languageInfo = sysApp.LanguageInfo;
            this.fileList = sysApp.FileList;

            systemDb.Close();
        }

        public void LoadFromFile(string path)
        {
            string fileID = string.Empty;
            var languageFile = MieStringTableDao.LoadFromXml(
                path, MieProduct.NProductLine.ALL, out fileID);
            this.languageInfo.AddFile(languageFile, false);
        }

        public void LoadFromFolder(
            string folderPath,
            MieProduct.NProductLine productLine,
            MieProduct.NLanguageType languageType,
            MieFileList fileList)
        {
            this.languageInfo = MieStringTableDao.LoadFromFolder(
                folderPath,
                productLine,
                languageType,
                fileList);
            Console.WriteLine(this.languageInfo.FileCount);
        }

        public void SaveLanguageConf(
            string outFolderPath, bool useReferenceID)
        {
            string langSig = string.Empty;
            string xml = string.Empty;
            //// リソースに埋め込んだ言語設定ファイルを出力する。
            if (useReferenceID)
            {
                langSig = "jpr";
                xml = Properties.Resources.language_jpr;
            }
            else
            {
                langSig = "jp";
                xml = Properties.Resources.language_jp;
            }

            var path = Path.Combine(
                outFolderPath,
                $@"exported\localized\{langSig}",
                "language.xml");
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write(xml);
            }
        }

        public void MargeFromFolder(
            string folderPath,
            MieProduct.NProductLine productLine,
            MieProduct.NLanguageType languageType,
           MieFileList fileList)
        {
            MieStringTableDao.MargeFromFolder(
                this.languageInfo,
                folderPath,
                productLine,
                languageType);

            Console.WriteLine(this.languageInfo.FileCount);
        }

        public void SaveToFolder(
            string japaneseFolderPath,
            MieTransSheetInfo transSheetInfo,
            bool useChatter,
            bool useConversation,
            bool useGame,
            bool useQuests,
            bool useMT,
            bool useReferenceID)
        {
            foreach (var langFile in this.languageInfo.Items.Values)
            {
                var fileListEntry = this.fileList.GetFileEntry(langFile.FileCode);

                bool go = false;
                if (fileListEntry.LanguageType == MieProduct.NLanguageType.Chatter && useChatter)
                {
                    go = true;
                }
                else if (fileListEntry.LanguageType == MieProduct.NLanguageType.Conversations && useConversation)
                {
                    go = true;
                }
                else if (fileListEntry.LanguageType == MieProduct.NLanguageType.Game && useGame)
                {
                    go = true;
                }
                else if (fileListEntry.LanguageType == MieProduct.NLanguageType.Quests && useQuests)
                {
                    go = true;
                }

                if (go)
                {
                    var fileID = this.fileList.GetFileID(langFile.FileCode);

                    string jpPath = string.Empty;
                    if (useReferenceID)
                    {
                        jpPath = Path.Combine(japaneseFolderPath, @"exported\localized\jpr\text");
                    }
                    else
                    {
                        jpPath = Path.Combine(japaneseFolderPath, @"exported\localized\jp\text");
                    }

                    var transSheetFile = transSheetInfo.GetFile(fileID);
                    MieStringTableDao.CreateXml(
                        fileID, langFile, jpPath, transSheetFile, useMT, useReferenceID);
                }
            }
        }

        /// <summary>
        /// MC用翻訳シートを出力する。
        /// </summary>
        /// <param name="transSheetInfo">翻訳シート情報</param>
        /// <param name="path">MC用翻訳シートのパス</param>
        public void SaveToCsvForMC(MieTransSheetInfo transSheetInfo, string path)
        {
            MieTransSheetDao.SaveToCsvForMC(transSheetInfo, path);
        }
    }
}
