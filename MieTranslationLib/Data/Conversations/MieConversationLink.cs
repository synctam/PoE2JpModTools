namespace MieTranslationLib.Data.Conversations
{
    using System.Text;
    using NLog;

    /// <summary>
    /// 会話リンク
    /// </summary>
    public class MieConversationLink
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieConversationLink(int fromNode, int toNode)
        {
            this.FromeNode = fromNode;
            this.ToNode = toNode;
        }

        public int FromeNode { get; } = -200;

        public int ToNode { get; } = -200;

        /// <summary>
        /// 会話リンクをテキスト化する
        /// </summary>
        /// <returns>テキスト化した会話リンク</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            var msg = $"\tFromeNode({this.FromeNode}) ToNode({this.ToNode})";
            buff.AppendLine(msg);

            return buff.ToString();
        }
    }
}
