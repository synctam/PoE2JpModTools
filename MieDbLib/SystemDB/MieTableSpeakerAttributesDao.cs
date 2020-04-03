namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.CharacterMap;
    using NLog;

    public class MieTableSpeakerAttributesDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// テーブルのクリア
        /// </summary>
        /// <param name="systemDb">Connection of SystemDB</param>
        public static void ClearTable(MieSystemDB systemDb)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = systemDb.Connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM SpeakerAttributes";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static void SaveToSysyemDB(MieSystemDB systemDb, MieCharacterAttributeFile speakerAttrBefore)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                SQLiteCommand cmd = systemDb.Connection.CreateCommand();
                cmd.CommandText = "INSERT INTO SpeakerAttributes VALUES(@ID,@Name,@Gender)";
                //// パラメータのセット
                cmd.Parameters.Add("ID", System.Data.DbType.Guid);
                cmd.Parameters.Add("Name", System.Data.DbType.String);
                cmd.Parameters.Add("Gender", System.Data.DbType.Int32);

                foreach (var speakerEntry in speakerAttrBefore.SpeakerItems.Values)
                {
                    cmd.Parameters["ID"].Value = speakerEntry.ID;
                    cmd.Parameters["Name"].Value = speakerEntry.Name;
                    cmd.Parameters["Gender"].Value = speakerEntry.Gender;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{ex.Message}\r\nSpeakerAttributes: ID({speakerEntry.ID.ToString()}) Name({speakerEntry.Name}) Gender({speakerEntry.Gender.ToString()})");
                    }
                }

                trans.Commit();
            }
        }

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieCharacterAttributeFile charAttr)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM SpeakerAttributes;";
            using (SQLiteDataReader sdr = command.ExecuteReader())
            {
                while (sdr.Read() == true)
                {
                    var byteID = (byte[])sdr["ID"];
                    var name = (string)sdr["Name"];
                    var intGender = (long)sdr["Gender"];

                    Guid id = new Guid(byteID);

                    MieCharacterAttributeFile.NGender gender =
                        (MieCharacterAttributeFile.NGender)Enum.ToObject(typeof(MieCharacterAttributeFile.NGender), intGender);

                    MieSpeakerAttributeEntry speakerEntry = new MieSpeakerAttributeEntry(id, name, gender);

                    charAttr.AddSpeakerEntry(speakerEntry);
                }
            }
        }
    }
}
