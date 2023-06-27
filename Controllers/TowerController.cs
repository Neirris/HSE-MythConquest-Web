using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using MythConquestWeb;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;





namespace MythConquestWeb.Controllers
{
    [ApiController]
    [Route("/getpotions")]
    public class TowerController : ControllerBase
    {
        private readonly ILogger<TowerController> _logger;

        public TowerController(ILogger<TowerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public TowerHp Get([FromHeader(Name = "Cookie")] string cookieValue)
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

                    cmd.CommandText = @"SELECT health_potions FROM inventory WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    int hp_potions = (int)cmd.ExecuteScalar();
                    TowerHp potions = new TowerHp
                    {
                        hp_potions = hp_potions.ToString()
                    };
                    return potions;
                }
            }
        }
    }

    [Route("/setpotions")]
    public class TowerHpNewController : ControllerBase
    {

        [HttpPost]
        public IActionResult Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];
            double curr_hp = 0;
            int max_hp = 0, price = 0, coins;

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT health_potions FROM inventory WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    int hp_potions = (int)cmd.ExecuteScalar();

                    if (hp_potions > 0)
                    {
                        cmd.CommandText = @"SELECT coins 
                                        FROM inventory WHERE user_account_id = @userID";
                        cmd.Parameters.AddWithValue("@userID", userID);
                        coins = (int)cmd.ExecuteScalar();

                        cmd.CommandText = @"SELECT curr_health_points, health_points 
                                            FROM equipment_profile WHERE user_account_id = @userID";
                        cmd.Parameters.AddWithValue("@userID", userID);
                        using var CharHpData = cmd.ExecuteReader();
                        while (CharHpData.Read())
                        {
                            curr_hp = CharHpData.GetDouble(0);
                            max_hp = CharHpData.GetInt32(1);
                        }
                        CharHpData.Close();

                        price = (int)Math.Round(max_hp - curr_hp)/10;

                        if (curr_hp < max_hp)
                        {
                            if (coins >= price)
                            {
                                coins = coins - price;
                                curr_hp = max_hp;

                                cmd.CommandText = @"UPDATE inventory
                                            SET health_potions = @curr_hp_potions
                                            WHERE user_account_id = @userID";
                                cmd.Parameters.AddWithValue("@userID", userID);
                                cmd.Parameters.AddWithValue("@curr_hp_potions", hp_potions - 1);
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = @"UPDATE inventory
                                            SET coins = @coins
                                            WHERE user_account_id = @userID";
                                cmd.Parameters.AddWithValue("@userID", userID);
                                cmd.Parameters.AddWithValue("@coins", coins);
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = @"UPDATE equipment_profile
                                            SET curr_health_points = @curr_hp
                                            WHERE user_account_id = @userID";
                                cmd.Parameters.AddWithValue("@userID", userID);
                                cmd.Parameters.AddWithValue("@curr_hp", curr_hp);
                                cmd.ExecuteNonQuery();
                                return Ok();
                            }
                        }
                    }
                    return BadRequest();
                }
            }
        }
    }

    [Route("/startTower")]
    public class TowerStartController : ControllerBase
    {
        private Player player;
        private Player enemy;

        [HttpPost]
        public IActionResult StartTower([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];
            int currDungeonID;
            try
            {
                currDungeonID = int.Parse(Request.Headers["currFloor"]);
            }
            catch
            {
                return BadRequest("NoFloorSelected");
            }


            player = GetCharData(userID);
            enemy = GetEnemyData(currDungeonID);
            SetRewardValues(currDungeonID, out int curr_reward_exp, out int curr_reward_coins);

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

                    battleLog.Add($"[ Ход {turnNumber} ] Оставшееся здоровье: {player.Name} - {player.HP} | {enemy.Name} - {enemy.HP}");
                } while (player.HP > 0 && enemy.HP > 0);

                if (player.HP == 0)
                {
                    enemy.AppendText(enemy.Name + " победил!", "#FF00FFFF", battleLog);
                }
                else
                {
                    string victoryMessage = player.Name + $" победил!\nНаграда: Опыт - {curr_reward_exp}, Монеты - {curr_reward_coins}";
                    player.AppendText(victoryMessage, "#FF00FF00", battleLog);
                    battleLog.Add(victoryMessage);
                    UpdateStats(userID, curr_reward_exp, curr_reward_coins);
                }
                UpdateHP(userID, player.HP);
            }
            else
            {
                player.AppendText("Недостаточно здоровья для сражений!", "#FF0000", battleLog);
            }

            // Возвращаем battleLog в виде ответа
            return Ok(battleLog);

        }

        public void UpdateHP(int userID, int currHp)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE equipment_profile
                                        SET curr_health_points = @curr_hp
                                        WHERE user_account_id  = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@curr_hp", currHp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStats(int userID, int curr_reward_exp, int curr_reward_coins)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE inventory
                                        SET coins = coins + @coins
                                        WHERE user_account_id  = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@coins", curr_reward_coins);
                    cmd.ExecuteNonQuery();

                    Dictionary<int, long> expDict = new Dictionary<int, long>();


                    cmd.CommandText = @"SELECT lvl, exp
                                        FROM exp_table";
                    using var getDataExp = cmd.ExecuteReader();
                    while (getDataExp.Read())
                    {
                        int lvl = getDataExp.GetInt32(0);
                        long exp = getDataExp.GetInt64(1);
                        expDict.Add(lvl, exp);
                    }
                    getDataExp.Close();
                    long current_exp = 1, required_exp = 1;
                    int current_lvl = 1;

                    cmd.CommandText = @"SELECT exp, exp_req, lvl
                                        FROM users_profile_data
                                        WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var getUserExp = cmd.ExecuteReader();

                    while (getUserExp.Read())
                    {
                        current_exp = getDataExp.GetInt64(0);
                        required_exp = getDataExp.GetInt64(1);
                        current_lvl = getUserExp.GetInt32(2);
                    }
                    getUserExp.Close();
                    current_exp += curr_reward_exp;

                    while (current_exp >= required_exp)
                    {
                        current_exp -= required_exp;
                        current_lvl++;
                        if (expDict.TryGetValue(current_lvl, out long nextLevelExp))
                        {
                            required_exp = nextLevelExp;
                        }
                        else
                        {
                            required_exp = 0;
                        }
                    }
                    if (current_exp < 0)
                    {
                        current_exp = Math.Abs(current_exp);
                        current_lvl++;
                        if (expDict.TryGetValue(current_lvl, out long nextLevelExp))
                        {
                            required_exp = nextLevelExp;
                        }
                        else
                        {
                            required_exp = 0;
                        }
                    }
                    cmd.CommandText = @"UPDATE users_profile_data
                                        SET lvl = @lvl, exp = @exp, exp_req = @exp_req, total_exp = total_exp + @total_exp
                                        WHERE user_account_id  = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@lvl", current_lvl);
                    cmd.Parameters.AddWithValue("@exp", current_exp);
                    cmd.Parameters.AddWithValue("@exp_req", required_exp);
                    cmd.Parameters.AddWithValue("@total_exp", curr_reward_exp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static Player GetCharData(int userID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    string char_name, currNick;
                    int defense = 0, attack = 0, crit = 0, initiative = 0, evasion = 0, curr_character = 0;
                    int curr_health_points = 0, max_hp = 0;

                    cmd.CommandText = @"SELECT curr_character, curr_health_points, defense, attack, crit, initiative, evasion, health_points
                                FROM equipment_profile 
                                WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);

                    using var fullCharData = cmd.ExecuteReader();
                    while (fullCharData.Read())
                    {
                        curr_character = fullCharData.GetInt32(0);
                        curr_health_points = Convert.ToInt32(fullCharData.GetDouble(1));
                        attack = fullCharData.GetInt32(3);
                        crit = fullCharData.GetInt32(4);
                        initiative = fullCharData.GetInt32(5);
                        evasion = fullCharData.GetInt32(6);
                        max_hp = fullCharData.GetInt32(7);
                    }
                    fullCharData.Close();

                    cmd.CommandText = @"SELECT nickname 
                                    FROM users_profile_data 
                                    WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    currNick = (string)cmd.ExecuteScalar();

                    cmd.CommandText = @"SELECT name
                                    FROM characters 
                                    WHERE character_id = @char_id";
                    cmd.Parameters.AddWithValue("@char_id", curr_character);
                    char_name = (string)cmd.ExecuteScalar();

                    return new Player(char_name, Convert.ToInt32(curr_health_points), attack, crit * 0.00001, evasion * 0.00002, initiative * 0.00005);
                }
            }
        }

        public void SetRewardValues(int currDungeonID, out int curr_reward_exp, out int curr_reward_coins)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    int reward_coins = 0, reward_exp = 0;

                    cmd.CommandText = @"SELECT reward_coins, reward_exp
                                FROM dungeons 
                                WHERE dungeon_id = @dungeon_id";
                    cmd.Parameters.AddWithValue("@dungeon_id", currDungeonID);

                    using var fullDungeonData = cmd.ExecuteReader();
                    while (fullDungeonData.Read())
                    {
                        reward_coins = fullDungeonData.GetInt32(0);
                        reward_exp = fullDungeonData.GetInt32(1);
                    }
                    fullDungeonData.Close();
                    curr_reward_exp = reward_exp;
                    curr_reward_coins = reward_coins;
                }
            }
        }


        public static Player GetEnemyData(int currDungeonID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    int monsterId = 0, monsterLvl = 0, monsterHp = 0, monsterAttck = 0, monsterDef = 0, reward_coins = 0, reward_exp = 0;
                    double mul_lvl, fin_mul;
                    string monsterName = "";

                    cmd.CommandText = @"SELECT monster, lvl, reward_coins, reward_exp
                                FROM dungeons 
                                WHERE dungeon_id = @dungeon_id";
                    cmd.Parameters.AddWithValue("@dungeon_id", currDungeonID);

                    using var fullDungeonData = cmd.ExecuteReader();
                    while (fullDungeonData.Read())
                    {
                        monsterId = fullDungeonData.GetInt32(0);
                        monsterLvl = fullDungeonData.GetInt32(1);
                    }
                    fullDungeonData.Close();

                    cmd.CommandText = @"SELECT name, attack, health_points, defense
                                FROM monsters 
                                WHERE monster_id = @monster_id";
                    cmd.Parameters.AddWithValue("@monster_id", monsterId);

                    using var fullMonsterData = cmd.ExecuteReader();
                    while (fullMonsterData.Read())
                    {
                        monsterName = fullMonsterData.GetString(0);
                        monsterAttck = fullMonsterData.GetInt32(1);
                        monsterHp = fullMonsterData.GetInt32(2);
                        monsterDef = fullMonsterData.GetInt32(3);
                    }
                    fullMonsterData.Close();

                    cmd.CommandText = @"SELECT mul_lvl
                                FROM lvl_multipliers 
                                WHERE mul_type = 'monster'";
                    mul_lvl = (double)cmd.ExecuteScalar();
                    fin_mul = (double)Math.Pow(mul_lvl, monsterLvl);

                    return new Player(monsterName, Convert.ToInt32(monsterHp * fin_mul), Convert.ToInt32(monsterAttck * fin_mul), 0.33, 0.33, 0.33);
                }
            }
        }
    }
    public class Player
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int ATK { get; set; }
        public double CritChance { get; set; }
        public double DodgeChance { get; set; }
        public double DoubleChance { get; set; }
        private Random random = new Random();

        public Player(string name, int hp, int atk, double critChance, double dodgeChance, double doubleChance)
        {
            Name = name;
            HP = hp;
            ATK = atk;
            CritChance = critChance;
            DodgeChance = dodgeChance;
            DoubleChance = doubleChance;
        }

        public void Attack(Player target, int turnNumber, List<string> battleLog)
        {
            int damage = ATK;
            if (random.NextDouble() < CritChance)
            {
                AppendText("[ Ход " + turnNumber + " ] " + Name + " наносит критический удар!", "#FF00FF00", battleLog);
                damage *= 2;
            }
            if (random.NextDouble() < target.DodgeChance)
            {
                AppendText("[ Ход " + turnNumber + " ] " + target.Name + " уклоняется от атаки!", "#FF00FFFF", battleLog);
            }
            else
            {
                target.HP -= damage;

                if (target.HP < 0)
                    target.HP = 0;

                AppendText("[ Ход " + turnNumber + " ] " + Name + " атакует " + target.Name + " и наносит " + damage + " урона.", "#FF00FF00", battleLog);

                AppendText("[ Ход " + turnNumber + " ] Оставшееся здоровье: " + Name + " - " + HP + " | " + target.Name + " - " + target.HP, "#f5deb3", battleLog);
            }

            if (target.HP <= 0)
            {
                return;
            }

            if (random.NextDouble() < DoubleChance)
            {
                AppendText("[ Ход " + turnNumber + " ] " + Name + " атакует снова!", "#FF00FF00", battleLog);
                turnNumber++;
                Attack(target, turnNumber, battleLog);
            }
        }

        public void AppendText(string text, string color, List<string> battleLog)
        {
            battleLog.Add(text);
        }
    }

}