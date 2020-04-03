namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.Language;
    using MieTranslationLib.MieUtils;
    using MieTranslationLib.Product;
    using NLog;

    public class MieTableLanguageDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM LanguageEntries";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static void SaveToSysyemDB(MieSystemDB systemDb, MieLanguageInfo langInfo)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                foreach (var langFile in langInfo.Items.Values)
                {
                    if (langFile.FileCode == 0)
                    {
                        var msg = $"Unknown FileCode({langFile.FileCode}).";
                        logger.Fatal(msg);
                        throw new Exception(msg);
                    }
                    else
                    {
                        SQLiteCommand cmd = systemDb.Connection.CreateCommand();
                        cmd.CommandText = "INSERT INTO LanguageEntries VALUES(@FileCode,@ID,@ReferenceID,@ReferenceText,@DefaultText,@FemaleText,@ProductLine,@UpdatedAt);";
                        //// パラメータのセット
                        cmd.Parameters.Add("FileCode", System.Data.DbType.Int64);
                        cmd.Parameters.Add("ID", System.Data.DbType.Int32);
                        cmd.Parameters.Add("ReferenceID", System.Data.DbType.Int64);
                        cmd.Parameters.Add("ReferenceText", System.Data.DbType.String);
                        cmd.Parameters.Add("DefaultText", System.Data.DbType.String);
                        cmd.Parameters.Add("FemaleText", System.Data.DbType.String);
                        cmd.Parameters.Add("ProductLine", System.Data.DbType.Int32);
                        cmd.Parameters.Add("UpdatedAt", System.Data.DbType.Int64);

                        foreach (var entry in langFile.Items)
                        {
                            cmd.Parameters["FileCode"].Value = langFile.FileCode;
                            cmd.Parameters["ID"].Value = entry.ID;
                            cmd.Parameters["ReferenceID"].Value = entry.ReferenceID;
                            cmd.Parameters["ReferenceText"].Value = MieHashTools.ComputeHashIds(entry.ReferenceID);
                            cmd.Parameters["DefaultText"].Value = entry.DefaultText;
                            cmd.Parameters["FemaleText"].Value = entry.FemaleText;
                            cmd.Parameters["ProductLine"].Value = (int)entry.ProductLine;
                            cmd.Parameters["UpdatedAt"].Value = entry.UpdatedAt.Ticks;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, $"LanguageEntries: FileCode({langFile.FileCode}) ID({entry.ID})");
                            }
                        }
                    }
                }

                trans.Commit();
            }
        }

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieLanguageInfo langInfo)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM LanguageEntries ORDER BY FileCode;";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read() == true)
                {
                    var fileCode = (long)reader["FileCode"];
                    var id = (int)(long)reader["ID"];
                    var referenceID = (long)reader["ReferenceID"];
                    var defaultText = (string)reader["DefaultText"];
                    var femaleText = (string)reader["FemaleText"];
                    var xProductLine = (long)reader["ProductLine"];
                    var xUpdatedAt = (long)reader["UpdatedAt"];

                    MieProduct.NProductLine productLine = (MieProduct.NProductLine)Enum.ToObject(typeof(MieProduct.NProductLine), xProductLine);
                    DateTime updateAt = new DateTime(xUpdatedAt);

                    MieLanguageEntry langEntry = new MieLanguageEntry(id, defaultText, femaleText, productLine, referenceID, updateAt);
                    langInfo.AddFileEntry(fileCode, langEntry);
                }
            }
        }
    }
}
