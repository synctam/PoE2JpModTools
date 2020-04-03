namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.Quests;
    using NLog;

    public class MieTableQuestsEntriesDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM QuestsNodeEntries";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// クエスト情報の保存
        /// </summary>
        /// <param name="systemDb">SystemDB</param>
        /// <param name="quests">クエスト情報</param>
        public static void SaveToDB(MieSystemDB systemDb, MieQuestsNodeInfo quests)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                foreach (var nodeFile in quests.Files.Values)
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

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieQuestsNodeInfo questsNodeInfo)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM QuestsNodeEntries ORDER BY FileCode;";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read() == true)
                {
                    var fileCode = (long)reader["FileCode"];
                    var nodeID = (int)(long)reader["NodeID"];
                    var xNodeType = (long)reader["NodeType"];
                    var xIsRootNode = (long)reader["IsRootNode"];

                    MieQuestsNodeEntry.NNodeType nodeType = (MieQuestsNodeEntry.NNodeType)Enum.ToObject(typeof(MieQuestsNodeEntry.NNodeType), xNodeType);
                    bool isRootNode = xIsRootNode == 0 ? false : true;

                    MieQuestsNodeEntry nodeEntry = new MieQuestsNodeEntry(
                        nodeType,
                        nodeID,
                        isRootNode);

                    var nodeFile = questsNodeInfo.GetNodeFile(fileCode);
                    var rc = nodeFile.AddFlatNodeEntry(nodeEntry);
                }
            }

            foreach (var nodeFile in questsNodeInfo.Files.Values)
            {
                nodeFile.BuildLink();
                nodeFile.UpdateDepth();
            }
        }

        /// <summary>
        /// 指定したファイルのクエストノードを保存する。
        /// </summary>
        /// <param name="connection">DB接続情報</param>
        /// <param name="nodeFile">NodeFile</param>
        /// <param name="fileCode">FileCode</param>
        private static void SaveToTable(SQLiteConnection connection, MieQuestsNodeFile nodeFile, long fileCode)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO QuestsNodeEntries VALUES(@FileCode,@NodeID,@NodeType,@IsRootNode);";
            //// パラメータのセット
            cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
            cmd.Parameters.Add("NodeID", System.Data.DbType.Int32);
            cmd.Parameters.Add("NodeType", System.Data.DbType.Int32);
            cmd.Parameters.Add("IsRootNode", System.Data.DbType.Boolean);

            foreach (var node in nodeFile.FlatNodes.Values)
            {
                //// 値のセット
                cmd.Parameters["FileCode"].Value = fileCode;
                cmd.Parameters["NodeID"].Value = node.NodeID;
                cmd.Parameters["NodeType"].Value = node.NodeType;
                cmd.Parameters["IsRootNode"].Value = node.IsRootNode;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
