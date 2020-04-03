namespace MieDbLib.SystemDB
{
    using System;
    using System.ComponentModel;
    using System.Data.SQLite;
    using MieTranslationLib.Data.FileList;
    using MieTranslationLib.Product;
    using NLog;

    public class MieTableFileListDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public enum NUpdateMode
        {
            Add,
            Update,
            Delete,
        }

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM FileList";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static MieFileList LoadFromSystemDB(MieSystemDB systemDB)
        {
            MieFileList fileList = new MieFileList();

            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM FileList;";
            using (SQLiteDataReader sdr = command.ExecuteReader())
            {
                while (sdr.Read() == true)
                {
                    var fileCode = (long)sdr["FileCode"];
                    var fileID = (string)sdr["FileID"];
                    var xLanguageType = (int)(long)sdr["LanguageType"];
                    var updatedAt = (long)sdr["UpdatedAt"];

                    MieProduct.NLanguageType languageType = (MieProduct.NLanguageType)Enum.ToObject(typeof(MieProduct.NLanguageType), xLanguageType);
                    MieFileEntry fileEntry = new MieFileEntry(fileCode, fileID, languageType, updatedAt);
                    fileList.AddEntry(fileEntry);
                }
            }

            return fileList;
        }

        /// <summary>
        /// FileList
        /// </summary>
        /// <param name="systemDb">SystemDB</param>
        /// <param name="fileList">FileListオブジェクト</param>
        /// <param name="updateMode">更新モード</param>
        public static void SaveToSystemDB(MieSystemDB systemDb, MieFileList fileList, NUpdateMode updateMode)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                SQLiteCommand cmd = systemDb.Connection.CreateCommand();
                switch (updateMode)
                {
                    case NUpdateMode.Add:
                        cmd.CommandText = "INSERT INTO FileList VALUES(@FileCode,@FileID,@LanguageType,@UpdatedAt);";
                        ExecuteUpsertCommand(cmd, fileList);
                        break;
                    case NUpdateMode.Update:
                        cmd.CommandText = "REPLACE INTO FileList VALUES(@FileCode,@FileID,@LanguageType,@UpdatedAt);";
                        ExecuteUpsertCommand(cmd, fileList);
                        break;
                    case NUpdateMode.Delete:
                        cmd.CommandText = "DELETE FROM FileList WHERE FileCode = @FileCode";
                        ExecuteDeleteCommand(cmd, fileList);
                        break;
                    default:
                        var msg = $"Unknown Update Mode({updateMode}).";
                        logger.Fatal(msg);
                        throw new InvalidEnumArgumentException(msg);
                }

                trans.Commit();
            }
        }

        private static void ExecuteUpsertCommand(SQLiteCommand cmd, MieFileList fileList)
        {
            //// パラメータのセット
            cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
            cmd.Parameters.Add("FileID", System.Data.DbType.String);
            cmd.Parameters.Add("LanguageType", System.Data.DbType.Int32);
            cmd.Parameters.Add("UpdatedAt", System.Data.DbType.Int64);
            foreach (var file in fileList.Items.Values)
            {
                //// SystemDBにデータがない場合のみ登録する。
                cmd.Parameters["FileCode"].Value = file.FileCode;
                cmd.Parameters["FileID"].Value = file.FileID;
                cmd.Parameters["LanguageType"].Value = (int)file.LanguageType;
                cmd.Parameters["UpdatedAt"].Value = file.UpdateAt.Ticks;

                cmd.ExecuteNonQuery();
            }
        }

        private static void ExecuteDeleteCommand(SQLiteCommand cmd, MieFileList fileList)
        {
            //// パラメータのセット
            cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
            foreach (var file in fileList.Items.Values)
            {
                //// SystemDBにデータがない場合のみ登録する。
                cmd.Parameters["FileCode"].Value = file.FileCode;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
