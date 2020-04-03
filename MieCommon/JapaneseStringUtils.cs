namespace S5mCommon_1F1F6148_9E9B_4F66_AEB6_EB749A40E94E
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class JapaneseStringUtils
    {
        /// <summary>
        /// 日本語処理関連汎用関数
        /// </summary>
        public class TJapaneseStringUtils
        {
            /// <summary>
            /// 日本語と日本語の間に指定した文字を挿入し、挿入後のテキストを返す。
            /// </summary>
            /// <param name="text">処理対象のテキスト</param>
            /// <param name="sp">挿入する文字</param>
            /// <returns>挿入後のテキスト</returns>
            public static string InsertSpace(string text, char sp)
            {
                StringBuilder buff = new StringBuilder();

                bool isJapaneseMode = false;
                char prevChar = '\0';
                foreach (char c in text)
                {
                    if (isJapaneseMode && IsNihongo(c))
                    {
                        //// prev:JP next:JP = スペースを挿入する。
                        buff.Append(sp);

                        buff.Append(c);
                        isJapaneseMode = true;
                    }
                    else if (!isJapaneseMode && IsNihongo(c))
                    {
                        //// prev:EN next:JP
                        buff.Append(c);
                        isJapaneseMode = true;
                    }
                    else if (isJapaneseMode && !IsNihongo(c))
                    {
                        //// prev:JP next:EN
                        if (c == ' ')
                        {
                            //// 日本語に次にある半角スペースは「EN SP」に変更する。
                            buff.Append('\u2002');
                            isJapaneseMode = true;
                        }
                        else
                        {
                            buff.Append(c);
                            isJapaneseMode = false;
                        }
                    }
                    else if (!isJapaneseMode && !IsNihongo(c))
                    {
                        //// prev:EN next:EN
                        buff.Append(c);
                        isJapaneseMode = false;
                    }

                    prevChar = c;
                }

                return buff.ToString();
            }

            /// <summary>
            /// 指定したテキストに含まれる日本語の文字数を返す。
            /// </summary>
            /// <param name="text">テキスト</param>
            /// <returns>日本語の文字数</returns>
            public static int NumJapaneseChars(string text)
            {
                int count = 0;
                foreach (var c in text)
                {
                    if (IsNihongo(c))
                    {
                        count++;
                    }
                }

                return count;
            }

            /// <summary>
            /// 指定したテキストに日本語が含まれているかどうかを判定する。
            /// </summary>
            /// <param name="text">テキスト</param>
            /// <returns>true: 日本語が含まれる。</returns>
            public static bool ContainsJapanese(string text)
            {
                foreach (char c in text)
                {
                    if (IsNihongo(c))
                    {
                        return true;
                    }
                    else
                    {
                        // 日本語じゃないのでスキップ
                    }
                }

                return false;
            }

            /// <summary>
            /// 指定した文字が日本語化どうかを判定する。
            /// </summary>
            /// <param name="c">文字</param>
            /// <returns>true: 日本語</returns>
            public static bool IsNihongo(char c)
            {
                if ((c == '\r') || (c == '\n') || (c == '\t'))
                {
                    // 日本語じゃないのでスキップ
                }
                else
                {
                    if (IsHiragana(c) || IsKanji(c) || IsKatakana(c))
                    {
                        return true;
                    }
                    else
                    {
                        // 日本語じゃないのでスキップ
                    }
                }

                return false;
            }

            /// <summary>
            /// 指定した文字が「カタカナ」かどうかを判定する。
            /// </summary>
            /// <param name="c">文字</param>
            /// <returns>カタカナの場合はtrue</returns>
            private static bool IsKatakana(char c)
            {
                return Regex.IsMatch(c.ToString(), @"^\p{IsKatakana}*$");
            }

            /// <summary>
            /// 指定した文字が「平仮名」かどうかを判定する。
            /// </summary>
            /// <param name="c">文字</param>
            /// <returns>平仮名の場合はtrue</returns>
            private static bool IsHiragana(char c)
            {
                return Regex.IsMatch(c.ToString(), @"^\p{IsHiragana}*$");
            }

            /// <summary>
            /// 指定した文字が「漢字」かどうかを判定する。
            /// </summary>
            /// <param name="c">文字</param>
            /// <returns>漢字の場合はtrue</returns>
            private static bool IsKanji(char c)
            {
                string matchingText =
                    @"[\p{IsCJKUnifiedIdeographs}" +
                    @"\p{IsCJKCompatibilityIdeographs}" +
                    @"\p{IsCJKUnifiedIdeographsExtensionA}]|" +
                    @"[\uD840-\uD869][\uDC00-\uDFFF]|\uD869[\uDC00-\uDEDF]";

                if (System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), matchingText))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
