namespace MieOELib.Data.Speakers
{
    using System;
    using System.IO;
    using System.Text;
    using MieTranslationLib.Data.CharacterMap;
    using Newtonsoft.Json;
    using NLog;

    public class MieOESpeakersDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// スピーカー属性ファイルを読み込みキャラクター属性に追加する。
        /// </summary>
        /// <param name="charAttributeFile">キャラクター属性</param>
        /// <param name="path">スピーカー属性ファイル</param>
        public static void AppendSpeakerAttribute(MieCharacterAttributeFile charAttributeFile, string path)
        {
            if (!File.Exists(path))
            {
                var msg = $"File not found.";
                throw new FileNotFoundException("File not found. path", path);
            }

            string jtext = File.ReadAllText(path, Encoding.UTF8);

            //// JSON中の"$type"はデシリアライズできないため、"MieGameDataTypeTag"にリネームして処理する。詳細はwiki参照。
            jtext = jtext.Replace("$type", "MieGameDataTypeTag");

            //// デシリアライズ
            var oeSpeakers = JsonConvert.DeserializeObject<MieOESpeakers>(jtext);
            foreach (var gameDataObject in oeSpeakers.GameDataObjects)
            {
                var tag = GetGameDataTypeTag(gameDataObject.MieGameDataTypeTag);
                switch (tag)
                {
                    case "SpeakerGameData":
                        var component = gameDataObject.Components[0];
                        //// スピーカー属性
                        var gender = GetGender(component.Gender);
                        MieSpeakerAttributeEntry attrEntry =
                           new MieSpeakerAttributeEntry(new Guid(gameDataObject.ID), gameDataObject.DebugName, gender);
                        charAttributeFile.AddSpeakerEntry(attrEntry);

                        break;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// DataTypeTagからDataTypeを返す。
        /// </summary>
        /// <param name="text">DataTypeTag</param>
        /// <returns>DataType</returns>
        private static string GetGameDataTypeTag(string text)
        {
            var tag = text.Replace("Game.GameData.", string.Empty).Replace(", Assembly-CSharp", string.Empty);

            return tag;
        }

        /// <summary>
        /// GenderテキストからGenderを返す。
        /// </summary>
        /// <param name="text">Genderテキスト</param>
        /// <returns>Gender</returns>
        private static MieCharacterAttributeFile.NGender GetGender(string text)
        {
            MieCharacterAttributeFile.NGender gender;
            switch (text.ToLower())
            {
                case "female":
                    gender = MieCharacterAttributeFile.NGender.Female;
                    break;
                case "male":
                    gender = MieCharacterAttributeFile.NGender.Male;
                    break;
                case "neuter":
                    gender = MieCharacterAttributeFile.NGender.Neuter;
                    break;
                default:
                    var msg = $"Unknown Gender type. Gender({text})";
                    logger.Fatal(msg);
                    throw new Exception(msg);
            }

            return gender;
        }
    }
}
