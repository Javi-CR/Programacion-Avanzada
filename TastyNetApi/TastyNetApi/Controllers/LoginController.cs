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

namespace TastyNetApi.Controllers

{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;

        public LoginController(IConfiguration conf, IHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }

        [HttpPost]
        [Route("CrearCuenta")]
        public IActionResult CrearCuenta(Users model)
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.Execute("CrearCuenta",
                    new { model.Identificacion, model.Nombre, model.Correo, model.Contrasenna });

                if (result > 0)
                {
                    respuesta.Codigo = 0;
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "Su información no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion(Users model)
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result =
                    context.QueryFirstOrDefault<Users>("IniciarSesion", new { model.Correo, model.Contrasenna });

                if (result != null)
                {
                    if (result.UsaClaveTemp && result.Vigencia < DateTime.Now)
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Su información de acceso temporal ha expirado";
                    }
                    else
                    {
                        //result.Token = GenerarToken(result);

                        respuesta.Codigo = 0;
                        respuesta.Contenido = result;
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
    }
}