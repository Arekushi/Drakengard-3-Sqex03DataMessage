using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    public class RequiredTextsIDs
    {
        public static readonly List<string> REQUIRED_TEXTS_IDS =
        [
            "Ability", "Ability_Name", "battle", "book", "boss_hint", "boss_hint_Name", "C_00", "C_00_Name",
            "C_1011", "C_1011_Name", "C_1021", "C_1021_Name", "C_1311", "C_1311_Name", "C_1411", "C_1411_Name",
            "C_2011", "C_2011_Name", "C_2021", "C_2021_Name", "C_2041", "C_2041_Name", "C_2311", "C_2311_Name",
            "C_2411", "C_2411_Name", "C_3011", "C_3011_Name", "C_3041", "C_3041_Name", "C_3111", "C_3111_Name",
            "C_3211", "C_3211_Name", "C_3221", "C_3221_Name", "C_3231", "C_3231_Name", "C_3241", "C_3241_Name",
            "C_3251", "C_3251_Name", "C_3311", "C_3311_Name", "C_4011", "C_4011_Name", "C_4021", "C_4021_Name",
            "C_4111", "C_4111_Name", "C_4121", "C_4121_Name", "C_4311", "C_4311_Name", "C_4321", "C_4321_Name",
            "C_4511", "C_4511_Name", "C_5011", "C_5011_Name", "Combo_Name", "Common", "Common_Name", "Common_Talk",
            "Common_Talk_Name", "database", "database_character", "database_character_Name", "DLC", "DLC_BGM",
            "DLC_dragon", "DLC_dragon_Name", "DLC_zero", "DLC_zero_Name", "gauntlet", "gauntlet_Name", "intermission",
            "Item", "Item_Name", "M_0010", "M_0010_Name", "M_0020", "M_0020_Name", "M_1011", "M_1011_Name", "M_1021",
            "M_1021_Name", "M_1031", "M_1031_Name", "M_1041", "M_1041_Name", "M_1311", "M_1311_Name", "M_1411",
            "M_1411_Name", "M_2011", "M_2011_Name", "M_2021", "M_2021_Name", "M_2031", "M_2031_Name", "M_2041",
            "M_2041_Name", "M_2051", "M_2051_Name", "M_2061", "M_2061_Name", "M_2311", "M_2311_Name", "M_2321",
            "M_2321_Name", "M_2411", "M_2411_Name", "M_3011", "M_3011_Name", "M_3021", "M_3021_Name", "M_3031",
            "M_3031_Name", "M_3041", "M_3041_Name", "M_3051", "M_3051_Name", "M_3061", "M_3061_Name", "M_3211",
            "M_3211_Name", "M_3221", "M_3221_Name", "M_3231", "M_3231_Name", "M_3241", "M_3241_Name", "M_3251",
            "M_3251_Name", "M_3311", "M_3311_Name", "M_3321", "M_3321_Name", "M_4011", "M_4011_Name", "M_4021",
            "M_4021_Name", "M_4031", "M_4031_Name", "M_4041", "M_4041_Name", "M_4111", "M_4111_Name", "M_4121",
            "M_4121_Name", "M_4131", "M_4131_Name", "M_4311", "M_4311_Name", "M_4321", "M_4321_Name", "M_4511",
            "M_4511_Name", "M_5011", "M_5011_Name", "M_5021", "M_5021_Name", "M_5031", "M_5031_Name", "M_5041",
            "M_5041_Name", "M_5111", "M_5111_Name", "M_5121", "M_5121_Name", "M_5311", "M_5311_Name", "M_5321",
            "M_5321_Name", "Mission_purpose", "mission_tips", "mission_tips_Name", "missionname", "missionname_Name",
            "misson_outline", "monster_name", "Option", "pausemenu", "ppm00", "ppm00_Name", "quest", "quest_Name",
            "Quest_clear_conditions", "Result", "SELECT_MENU", "staffrollmsg", "story_outline", "title_menu", "Tutorial",
            "Weapon", "Weapon_Name", "weapon_effect", "weapon_story", "weapon_story_Name"
        ];

        public static List<string> ValidateJsonAttributes(string jsonFilePath)
        {
            JObject jsonObject = JObject.Parse(File.ReadAllText(jsonFilePath));
            List<string> missingAttributes = [];

            foreach (var attribute in REQUIRED_TEXTS_IDS)
            {
                if (jsonObject[attribute] == null)
                {
                    missingAttributes.Add(attribute);
                }
            }

            return missingAttributes;
        }

        public static List<string> ValidateTxtFiles(List<string> filesNames)
        {
            List<string> missingFiles = [];

            foreach (string textID in REQUIRED_TEXTS_IDS)
            {
                if (!filesNames.Contains(textID))
                {
                    missingFiles.Add(textID);
                }
            }

            return missingFiles;
        }
    }
}
