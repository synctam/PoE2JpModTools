// ******************************************************************************
// Copyright (c) 2015-2019 synctam
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace MonoOptions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mono.Options;

    /// <summary>
    /// コマンドライン オプション
    /// </summary>
    public class TOptions
    {
        //// ******************************************************************************
        //// Property fields
        //// ******************************************************************************
        private TArgs args;
        private bool isError = false;
        private StringWriter errorMessage = new StringWriter();
        private OptionSet optionSet;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arges">コマンドライン引数</param>
        public TOptions(string[] arges)
        {
            this.args = new TArgs();
            this.Settings(arges);
            if (this.IsError)
            {
                this.ShowErrorMessage();
                this.ShowUsage();
            }
            else
            {
                this.CheckOption();
                if (this.IsError)
                {
                    this.ShowErrorMessage();
                    this.ShowUsage();
                }
                else
                {
                    // skip
                }
            }
        }

        //// ******************************************************************************
        //// Property
        //// ******************************************************************************

        /// <summary>
        /// コマンドライン オプション
        /// </summary>
        public TArgs Arges { get { return this.args; } }

        /// <summary>
        /// コマンドライン オプションのエラー有無
        /// </summary>
        public bool IsError { get { return this.isError; } }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage.ToString(); } }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        public void ShowUsage()
        {
            TextWriter writer = Console.Error;
            this.ShowUsage(writer);
        }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowUsage(TextWriter textWriter)
        {
            StringWriter msg = new StringWriter();

            string exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            msg.WriteLine(string.Empty);
            msg.WriteLine($@"使い方：");
            msg.WriteLine($@"日本語化MODを作成する。");
            msg.WriteLine(
                $@"  usage: {exeName} -i <original lang folder path> -o <japanized lang folder path>" +
                $@" -s <Trans Sheet path> [-m] [-r]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  翻訳シート(-s)とオリジナルの言語フォルダー(-i)から日本語の言語フォルダーに日本語化MOD(-o)を作成する。");
            msg.WriteLine(
                $@"    {exeName} -i Localisation\EN -o Localisation\JP" +
                $@" -s SuTransSheet.csv");
            msg.WriteLine($@"終了コード:");
            msg.WriteLine($@" 0  正常終了");
            msg.WriteLine($@" 1  異常終了");
            msg.WriteLine();

            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(msg.ToString());
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        public void ShowErrorMessage()
        {
            TextWriter writer = Console.Error;
            this.ShowErrorMessage(writer);
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowErrorMessage(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(this.ErrorMessage);
        }

        /// <summary>
        /// オプション文字の設定
        /// </summary>
        /// <param name="args">args</param>
        private void Settings(string[] args)
        {
            this.optionSet = new OptionSet()
            {
                { "c|conv="    , this.args.FileNameConvasationText , v => this.args.FileNameConvasation = v},
                { "s|system="  , this.args.FileNameSystemText      , v => this.args.FileNameSystem      = v},
                { "t|chatter=" , this.args.FileNameChatterText     , v => this.args.FileNameChatter     = v},
                { "d|db="      , this.args.FileNameSystemDBText    , v => this.args.FileNameSystemDB    = v},
                { "o|out="     , this.args.FolderNameOutText       , v => this.args.FolderNameOut       = v},
                { "r"          , this.args.UseReplaceText          , v => this.args.UseReplace          = v != null},
                { "h|help"     , "ヘルプ"                          , v => this.args.Help                = v != null},
            };

            List<string> extra;
            try
            {
                extra = this.optionSet.Parse(args);
                if (extra.Count > 0)
                {
                    // 指定されたオプション以外のオプションが指定されていた場合、
                    // extra に格納される。
                    // 不明なオプションが指定された。
                    this.SetErrorMessage($"{Environment.NewLine}エラー：不明なオプションが指定されました。");
                    extra.ForEach(t => this.SetErrorMessage(t));
                    this.isError = true;
                }
            }
            catch (OptionException e)
            {
                ////パースに失敗した場合OptionExceptionを発生させる
                this.SetErrorMessage(e.Message);
                this.isError = true;
            }
        }

        /// <summary>
        /// オプションのチェック
        /// </summary>
        private void CheckOption()
        {
            //// -h
            if (this.Arges.Help)
            {
                this.SetErrorMessage();
                this.isError = false;
                return;
            }

            if (this.IsErrorSheetChatterFiles())
            {
                return;
            }

            if (this.IsErrorSheetSystemFiles())
            {
                return;
            }

            if (this.IsErrorSheetConvFiles())
            {
                return;
            }

            if (this.IsErrorSystemDbFile())
            {
                return;
            }

            if (this.IsErrorOutFolder())
            {
                return;
            }

            this.isError = false;
            return;
        }

        private bool IsErrorOutFolder()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FolderNameOut))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-o)出力フォルダーのパスを指定してください。");
                this.isError = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// 翻訳シート（Chatter）の有無を確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorSheetChatterFiles()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameChatter))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-t)翻訳シート（Chatter）のパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameChatter))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-t)翻訳シート（chatter）が見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameChatter)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 翻訳シート（会話）の有無を確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorSheetConvFiles()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameConvasation))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-c)翻訳シート（会話）のパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameConvasation))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-c)翻訳シート（会話）が見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameConvasation)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 翻訳シート（システム）の有無を確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorSheetSystemFiles()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSystem))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-s)翻訳シート（システム）のパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameSystem))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-s)翻訳シート（システム）が見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameSystem)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private bool IsErrorSystemDbFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSystemDB))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}エラー：" +
                    $"(-d)SystemDBのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameSystemDB))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：" +
                        $"(-d)SystemDBが見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameSystemDB)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private void SetErrorMessage(string errorMessage = null)
        {
            if (errorMessage != null)
            {
                this.errorMessage.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// オプション項目
        /// </summary>
        public class TArgs
        {
            public string FileNameConvasation { get; internal set; }

            public string FileNameConvasationText { get; internal set; } = $"翻訳シート（会話）のパス。";

            public string FileNameSystem { get; internal set; }

            public string FileNameSystemText { get; internal set; } = $"翻訳シート（システム）のパス。";

            public string FileNameChatter { get; internal set; }

            public string FileNameChatterText { get; internal set; } = $"翻訳シート（Chatter）のパス。";

            public string FileNameSystemDB { get; internal set; }

            public string FileNameSystemDBText { get; internal set; } = $"SystemDBのパス。";

            public string FolderNameOut { get; internal set; }

            public string FolderNameOutText { get; internal set; } = $"出力フォルダーのパス";

            public bool UseReplace { get; internal set; }

            public string UseReplaceText { get; internal set; } =
                $"出力用言語ファイルが既に存在する場合はを上書きする。";

            public bool Help { get; set; }
        }
    }
}
