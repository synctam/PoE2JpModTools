namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.Chatter;
    using NLog;

    public class MieTableChatterEntriesDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ChatterNodeEntries";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// チャッター情報の保存
        /// </summary>
        /// <param name="systemDb">SystemDB</param>
        /// <param name="chatter">チャッター情報</param>
        public static void SaveToDB(MieSystemDB systemDb, MieChatterNodeInfo chatter)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                foreach (var nodeFile in chatter.Files.Values)
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

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieChatterNodeInfo chatterNodeInfo)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM ChatterNodeEntries ORDER BY FileCode;";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read() == true)
                {
                    var fileCode = (long)reader["FileCode"];
                    var nodeID = (int)(long)reader["NodeID"];
                    var xNodeType = (long)reader["NodeType"];
                    var xIsRootNode = (long)reader["IsRootNode"];

                    MieChatterNodeEntry.NNodeType nodeType =
                        (MieChatterNodeEntry.NNodeType)Enum.ToObject(typeof(MieChatterNodeEntry.NNodeType), xNodeType);
                    bool isRootNode = xIsRootNode == 0 ? false : true;

                    MieChatterNodeEntry nodeEntry = new MieChatterNodeEntry(
                        nodeType,
                        nodeID,
                        isRootNode);

                    var nodeFile = chatterNodeInfo.GetNodeFile(fileCode);
                    var rc = nodeFile.AddFlatNodeEntry(nodeEntry);
                }
            }

            foreach (var nodeFile in chatterNodeInfo.Files.Values)
            {
                nodeFile.BuildLink();
                nodeFile.UpdateDepth();
            }
        }

        /// <summary>
        /// 指定したファイルのチャッターノードを保存する。
        /// </summary>
        /// <param name="connection">DB接続情報</param>
        /// <param name="nodeFile">NodeFile</param>
        /// <param name="fileCode">FileCode</param>
        private static void SaveToTable(SQLiteConnection connection, MieChatterNodeFile nodeFile, long fileCode)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO ChatterNodeEntries VALUES(@FileCode,@NodeID,@NodeType,@IsRootNode);";
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
