namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using System.IO;
    using MieDbLib.SQLite;
    using NLog;

    public class MieSystemDB
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SQLiteConnection Connection { get; private set; } = null;

        public void Open(string path)
        {
            if (this.Connection == null)
            {
                this.Connection = MieSQLiteIO.Open(path);
            }
        }

        public void Close()
        {
            this.Connection.Close();
            this.Connection = null;
        }

        /// <summary>
        /// データベースを最適化する。
        /// </summary>
        public void CompactDatabase()
        {
            if (this.Connection != null)
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandText = "vacuum;";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// データベースを作成する。
        /// </summary>
        /// <param name="databasePath">データベースのパス</param>
        /// <param name="schemaPath">システムデータベースのスキーマのパス</param>
        /// <param name="isReplace">上書きの有無</param>
        public void CreateSystemDB(string databasePath, string schemaPath, bool isReplace)
        {
            if (File.Exists(databasePath) && !isReplace)
            {
                throw new Exception($"Database '{databasePath}' already exists.");
            }
            else
            {
                var mieSQLiteIO = new MieSQLiteIO();
                mieSQLiteIO.CreateDB(databasePath);
                mieSQLiteIO.CreateSystemTableByScheme(databasePath, schemaPath);
            }
        }
    }
}
