using Microsoft.AspNetCore.Mvc;
using TastyNet.Servicios;
using TastyNet.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace TastyNet.Controllers
{
    [Route("Recetas")]
    [ApiController]
    public class RecetasController : Controller
    {
        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        [HttpPost("CrearReceta")]
        public async Task<IActionResult> CrearReceta([FromBody] Recipe model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("El modelo o el nombre de la receta no puede ser nulo.");

            try
            {
                var message = await _recetaService.CrearRecetaAsync(model);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("ObtenerRecetasFavoritas")]
        public async Task<IActionResult> ObtenerRecetasFavoritas()
        {
            var userId = 1; // Usuario fijo para prueba
            try
            {
                var recetas = await _recetaService.ObtenerRecetasFavoritasAsync(userId);
                return Json(recetas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("EliminarReceta/{recipeId}")]
        public async Task<IActionResult> EliminarReceta(long recipeId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Llama al procedimiento almacenado para eliminar la receta y sus datos relacionados
                using var command = new SqlCommand("DeleteRecipe", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@RecipeId", recipeId);
                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return Ok(new { Message = "Receta eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al eliminar la receta: {ex.Message}");
            }
        }








    }
}
