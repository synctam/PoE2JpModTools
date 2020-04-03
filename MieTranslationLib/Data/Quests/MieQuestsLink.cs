namespace MieTranslationLib.Data.Quests
{
    using System.Text;
    using NLog;

    /// <summary>
    /// クエストリンク
    /// </summary>
    public class MieQuestsLink
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieQuestsLink(int fromNode, int toNode)
        {
            this.FromeNode = fromNode;
            this.ToNode = toNode;
        }

        public int FromeNode { get; } = -200;

        public int ToNode { get; } = -200;

        /// <summary>
        /// クエストリンクをテキスト化する
        /// </summary>
        /// <returns>テキスト化したクエストリンク</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            var msg = $"\tFromeNode({this.FromeNode}) ToNode({this.ToNode})";
            buff.AppendLine(msg);

            return buff.ToString();
        }
    }
}
