namespace MieOELib.Data.StringTable
{
    using System.Collections.Generic;

    public class MieStringTableFile
    {
        public MieStringTableFile(string fileID)
        {
            this.FileID = fileID;
        }

        public List<MieStringTableEntry> Items { get; } =
            new List<MieStringTableEntry>();

        public string FileID { get; } = string.Empty;
    }
}
