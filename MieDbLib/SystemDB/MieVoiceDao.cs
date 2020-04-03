namespace MieDbLib.SystemDB
{
    using System.Data;
    using System.Data.SQLite;
    using MieDbLib.SQLite;
    ////using MieVoiceLib;

    public class MieVoiceDao
    {
        private SQLiteConnection connection = null;
        private SQLiteTransaction transaction = null;

        public SQLiteConnection Connection { get { return this.connection; } }

        public void Open(string voiceDatabasePath)
        {
            this.connection = MieSQLiteIO.Open(voiceDatabasePath);
        }

        public void Close()
        {
            this.connection.Close();
            this.connection = null;
        }

        public void StartTransaction()
        {
            this.transaction = this.connection.BeginTransaction();
        }

        public void CompactDB(string databasePath)
        {
            MieSQLiteIO mieSQLiteIO = new MieSQLiteIO();
            mieSQLiteIO.CompactDB(databasePath);
        }

        public void Clear()
        {
            SQLiteCommand cmd = this.connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Voices;";
            cmd.ExecuteNonQuery();
        }

        public void Commit()
        {
            this.transaction.Commit();
        }

        ////public MieVoiceEntry ReadVoice(SQLiteConnection con, string id)
        ////{
        ////    SQLiteCommand cmd = con.CreateCommand();
        ////    cmd.CommandText = @"SELECT * FROM Voices WHERE ID=?;";
        ////    cmd.Parameters.Add(new SQLiteParameter(DbType.String, id));
        ////    using (SQLiteDataReader sdr = cmd.ExecuteReader())
        ////    {
        ////        while (sdr.Read() == true)
        ////        {
        ////            var sha1 = (byte[])sdr["SHA1"];
        ////            var voice = (byte[])sdr["Voice"];
        ////            MieVoiceEntry mieVoiceEntry = new MieVoiceEntry(id, sha1, voice);
        ////            return mieVoiceEntry;
        ////        }

        ////        return null;
        ////    }
        ////}

        public void WriteVoice(string id, byte[] hash, byte[] voice)
        {
            SQLiteCommand cmd = this.connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Voices VALUES(@ID,@SHA1,@Voice);";
            cmd.Parameters.Add("ID", DbType.String);
            cmd.Parameters.Add("SHA1", DbType.Binary);
            cmd.Parameters.Add("Voice", DbType.Binary);

            cmd.Parameters["ID"].Value = id;
            cmd.Parameters["SHA1"].Value = hash;
            cmd.Parameters["Voice"].Value = voice;

            cmd.ExecuteNonQuery();
        }
    }
}
