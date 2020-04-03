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
            msg.WriteLine($@"SystemDBを初期化する。");
            msg.WriteLine($@"  usage: {exeName} --init -s <SystemDB> -q <schema>");
            msg.WriteLine($@"SystemDBにデータを取り込む。");
            msg.WriteLine($@"  usage: {exeName} -l <Lang> -d <Design> -s <SystemDB>");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  SystemDBにデータを取り込む。");
            msg.WriteLine($@"    {exeName} -l data\localized\en -d data\design -s PoE2SystemDB.sqlite");
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
                { "l=|lang="           , this.Arges.FileNameLangText      , v => this.Arges.FileNameLang       = v},
                { "d=|design="         , this.Arges.FileNameDesignText    , v => this.Arges.FileNameDesign     = v},
                { "s=|systemdb="       , this.Arges.FileNameSystemDBText  , v => this.Arges.FileNameSystemDB   = v},
                { "p=|product="        , this.Arges.ProductLineText       , v => this.Arges.ProductLine        = v},
                { "q=|sql="            , this.Arges.SchemaPathText        , v => this.Arges.SchemaPath         = v},
                { "init"               , this.Arges.IsInitText            , v => this.Arges.IsInit             = v != null},
                { "r|replace"          , this.Arges.IsReplaceText         , v => this.Arges.IsReplace          = v != null},
                { "h|help"             , "ヘルプ"                         , v => this.Arges.Help               = v != null}
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

            if (this.Arges.IsInit)
            {
                if (string.IsNullOrWhiteSpace(this.Arges.FileNameSystemDB))
                {
                    this.SetErrorMessage($@"エラー：(-s)SystemDBのパス指定してください。");
                    this.IsError = true;

                    return;
                }

                if (File.Exists(this.Arges.FileNameSystemDB) && !this.Arges.IsReplace)
                {
                    this.SetErrorMessage(
                        $"エラー：(-s)SystemDBが既に存在します。{Environment.NewLine}" +
                        $"(上書きする場合は -r オプションを指定してください・。");
                    this.IsError = true;

                    return;
                }

                if (string.IsNullOrWhiteSpace(this.Arges.SchemaPath))
                {
                    this.SetErrorMessage($@"エラー：(-q)SystemDBのスキーマのパス指定してください。");
                    this.IsError = true;

                    return;
                }

                if (!File.Exists(this.Arges.SchemaPath))
                {
                    this.SetErrorMessage(
                        $"エラー：(-q)SystemDBのスキーマが見つかりません。{Environment.NewLine}" +
                        $"({Path.GetFullPath(this.Arges.SchemaPath)})");
                    this.IsError = true;

                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.Arges.FileNameLang))
                {
                    this.SetErrorMessage($@"エラー：(-l)言語フォルダーのパスを指定してください。");
                    this.IsError = true;

                    return;
                }

                if (!Directory.Exists(this.Arges.FileNameLang))
                {
                    this.SetErrorMessage($@"エラー：(-l)言語フォルダーがありません。{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameLang)})");
                    this.IsError = true;

                    return;
                }

                if (string.IsNullOrWhiteSpace(this.Arges.FileNameDesign))
                {
                    this.SetErrorMessage($@"エラー：(-d)デザイン・フォルダーのパスを指定してください。");
                    this.IsError = true;

                    return;
                }

                if (!Directory.Exists(this.Arges.FileNameDesign))
                {
                    this.SetErrorMessage($@"エラー：(-d)デザイン・フォルダーがありません。{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameDesign)})");
                    this.IsError = true;

                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSystemDB))
            {
                this.SetErrorMessage($@"エラー：(-s)SystemDBのパスを指定してください。");
                this.IsError = true;

                return;
            }

            if (File.Exists(this.Arges.FileNameSystemDB))
            {
                if (!this.Arges.IsReplace)
                {
                    this.SetErrorMessage($@"エラー：(-s)SystemDBがすでに存在します。{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameSystemDB)}){Environment.NewLine}上書きする場合は -r オプションを指定してください。");
                    this.IsError = true;
                }

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
            public string FileNameLang { get; set; }

            public string FileNameLangText { get; set; } = @"言語フォルダーのパス名。例：localized\en";

            public string FileNameDesign { get; set; }

            public string FileNameDesignText { get; set; } = @"付加フォルダーのパス名。例：design";

            public string FileNameSystemDB { get; internal set; }

            public string FileNameSystemDBText { get; internal set; } = @"System Databaseのパス。";

            public string ProductLine { get; internal set; }

            public string ProductLineText { get; internal set; } = @"ProductLine";

            public string SchemaPath { get; internal set; }

            public string SchemaPathText { get; internal set; } =
                $"システムDBのスキーマのパス";

            public bool IsReplace { get; internal set; }

            public string IsReplaceText { get; internal set; } = @"DBがすでに存在する場合は、上書きする。";

            public bool IsInit { get; internal set; }

            public string IsInitText { get; internal set; } = @"データベースを初期化する。";

            public bool Help { get; set; }
        }
    }
}
