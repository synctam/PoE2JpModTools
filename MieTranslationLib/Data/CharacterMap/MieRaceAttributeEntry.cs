namespace MieTranslationLib.Data.CharacterMap
{
    using System;
    using System.Text;
    using NLog;

    /// <summary>
    /// 種族
    /// </summary>
    public class MieRaceAttributeEntry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MieRaceAttributeEntry(Guid raceID, string name)
        {
            this.RaceID = raceID;
            this.Name = name;
        }

        /// <summary>
        /// 種族ID
        /// </summary>
        public Guid RaceID { get; }

        /// <summary>
        /// 種族名
        /// </summary>
        public string Name { get; }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"RaceID({this.RaceID.ToString()}) Name({this.Name})");

            return buff.ToString();
        }
    }
}
