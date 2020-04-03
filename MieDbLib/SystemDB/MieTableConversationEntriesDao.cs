namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.Conversations;
    using NLog;

    public class MieTableConversationEntriesDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ConversationNodeEntries";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// 会話情報の保存
        /// </summary>
        /// <param name="systemDb">SystemDB</param>
        /// <param name="convInfo">会話情報</param>
        public static void SaveToDB(MieSystemDB systemDb, MieConversationNodeInfo convInfo)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                foreach (var nodeFile in convInfo.Files.Values)
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
            command.CommandText = @"SELECT * FROM ConversationNodeEntries ORDER BY FileCode;";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read() == true)
                {
                    var fileCode = (long)reader["FileCode"];
                    var nodeID = (int)(long)reader["NodeID"];
                    var xNodeType = (long)reader["NodeType"];
                    var xSpeakerGuid = (byte[])reader["SpeakerGuid"];
                    var xListenerGuid = (byte[])reader["ListenerGuid"];
                    var xIsRootNode = (long)reader["IsRootNode"];
                    var xIsQuestionNode = (long)reader["IsQuestionNode"];

                    MieConversationNodeEntry.NNodeType nodeType = (MieConversationNodeEntry.NNodeType)Enum.ToObject(typeof(MieConversationNodeEntry.NNodeType), xNodeType);
                    Guid speakerGuid = new Guid(xSpeakerGuid);
                    Guid listenerGuid = new Guid(xListenerGuid);
                    bool isRootNode = xIsRootNode == 0 ? false : true;
                    bool isQuestionNode = xIsQuestionNode == 0 ? false : true;

                    MieConversationNodeEntry nodeEntry = new MieConversationNodeEntry(
                        nodeType,
                        nodeID,
                        speakerGuid,
                        listenerGuid,
                        isRootNode,
                        isQuestionNode);

                    var nodeFile = convNodeInfo.GetNodeFile(fileCode);
                    var rc = nodeFile.AddFlatNodeEntry(nodeEntry);
                }
            }

            foreach (var nodeFile in convNodeInfo.Files.Values)
            {
                nodeFile.BuildLink();
                nodeFile.UpdateDepth();
            }
        }

        /// <summary>
        /// 指定したファイルの会話ノードを保存する。
        /// </summary>
        /// <param name="connection">DB接続情報</param>
        /// <param name="nodeFile">NodeFile</param>
        /// <param name="fileCode">FileCode</param>
        private static void SaveToTable(SQLiteConnection connection, MieConversationNodeFile nodeFile, long fileCode)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO ConversationNodeEntries VALUES(@FileCode,@NodeID,@NodeType,@SpeakerGuid,@ListenerGuid,@IsRootNode,@IsQuestionNode);";
            //// パラメータのセット
            cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
            cmd.Parameters.Add("NodeID", System.Data.DbType.Int32);
            cmd.Parameters.Add("NodeType", System.Data.DbType.Int32);
            cmd.Parameters.Add("SpeakerGuid", System.Data.DbType.Guid);
            cmd.Parameters.Add("ListenerGuid", System.Data.DbType.Guid);
            cmd.Parameters.Add("IsRootNode", System.Data.DbType.Boolean);
            cmd.Parameters.Add("IsQuestionNode", System.Data.DbType.Boolean);

            foreach (var node in nodeFile.FlatNodes.Values)
            {
                //// 値のセット
                cmd.Parameters["FileCode"].Value = fileCode;
                cmd.Parameters["NodeID"].Value = node.NodeID;
                cmd.Parameters["NodeType"].Value = node.NodeType;
                cmd.Parameters["SpeakerGuid"].Value = node.SpeakerGuid;
                cmd.Parameters["ListenerGuid"].Value = node.ListenerGuid;
                cmd.Parameters["IsRootNode"].Value = node.IsRootNode;
                cmd.Parameters["IsQuestionNode"].Value = node.IsQuestionNode;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ConversationNodeEntries: FileCode({fileCode}) NodeID({node.NodeID})");
                }
            }
        }
    }
}
