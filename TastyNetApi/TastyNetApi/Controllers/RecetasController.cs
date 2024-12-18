using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

using TastyNetApi.Models;
using TastyNetApi.Request;

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
        public async Task<IActionResult> CrearReceta([FromBody] RecipeRequest receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        private async Task<List<dynamic>> GetFavoriteRecipesWithDetailsAsync(SqlConnection connection, long userId)
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

        [HttpGet("ObtenerRecetasDestacadas")]
        public async Task<IActionResult> ObtenerRecetasDestacadas()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                var recetasDestacadas = await GetFeaturedRecipesAsync(connection);
                return Ok(recetasDestacadas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerRecetasDestacadas: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al obtener las recetas destacadas.");
            }
        }

        private async Task<List<RecipesFeaturedRequest>> GetFeaturedRecipesAsync(SqlConnection connection)
        {
            using var command = new SqlCommand("GetFeaturedRecipes", connection) { CommandType = CommandType.StoredProcedure };

            using var reader = await command.ExecuteReaderAsync();
            var result = new List<RecipesFeaturedRequest>();

            while (await reader.ReadAsync())
            {
                result.Add(new RecipesFeaturedRequest
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Image = reader.GetString(2),
                });
            }

            return result;
        }
        [HttpGet("BuscarRecetas")]
        public async Task<IActionResult> BuscarRecetas(string searchTerm)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                var recetasEncontradas = await SearchRecipesAsync(connection, searchTerm);
                return Ok(recetasEncontradas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarRecetas: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al buscar las recetas.");
            }
        }

        private async Task<List<SearchRequest>> SearchRecipesAsync(SqlConnection connection, string searchTerm)
        {
            using var command = new SqlCommand("SearchRecipes", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@SearchTerm", searchTerm);

            using var reader = await command.ExecuteReaderAsync();
            var result = new List<SearchRequest>();

            while (await reader.ReadAsync())
            {
                result.Add(new SearchRequest
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    CategoryName = reader.GetString(2),
                });
            }

            return result;
        }
        [HttpGet("ObtenerDetallesReceta/{recipeId}")]
        public async Task<IActionResult> ObtenerDetallesReceta(long recipeId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                var recetaDetalles = await GetRecipeDetailsAsync(connection, recipeId);
                return Ok(recetaDetalles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDetallesReceta: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al obtener los detalles de la receta.");
            }
        }
        private async Task<RecipeDetailsRequest> GetRecipeDetailsAsync(SqlConnection connection, long recipeId)
        {
            using var command = new SqlCommand("GetRecipeDetails", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@RecipeId", recipeId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new RecipeDetailsRequest
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Ingredients = reader.IsDBNull(2) ? null : reader.GetString(2).Split("; "),
                    Steps = reader.IsDBNull(3) ? null : reader.GetString(3).Split("; "),
                };
            }

            return null; 
        }

        [HttpGet("ObtenerTodasLasCategorias")]
        public async Task<IActionResult> ObtenerTodasLasCategorias()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                using var command = new SqlCommand("GetAllCategories", connection) { CommandType = CommandType.StoredProcedure };
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<AllCategoriesRequest>();
                while (await reader.ReadAsync())
                {
                    result.Add(new AllCategoriesRequest
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1)
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerTodasLasCategorias: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al obtener las categorías.");
            }
        }

        [HttpGet("ObtenerRecetasPorCategoria/{categoryId}")]
        public async Task<IActionResult> ObtenerRecetasPorCategoria(long categoryId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                using var command = new SqlCommand("GetRecipesByCategory", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                using var reader = await command.ExecuteReaderAsync();
                var result = new List<RecipesByCategoryRequest>();

                while (await reader.ReadAsync())
                {
                    result.Add(new RecipesByCategoryRequest
                    {
                        Id = reader.GetInt64(0),
                        CategoryId = reader.GetInt64(1),
                        UserId = reader.GetInt64(2),
                        Name = reader.GetString(3),
                        Image = reader.GetString(4),
                        CreatedRecipes = reader.GetDateTime(5)
                    });
                }
                var formattedResult = result.Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.CategoryId,
                    r.UserId,
                    r.Image,
                    CreatedRecipes = r.CreatedRecipes.ToString("dd/MM/yyyy HH:mm:ss")
                });
                return Ok(formattedResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerRecetasPorCategoria: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al obtener las recetas.");
            }
        }

        [HttpGet("ObtenerSubcategoriasPorCategoria/{parentCategoryId}")]
        public async Task<IActionResult> ObtenerSubcategoriasPorCategoria(long parentCategoryId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                using var command = new SqlCommand("GetSubcategoriesByCategory", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);

                using var reader = await command.ExecuteReaderAsync();
                var result = new List<SubCategoriesByCategoryRequest>();

                while (await reader.ReadAsync())
                {
                    result.Add(new SubCategoriesByCategoryRequest
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1)
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerSubcategoriasPorCategoria: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Error al obtener las subcategorías.");
            }
        }
    }
}
