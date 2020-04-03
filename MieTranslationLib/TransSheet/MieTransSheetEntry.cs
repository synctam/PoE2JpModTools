namespace MieTranslationLib.TransSheet
{
    using System;
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using MieTranslationLib.Product;
    using NLog;

    public class MieTransSheetEntry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieTransSheetEntry() { }

        public MieTransSheetEntry(
            string fileID,
            int id,
            string defaultText,
            string femaleText)
        {
            this.FileID = fileID;
            this.ID = id;
            this.DefaultText = defaultText;
            this.FemaleText = femaleText;
        }

        public enum NMaintenanceType
        {
            None,
            Added,
            Deleted,
            Updated,
        }

        public string Scene { get; set; } = string.Empty;

        public string SpeakerName { get; set; } = string.Empty;

        public string ListenerName { get; set; } = string.Empty;

        public string FileID { get; set; } = string.Empty;

        public string FileIDExt
        {
            get
            {
                return $"{this.FileID}:{this.SortKey:00000}:{this.ID}:{this.ReferenceID}";
            }

            set
            {
                var fields = value.Split(':');
                this.FileID = fields[0];
                this.SortKey = Convert.ToInt32(fields[1].TrimStart('0'));
                this.ID = Convert.ToInt32(fields[2]);
                this.ReferenceID = fields[3];
            }
        }

        public int ID { get; set; } = -1;

        public string DefaultText { get; set; } = string.Empty;

        public string DefaultTranslationText { get; set; } = string.Empty;

        public string FemaleText { get; set; } = string.Empty;

        public string FemaleTranslationText { get; set; } = string.Empty;

        public string MachineTranslation { get; set; } = string.Empty;

        public string RaceName { get; set; } = string.Empty;

        public NMaintenanceType MaintenanceType { get; set; } = NMaintenanceType.None;

        /// <summary>
        /// 会話ツリーの階層の深さ。0: root, 1: 階層１
        /// </summary>
        public int Depth { get; set; } = 0;

        /// <summary>
        /// 会話ツリーの深さを示すテキスト。
        /// </summary>
        public string DisplayDepth { get; set; }

        public string ReferenceID { get; set; } = string.Empty;

        /// <summary>
        /// FileID毎の 1 から始まる連番。翻訳シートの並べ替え時に使用する。
        /// </summary>
        public int SortKey { get; set; } = 0;

        /// <summary>
        /// 製品区分
        /// </summary>
        public MieProduct.NProductLine ProductLine { get; set; } = MieProduct.NProductLine.None;

        /// <summary>
        /// 言語区分
        /// </summary>
        public MieProduct.NLanguageType LanguageType { get; set; } = MieProduct.NLanguageType.Conversations;

        /// <summary>
        /// Node type
        /// </summary>
        public string NodeType { get; set; } = string.Empty;

        public MieCharacterAttributeFile.NGender Gender { get; set; } = MieCharacterAttributeFile.NGender.Unknown;

        public string GenderText
        {
            get
            {
                if (this.Gender == MieCharacterAttributeFile.NGender.Unknown)
                {
                    return string.Empty;
                }
                else
                {
                    return this.Gender.ToString();
                }
            }
        }

        /// <summary>
        /// Depth値からツリーの階層を示す文字列を返す。
        /// Depth は 0 から始まるが、表示上は 1 から始める。
        /// depth=3 の場合は "___4" を返す。
        /// </summary>
        /// <returns>ツリーの階層を示す文字列</returns>
        public string GetDisplayDepth()
        {
            if (this.Depth < 0)
            {
                //// Zombie node は何も表示しない。
                return string.Empty;
            }
            else
            {
                //// Depth は 0 から始まるが、表示上は 1 から始める。
                int displayDepth = this.Depth;

                var depthText = new string('_', displayDepth + 1);
                return $"{depthText}{displayDepth}[{this.ID}]";
            }
        }

        public string Translate(string english, bool useMT, bool useReferenceID)
        {
            //// useMT |  e  |  j  | mt | result
            //// ------+-----+-----+----+--------
            ////   -   |  x  |  -  |  - | e
            ////   o   |  o  |  o  |  o | j
            ////   o   |  o  |  o  |  x | j
            ////   o   |  o  |  x  |  o | mt
            ////   o   |  o  |  x  |  x | e
            ////   x   |  o  |  o  |  - | j
            ////   x   |  o  |  x  |  - | e

            bool e = !string.IsNullOrWhiteSpace(english);
            bool j = !string.IsNullOrWhiteSpace(this.DefaultTranslationText);
            bool mt = !string.IsNullOrWhiteSpace(this.MachineTranslation);

            var referenceID = string.Empty;
            if (useReferenceID)
            {
                referenceID = $"{this.ReferenceID}:";
            }

            if (string.IsNullOrWhiteSpace(english))
            {
                return $"{referenceID}{english}";
            }

            //// 機械翻訳を利用する。
            if (useMT && e && j && mt)
            {
                return $"{referenceID}{this.DefaultTranslationText}";
            }
            else if (useMT && e && j && !mt)
            {
                return $"{referenceID}{this.DefaultTranslationText}";
            }
            else if (useMT && e && !j && mt)
            {
                return $"{referenceID}{this.MachineTranslation}";
            }
            else if (useMT && e && !j && !mt)
            {
                return $"{referenceID}{english}";
            }
            else if (!useMT && e && j)
            {
                return $"{referenceID}{this.DefaultTranslationText}";
            }
            else if (!useMT && e && !j)
            {
                return $"{referenceID}{english}";
            }
            else
            {
                throw new Exception("unknown error");
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FileID({this.FileID}) ID({this.ID})  DefaultText({this.DefaultText}) Trans({this.DefaultTranslationText})");

            return buff.ToString();
        }
    }
}