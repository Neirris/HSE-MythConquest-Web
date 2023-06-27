using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;
using Newtonsoft.Json.Linq;

namespace MythConquestWeb.Controllers
{
    [Route("/inventorychars")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Dictionary<string, ProfileStats> Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT char_id FROM inventory_characters WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);

                    List<int> charIDs = new List<int>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int charID = reader.GetInt32(0);
                            charIDs.Add(charID);
                        }
                    }

                    Dictionary<string, ProfileStats> profileDataDict = new Dictionary<string, ProfileStats>();

                    string char_name = "", char_gender = "", char_url = "";
                    int class_id = 0, rarity = 0, char_value = 0, health_points = 0, defense = 0, attack = 0, crit = 0, initiative = 0, evasion = 0;

                    foreach (int charID in charIDs)
                    {
                        cmd.CommandText = @"SELECT name, class, rarity, gender, value, health_points, defense, attack, crit, initiative, evasion, appearance_url
                                    FROM characters 
                                    WHERE character_id = @char_id";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@char_id", charID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                char_name = reader.GetString(0);
                                class_id = reader.GetInt32(1);
                                rarity = reader.GetInt32(2);
                                char_gender = reader.GetString(3);
                                char_value = reader.GetInt32(4);
                                health_points = reader.GetInt32(5);
                                defense = reader.GetInt32(6);
                                attack = reader.GetInt32(7);
                                crit = reader.GetInt32(8);
                                initiative = reader.GetInt32(9);
                                evasion = reader.GetInt32(10);
                                char_url = reader.GetString(11);
                            }
                            reader.Close();
                            cmd.CommandText = @"SELECT name
                                            FROM classes 
                                            WHERE class_id = @class_id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@class_id", class_id);

                            string class_name = (string)cmd.ExecuteScalar();

                            ProfileStats profileData = new ProfileStats
                            {
                                char_id = charID.ToString(),
                                name = char_name,
                                appearance_url = char_url,
                                char_class = class_name,
                                rarity = new string('☆', rarity),
                                gender = char_gender,
                                value = char_value.ToString(),
                                health_points = health_points.ToString(),
                                defense = defense.ToString(),
                                attack = attack.ToString(),
                                crit = crit.ToString(),
                                initiative = initiative.ToString(),
                                evasion = evasion.ToString()
                            };

                            profileDataDict[charID.ToString()] = profileData;
                        }
                    }

                    return profileDataDict;
                }
            }
        }
    }

    [Route("/equipchar")]
    public class InventoryEquipController : ControllerBase
    {
        [HttpPost]
        public IActionResult EquipChar([FromHeader(Name = "Cookie")] string cookieValue)
        {
            int.TryParse(Request.Headers["curr_char_id"], out int curr_char_id);

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    int ascensionLvl;
                    cmd.CommandText = @"SELECT ascension_lvl FROM inventory_characters
                                        WHERE user_account_id = @userID AND char_id = @charID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@charID", curr_char_id);
                    ascensionLvl = (int)cmd.ExecuteScalar();

                    cmd.CommandText = @"UPDATE equipment_profile
                                        SET curr_character = @char_id
                                        WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@char_id", curr_char_id);
                    cmd.ExecuteNonQuery();
                }
                return Ok();
            }
        }

    }
}
