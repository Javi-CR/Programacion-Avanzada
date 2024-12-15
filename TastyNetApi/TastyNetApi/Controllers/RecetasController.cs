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
            if (receta == null || string.IsNullOrWhiteSpace(receta.Name) || receta.UserId <= 0)
            {
                return BadRequest("Datos inválidos. Asegúrate de proporcionar un nombre válido, un UserId válido y otros datos necesarios.");
            }

            receta.Ingredients ??= new List<Ingredient>();
            receta.RecipeSteps ??= new List<RecipeStep>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Validar que el UserId exista en la tabla Users
                if (!await UserExistsAsync(connection, transaction, receta.UserId))
                {
                    return BadRequest($"El usuario con ID {receta.UserId} no existe en la base de datos.");
                }

                // Insertar la receta y obtener su ID
                var recipeId = await InsertRecipeAsync(connection, transaction, receta.Name, receta.CategoryId, receta.UserId);

                // Insertar los ingredientes
                foreach (var ingredient in receta.Ingredients)
                {
                    await InsertIngredientAsync(connection, transaction, recipeId, ingredient.Name, ingredient.Quantity);
                }

                // Insertar los pasos
                foreach (var step in receta.RecipeSteps)
                {
                    if (step == null || string.IsNullOrWhiteSpace(step.Description))
                    {
                        Console.WriteLine("Paso nulo o inválido encontrado, omitiendo...");
                        continue;
                    }
                    await InsertRecipeStepAsync(connection, transaction, recipeId, step.StepNumber, step.Description);
                }

                // Agregar la receta a favoritos
                await InsertFavoriteAsync(connection, transaction, receta.UserId, recipeId);

                await transaction.CommitAsync();
                Console.WriteLine("Transacción confirmada exitosamente.");
                return Ok(new { Message = "Receta creada exitosamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error en CrearReceta: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno al crear la receta: {ex.Message}");
            }
        }

        [HttpGet("ObtenerRecetasFavoritas")]
        public async Task<IActionResult> ObtenerRecetasFavoritas(long userId)
        {
            if (userId <= 0)
            {
                return BadRequest("El UserId proporcionado no es válido.");
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                var recetasFavoritas = await GetFavoriteRecipesWithDetailsAsync(connection, userId);
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

        private async Task<bool> UserExistsAsync(SqlConnection connection, SqlTransaction transaction, long userId)
        {
            const string query = "SELECT COUNT(1) FROM Users WHERE Id = @UserId";
            using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@UserId", userId);
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
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

        private async Task<List<dynamic>> GetFavoriteRecipesWithDetailsAsync(SqlConnection connection, long userId)
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
                result.Add(new
                {
                    RecipeId = reader.GetInt64(0),
                    RecipeName = reader.GetString(1),
                    CategoryName = reader.GetString(2),
                    Ingredients = reader.IsDBNull(3) ? null : reader.GetString(3).Split("; ").Select(i =>
                    {
                        var parts = i.Split(':');
                        return new { Name = parts[0], Quantity = parts.Length > 1 ? parts[1] : null };
                    }).ToList(),
                    Steps = reader.IsDBNull(4) ? null : reader.GetString(4).Split("; ").Select(s =>
                    {
                        var parts = s.Split(": ");
                        return new { StepNumber = int.Parse(parts[0]), Description = parts.Length > 1 ? parts[1] : null };
                    }).ToList()
                });
            }

            return result;
        }

        [HttpDelete("EliminarReceta/{recipeId}")]
        public async Task<IActionResult> EliminarReceta(long recipeId)
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
    }
}
