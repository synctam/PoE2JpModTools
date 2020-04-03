namespace MieTranslationLib.Data.Chatter
{
    using System.Text;

    public class MieChatterLink
    {
        public MieChatterLink(int fromNodeID, int toNodeID)
        {
            this.FromeNode = fromNodeID;
            this.ToNode = toNodeID;
        }

        public int FromeNode { get; } = -200;

        public int ToNode { get; } = -200;

        /// <summary>
        /// チャッターリンクをテキスト化する
        /// </summary>
        /// <returns>テキスト化したチャッターリンク</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            var msg = $"\tFromeNode({this.FromeNode}) ToNode({this.ToNode})";
            buff.AppendLine(msg);

            return buff.ToString();
        }
    }
}
