using System.Data;
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

            var recipeDictionary = new Dictionary<long, RecipeViewModel>();

            var recipes = connection.Query<RecipeViewModel, IngredientViewModel, StepViewModel, RecipeViewModel>(
                "GetFavoriteRecipes", // Llamada al procedimiento almacenado
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
                new { UserId = userId }, // Parámetro del procedimiento almacenado
                splitOn: "IngredientName,StepNumber",
                commandType: CommandType.StoredProcedure // Especifica que es un procedimiento almacenado
            );

            return recipes.Distinct().ToList();
        }

    }
}
