using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TastyNetApi.Models;

namespace TastyNetApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {

        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;

        public ProfileController(IConfiguration conf, IHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }

        [HttpGet]
        [Route("CheckUser")]
        public IActionResult CheckUser(long Consecutivo)
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.QueryFirstOrDefault<Users>("CheckUserID", new { Id = Consecutivo });

                if (result != null)
                {
                    respuesta.Codigo = 0;
                    respuesta.Contenido = result;
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "No se encontró la información del usuario";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("UpdateProfile")]
        public IActionResult UpdateProfile(Users model)
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.Execute("UpdateProfile", new { model.Id, model.Name, model.Email, model.ProfilePicture });

                if (result > 0)
                {
                    respuesta.Codigo = 0;
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "La información del producto no se ha actualizado correctamente";
                }

                return Ok(respuesta);
            }
        }


    }
}
