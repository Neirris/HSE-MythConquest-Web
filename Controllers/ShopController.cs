using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;
using Newtonsoft.Json.Linq;

namespace MythConquestWeb.Controllers
{
    [Route("/getshopticketprice")]
    public class ShopControllerTickets : ControllerBase
    {
        private readonly ILogger<ShopControllerTickets> _logger;

        public ShopControllerTickets(ILogger<ShopControllerTickets> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public TicketPrice Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            int l_char_ticket_price;

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'ticket_character'";
                    l_char_ticket_price = Convert.ToInt32(cmd.ExecuteScalar());

                    TicketPrice tprice = new TicketPrice
                    {
                        ticket_price = l_char_ticket_price.ToString()
                    };
                    return tprice;
                }

            }
            return null;
        }
    }

    [Route("/getshopcoins")]
    public class ShopControllerCoins : ControllerBase
    {
        private readonly ILogger<ShopControllerCoins> _logger;

        public ShopControllerCoins(ILogger<ShopControllerCoins> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public CurrentCoins Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            int l_coins = 0, l_char_tickets = 0;

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT coins, tickets_characters FROM inventory WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var coinReader = cmd.ExecuteReader();
                    while (coinReader.Read())
                    {
                        l_coins = (int)coinReader.GetInt32(0);
                        l_char_tickets = (int)coinReader.GetInt32(1);
                    }
                    coinReader.Close();

                    CurrentCoins coinsdata = new CurrentCoins
                    {
                        coins = l_coins.ToString(),
                        curr_ticket = l_char_tickets.ToString()
                    };
                    return coinsdata;
                }

            }
            return null;
        }
    }

    [Route("/getequipmentlvl")]
    public class ShopControllerEquipmentLvl : ControllerBase
    {
        private readonly ILogger<ShopControllerEquipmentLvl> _logger;

        public ShopControllerEquipmentLvl(ILogger<ShopControllerEquipmentLvl> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public CurrentEquipmentLvl Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            int lvl_weapon = 0, lvl_armor = 0, lvl_greaves = 0, lvl_boots = 0, lvl_gloves = 0, lvl_jewelry = 0;

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT weapon_lvl, armor_lvl, greaves_lvl, boots_lvl, gloves_lvl, jewelry_lvl FROM inventory_equipment_items WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var reader = cmd.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            lvl_weapon = (int)reader.GetInt64(0);
                            lvl_armor = (int)reader.GetInt64(1);
                            lvl_greaves = (int)reader.GetInt64(2);
                            lvl_boots = (int)reader.GetInt64(3);
                            lvl_gloves = (int)reader.GetInt64(4);
                            lvl_jewelry = (int)reader.GetInt64(5);
                        }
                    }
                    CurrentEquipmentLvl eLvl = new CurrentEquipmentLvl
                    {
                        lvl_weapon = lvl_weapon.ToString(),
                        lvl_armor = lvl_armor.ToString(),
                        lvl_greaves = lvl_greaves.ToString(),
                        lvl_boots = lvl_boots.ToString(),
                        lvl_gloves = lvl_gloves.ToString(),
                        lvl_jewelry = lvl_jewelry.ToString(),

                    };
                    return eLvl;
                }

            }
            return null;
        }
    }


    [Route("/getequipmentprice")]
    public class ShopControllerEquipmentPrice : ControllerBase
    {
        private readonly ILogger<ShopControllerEquipmentPrice> _logger;

        public ShopControllerEquipmentPrice(ILogger<ShopControllerEquipmentPrice> logger)
        {
            _logger = logger;
        }

        private static int GetItemPrice(int basePrice, int level)
        {
            double itemMul = 1.15;
            if (level == 1)
            {
                return basePrice;
            }
            else
            {
                return (int)Math.Round((basePrice * Math.Pow(itemMul, level)));
            }
        }

        [HttpGet]
        public EquipmentPrice Get([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string connectionString = Startup.connString;
            int base_weapon_price = 0, base_armor_price = 0, base_greaves_price = 0, base_boots_price = 0, base_gloves_price = 0, base_jewelry_price = 0;
            int lvl_weapon = 0, lvl_armor = 0, lvl_greaves = 0, lvl_boots = 0, lvl_gloves = 0, lvl_jewelry = 0;

            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'weapon'";
                    base_weapon_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'armor'";
                    base_armor_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'greaves'";
                    base_greaves_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'boots'";
                    base_boots_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'gloves'";
                    base_gloves_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT price FROM shop_normal WHERE item = 'jewelry'";
                    base_jewelry_price = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = @"SELECT weapon_lvl, armor_lvl, greaves_lvl, boots_lvl, gloves_lvl, jewelry_lvl FROM inventory_equipment_items WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var reader = cmd.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            lvl_weapon = (int)reader.GetInt64(0);
                            lvl_armor = (int)reader.GetInt64(1);
                            lvl_greaves = (int)reader.GetInt64(2);
                            lvl_boots = (int)reader.GetInt64(3);
                            lvl_gloves = (int)reader.GetInt64(4);
                            lvl_jewelry = (int)reader.GetInt64(5);
                        }
                    }
                }
                EquipmentPrice ePrice = new EquipmentPrice
                {
                    weapon_price = GetItemPrice(base_weapon_price, lvl_weapon).ToString(),
                    armor_price = GetItemPrice(base_armor_price, lvl_armor).ToString(),
                    greaves_price = GetItemPrice(base_greaves_price, lvl_greaves).ToString(),
                    boots_price = GetItemPrice(base_boots_price, lvl_boots).ToString(),
                    gloves_price = GetItemPrice(base_gloves_price, lvl_gloves).ToString(),
                    jewelry_price = GetItemPrice(base_jewelry_price, lvl_jewelry).ToString()
                };
                return ePrice;
            }
            return null;
        }
    }

    [Route("/equipupgrade")]
    public class ShopEquipUpgradeController : ControllerBase
    {
        [HttpPost]
        public IActionResult EquipUpgrade([FromHeader(Name = "Cookie")] string cookieValue)
        {
            var equipment = JObject.Parse(Request.Headers["equipLvl"]);
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE inventory_equipment_items 
                                        SET weapon_lvl = @weapon_lvl, armor_lvl = @armor_lvl, greaves_lvl = @greaves_lvl, boots_lvl = @boots_lvl, gloves_lvl = @gloves_lvl, jewelry_lvl = @jewelry_lvl 
                                        WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@weapon_lvl", (int)equipment["lvl_weapon"]);
                    cmd.Parameters.AddWithValue("@armor_lvl", (int)equipment["lvl_armor"]);
                    cmd.Parameters.AddWithValue("@greaves_lvl", (int)equipment["lvl_greaves"]);
                    cmd.Parameters.AddWithValue("@boots_lvl", (int)equipment["lvl_boots"]);
                    cmd.Parameters.AddWithValue("@gloves_lvl", (int)equipment["lvl_gloves"]);
                    cmd.Parameters.AddWithValue("@jewelry_lvl", (int)equipment["lvl_jewelry"]);
                    cmd.ExecuteNonQuery();

                    var coins = int.Parse(Request.Headers["coins"]);
                    cmd.CommandText = @"UPDATE inventory SET coins = @coins_new WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@coins_new", coins);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }

    }

    [Route("/buyticket")]
    public class ShopBuyTicketController : ControllerBase
    {
        [HttpPost]
        public IActionResult BuyTicket([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];
            var coins = int.Parse(Request.Headers["coins"]);
            var ticket = int.Parse(Request.Headers["ticket"]);

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE inventory SET coins = @coins_new, tickets_characters = @tickets_new WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@coins_new", coins);
                    cmd.Parameters.AddWithValue("@tickets_new", ticket);
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok();
        }

    }
    [Route("/summonchar")]
    public class ShopSummonCharController : ControllerBase
    {
        [HttpPost]
        public IActionResult SummonChar([FromHeader(Name = "Cookie")] string cookieValue)
        {
            string jsonString = cookieValue.Substring(cookieValue.IndexOf("{"), cookieValue.LastIndexOf("}") - cookieValue.IndexOf("{") + 1);
            JObject jsonObject = JObject.Parse(jsonString);
            int userID = (int)jsonObject["userId"];

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    int itemId, ascension_lvl, total_drops;
                    int ticket = 0, coins = 0;

                    cmd.CommandText = @"SELECT coins, tickets_characters FROM inventory WHERE user_account_id = @userID";
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using var coinReader = cmd.ExecuteReader();
                    while (coinReader.Read())
                    {
                        coins = (int)coinReader.GetInt32(0);
                        ticket = (int)coinReader.GetInt32(1);
                    }
                    coinReader.Close();

                    if (ticket > 0)
                    {
                        ticket -= 1;
                        cmd.CommandText = @"UPDATE inventory SET tickets_characters = @tickets_new WHERE user_account_id = @userID";
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@tickets_new", ticket);
                        cmd.ExecuteNonQuery();
                        while (true)
                        {
                            cmd.CommandText = @"SELECT char_id FROM shop_gacha_characters 
                                    WHERE random() <= chance / 100.0
                                    ORDER BY random() LIMIT 1";
                            var try_itemId = cmd.ExecuteScalar();
                            if (try_itemId != null)
                            {
                                itemId = Convert.ToInt32(try_itemId);
                                break;
                            }
                        }

                        cmd.CommandText = @"SELECT ascension_lvl FROM inventory_characters
                                        WHERE char_id = @itemId AND user_account_id = @userID
                                        LIMIT 1";
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@itemId", itemId);
                        var try_ascension_lvl = cmd.ExecuteScalar();
                        if (try_ascension_lvl == null)
                            ascension_lvl = 1;
                        else
                            ascension_lvl = Convert.ToInt32(try_ascension_lvl);

                        cmd.CommandText = @"SELECT total_drops FROM inventory_characters
                                        WHERE char_id = @itemId AND user_account_id = @userID
                                        LIMIT 1";
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@itemId", itemId);

                        var try_total_drops = cmd.ExecuteScalar();
                        if (try_total_drops == null)
                            total_drops = 1;
                        else
                            total_drops = Convert.ToInt32(try_total_drops);

                        cmd.CommandText = @"SELECT rarity FROM characters
                                        WHERE character_id = @itemId 
                                        LIMIT 1";
                        cmd.Parameters.AddWithValue("@itemId", itemId);
                        int itemRarity = (int)cmd.ExecuteScalar();
                        int maxAscension = itemRarity + 1;
                        if (try_ascension_lvl == null)
                        {
                            cmd.CommandText = @"INSERT INTO inventory_characters (user_account_id, char_id, ascension_lvl, total_drops)
                                            OVERRIDING SYSTEM VALUE
                                            VALUES (@userID, @char_id, @ascension_lvl, @total_drops);";
                            cmd.Parameters.AddWithValue("@userID", userID);
                            cmd.Parameters.AddWithValue("@char_id", itemId);
                            cmd.Parameters.AddWithValue("@ascension_lvl", ascension_lvl);
                            cmd.Parameters.AddWithValue("@total_drops", 1);
                            cmd.ExecuteNonQuery();
                            ascension_lvl = 1;

                            cmd.CommandText = @"SELECT name, appearance_url 
                                        FROM characters 
                                        WHERE character_id = @char_id";
                            cmd.Parameters.AddWithValue("@char_id", itemId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string name = (string)reader.GetString(0);
                                    string appearance_url = (string)reader.GetString(1);
                                    var result = new DropResult
                                    {
                                        name = name,
                                        appearance_url = appearance_url,
                                        itemRarity = itemRarity.ToString(),
                                        ascension_lvl = ascension_lvl.ToString()
                                    };
                                    return new OkObjectResult(result);
                                }
                            }
                        }
                        else
                        {
                            total_drops += 1;
                            if (ascension_lvl < maxAscension)
                            {
                                ascension_lvl += 1;
                            }
                            if (ascension_lvl == maxAscension)
                            {
                                coins += Convert.ToInt32(100 * Math.Pow(maxAscension, 2));
                                cmd.CommandText = @"UPDATE inventory SET coins = @coins_new WHERE user_account_id = @userID";
                                cmd.Parameters.AddWithValue("@userID", userID);
                                cmd.Parameters.AddWithValue("@coins_new", coins);
                                cmd.ExecuteNonQuery();
                            }
                            cmd.CommandText = @"UPDATE inventory_characters 
                                            SET ascension_lvl = @ascension_lvl, total_drops = @total_drops
                                            WHERE user_account_id = @userID AND char_id = @char_id";
                            cmd.Parameters.AddWithValue("@userID", userID);
                            cmd.Parameters.AddWithValue("@ascension_lvl", ascension_lvl);
                            cmd.Parameters.AddWithValue("@total_drops", total_drops);
                            cmd.Parameters.AddWithValue("@char_id", itemId);
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = @"SELECT name, appearance_url 
                                        FROM characters 
                                        WHERE character_id = @char_id";
                            cmd.Parameters.AddWithValue("@char_id", itemId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string name = (string)reader.GetString(0);
                                    string appearance_url = (string)reader.GetString(1);
                                    var result = new DropResult
                                    {
                                        name = name,
                                        appearance_url = appearance_url,
                                        itemRarity = itemRarity.ToString(),
                                        ascension_lvl = ascension_lvl.ToString()
                                    };
                                    return new OkObjectResult(result);
                                }
                            }
                        }
                    }              
                }

            }
            return Ok();
        }

    }
}
