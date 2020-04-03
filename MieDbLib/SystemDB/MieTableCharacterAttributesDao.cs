namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.CharacterMap;
    using NLog;

    public class MieTableCharacterAttributesDao
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
                    cmd.CommandText = "DELETE FROM CharacterAttributes";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// 話者情報をDBに格納する。
        /// </summary>
        /// <param name="systemDb">Connection of SystemDB</param>
        /// <param name="charFile">CharacterAttributeFile</param>
        public static void SaveToSysyemDB(MieSystemDB systemDb, MieCharacterAttributeFile charFile)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                SQLiteCommand cmd = systemDb.Connection.CreateCommand();
                cmd.CommandText = "INSERT INTO CharacterAttributes VALUES(@ID,@Name,@Gender,@SpeakerID,@RaceID);";
                //// パラメータのセット
                cmd.Parameters.Add("ID", System.Data.DbType.Guid);
                cmd.Parameters.Add("Name", System.Data.DbType.String);
                cmd.Parameters.Add("Gender", System.Data.DbType.Int32);
                cmd.Parameters.Add("SpeakerID", System.Data.DbType.Guid);
                cmd.Parameters.Add("RaceID", System.Data.DbType.Guid);

                foreach (var charEntry in charFile.CharacterItems.Values)
                {
                    cmd.Parameters["ID"].Value = charEntry.ID;
                    cmd.Parameters["Name"].Value = charEntry.Name;
                    cmd.Parameters["Gender"].Value = charEntry.Gender;
                    cmd.Parameters["SpeakerID"].Value = charEntry.SpeakerID;
                    cmd.Parameters["RaceID"].Value = charEntry.RaceID;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{ex.Message}\r\nCharacterAttributes: ID({charEntry.ID.ToString()}) Name({charEntry.Name}) Gender({charEntry.Gender.ToString()}) SpeakerID({charEntry.SpeakerID.ToString()}) RaceID({charEntry.RaceID.ToString()})");
                    }
                }

                trans.Commit();
            }
        }

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieCharacterAttributeFile charAttr)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM CharacterAttributes;";
            using (SQLiteDataReader sdr = command.ExecuteReader())
            {
                while (sdr.Read() == true)
                {
                    var byteID = (byte[])sdr["ID"];
                    var name = (string)sdr["Name"];
                    var intGender = (long)sdr["Gender"];
                    var byteSpeakerID = (byte[])sdr["SpeakerID"];
                    var byteRaceID = (byte[])sdr["RaceID"];

                    Guid id = new Guid(byteID);
                    Guid speakerID = new Guid(byteSpeakerID);
                    Guid raceID = new Guid(byteRaceID);

                    MieCharacterAttributeFile.NGender gender =
                        (MieCharacterAttributeFile.NGender)Enum.ToObject(typeof(MieCharacterAttributeFile.NGender), intGender);

                    MieCharacterAttributeEntry charEntry = new MieCharacterAttributeEntry(id, name, gender, speakerID, raceID);
                    charAttr.AddCharacterAttributeEntry(charEntry);
                }
            }
        }
    }
}
