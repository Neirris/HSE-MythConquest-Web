using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;

namespace MythConquestWeb.Controllers
{
    [Route("/getpotions")]
    public class TowerController : ControllerBase
    {
        private readonly ILogger<TowerController> _logger;

        public TowerController(ILogger<TowerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public TowerHp Get()
        {
            string connectionString = Startup.connString;
            int userID = 1;

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

            return null;
        }
    }
}
