namespace MieTranslationLib.Data.Conversations
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using NLog;

    public class MieConversationNodeInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 会話ファイルの辞書。キーはFileCode。
        /// </summary>
        public IDictionary<long, MieConversationNodeFile> Files { get; } =
            new SortedDictionary<long, MieConversationNodeFile>();

        /// <summary>
        /// 特殊キャラクター情報
        /// ToDo: 使いみちはあるのか？
        /// </summary>
        private MieCharacterSpecial MieSpecialCharacter { get; } = new MieCharacterSpecial();

        /// <summary>
        /// 会話ファイルを追加する。
        /// 会話ファイルがすでに存在する場合は会話ファイル内の会話エントリーをマージする。
        /// </summary>
        /// <param name="conversationsNodeFile">会話ファイル</param>
        public void AddFile(MieConversationNodeFile conversationsNodeFile)
        {
            if (conversationsNodeFile == null)
            {
                var msg = $"MieConversationNodeFile is null.";
                logger.Fatal(msg);
                throw new Exception(msg);
            }

            if (this.Files.ContainsKey(conversationsNodeFile.FileCode))
            {
                //// すでにクエストファイルが存在する場合は、エントリーをマージする。
                var file = this.Files[conversationsNodeFile.FileCode];
                file.MargeFile(conversationsNodeFile);
            }
            else
            {
                this.Files.Add(conversationsNodeFile.FileCode, conversationsNodeFile);
            }
        }

        /// <summary>
        /// 会話Node情報をテキスト化する。
        /// </summary>
        /// <returns>テキスト化した会話Node情報</returns>
        public string ToNodeString()
        {
            //// ToDo: Node情報のテキスト化
            throw new NotImplementedException("Node情報のテキスト化");
        }

        /// <summary>
        /// 会話リンク情報をテキスト化する
        /// </summary>
        /// <param name="viewStopNode">StopNodeの表示有無</param>
        /// <returns>テキスト化した会話リンク情報</returns>
        public string ToLinkString(bool viewStopNode)
        {
            StringBuilder buff = new StringBuilder();

            foreach (var file in this.Files.Values)
            {
                buff.Append(file.ToLinkString(viewStopNode));
            }

            return buff.ToString();
        }

        /// <summary>
        /// リンクの From/To 情報をテキスト化する。
        /// </summary>
        /// <returns>テキスト化した From/To 情報</returns>
        public string ToLinksFromTo()
        {
            StringBuilder buff = new StringBuilder();

            foreach (var file in this.Files.Values)
            {
                buff.Append(file.ToLinksFromToString());
            }

            return buff.ToString();
        }

        /// <summary>
        /// キャラクターマップをテキスト化する。
        /// </summary>
        /// <returns>テキスト化したキャラクターマップ</returns>
        public string ToCharacterMapString()
        {
            StringBuilder buff = new StringBuilder();

            foreach (var file in this.Files.Values)
            {
                buff.AppendLine($"{file.FileCode}");
                buff.Append($"{file.CharacterMapFile.ToString()}");
            }

            return buff.ToString();
        }

        public MieConversationNodeFile GetNodeFile(long fileCode)
        {
            if (this.Files.ContainsKey(fileCode))
            {
                var result = this.Files[fileCode];
                return result;
            }
            else
            {
                var nodeFile = new MieConversationNodeFile(fileCode);
                this.Files.Add(nodeFile.FileCode, nodeFile);
                return nodeFile;
            }
        }
    }
}
