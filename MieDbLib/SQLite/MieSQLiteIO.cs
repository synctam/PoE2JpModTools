namespace MieDbLib.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using NLog;

    public class MieSQLiteIO
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static SQLiteConnection Open(string path)
        {
            if (!File.Exists(path))
            {
                var msg = $"File not found({path}).";
                logger.Fatal(msg);
                throw new FileNotFoundException(msg, path);
            }

            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3,
                LegacyFormat = false,
                //// 非同期でIO処理を行う。
                SyncMode = SynchronizationModes.Off,
                //// ジャーナルをメモリに取る。
                JournalMode = SQLiteJournalModeEnum.Memory,
                //// UTF-8 を使用する。
                UseUTF16Encoding = false
            };

            SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString);
            connection.Open();

            return connection;
        }

        public static void Close(SQLiteConnection con)
        {
            con.Close();
            con = null;
        }

        /// <summary>
        /// DBを削除する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        public void DropDB(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// DBを作成する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        public void CreateDB(string path)
        {
            try
            {
                this.DropDB(path);

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path
                };

                using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// DBにテーブルを作成する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        /// <param name="tableName">テーブル名</param>
        /// <param name="ddl">テーブル作成用SQL</param>
        public void CreateTable(string path, string tableName, string ddl)
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = path
            };

            string dropTable = $"DROP TABLE IF EXISTS {tableName}";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = dropTable;
                    command.ExecuteNonQuery();

                    command.CommandText = ddl;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// システムテーブルの作成
        /// </summary>
        /// <param name="dbPath">データベースのパス</param>
        /// <param name="schemePath">スキーマのパス</param>
        public void CreateSystemTableByScheme(string dbPath, string schemePath)
        {
            var connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = dbPath
            };

            var sql = File.ReadAllText(schemePath, Encoding.UTF8);

            using (var connection = new SQLiteConnection(connectionString.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// DBをコピーする。
        /// 出力先が書き込み禁止の場合は解除してコピーする。
        /// </summary>
        /// <param name="enDbPath">コピー元</param>
        /// <param name="newDbPath">コピー先</param>
        public void CopyDatabase(string enDbPath, string newDbPath)
        {
            File.Copy(enDbPath, newDbPath, true);
            //// 書き込み禁止であれば解除する。
            FileAttributes attr = File.GetAttributes(newDbPath);
            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(newDbPath, attr & (~FileAttributes.ReadOnly));
            }
        }

        /// <summary>
        /// テーブルにデータを挿入する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        /// <param name="tableName">テーブル名</param>
        /// <param name="items">オブジェクト</param>
        [Obsolete("未実装")]
        public void InsertData(string path, string tableName, Dictionary<string, string> items)
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3,
                LegacyFormat = false,
                //// 非同期でIO処理を行う。
                SyncMode = SynchronizationModes.Off,
                //// ジャーナルをメモリに取る。
                JournalMode = SQLiteJournalModeEnum.Memory,
                //// UTF-8 を使用する。
                UseUTF16Encoding = false
            };

            using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction sqlt = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"insert into {tableName} (LocaId,LocaText) values(@LocaId,@LocaText);";
                        command.Parameters.Add("LocaId", System.Data.DbType.String);
                        command.Parameters.Add("LocaText", System.Data.DbType.String);
                        foreach (var itemPair in items)
                        {
                            string key = itemPair.Key;

                            var value = itemPair.Value;
                            command.Parameters["LocaId"].Value = key;
                            command.Parameters["LocaText"].Value = value;
                            command.ExecuteNonQuery();
                        }
                    }

                    sqlt.Commit();
                }
            }
        }

        /// <summary>
        /// DBを最適化する。
        /// </summary>
        /// <param name="path">DBのパス</param>
        public void CompactDB(string path)
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = path
            };

            string compactDB = $"VACUUM;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = compactDB;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
