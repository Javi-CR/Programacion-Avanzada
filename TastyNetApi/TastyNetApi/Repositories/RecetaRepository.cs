using System.Data.SqlClient;
using Dapper;
using TastyNetApi.Models;

namespace TastyNetApi.Repositories
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly string _connectionString;

        public RecetaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CrearReceta(RecetaCreateModel receta)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var recipeId = connection.ExecuteScalar<int>(
                    "EXEC InsertRecipe @Name, @CategoryId, @UserId",
                    new { receta.Name, receta.CategoryId, UserId = 1 },
                    transaction);

                foreach (var ingredient in receta.Ingredients)
                {
                    connection.Execute(
                        "EXEC InsertIngredient @RecipeId, @Name, @Quantity",
                        new { RecipeId = recipeId, ingredient.Name, ingredient.Quantity },
                        transaction);
                }

                foreach (var step in receta.Steps)
                {
                    connection.Execute(
                        "EXEC InsertRecipeStep @RecipeId, @StepNumber, @Description",
                        new { RecipeId = recipeId, step.StepNumber, step.Description },
                        transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public List<RecipeViewModel> ObtenerRecetasFavoritas(long userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT 
                    r.Id,
                    r.Name,
                    c.Name AS Category,
                    i.Name AS IngredientName,
                    i.Quantity,
                    s.StepNumber,
                    s.Description AS StepDescription
                FROM Favorites f
                INNER JOIN Recipes r ON f.RecipeId = r.Id
                INNER JOIN Categories c ON r.CategoryId = c.Id
                LEFT JOIN Ingredients i ON r.Id = i.RecipeId
                LEFT JOIN RecipeSteps s ON r.Id = s.RecipeId
                WHERE f.UserId = @UserId
                ORDER BY r.Id, s.StepNumber";

            var recipeDictionary = new Dictionary<long, RecipeViewModel>();

            var recipes = connection.Query<RecipeViewModel, IngredientViewModel, StepViewModel, RecipeViewModel>(
                query,
                (recipe, ingredient, step) =>
                {
                    if (!recipeDictionary.TryGetValue(recipe.Id, out var recipeEntry))
                    {
                        recipeEntry = recipe;
                        recipeEntry.Ingredients = new List<IngredientViewModel>();
                        recipeEntry.Steps = new List<StepViewModel>();
                        recipeDictionary.Add(recipe.Id, recipeEntry);
                    }

                    if (ingredient != null)
                    {
                        recipeEntry.Ingredients.Add(ingredient);
                    }

                    if (step != null)
                    {
                        recipeEntry.Steps.Add(step);
                    }

                    return recipeEntry;
                },
                new { UserId = userId },
                splitOn: "IngredientName,StepNumber"
            );

            return recipes.Distinct().ToList();
        }
    }
}
