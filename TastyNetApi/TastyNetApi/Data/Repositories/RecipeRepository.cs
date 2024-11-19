using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TastyNetApi.Models.DTOs;

namespace TastyNestApi.Data.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IDbConnection _dbConnection;

        public RecipeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Obtener lista de recetas con búsqueda y filtro por categoría
        public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(string? search, long? category)
        {
            var query = @"SELECT Id, Name, Description, Image 
                          FROM Recipes
                          WHERE (@Search IS NULL OR Name LIKE '%' + @Search + '%' OR Description LIKE '%' + @Search + '%')
                          AND (@Category IS NULL OR CategoryId = @Category)";

            return await _dbConnection.QueryAsync<RecipeDto>(query, new { Search = search, Category = category });
        }

        // Obtener detalles completos de una receta por ID
        public async Task<RecipeDetailsDto?> GetRecipeByIdAsync(long id)
        {
            // Query principal para la receta
            var recipeQuery = @"SELECT Id, Name, Description, Image 
                                FROM Recipes 
                                WHERE Id = @Id";

            // Query para los ingredientes de la receta
            var ingredientsQuery = @"SELECT Name, Quantity 
                                     FROM Ingredients 
                                     WHERE RecipeId = @RecipeId";

            // Query para los pasos de preparación de la receta
            var stepsQuery = @"SELECT StepNumber, Description 
                               FROM RecipeSteps 
                               WHERE RecipeId = @RecipeId
                               ORDER BY StepNumber";

            // Ejecutar la consulta principal
            var recipe = await _dbConnection.QuerySingleOrDefaultAsync<RecipeDetailsDto>(recipeQuery, new { Id = id });
            if (recipe == null) return null;

            // Agregar ingredientes
            recipe.Ingredients = (await _dbConnection.QueryAsync<IngredientDto>(ingredientsQuery, new { RecipeId = id })).ToList();

            // Agregar pasos de preparación
            recipe.Steps = (await _dbConnection.QueryAsync<StepDto>(stepsQuery, new { RecipeId = id })).ToList();

            return recipe;
        }

        // Crear una nueva receta con ingredientes y pasos
        public async Task<long> CreateRecipeAsync(CreateRecipeDto recipeDto)
        {
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insertar la receta principal
                var recipeQuery = @"INSERT INTO Recipes (Name, Description, Image, CategoryId, UserId)
                                    OUTPUT INSERTED.Id
                                    VALUES (@Name, @Description, @Image, @CategoryId, @UserId)";

                var recipeId = await _dbConnection.ExecuteScalarAsync<long>(recipeQuery, recipeDto, transaction);

                // Insertar los ingredientes relacionados
                foreach (var ingredient in recipeDto.Ingredients)
                {
                    var ingredientQuery = @"INSERT INTO Ingredients (RecipeId, Name, Quantity) 
                                            VALUES (@RecipeId, @Name, @Quantity)";
                    await _dbConnection.ExecuteAsync(ingredientQuery, new { RecipeId = recipeId, ingredient.Name, ingredient.Quantity }, transaction);
                }

                // Insertar los pasos de preparación relacionados
                foreach (var step in recipeDto.Steps)
                {
                    var stepQuery = @"INSERT INTO RecipeSteps (RecipeId, StepNumber, Description) 
                                      VALUES (@RecipeId, @StepNumber, @Description)";
                    await _dbConnection.ExecuteAsync(stepQuery, new { RecipeId = recipeId, step.StepNumber, step.Description }, transaction);
                }

                // Confirmar la transacción
                transaction.Commit();
                return recipeId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
