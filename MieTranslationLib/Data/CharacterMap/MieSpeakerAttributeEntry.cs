namespace MieTranslationLib.Data.CharacterMap
{
    using System;

    public class MieSpeakerAttributeEntry
    {
        public MieSpeakerAttributeEntry(Guid id, string name, MieCharacterAttributeFile.NGender gender)
        {
            this.ID = id;
            this.Name = name;
            this.Gender = gender;
        }

        private MieSpeakerAttributeEntry() { }

        public Guid ID { get; }

        public string Name { get; }

        public MieCharacterAttributeFile.NGender Gender { get; }

        public override string ToString()
        {
            return $"ID({this.ID}) Name({this.Name}) Gender({this.Gender})";
        }
    }
}
