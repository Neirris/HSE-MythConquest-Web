using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;
using Newtonsoft.Json.Linq;
using MythConquestWeb.Controllers;

namespace MythConquestWeb.Controllers
{
    [Route("/getusers")]
    public class ArenaController : ControllerBase
    {
        private readonly ILogger<TowerController> _logger;

        public ArenaController(ILogger<TowerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<string> Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            List<string> nicknames = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
                JObject jsonObject = JObject.Parse(jsonString);
                int userID = (int)jsonObject["userId"];
                using (var cmd = new NpgsqlCommand("SELECT nickname FROM users_profile_data WHERE user_account_id <> @userID ORDER BY nickname ASC", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nicknames.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return nicknames;
        }
    }

    [Route("/startArena")]
    public class ArenaStartController : ControllerBase
    {
        private Player player;
        private Player enemy;

        [HttpPost]
        public IActionResult StartArena([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];
            var EnemyPlayerNickname = "Zero";
            try
            {
                EnemyPlayerNickname = Request.Headers["currEnemyPlayer"].ToString();
                if (EnemyPlayerNickname == "")
                {
                    return BadRequest("NoUserSelected");
                }
            }
            catch { return BadRequest("NoUserSelected");}
            int enemyId = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT users_profile_data_id
                    FROM users_profile_data 
                    WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@nickname", EnemyPlayerNickname);
                    enemyId = (int)cmd.ExecuteScalar();

                }
            }

            player = TowerStartController.GetCharData(userID);
            enemy = TowerStartController.GetCharData(enemyId);

            List<string> battleLog = new List<string>();
            if (player.HP != 0)
            {
                int turnNumber = 1;
                do
                {
                    player.Attack(enemy, turnNumber, battleLog);
                    if (enemy.HP > 0)
                    {
                        enemy.Attack(player, turnNumber, battleLog);
                    }
                    turnNumber++;
                } while (player.HP > 0 && enemy.HP > 0);

                if (player.HP == 0)
                {
                    enemy.AppendText(enemy.Name + " победил!", "#FF00FFFF", battleLog);
                }
                else
                {
                    string victoryMessage = player.Name + $" победил!";
                    player.AppendText(victoryMessage, "#FF00FF00", battleLog);
                }
            }
            else
            {
                player.AppendText("Недостаточно здоровья для сражений!", "#FF0000", battleLog);
            }
            return Ok(battleLog);

        }
    }
}