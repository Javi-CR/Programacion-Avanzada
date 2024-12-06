using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TastyNetApi.Models;

namespace TastyNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RecetasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("CrearReceta")]
        public async Task<IActionResult> CrearReceta([FromBody] Recipe receta)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var recipeId = await InsertRecipeAsync(connection, transaction, receta.Name, receta.CategoryId, receta.UserId);

                foreach (var ingredient in receta.Ingredients)
                {
                    await InsertIngredientAsync(connection, transaction, recipeId, ingredient.Name, ingredient.Quantity);
                }

                foreach (var step in receta.RecipeSteps)
                {
                    await InsertRecipeStepAsync(connection, transaction, recipeId, step.StepNumber, step.Description);
                }

                await InsertFavoriteAsync(connection, transaction, receta.UserId, recipeId);

                await transaction.CommitAsync();
                return Ok(new { Message = "Receta creada exitosamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error en CrearReceta: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al crear la receta.");
            }
        }

        [HttpGet("ObtenerRecetasFavoritas")]
        public async Task<IActionResult> ObtenerRecetasFavoritas(long userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                var recetasFavoritas = await GetFavoriteRecipesAsync(connection, userId);
                return Ok(recetasFavoritas);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error en ObtenerRecetasFavoritas: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en ObtenerRecetasFavoritas: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error inesperado al obtener las recetas favoritas.");
            }
        }

        private async Task<long> InsertRecipeAsync(SqlConnection connection, SqlTransaction transaction, string name, long categoryId, long userId)
        {
            using var command = new SqlCommand("InsertRecipe", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.Parameters.AddWithValue("@UserId", userId);

            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }

        private async Task InsertIngredientAsync(SqlConnection connection, SqlTransaction transaction, long recipeId, string name, string quantity)
        {
            using var command = new SqlCommand("InsertIngredient", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Quantity", quantity);

            await command.ExecuteNonQueryAsync();
        }

        private async Task InsertRecipeStepAsync(SqlConnection connection, SqlTransaction transaction, long recipeId, int stepNumber, string description)
        {
            using var command = new SqlCommand("InsertRecipeStep", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            command.Parameters.AddWithValue("@StepNumber", stepNumber);
            command.Parameters.AddWithValue("@Description", description);

            await command.ExecuteNonQueryAsync();
        }

        private async Task InsertFavoriteAsync(SqlConnection connection, SqlTransaction transaction, long userId, long recipeId)
        {
            using var command = new SqlCommand("InsertFavorite", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RecipeId", recipeId);

            await command.ExecuteNonQueryAsync();
        }

        private async Task<List<dynamic>> GetFavoriteRecipesAsync(SqlConnection connection, long userId)
        {
            try
            {
                using var command = new SqlCommand("GetFavoriteRecipes", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = await command.ExecuteReaderAsync();
                var result = new List<dynamic>();

                while (await reader.ReadAsync())
                {
                    try
                    {
                        result.Add(new
                        {
                            RecipeId = reader.GetInt64(0),
                            RecipeName = reader.GetString(1),
                            CategoryName = reader.GetString(2),
                            IngredientName = reader.IsDBNull(3) ? null : reader.GetString(3),
                            IngredientQuantity = reader.IsDBNull(4) ? null : reader.GetString(4),
                            StepNumber = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                            StepDescription = reader.IsDBNull(6) ? null : reader.GetString(6)
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error al procesar el registro actual: {ex.Message}\nStackTrace: {ex.StackTrace}", ex);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al ejecutar el procedimiento almacenado: {ex.Message}\nStackTrace: {ex.StackTrace}", ex);
            }
        }



        [HttpDelete("EliminarReceta/{recipeId}")]
        public async Task<IActionResult> EliminarReceta(long recipeId)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DeleteRecipe", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@RecipeId", recipeId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                    return NotFound(new { Message = "Receta no encontrada." });

                return Ok(new { Message = "Receta eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la receta: {ex.Message}");
            }
        }


    }
}
