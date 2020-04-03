namespace MieTranslationLib.Data.CharacterMap
{
    using System;

    public class MieCharacterAttributeEntry
    {
        public MieCharacterAttributeEntry(Guid id, string name, MieCharacterAttributeFile.NGender gender, Guid speakerID, Guid race)
        {
            this.ID = id;
            this.Name = name;
            this.Gender = gender;
            this.SpeakerID = speakerID;
            this.RaceID = race;
        }

        private MieCharacterAttributeEntry() { }

        public Guid ID { get; }

        public string Name { get; }

        public MieCharacterAttributeFile.NGender Gender { get; }

        public Guid SpeakerID { get; }

        public Guid RaceID { get; }

        public override string ToString()
        {
            return $"ID({this.ID}) Name({this.Name}) Gender({this.Gender}) SpeakerID({this.SpeakerID}) RaceID({this.RaceID.ToString()})";
        }
    }
}
