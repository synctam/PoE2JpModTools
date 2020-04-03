namespace MieTranslationLib.Data.CharacterMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NLog;

    public class MieCharacterAttributeFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public enum NGender
        {
            Male = 0,
            Female = 1,
            Neuter = 2,

            /// <summary>
            /// 不明
            /// </summary>
            Unknown = 10000,
        }

        public IDictionary<Guid, MieCharacterAttributeEntry> CharacterItems { get; } = new Dictionary<Guid, MieCharacterAttributeEntry>();

        public IDictionary<Guid, MieSpeakerAttributeEntry> SpeakerItems { get; } = new Dictionary<Guid, MieSpeakerAttributeEntry>();

        public IDictionary<Guid, MieRaceAttributeEntry> RaceItems { get; } = new Dictionary<Guid, MieRaceAttributeEntry>();

        public void AddCharacterAttributeEntry(MieCharacterAttributeEntry entry)
        {
            if (this.CharacterItems.ContainsKey(entry.ID))
            {
                var msg = $"Duplicate GUID({entry.ID})";
                throw new Exception(msg);
            }
            else
            {
                this.CharacterItems.Add(entry.ID, entry);
            }
        }

        /// <summary>
        /// SpeakerGuidからキャラクター属性を返す。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>キャラクター属性</returns>
        public MieCharacterAttributeEntry GetCharacterAttributeEntryBySpeakerID(Guid guid)
        {
            if (guid.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                return null;
            }

            foreach (var entry in this.CharacterItems.Values)
            {
                if (entry.SpeakerID.Equals(guid))
                {
                    return entry;
                }
            }

            foreach (var speakerEntry in this.SpeakerItems.Values)
            {
                if (speakerEntry.ID.Equals(guid))
                {
                    return new MieCharacterAttributeEntry(speakerEntry.ID, speakerEntry.Name, speakerEntry.Gender, default(Guid), default(Guid));
                }
            }

            return null;
        }

        public string ToCharacterString()
        {
            StringBuilder buff = new StringBuilder();

            this.CharacterItems.Values
                .OrderBy(entry => entry.Name)
                .ToList()
                .ForEach(entry => buff.AppendLine(entry.ToString()));

            return buff.ToString();
        }

        public string ToSpeakerString()
        {
            StringBuilder buff = new StringBuilder();

            this.SpeakerItems.Values
                .OrderBy(entry => entry.Name)
                .ToList()
                .ForEach(entry => buff.AppendLine(entry.ToString()));

            return buff.ToString();
        }

        public string ToRaceString()
        {
            StringBuilder buff = new StringBuilder();

            this.RaceItems.Values
                .OrderBy(entry => entry.Name)
                .ToList()
                .ForEach(entry => buff.Append(entry.ToString()));

            return buff.ToString();
        }

        public void AddSpeakerEntry(MieSpeakerAttributeEntry entry)
        {
            if (this.SpeakerItems.ContainsKey(entry.ID))
            {
                var msg = $"Duplicate GUID({entry.ID})";
                logger.Error(msg);
                Console.WriteLine(msg);
                ////throw new Exception(msg);
            }
            else
            {
                this.SpeakerItems.Add(entry.ID, entry);
            }
        }

        public MieSpeakerAttributeEntry GetSpeakerAttributeEntry(Guid guid)
        {
            if (this.SpeakerItems.ContainsKey(guid))
            {
                var result = this.SpeakerItems[guid];

                return result;
            }
            else
            {
                return null;
            }
        }

        public void AddRaceEntry(MieRaceAttributeEntry entry)
        {
            if (this.RaceItems.ContainsKey(entry.RaceID))
            {
                var msg = $"Duplicate GUID({entry.RaceID})";
                logger.Error(msg);
                Console.WriteLine(msg);
                ////throw new Exception(msg);
            }
            else
            {
                this.RaceItems.Add(entry.RaceID, entry);
            }
        }

        public MieRaceAttributeEntry GetRaceAttributeEntry(Guid guid)
        {
            if (this.RaceItems.ContainsKey(guid))
            {
                var result = this.RaceItems[guid];

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
