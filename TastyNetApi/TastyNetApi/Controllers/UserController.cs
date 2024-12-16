using Dapper;
using TastyNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TastyNetApi.Request;

namespace TastyNetApi.Controllers

{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;

        public UserController(IConfiguration conf, IHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                context.Open();
                using (var transaction = context.BeginTransaction())
                {
                    var respuesta = new Respuesta();

                    try
                    {
                        var encryptedPassword = Encrypt(model.Password);  
                        var result = context.Execute("CreateUser",
                            new
                            {
                                IdentificationNumber = model.IdentificationNumber.ToString(),
                                model.Name,
                                model.Email,
                                Password = encryptedPassword
                            }, transaction: transaction);

                        if (result > 0)
                        {                          
                            transaction.Commit();
                            respuesta.Codigo = 0;
                            respuesta.Mensaje = "Su información se ha registrado correctamente";
                        }
                        else
                        {
                            transaction.Rollback();
                            respuesta.Codigo = -1;
                            respuesta.Mensaje = "Su información no se ha registrado correctamente (Usuario fallido)";
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = $"Error: {ex.Message}";
                    }

                    return Ok(respuesta);
                }
            }
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.QueryFirstOrDefault<Users>("Login", new { model.Email});

                if (result != null)
                {
                    string decryptedPassword = Decrypt(result.Password);
                    if (decryptedPassword == model.Password)
                    {
                        if (result.UseTempPassword && result.Validity < DateTime.Now)
                        {
                            respuesta.Codigo = -1;
                            respuesta.Mensaje = "Su información de acceso temporal ha expirado";
                        }
                        else
                        {
                            respuesta.Codigo = 0;
                            respuesta.Mensaje = $"Bienvenido usuario, {result.Name}";
                            respuesta.Contenido = result;
                        }
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Contraseña incorrecta";
                    }
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "Su información no se encontró en nuestro sistema";
                }

                return Ok(respuesta);
            }
        }

        private string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_conf.GetSection("Variables:Llave").Value!);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private string Decrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(texto);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_conf.GetSection("Variables:Llave").Value!);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }

            }
        }
    }
}    
