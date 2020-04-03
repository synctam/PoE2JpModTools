namespace MieDbLib.SystemDB
{
    using System.IO;
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Data.Chatter;
    using MieTranslationLib.Data.Conversations;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.Data.Quests;

    public class MieSystemDbApp
    {
        public MieCharacterAttributeFile CharacterAttributeFile { get; private set; } = null;

        public MieLanguageInfo LanguageInfo { get; private set; } = null;

        public MieFileList FileList { get; private set; } = null;

        public MieConversationNodeInfo ConversationInfo { get; private set; } = null;

        public MieQuestsNodeInfo QuestsInfo { get; private set; } = null;

        public MieChatterNodeInfo ChatterInfo { get; private set; } = null;

        public void LoadFromDB(MieSystemDB systemDb)
        {
            //// キャラクター情報の作成。
            {
                this.CharacterAttributeFile = new MieCharacterAttributeFile();
                MieTableCharacterAttributesDao.LoadFromSystemDB(systemDb, this.CharacterAttributeFile);
                MieTableSpeakerAttributesDao.LoadFromSystemDB(systemDb, this.CharacterAttributeFile);
                MieTableRaceAttributesDao.LoadFromSystemDB(systemDb, this.CharacterAttributeFile);
            }

            //// 言語情報を作成。
            {
                this.LanguageInfo = new MieLanguageInfo();
                MieTableLanguageDao.LoadFromSystemDB(systemDb, this.LanguageInfo);
                this.FileList = MieTableFileListDao.LoadFromSystemDB(systemDb);
            }

            //// 会話ノード情報を作成。
            {
                this.ConversationInfo = new MieConversationNodeInfo();
                MieTableConversationNodeLinksDao.LoadFromSystemDB(systemDb, this.ConversationInfo);
                MieTableConversationEntriesDao.LoadFromSystemDB(systemDb, this.ConversationInfo);
            }

            //// クエストノード情報を作成。
            {
                this.QuestsInfo = new MieQuestsNodeInfo();
                MieTableQuestsNodeLinksDao.LoadFromSystemDB(systemDb, this.QuestsInfo);
                MieTableQuestsEntriesDao.LoadFromSystemDB(systemDb, this.QuestsInfo);
            }

            //// チャッターノード情報を作成。
            {
                this.ChatterInfo = new MieChatterNodeInfo();
                MieTableChatterNodeLinksDao.LoadFromSystemDB(systemDb, this.ChatterInfo);
                MieTableChatterEntriesDao.LoadFromSystemDB(systemDb, this.ChatterInfo);
            }
        }

        public void ToCharacterAttributeString(string path)
        {
            File.WriteAllText(path, this.CharacterAttributeFile.ToCharacterString(), Encoding.UTF8);
        }

        public void ToSpeakerAttributeString(string path)
        {
            File.WriteAllText(path, this.CharacterAttributeFile.ToSpeakerString(), Encoding.UTF8);
        }

        public void ToRaceAttributeString(string path)
        {
            File.WriteAllText(path, this.CharacterAttributeFile.ToRaceString(), Encoding.UTF8);
        }

        public void ToLanguageString(string path)
        {
            File.WriteAllText(path, this.LanguageInfo.ToString(true), Encoding.UTF8);
        }

        public void ToFileListString(string path)
        {
            File.WriteAllText(path, this.FileList.ToString(), Encoding.UTF8);
        }

        public void ToConversationLinksFromToString(string path)
        {
            File.WriteAllText(path, this.ConversationInfo.ToLinksFromTo(), Encoding.UTF8);
        }

        public void ToConversationLinkString(string path, bool viewStopNode)
        {
            File.WriteAllText(path, this.ConversationInfo.ToLinkString(viewStopNode), Encoding.UTF8);
        }

        public void ToConversationNodeStringString(string path)
        {
            File.WriteAllText(path, this.ConversationInfo.ToNodeString(), Encoding.UTF8);
        }

        public void ToQuestsLinksFromToString(string path)
        {
            File.WriteAllText(path, this.QuestsInfo.ToLinksFromTo(), Encoding.UTF8);
        }

        public void ToQuestsLinkString(string path, bool viewStopNode)
        {
            File.WriteAllText(path, this.QuestsInfo.ToLinkString(viewStopNode), Encoding.UTF8);
        }

        public void ToChatterLinksFromToString(string path)
        {
            File.WriteAllText(path, this.ChatterInfo.ToLinksFromTo(), Encoding.UTF8);
        }

        public void ToChatterLinkString(string path, bool viewStopNode)
        {
            File.WriteAllText(path, this.ChatterInfo.ToLinkString(viewStopNode), Encoding.UTF8);
        }
    }
}
