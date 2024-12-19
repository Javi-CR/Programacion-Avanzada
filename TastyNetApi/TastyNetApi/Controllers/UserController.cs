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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

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
                        var verificationToken = GenerarCodigoFuerte();

                        var parameters = new DynamicParameters();
                        parameters.Add("@IdentificationNumber", model.IdentificationNumber.ToString());
                        parameters.Add("@Name", model.Name);
                        parameters.Add("@Email", model.Email);
                        parameters.Add("@Password", encryptedPassword);
                        parameters.Add("@VerificationToken", verificationToken);

                        var result = context.Execute("CreateUser", parameters, transaction: transaction);


                        if (result > 0)
                        {
                            var ruta = Path.Combine(_env.ContentRootPath, "CorreoVerificacion.html");
                            var html = System.IO.File.ReadAllText(ruta);

                            html = html.Replace("@@Name", model.Name);
                            html = html.Replace("@@VerificationCode", verificationToken);
                            EnviarCorreo(model.Email, "Verificación de correo electrónico", html);

                            transaction.Commit();
                            respuesta.Codigo = 0;
                            respuesta.Mensaje = "Registro exitoso. Por favor verifica tu correo electrónico usando el código enviado.";
                            return Ok(respuesta);

                        }
                        else
                        {
                            transaction.Rollback();
                            respuesta.Codigo = -1;
                            respuesta.Mensaje = "No se pudo completar el registro.";
                            return Conflict(respuesta);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = $"Error: {ex.Message}";
                        return StatusCode(500, respuesta);
                    }
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
                var user = context.QueryFirstOrDefault<Users>("Login", new { model.Email });

                if (user != null)
                {
                    if (user.LockedUntil != null && DateTime.Now < user.LockedUntil)
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Su cuenta está bloqueada. Intente nuevamente después de " + user.LockedUntil;
                        return Conflict(respuesta); 
                    }

                    if (user.ValidatedEmail == false)
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Su cuenta aun no esta activada. Ingrese a su correo y verifique su email";
                        return BadRequest(respuesta); 
                    }

                    string decryptedPassword = string.Empty;

                    if (user.UseTempPassword == true)
                    {
                        decryptedPassword = user.Password;
                    }
                    else
                    {
                        decryptedPassword = Decrypt(user.Password);
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", user.Id);
                    


                    if (decryptedPassword == model.Password)
                    {
                        context.Execute("ResetFailedAttempts", parameters, commandType: CommandType.StoredProcedure);
                        respuesta.Codigo = 0;
                        respuesta.Mensaje = $"Bienvenido, {user.Name}";
                        respuesta.Contenido = user;
                        return Ok(respuesta);

                    }
                    else
                    {
                        context.Execute("IncrementFailedAttempts", parameters, commandType: CommandType.StoredProcedure);

                        var attempts = user.FailedAttempts + 1;
                        respuesta.Codigo = -1;

                        if (attempts >= 5)
                        {
                            context.Execute("LockUserAccount", parameters, commandType: CommandType.StoredProcedure);
                            respuesta.Mensaje = "Cuenta bloqueada por múltiples intentos fallidos.";
                            return Conflict(respuesta);
                        }
                        
                        respuesta.Mensaje = "Contraseña incorrecta.";
                        return Unauthorized(respuesta);
                    }
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "Usuario no encontrado.";
                }

                return NotFound(respuesta);
            }
        }


        [HttpGet]
        [Route("ConsultarUsuarios")]
        public IActionResult ConsultarUsuarios()
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.Query<Users>("GetAllUsers", new { });

                if (result.Any())
                {
                    respuesta.Codigo = 0;
                    respuesta.Contenido = result;
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "There are no registered users at this time";
                }

                return Ok(respuesta);
            }
        }



        [HttpDelete]
        [Route("DeleteAccount")]
        public IActionResult DeleteAccount(int userId)
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();

                try
                {
                    var rowsAffected = context.Execute("DeleteUserAccount", new { UserId = userId }, commandType: CommandType.StoredProcedure);

                    if (rowsAffected > 0)
                    {
                        respuesta.Codigo = 0;
                        respuesta.Mensaje = "Usuario eliminado exitosamente.";
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Usuario no encontrado.";
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Codigo = 500;
                    respuesta.Mensaje = "Error interno del servidor.";
                }

                return Ok(respuesta);
            }
        }





        [HttpPost]
        [Route("RecuperarAcceso")]
        public IActionResult RecuperarAcceso(AccessRecoveryRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();

                var result = context.QueryFirstOrDefault<Users>("ValidarUsuario", new { model.Email });

                if (result != null)
                {
                    var Codigo = GenerarCodigoFuerte();
                    var Password = Encrypt(Codigo);
                    var UseTempPassword = true;
                    var Validity = DateTime.Now.AddMinutes(10);
                    context.Execute("ActualizarContrasenna", new { result.Id, Password, UseTempPassword, Validity });

                    var ruta = Path.Combine(_env.ContentRootPath, "RecuperarAcceso.html");
                    var html = System.IO.File.ReadAllText(ruta);

                    html = html.Replace("@@Name", result.Name);
                    html = html.Replace("@@Password", Codigo);
                    html = html.Replace("@@Validity", Validity.ToString("dd/MM/yyyy hh:mm tt"));

                    EnviarCorreo(result.Email, "Recuperar Accesos Sistema", html);

                    respuesta.Codigo = 0;
                    respuesta.Contenido = result;
                    return Ok(respuesta);
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "Su información no se encontró en nuestro sistema";
                }

                return NotFound(respuesta);
            }
        }

        [HttpPost]
        [Route("VerifyEmail")]
        public IActionResult VerifyEmail([FromBody] VerifyTokenRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(model.VerificationToken))
            {
                return BadRequest("Token es requerido.");
            }

            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@VerificationToken", (model.VerificationToken));
                    parameters.Add("@ReturnValue", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.ReturnValue);

                    context.Execute(
                        "VerifyToken",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    int returnValue = parameters.Get<int>("@ReturnValue");

                    if (returnValue > 0)
                    {
                        respuesta.Codigo = 0;
                        respuesta.Mensaje = "Cuenta verificada exitosamente. Ya puedes iniciar sesión.";
                        return Ok(respuesta);
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "El token de verificación es inválido o ha expirado.";
                        return BadRequest(respuesta);
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = $"Error: {ex.Message}";
                    return StatusCode(500, respuesta);
                }

            }
        }

        [HttpGet]
        [Route("CheckTokenExistence")]
        public IActionResult CheckTokenExistence(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("El email es requerido.");
            }
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Email", email);

                    parameters.Add("@ReturnValue", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.ReturnValue);

                    context.Execute(
                        "CheckIfTokenExistsForEmail",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    int returnValue = parameters.Get<int>("@ReturnValue");

                    if (returnValue > 0)
                    {
                        respuesta.Codigo = 0;
                        respuesta.Mensaje = "Token encontrado.";
                        return Ok(respuesta);
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "El token no existe con este email o ha expirado.";
                        return NotFound(respuesta);
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = $"Error: {ex.Message}";
                    return StatusCode(500, respuesta);
                }
            }
        }

        private string GenerarCodigoFuerte()
        {
            int length = 18;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+<>?";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }



        private void EnviarCorreo(string destino, string asunto, string contenido)
        {
            if (string.IsNullOrWhiteSpace(destino) || !EsCorreoValido(destino))
                throw new ArgumentException("El correo del destinatario está vacío.");

            destino = destino.Trim();

            try
            {
                MailAddress recipient = new MailAddress(destino);

                string cuenta = _conf.GetSection("Variables:CorreoEmail").Value!;
                string contrasenna = _conf.GetSection("Variables:ClaveEmail").Value!;

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(cuenta),
                    Subject = asunto,
                    Body = contenido,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };
                message.To.Add(recipient);

                SmtpClient client = new SmtpClient("smtp.office365.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(cuenta, contrasenna),
                    EnableSsl = true
                };

                client.Send(message);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Formato inválido para la dirección de correo: " + destino);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Error SMTP: " + smtpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general: " + ex.Message);
                throw;
            }
        }


        private bool EsCorreoValido(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, patron);
        }


        private string GenerarToken(Users model)
        {
            string SecretKey = _conf.GetSection("Variables:Llave").Value!;

            List<Claim> claims = new List<Claim>();
            {
                claims.Add(new Claim("IdUsuario", model.Id.ToString()));
                claims.Add(new Claim("IdRol", model.RoleId.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
