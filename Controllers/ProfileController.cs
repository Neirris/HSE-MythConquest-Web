using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;
using Newtonsoft.Json.Linq;

namespace MythConquestWeb.Controllers
{

    [ApiController]
    [Route("/profileinfo")]
    public class ProfileInfoController : ControllerBase
    {
        private readonly ILogger<ProfileInfoController> _logger;

        public ProfileInfoController(ILogger<ProfileInfoController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public ProfileInfo Get([FromHeader(Name = "Cookie")] string cookieValue)
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

                    cmd.CommandText = @"SELECT coins FROM inventory WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    int l_coins = (int)cmd.ExecuteScalar();

                    cmd.CommandText = @"SELECT nickname, lvl, user_pfp, exp, exp_req, wins, loses FROM users_profile_data WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string l_nickname = reader.GetString(0);
                            int l_lvl = reader.GetInt32(1);
                            string l_user_pfp = reader.GetString(2);
                            int l_exp = reader.GetInt32(3);
                            int l_exp_req = reader.GetInt32(4);
                            int l_wins = reader.GetInt32(5);
                            int l_loses = reader.GetInt32(6);

                            ProfileInfo profileData = new ProfileInfo
                            {
                                nickname = l_nickname,
                                lvl = l_lvl.ToString(),
                                user_pfp = l_user_pfp,
                                exp = l_exp.ToString(),
                                exp_req = l_exp_req.ToString(),
                                wins = l_wins.ToString(),
                                loses = l_loses.ToString(),
                                coins = l_coins.ToString()
                            };

                            return profileData;
                        }
                    }
                }
            }

            return null;
        }
    }

    [Route("/profilestats")]
    public class ProfileStatsController : ControllerBase
    {
        private readonly ILogger<ProfileStatsController> _logger;

        public ProfileStatsController(ILogger<ProfileStatsController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public ProfileStats Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            int curr_char_id = 0, curr_ascensionLvl = 0, curr_rarity = 0, char_class = 0, char_value = 0;
            string char_name = "", char_url = "", class_name = "", char_gender = "";

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT curr_character
                                        FROM equipment_profile
                                        WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    curr_char_id = (int)cmd.ExecuteScalar();

                    cmd.CommandText = @"SELECT rarity, name, appearance_url, class, gender, value                         
                                        FROM characters 
                                WHERE character_id = @char_id";
                    cmd.Parameters.AddWithValue("@char_id", curr_char_id);
                    using var CharData = cmd.ExecuteReader();
                    while (CharData.Read())
                    {
                        curr_rarity = CharData.GetInt32(0);
                        char_name = CharData.GetString(1);
                        char_url = CharData.GetString(2);
                        char_class = CharData.GetInt32(3);
                        char_gender = CharData.GetString(4);
                        char_value = CharData.GetInt32(5);
                    }
                    CharData.Close();

                    cmd.CommandText = @"SELECT name
                                        FROM classes 
                                        WHERE class_id = @class_id";
                    cmd.Parameters.AddWithValue("@class_id", char_class);
                    class_name = (string)cmd.ExecuteScalar();


                    cmd.CommandText = @"SELECT ascension_lvl FROM inventory_characters
                                        WHERE user_account_id = @userID AND char_id = @charID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@charID", curr_char_id);
                    curr_ascensionLvl = (int)cmd.ExecuteScalar();

                    string stars = new string('☆', curr_rarity);
                    string oldText = "☆";
                    string newText = "★";
                    int maxReplacements = curr_ascensionLvl - 1;

                    int replacements = 0;
                    int index = 0;

                    while (replacements < maxReplacements && (index = stars.IndexOf(oldText, index, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        stars = stars.Remove(index, oldText.Length).Insert(index, newText);
                        index += newText.Length;
                        replacements++;
                    }

                    int l_health_points = 0, l_defense = 0, l_attack = 0, l_crit = 0, l_initiative = 0, l_evasion = 0;
                    double curr_hp = 0;

                    cmd.CommandText = @"SELECT health_points, defense, attack, crit, initiative, evasion, curr_health_points
                                        FROM equipment_profile
                                        WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var fullCharData = cmd.ExecuteReader();
                    while (fullCharData.Read())
                    {
                        l_health_points = fullCharData.GetInt32(0);
                        l_defense = fullCharData.GetInt32(1);
                        l_attack = fullCharData.GetInt32(2);
                        l_crit = fullCharData.GetInt32(3);
                        l_initiative = fullCharData.GetInt32(4);
                        l_evasion = fullCharData.GetInt32(5);
                        curr_hp = fullCharData.GetDouble(6);
                    }
                    fullCharData.Close();

                    ProfileStats profileData = new ProfileStats
                    {
                        name = char_name,
                        appearance_url = char_url,
                        char_class = class_name,
                        rarity = stars,
                        gender = char_gender,
                        value = char_value.ToString(),
                        health_points = $"{curr_hp} / {l_health_points}",
                        defense = l_defense.ToString(),
                        attack = l_attack.ToString(),
                        crit = l_crit.ToString(),
                        initiative = l_initiative.ToString(),
                        evasion = l_evasion.ToString()
                    };
                    return profileData;
                }
            }

            return null;
        }
    }

    [Route("/pfpupdate")]
    public class ProfilePfpUpdateController : ControllerBase
    {
        [HttpPost]
        public IActionResult PfpUpdate([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            var imageUrl = Request.Headers["newPfp"].ToString();

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE users_profile_data SET user_pfp = @userPfp WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@userPfp", imageUrl);
                    cmd.ExecuteNonQuery();
                }
                return Ok();
            }

        }
    }

}