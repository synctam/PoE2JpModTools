namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.Conversations;
    using NLog;

    public class MieTableConversationNodeLinksDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ConversationNodeLinks";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static void SaveToDB(MieSystemDB systemDb, MieConversationNodeInfo nodeInfo)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                foreach (var nodeFile in nodeInfo.Files.Values)
                {
                    if (nodeFile.FileCode == 0)
                    {
                        var msg = $"Unknown FileCode({nodeFile.FileCode}). Skipping File...";
                        logger.Warn(msg);
                    }
                    else
                    {
                        SaveToTable(systemDb.Connection, nodeFile, nodeFile.FileCode);
                    }
                }

                trans.Commit();
            }
        }

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieConversationNodeInfo convNodeInfo)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT FileCode,FromNodeID,ToNodeID FROM ConversationNodeLinks ORDER BY FileCode;";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read() == true)
                {
                    var fileCode = (long)reader["FileCode"];
                    var fromNodeID = (int)(long)reader["FromNodeID"];
                    var toNodeID = (int)(long)reader["ToNodeID"];

                    var nodeFile = convNodeInfo.GetNodeFile(fileCode);
                    MieConversationLink nodeLink = new MieConversationLink(fromNodeID, toNodeID);
                    nodeFile.AddLinkEntry(nodeLink);
                }
            }
        }

        private static void SaveToTable(SQLiteConnection connection, MieConversationNodeFile nodeFile, long fileCode)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO ConversationNodeLinks VALUES(@FileCode,@FromNodeID,@ToNodeID);";
            //// パラメータのセット
            cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
            cmd.Parameters.Add("FromNodeID", System.Data.DbType.Int32);
            cmd.Parameters.Add("ToNodeID", System.Data.DbType.Int32);
            foreach (var link in nodeFile.Links)
            {
                cmd.Parameters["FileCode"].Value = fileCode;
                cmd.Parameters["FromNodeID"].Value = link.FromeNode;
                cmd.Parameters["ToNodeID"].Value = link.ToNode;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ConversationNodeLinks: FileCode({fileCode}) FromeNode({link.FromeNode}) ToNode({link.ToNode})");
                }
            }
        }
    }
}
