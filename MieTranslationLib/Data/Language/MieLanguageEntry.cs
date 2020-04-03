namespace MieTranslationLib.Data.Language
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using MieTranslationLib.Product;
    using NLog;

    /// <summary>
    /// 言語エントリー
    /// </summary>
    public class MieLanguageEntry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieLanguageEntry(int id, string defaultText, string femaleText, MieProduct.NProductLine productLine, long referenceID, DateTime updateDateTime)
        {
            this.ID = id;
            this.DefaultText = defaultText;
            this.FemaleText = femaleText;
            this.ProductLine = productLine;
            this.UpdatedAt = updateDateTime;
            if (string.IsNullOrEmpty(this.FemaleText))
            {
                this.HasFemale = false;
            }
            else
            {
                this.HasFemale = true;
            }

            this.ReferenceID = referenceID;
        }

        public MieLanguageEntry(int id, string defaultText, string femaleText, MieProduct.NProductLine productLine, long referenceID)
            : this(id, defaultText, femaleText, productLine, referenceID, DateTime.UtcNow)
        {
        }

        private MieLanguageEntry() { }

        /// <summary>
        /// エントリーID
        /// </summary>
        public int ID { get; } = -1;

        /// <summary>
        /// 標準テキスト
        /// </summary>
        public string DefaultText { get; } = string.Empty;

        /// <summary>
        /// 女性用テキスト
        /// </summary>
        public string FemaleText { get; } = string.Empty;

        /// <summary>
        /// 女性用テキストの有無。
        /// </summary>
        public bool HasFemale { get; } = false;

        /// <summary>
        /// 製品区分
        /// </summary>
        public MieProduct.NProductLine ProductLine { get; } = MieProduct.NProductLine.None;

        /// <summary>
        /// ReferenceID
        /// </summary>
        public long ReferenceID { get; }

        /// <summary>
        /// 更新日
        /// </summary>
        public DateTime UpdatedAt { get; }

        /// <summary>
        /// エントリーのクローンを作成する。
        /// </summary>
        /// <returns>クローン</returns>
        [Obsolete("使用禁止")]
        public MieLanguageEntry Clone()
        {
            var mieLanguageEntry = new MieLanguageEntry(this.ID, this.DefaultText, this.FemaleText, this.ProductLine, this.ReferenceID, this.UpdatedAt);

            return mieLanguageEntry;
        }

        /// <summary>
        /// このオブジェクトをテキスト化する。DefaultTextは先頭の１０文字のみを出力する。
        /// </summary>
        /// <param name="isShortText">短縮形式</param>
        /// <returns>オブジェクトのテキスト</returns>
        public string ToString(bool isShortText)
        {
            StringBuilder buff = new StringBuilder();

            var text = this.DefaultText;
            if (isShortText)
            {
                text = (this.DefaultText + new string(' ', 10)).Substring(0, 10).TrimEnd() + "...";
            }

            buff.AppendLine($"\tID({this.ID}) ProductLine({this.ProductLine}) ReferenceID({this.ReferenceID}) Date({this.UpdatedAt.ToLocalTime()}) Text({text})");

            return buff.ToString();
        }

        /// <summary>
        /// テキスト中の属性文字を除去する。
        /// 属性文字とはタグ括弧で括られた部分です。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="useTagedLineFeed">改行をTAG化する</param>
        /// <returns>属性文字列を除去したテキスト</returns>
        public string GetPlainText(string text, bool useTagedLineFeed)
        {
            StringBuilder buff = new StringBuilder(text);

            MatchCollection mc = Regex.Matches(text, @"<.*?>", RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                buff.Replace(m.Value, string.Empty);
            }

            if (useTagedLineFeed)
            {
                buff.Replace("\r\n", "<LF>").Replace("\n", "<LF>").Replace("\r", "<LF>");
            }

            return buff.ToString();
        }
    }
}
