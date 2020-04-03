namespace MieTranslationLib.Data.CharacterMap
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 特殊なキャラクターGUID
    /// 用語集で置き換え可能なため、不要かもしれない。
    /// </summary>
    public class MieCharacterSpecial
    {
        public MieCharacterSpecial()
        {
            this.Items.Add(new Guid("12345678-1234-1234-1234-123456789abc"), NSpecialCharacterType.Invalid);
            this.Items.Add(new Guid("b1a8e901-0000-0000-0000-000000000000"), NSpecialCharacterType.Player);
            this.Items.Add(new Guid("c1a11e91-0000-0000-0000-000000000000"), NSpecialCharacterType.Chatter);
            this.Items.Add(new Guid("6a99a109-0000-0000-0000-000000000000"), NSpecialCharacterType.Narrator);
            this.Items.Add(new Guid("011111e9-0000-0000-0000-000000000000"), NSpecialCharacterType.Owner);
            this.Items.Add(new Guid("5bea12e9-0000-0000-0000-000000000000"), NSpecialCharacterType.Speaker);
            this.Items.Add(new Guid("7d150000-0000-0000-0000-000000000000"), NSpecialCharacterType.This);
            this.Items.Add(new Guid("1a26e100-0000-0000-0000-000000000000"), NSpecialCharacterType.Target);
            this.Items.Add(new Guid("1a26e120-0000-0000-0000-000000000000"), NSpecialCharacterType.MainTarget);
            this.Items.Add(new Guid("005e9000-0000-0000-0000-000000000000"), NSpecialCharacterType.User);
            this.Items.Add(new Guid("b1a7e000-0000-0000-0000-000000000000"), NSpecialCharacterType.Party);
            this.Items.Add(new Guid("b1a7ea77-0000-0000-0000-000000000000"), NSpecialCharacterType.PartyAll);
            this.Items.Add(new Guid("b1a7ea1e-0000-0000-0000-000000000000"), NSpecialCharacterType.PartyAny);
            this.Items.Add(new Guid("039202e3-0000-0000-0000-000000000000"), NSpecialCharacterType.Hovered);
            this.Items.Add(new Guid("5426ab73-0000-0000-0000-000000000000"), NSpecialCharacterType.Listener);
            this.Items.Add(new Guid("2b850000-0000-0000-0000-000000000000"), NSpecialCharacterType.AnimalCompanion);
            this.Items.Add(new Guid("2b850001-0000-0000-0000-000000000000"), NSpecialCharacterType.AnimalCompanionMaster);
            this.Items.Add(new Guid("51070000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot0);
            this.Items.Add(new Guid("51071000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot1);
            this.Items.Add(new Guid("51072000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot2);
            this.Items.Add(new Guid("51073000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot3);
            this.Items.Add(new Guid("51074000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot4);
            this.Items.Add(new Guid("51075000-0000-0000-0000-000000000000"), NSpecialCharacterType.Slot5);
            this.Items.Add(new Guid("4e3d0000-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified0);
            this.Items.Add(new Guid("4e3d0001-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified1);
            this.Items.Add(new Guid("4e3d0002-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified2);
            this.Items.Add(new Guid("4e3d0003-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified3);
            this.Items.Add(new Guid("4e3d0004-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified4);
            this.Items.Add(new Guid("4e3d0005-0000-0000-0000-000000000000"), NSpecialCharacterType.Specified5);
            this.Items.Add(new Guid("6dcee000-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck0);
            this.Items.Add(new Guid("6dcee001-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck1);
            this.Items.Add(new Guid("6dcee002-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck2);
            this.Items.Add(new Guid("6dcee003-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck3);
            this.Items.Add(new Guid("6dcee004-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck4);
            this.Items.Add(new Guid("6dcee005-0000-0000-0000-000000000000"), NSpecialCharacterType.SkillCheck5);
        }

        public enum NSpecialCharacterType
        {
            Invalid,
            Player,
            Chatter,
            Narrator,
            Owner,
            Speaker,
            This,
            Target,
            MainTarget,
            User,
            Party,
            PartyAll,
            PartyAny,
            Hovered,
            Listener,
            AnimalCompanion,
            AnimalCompanionMaster,
            Slot0,
            Slot1,
            Slot2,
            Slot3,
            Slot4,
            Slot5,
            Specified0,
            Specified1,
            Specified2,
            Specified3,
            Specified4,
            Specified5,
            SkillCheck0,
            SkillCheck1,
            SkillCheck2,
            SkillCheck3,
            SkillCheck4,
            SkillCheck5,
        }

        public Dictionary<Guid, NSpecialCharacterType> Items { get; } = new Dictionary<Guid, NSpecialCharacterType>();

        public NSpecialCharacterType GetSpecialCharacterType(Guid guid)
        {
            if (this.Items.ContainsKey(guid))
            {
                return this.Items[guid];
            }
            else
            {
                var msg = $"Unknown special character guid({guid.ToString()})";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }
        }
    }
}
