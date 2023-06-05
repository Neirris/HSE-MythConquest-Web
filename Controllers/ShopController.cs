using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;

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
        public TicketPrice Get()
        {
            string connectionString = Startup.connString;
            int userID = 1, l_char_ticket_price;

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
        public CurrentCoins Get()
        {
            string connectionString = Startup.connString;
            int userID = 1, l_coins = 0, l_char_tickets = 0;

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
        public CurrentEquipmentLvl Get()
        {
            string connectionString = Startup.connString;
            int userID = 1;
            int lvl_weapon = 0, lvl_armor = 0, lvl_greaves = 0, lvl_boots = 0, lvl_gloves = 0, lvl_jewelry = 0;

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
        public EquipmentPrice Get()
        {
            string connectionString = Startup.connString;
            int userID = 1;
            int base_weapon_price = 0, base_armor_price = 0, base_greaves_price = 0, base_boots_price = 0, base_gloves_price = 0, base_jewelry_price = 0;
            int lvl_weapon = 0, lvl_armor = 0, lvl_greaves = 0, lvl_boots = 0, lvl_gloves = 0, lvl_jewelry = 0;

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
}
