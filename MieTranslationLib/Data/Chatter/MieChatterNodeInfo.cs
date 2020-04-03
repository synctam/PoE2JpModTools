namespace MieTranslationLib.Data.Chatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NLog;

    public class MieChatterNodeInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 会話ファイルの辞書。キーはFileCode。
        /// </summary>
        public IDictionary<long, MieChatterNodeFile> Files { get; } =
            new SortedDictionary<long, MieChatterNodeFile>();

        /// <summary>
        /// チャッターファイルを追加する。
        /// すでにチャッターファイルが存在する場合は、エントリーをマージする。
        /// </summary>
        /// <param name="chatterFile">チャッターファイル</param>
        public void AddFile(MieChatterNodeFile chatterFile)
        {
            if (chatterFile == null)
            {
                var msg = $"MieChatterNodeFile is null.";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            if (this.Files.ContainsKey(chatterFile.FileCode))
            {
                //// すでにチャッターファイルが存在する場合は、エントリーをマージする。
                var file = this.Files[chatterFile.FileCode];
                file.MargeFile(chatterFile);
            }
            else
            {
                this.Files.Add(chatterFile.FileCode, chatterFile);
            }
        }

        /// <summary>
        /// チャッターリンク情報をテキスト化する
        /// </summary>
        /// <param name="viewStopNode">StopNodeの表示有無</param>
        /// <returns>テキスト化した会話リンク情報</returns>
        public string ToLinkString(bool viewStopNode)
        {
            StringBuilder buff = new StringBuilder();

            this.Files.Values
                .ToList()
                .ForEach(x =>
                {
                    buff.Append(x.ToLinkString(viewStopNode));
                });

            return buff.ToString();
        }

        /// <summary>
        /// リンクの From/To 情報をテキスト化する。
        /// </summary>
        /// <returns>テキスト化した From/To 情報</returns>
        public string ToLinksFromTo()
        {
            StringBuilder buff = new StringBuilder();

            this.Files.Values
                .ToList()
                .ForEach(x =>
                {
                    buff.Append(x.ToLinksFromToString());
                });

            return buff.ToString();
        }

        public MieChatterNodeFile GetNodeFile(long fileCode)
        {
            if (this.Files.ContainsKey(fileCode))
            {
                var result = this.Files[fileCode];
                return result;
            }
            else
            {
                var nodeFile = new MieChatterNodeFile(fileCode);
                this.Files.Add(nodeFile.FileCode, nodeFile);
                return nodeFile;
            }
        }
    }
}
