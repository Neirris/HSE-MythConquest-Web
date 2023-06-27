using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Npgsql;
using MythConquestWeb;

namespace MythConquestWeb.Controllers
{
    [Route("/register")]
    [ApiController]
    public class AuthRegisterController : ControllerBase
    {
        public static string ConvertHashSHA512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var res = BitConverter.ToString(hashedInputBytes).Replace("-", "").ToLower();
                return res;
            }
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            string login = user.userName;
            string password = user.userPass;
            // Логика для регистрации пользователя
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    bool isUserExists = false;
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT EXISTS (SELECT 1 FROM users WHERE user_login = @login)";
                    cmd.Parameters.AddWithValue("@login", login);
                    isUserExists = (bool)cmd.ExecuteScalar();

                    if (isUserExists)
                    {
                        return BadRequest("UsersExists");
                    }
                    else
                    {
                        string token = ConvertHashSHA512(login + password);
                        password = ConvertHashSHA512(password);
                        cmd.CommandText = @"INSERT INTO users (user_login, user_password, user_token)
                                            VALUES (@login, @password, @token)";
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@token", token);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"SELECT user_id FROM users WHERE user_token = @token";
                        cmd.Parameters.AddWithValue("@token", token);
                        int uid = (int)cmd.ExecuteScalar();

                        user.userId = uid;
                        user.userName = login;
                        user.userPass = "??";
                        user.isAuth = true;
                        user.isAdmin = false;
                        //сохранения пользователя в куки
                        HttpContext.Response.Cookies.Append("User", JsonConvert.SerializeObject(user));
                        return new OkObjectResult(user);
                    }
                }

            }
        }
    }

    [Route("/logout")]
    public class AuthLogoutController : ControllerBase
    {
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("User");
            User user = new User
            {
                userId = -1,
                userName = "??",
                userPass = "??",
                isAuth = false,
                isAdmin = false
            };
            HttpContext.Response.Cookies.Append("User", JsonConvert.SerializeObject(user));
            return Ok();
        }
    }

    [Route("/fcheck")]
    public class AuthFirstCheckController : ControllerBase
    {
        [HttpPost]
        public IActionResult FirstVisit()
        {
            // Проверка, существует ли куки "User"
            if (!Request.Cookies.ContainsKey("User"))
            {
                // Создание нового объекта User
                User user = new User
                {
                    userId = -1,
                    userName = "??",
                    userPass = "??",
                    isAuth = false,
                    isAdmin = false
                };

                // Сохранение пользователя в куки
                HttpContext.Response.Cookies.Append("User", JsonConvert.SerializeObject(user));
            }
            return Ok();
        }
    }

    [Route("/login")]
    public class AuthLoginController : ControllerBase
    {
        public static string ConvertHashSHA512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var res = BitConverter.ToString(hashedInputBytes).Replace("-", "").ToLower();
                return res;
            }
        }
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            string login = user.userName;
            string password = user.userPass;
            using (NpgsqlConnection conn = new NpgsqlConnection(Startup.connString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    bool isUserExists = false;
                    bool isCorrect = false;
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT EXISTS (SELECT 1 FROM users WHERE user_login = @login)";
                    cmd.Parameters.AddWithValue("@login", login);
                    isUserExists = (bool)cmd.ExecuteScalar();

                    if (!isUserExists)
                    {
                        return BadRequest("NoUsersExists");
                    }
                    if (isUserExists)
                    {
                        cmd.CommandText = @"SELECT EXISTS (SELECT 1 FROM users WHERE user_login = @login AND user_password = @password)";
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", ConvertHashSHA512(password));
                        isCorrect = (bool)cmd.ExecuteScalar();

                        if (!isCorrect)
                        {
                            return BadRequest("InvalidPassword");
                        }
                        if (isCorrect)
                        {
                            string token = ConvertHashSHA512(login + password);
                            cmd.CommandText = @"SELECT user_id FROM users WHERE user_token = @token";
                            cmd.Parameters.AddWithValue("@token", token);
                            int uid = (int)cmd.ExecuteScalar();
                            user.userId = uid;
                            user.userName = login;
                            user.userPass = "??";
                            user.isAuth = true;
                            user.isAdmin = false;
                            HttpContext.Response.Cookies.Append("User", JsonConvert.SerializeObject(user));
                            return new OkObjectResult(user);
                        }
                    }
                }

            }
            return BadRequest();
        }
    }


}
