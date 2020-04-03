namespace MieDbLib.SystemDB
{
    using System;
    using System.Data.SQLite;
    using MieTranslationLib.Data.CharacterMap;
    using NLog;

    public class MieTableRaceAttributesDao
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
                    cmd.CommandText = "DELETE FROM RaceAttributes";
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static void SaveToSysyemDB(MieSystemDB systemDb, MieCharacterAttributeFile raceAttrBefore)
        {
            using (SQLiteTransaction trans = systemDb.Connection.BeginTransaction())
            {
                SQLiteCommand cmd = systemDb.Connection.CreateCommand();
                cmd.CommandText = "INSERT INTO RaceAttributes VALUES(@RaceID,@Name)";
                //// パラメータのセット
                cmd.Parameters.Add("RaceID", System.Data.DbType.Guid);
                cmd.Parameters.Add("Name", System.Data.DbType.String);

                foreach (var raceEntry in raceAttrBefore.RaceItems.Values)
                {
                    cmd.Parameters["RaceID"].Value = raceEntry.RaceID;
                    cmd.Parameters["Name"].Value = raceEntry.Name;

                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public static void LoadFromSystemDB(MieSystemDB systemDB, MieCharacterAttributeFile raceAttr)
        {
            SQLiteCommand command = systemDB.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM RaceAttributes";
            using (SQLiteDataReader sdr = command.ExecuteReader())
            {
                while (sdr.Read() == true)
                {
                    var byteRaceID = (byte[])sdr["RaceID"];
                    var name = (string)sdr["Name"];

                    Guid raceID = new Guid(byteRaceID);

                    MieRaceAttributeEntry raceEntry = new MieRaceAttributeEntry(raceID, name);

                    raceAttr.AddRaceEntry(raceEntry);
                }
            }
        }
    }
}
