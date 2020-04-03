namespace MieTranslationLib.Data.Conversations
{
    using System.Collections.Generic;

    public class MieConversationFlatNodeFile
    {
        public ICollection<int> FlatNodeIDs { get; } = new HashSet<int>();

        public IList<MieConversationNodeEntry> FlatNodes { get; } = new List<MieConversationNodeEntry>();
    }
}
