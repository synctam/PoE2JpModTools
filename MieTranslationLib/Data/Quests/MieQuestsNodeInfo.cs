namespace MieTranslationLib.Data.Quests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NLog;

    public class MieQuestsNodeInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 会話ファイルの辞書。キーはFileCode。
        /// </summary>
        public IDictionary<long, MieQuestsNodeFile> Files { get; } =
            new SortedDictionary<long, MieQuestsNodeFile>();

        /// <summary>
        /// クエストファイルを追加する。
        /// すでにクエストファイルが存在する場合は、エントリーをマージする。
        /// </summary>
        /// <param name="questsFile">クエストファイル</param>
        public void AddFile(MieQuestsNodeFile questsFile)
        {
            if (questsFile == null)
            {
                var msg = $"MieQuestsNodeFile is null.";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            if (this.Files.ContainsKey(questsFile.FileCode))
            {
                //// すでにクエストファイルが存在する場合は、エントリーをマージする。
                var file = this.Files[questsFile.FileCode];
                file.MargeFile(questsFile);
            }
            else
            {
                this.Files.Add(questsFile.FileCode, questsFile);
            }
        }

        /// <summary>
        /// クエストリンク情報をテキスト化する
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

        public MieQuestsNodeFile GetNodeFile(long fileCode)
        {
            if (this.Files.ContainsKey(fileCode))
            {
                var result = this.Files[fileCode];
                return result;
            }
            else
            {
                var nodeFile = new MieQuestsNodeFile(fileCode);
                this.Files.Add(nodeFile.FileCode, nodeFile);
                return nodeFile;
            }
        }
    }
}
