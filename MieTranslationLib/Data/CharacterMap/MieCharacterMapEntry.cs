namespace MieTranslationLib.Data.CharacterMap
{
    using System;

    /// <summary>
    /// キャラクターマップ エントリ
    /// </summary>
    public class MieCharacterMapEntry
    {
        public MieCharacterMapEntry(Guid guid)
        {
            this.CharacterID = guid;
        }

        public Guid CharacterID { get; }

        public override string ToString()
        {
            return $"{this.CharacterID.ToString()}";
        }
    }
}
