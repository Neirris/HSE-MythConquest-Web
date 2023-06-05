using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MythConquestWeb;

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
        public List<string> Get()
        {
            List<string> nicknames = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();

                int userID = 1;
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
}
