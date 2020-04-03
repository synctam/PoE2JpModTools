namespace MieTranslationLib.Product
{
    using System.ComponentModel;

    public class MieProduct
    {
        /// <summary>
        /// 言語タイプ
        /// </summary>
        public enum NLanguageType
        {
            Conversations,
            Game,
            Quests,
            Chatter,
        }

        /// <summary>
        /// 製品タイプ
        /// </summary>
        public enum NProductLine
        {
            None /******/ = 0b0000_0000_0000_0000_0000_0000_0000_0000,
            Vanilla/****/ = 0b0000_0000_0000_0000_0000_0000_0000_0010,
            DLC1/*******/ = 0b0000_0000_0000_0000_0000_0000_0000_0100,
            DLC2/*******/ = 0b0000_0000_0000_0000_0000_0000_0000_1000,
            DLC3/*******/ = 0b0000_0000_0000_0000_0000_0000_0001_0000,
            DLC4/*******/ = 0b0000_0000_0000_0000_0000_0000_0010_0000,
            DLC5/*******/ = 0b0000_0000_0000_0000_0000_0000_0100_0000,

            LaxA/*******/ = 0b0000_0000_0000_0000_0001_0000_0000_0000,
            LaxB/*******/ = 0b0000_0000_0000_0000_0010_0000_0000_0000,
            LaxC/*******/ = 0b0000_0000_0000_0000_0100_0000_0000_0000,
            LaxD/*******/ = 0b0000_0000_0000_0000_1000_0000_0000_0000,
            LaxE/*******/ = 0b0000_0000_0000_0001_0000_0000_0000_0000,
            LaxF/*******/ = 0b0000_0000_0000_0010_0000_0000_0000_0000,
            LaxG/*******/ = 0b0000_0000_0000_0100_0000_0000_0000_0000,
            LaxH/*******/ = 0b0000_0000_0000_1000_0000_0000_0000_0000,
            LaxI/*******/ = 0b0000_0000_0001_0000_0000_0000_0000_0000,

            ALL/********/ = 0b0111_1111_1111_1111_1111_1111_1111_1111,
        }

        public static NProductLine GetProductLineFromText(string text)
        {
            switch (text.ToLower().Trim())
            {
                case "vanilla":
                    return NProductLine.Vanilla;
                case "laxa":
                    return NProductLine.LaxA;
                case "laxb":
                    return NProductLine.LaxB;
                case "laxc":
                    return NProductLine.LaxC;
                case "laxd":
                    return NProductLine.LaxD;
                case "laxe":
                    return NProductLine.LaxE;
                case "laxf":
                    return NProductLine.LaxF;
                case "laxg":
                    return NProductLine.LaxG;
                case "laxh":
                    return NProductLine.LaxH;
                case "laxi":
                    return NProductLine.LaxI;
                case "dlc1":
                    return NProductLine.DLC1;
                case "dlc2":
                    return NProductLine.DLC2;
                case "dlc3":
                    return NProductLine.DLC3;
                case "dlc4":
                    return NProductLine.DLC4;
                case "dlc5":
                    return NProductLine.DLC5;
                default:
                    var msg = $"Unknown ProductLine text({text})";
                    throw new InvalidEnumArgumentException(msg);
            }
        }
    }
}
