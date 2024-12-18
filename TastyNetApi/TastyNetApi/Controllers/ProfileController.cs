using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TastyNetApi.Models;
using TastyNetApi.Request;

namespace TastyNetApi.Controllers
{
    //[Authorize]
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
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var respuesta = new Respuesta();
                var result = context.QueryFirstOrDefault<Users>("CheckUserID", new { Id = Consecutivo });

                if (result != null)
                {
                    respuesta.Codigo = 0;
                    respuesta.Contenido = result;
                    return Ok(respuesta);
                }
                else
                {
                    respuesta.Codigo = -1;
                    respuesta.Mensaje = "No se encontró la información del usuario";
                    return NotFound(respuesta);
                }
            }
        }


        [HttpPut]
        [Route("UpdateProfile")]
        public IActionResult UpdateProfile(ProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(model.ProfilePicture))
            {
                if (!model.ProfilePicture.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                    !model.ProfilePicture.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("ProfilePicture", "Solo se permiten imágenes con extensiones .JPG o .PNG.");
                    return BadRequest(ModelState);
                }
            }

            var respuesta = new Respuesta();

            try
            {
                using (var connection = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@Id", model.Id);
                            parameters.Add("@Name", model.Name);
                            parameters.Add("@Email", model.Email);
                            parameters.Add("@ProfilePicture", model.ProfilePicture);
                            parameters.Add("@Changes", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);

                            var result = connection.Execute(
                                "UpdateProfile",
                                parameters,
                                transaction: transaction,
                                commandType: System.Data.CommandType.StoredProcedure
                            );

                            var changesLog = parameters.Get<string>("@Changes");

                            if (result > 0)
                            {
                                transaction.Commit();

                                respuesta.Codigo = 0;
                                respuesta.Mensaje = $"Perfil actualizado correctamente. {changesLog}";
                                respuesta.Contenido = $"Updated as: {changesLog}"; 

                                return Ok(respuesta);
                            }
                            else
                            {
                                transaction.Rollback();
                                respuesta.Codigo = -1;
                                respuesta.Mensaje = "La información del perfil no se ha actualizado correctamente.";
                                return Conflict(respuesta);
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Codigo = -1;
                respuesta.Mensaje = $"Error en la actualización: {ex.Message}";
                return StatusCode(500, respuesta);
            }
        }

        [HttpPut]
        [Route("InsertFavoriteRecipeProfile/{userId}/{recipeId}")]
        public IActionResult InsertFavoriteRecipe(long userId, long recipeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = new Respuesta();

            try
            {
                using (var connection = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            var parameters = new DynamicParameters();
                            parameters.Add("@UserId", userId);
                            parameters.Add("@RecipeId", recipeId);
                            var result = connection.Execute(
                                "AgregarAFavoritos",
                                parameters,
                                transaction: transaction,
                                commandType: System.Data.CommandType.StoredProcedure
                            );

                            if (result > 0)
                            {
                                transaction.Commit();
                                respuesta.Codigo = 1;
                                respuesta.Mensaje = "Receta agregada a favoritos exitosamente.";
                                return Ok(respuesta);
                            }
                            else
                            {
                                transaction.Rollback();
                                respuesta.Codigo = 0;
                                respuesta.Mensaje = "No se pudo agregar la receta a favoritos, la receta ya existe en favoritos.";
                                return Conflict(respuesta);
                            }

                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Codigo = -1;
                respuesta.Mensaje = $"Error en la conexión o transacción: {ex.Message}";
                return StatusCode(500, respuesta);
            }
        }


        [HttpGet]
        [Route("GetFavoriteRecipes/{userId}")]
        public IActionResult GetFavoriteRecipes(long userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = new Respuesta();
            try
            {
                using (var connection = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
                {
                    connection.Open();

                    var favoriteRecipes = connection.Query<FavoriteRecipeRequest>(
                        "GetFavoriteRecipes",
                        new { UserId = userId },
                        commandType: CommandType.StoredProcedure
                    ).ToList();

                    if (favoriteRecipes.Any())
                    {
                        respuesta.Codigo = 0;
                        respuesta.Mensaje = "Todas tus recetas favoritas";
                        respuesta.Contenido = favoriteRecipes;
                        return Ok(respuesta);
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "No se encontraron recetas favoritas para este usuario.";
                        return NotFound(respuesta);
                    }

                }
            }
            catch (Exception ex)
            {
                respuesta.Codigo = -1;
                respuesta.Mensaje = $"Error al obtener las recetas favoritas: {ex.Message}";
                return StatusCode(500, respuesta);
            }
        }


    }
}
