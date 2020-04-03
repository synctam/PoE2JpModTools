namespace MieTranslationLib.Data.CharacterMap
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NLog;

    public class MieCharacterMapFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// キャラクターマップ エントリーの辞書。キーはGUID。
        /// </summary>
        private IDictionary<Guid, MieCharacterMapEntry> Items { get; } = new SortedDictionary<Guid, MieCharacterMapEntry>();

        /// <summary>
        /// キャラクター エントリーを追加する。
        /// </summary>
        /// <param name="characterEntry">キャラクター エントリー</param>
        public void AddEntry(MieCharacterMapEntry characterEntry)
        {
            if (this.Items.ContainsKey(characterEntry.CharacterID))
            {
                var msg = $"Duplicate guid({characterEntry.CharacterID})";
                logger.Warn(msg);
                Console.WriteLine(msg);
            }
            else
            {
                this.Items.Add(characterEntry.CharacterID, characterEntry);
            }
        }

        /// <summary>
        /// 指定したキャラクターファイルを追加する。
        /// </summary>
        /// <param name="characterMapFile">キャラクターファイル</param>
        public void MargeFile(MieCharacterMapFile characterMapFile)
        {
            foreach (var newEntry in characterMapFile.Items.Values)
            {
                this.AddEntry(newEntry);
            }
        }

        /// <summary>
        /// 指定したGUIDのキャラクターエントリーを返す。
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <returns>キャラクターエントリー</returns>
        public MieCharacterMapEntry GetEntry(Guid guid)
        {
            if (this.Items.ContainsKey(guid))
            {
                return this.Items[guid];
            }
            else
            {
                throw new Exception($"Map entry not found. GUID({guid.ToString()})");
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            foreach (var entry in this.Items.Values)
            {
                buff.AppendLine($"\t{entry.ToString()}");
            }

            return buff.ToString();
        }
    }
}
