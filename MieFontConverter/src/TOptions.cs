// ******************************************************************************
// Copyright (c) 2015-2016 synctam
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
    using MieFontLib;
    using Mono.Options;

    /// <summary>
    /// コマンドライン オプション
    /// </summary>
    public class TOptions
    {
        private StringWriter errorMessage = new StringWriter();
        private OptionSet optionSet;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arges">コマンドライン引数</param>
        public TOptions(string[] arges)
        {
            this.Arges = new TArgs();
            this.Settings(arges);
            if (!this.IsError)
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
        public TArgs Arges { get; }

        /// <summary>
        /// コマンドライン オプションのエラー有無
        /// </summary>
        public bool IsError { get; private set; } = false;

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
            msg.WriteLine($@"TMPの座標情報を変換する。");
            msg.WriteLine($@"  usage: {exeName} -s <source> -t <target> -f <format> [-o <output>] [-l]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  変換元(-s)と変換先(-t)を読み込み、new_fontフォルダーに'Type 2'形式で座標情報と検証リストを出力する。");
            msg.WriteLine($@"    {exeName} -s resources_00014.-2 -t sharedassets0_00001.114 -o new_font -f 2 -l");
            msg.WriteLine($@"終了コード:");
            msg.WriteLine($@" 0  正常終了");
            msg.WriteLine($@" 1  異常終了");

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
                { "s|source="       , this.Arges.FileNameSourceText    , v => this.Arges.FileNameSource   = v},
                { "t|target="       , this.Arges.FileNameTargetText    , v => this.Arges.FileNameTarget   = v},
                { "o|output="       , this.Arges.FolderNameOutputText  , v => this.Arges.FolderNameOutput = v},
                { "f|format="       , this.Arges.FormatTypeText        , v => this.Arges.FormatType       = v},
                { "a|ascender"      , this.Arges.AscenderText          , v => this.Arges.Ascender         = v != null},
                { "l|list"          , this.Arges.UseListText           , v => this.Arges.UseList          = v != null},
                { "d|detail"        , this.Arges.IsDetailText          , v => this.Arges.IsDetail         = v != null},
                { "h|help"          , "ヘルプ"                         , v => this.Arges.Help             = v != null}
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
                    this.SetErrorMessage("エラー：不明なオプションが指定されました。");
                    extra.ForEach(t => this.SetErrorMessage(t));
                    this.IsError = true;
                }
            }
            catch (OptionException e)
            {
                ////パースに失敗した場合OptionExceptionを発生させる
                this.SetErrorMessage(e.Message);
                this.IsError = true;
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
                this.IsError = false;
                return;
            }

            //// -s
            {
                if (string.IsNullOrWhiteSpace(this.Arges.FileNameSource))
                {
                    this.SetErrorMessage($@"(-s)変換元のファイルのパスを指定してください。");
                    this.IsError = true;
                    return;
                }
                else if (!File.Exists(this.Arges.FileNameSource))
                {
                    this.SetErrorMessage($@"(-s)変換元のファイルが存在しません。File({this.Arges.FileNameSource})");
                    this.IsError = true;
                    return;
                }
            }

            //// -t
            {
                if (string.IsNullOrWhiteSpace(this.Arges.FileNameTarget))
                {
                    this.SetErrorMessage($@"(-t)変換先のファイルのパスを指定してください。");
                    this.IsError = true;
                    return;
                }
                else if (!File.Exists(this.Arges.FileNameTarget))
                {
                    this.SetErrorMessage($@"(-t)変換先のファイルが存在しません。File({this.Arges.FileNameTarget})");
                    this.IsError = true;
                    return;
                }
            }

            //// -o
            {
                if (string.IsNullOrWhiteSpace(this.Arges.FolderNameOutput))
                {
                    //// 出力フォルダー省略時
                    var fullPath = Path.GetFullPath(this.Arges.FileNameTarget);
                    var outdir = Path.GetDirectoryName(fullPath);
                    var outFolder = Path.Combine(outdir, "new_font");
                    this.Arges.FolderNameOutput = outFolder;
                }

                //// 出力フォルダー名のチェック
                var invalidChars = Path.GetInvalidPathChars();
                if (this.Arges.FolderNameOutput.IndexOfAny(invalidChars) < 0)
                {
                    //// 有効な出力フォルダー名
                }
                else
                {
                    //// 無効な出力フォルダー名
                    this.SetErrorMessage($@"(-o)出力フォルダー名に使用できない文字が使われています。{Environment.NewLine}{this.Arges.FolderNameOutput}");
                    this.IsError = true;
                    return;
                }
            }

            //// -f
            if (string.IsNullOrWhiteSpace(this.Arges.FormatType))
            {
                //// 無効なファイル名式
                this.SetErrorMessage($@"(-f)座標ファイルの出力形式を指定してください。");
                this.IsError = true;
                return;
            }

            switch (this.Arges.FormatType.ToLower())
            {
                case "1":
                    this.Arges.EnumFormatType = MieFont.NFormatType.Type1;
                    break;
                case "2":
                    this.Arges.EnumFormatType = MieFont.NFormatType.Type2;
                    break;
                case "3":
                    this.Arges.EnumFormatType = MieFont.NFormatType.Type3;
                    break;
                case "4":
                    this.Arges.EnumFormatType = MieFont.NFormatType.Type4;
                    break;
                case "5":
                    this.Arges.EnumFormatType = MieFont.NFormatType.Type5;
                    break;
                case "poe2":
                    this.Arges.EnumFormatType = MieFont.NFormatType.PoE2;
                    break;
                default:
                    this.Arges.EnumFormatType = MieFont.NFormatType.Unknown;
                    this.SetErrorMessage($@"(-f)座標ファイルの出力形式に誤りがあります。{Environment.NewLine}Format type({this.Arges.FormatType})");
                    this.IsError = true;
                    return;
            }

            this.IsError = false;
            return;
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
            public MieFont.NFormatType EnumFormatType { get; set; } = MieFont.NFormatType.Unknown;

            public string FileNameSource { get; set; }

            public string FileNameSourceText { get; set; } = @"変換元の座標情報ファイルのパス名。";

            public string FileNameTarget { get; set; }

            public string FileNameTargetText { get; set; } = @"変換先の座標情報ファイルのパス名。";

            public string FolderNameOutput { get; internal set; }

            public string FolderNameOutputText { get; internal set; } = $@"出力フォルダーのパス。{Environment.NewLine}省略時は変換先フォルダー内に'new_font'フォルダーを作成する。";

            public bool UseList { get; internal set; }

            public string UseListText { get; internal set; } = @"検証リストを出力する。";

            public string IsDetailText { get; internal set; } = @"検証リスト出力時に詳細情報を出力する。";

            public bool IsDetail { get; internal set; }

            public string FormatTypeText { get; internal set; } = $@"座標ファイルの出力形式。{Environment.NewLine}例：Type 2 の時 -t 2";

            public string FormatType { get; internal set; }

            public string AscenderText { get; internal set; } = $"Ascenderの値を調整する。{Environment.NewLine}PoE2のAIエディターで文字が表示されない場合にのみ指定する。";

            public bool Ascender { get; internal set; }

            public bool Help { get; set; }
        }
    }
}
