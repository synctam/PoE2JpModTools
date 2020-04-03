namespace MieTranslationLib.Data.Language
{
    using System;
    using System.Collections.Generic;
    using NLog;

    /// <summary>
    /// ファイル属性情報
    /// </summary>
    public class MieLanguageFileAttributeInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ファイル属性の辞書。キーはFileCode。
        /// </summary>
        public IDictionary<long, MieLanguageFileAttribute> Items { get; } = new Dictionary<long, MieLanguageFileAttribute>();

        /// <summary>
        /// ファイル属性を追加する。
        /// </summary>
        /// <param name="mieLanguageFileAttribute">ファイル属性</param>
        /// <returns>追加したファイル属性</returns>
        public MieLanguageFileAttribute AddAttribute(MieLanguageFileAttribute mieLanguageFileAttribute)
        {
            if (this.Items.ContainsKey(mieLanguageFileAttribute.FileCode))
            {
                throw new Exception($"Duplicate FileID({mieLanguageFileAttribute.FileCode}).");
            }
            else
            {
                this.Items.Add(mieLanguageFileAttribute.FileCode, mieLanguageFileAttribute);
                return mieLanguageFileAttribute;
            }
        }

        /// <summary>
        /// 指定したFileIDのファイル属性を返す。
        /// </summary>
        /// <param name="fileCode">FileCode</param>
        /// <returns>ファイル属性</returns>
        public MieLanguageFileAttribute GetAttribute(long fileCode)
        {
            if (this.Items.ContainsKey(fileCode))
            {
                return this.Items[fileCode];
            }
            else
            {
                var newAttribute = new MieLanguageFileAttribute(fileCode, string.Empty);
                this.AddAttribute(newAttribute);
                logger.Trace($"Create attribute. FileID({fileCode}).");

                return newAttribute;
            }
        }
    }
}
