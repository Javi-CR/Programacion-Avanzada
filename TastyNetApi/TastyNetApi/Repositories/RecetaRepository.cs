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

        public bool AgregarAFavoritos(long userId, long recipeId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                // Llama al procedimiento almacenado para agregar a favoritos
                connection.Execute(
                    "EXEC AgregarAFavoritos @UserId, @RecipeId",
                    new { UserId = userId, RecipeId = recipeId }
                );
                return true;
            }
            catch
            {
                return false; // Maneja el error si ocurre
            }
        }

        public long CrearRecetaYObtenerId(RecetaCreateModel receta)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Insertar receta y obtener el ID generado
                var recipeId = connection.ExecuteScalar<long>(
                    "EXEC InsertRecipe @Name, @CategoryId, @UserId",
                    new { receta.Name, receta.CategoryId, UserId = 1 }, // Usuario fijo por ahora
                    transaction
                );

                // Insertar ingredientes
                foreach (var ingredient in receta.Ingredients)
                {
                    connection.Execute(
                        "EXEC InsertIngredient @RecipeId, @Name, @Quantity",
                        new { RecipeId = recipeId, ingredient.Name, ingredient.Quantity },
                        transaction
                    );
                }

                // Insertar pasos
                foreach (var step in receta.Steps)
                {
                    connection.Execute(
                        "EXEC InsertRecipeStep @RecipeId, @StepNumber, @Description",
                        new { RecipeId = recipeId, step.StepNumber, step.Description },
                        transaction
                    );
                }

                transaction.Commit();
                return recipeId;
            }
            catch
            {
                transaction.Rollback();
                return 0; // Indica un error
            }
        }


        public List<RecipeViewModel> ObtenerRecetasFavoritas(long userId)
        {
            using var connection = new SqlConnection(_connectionString);

            var recipeDictionary = new Dictionary<long, RecipeViewModel>();

            var recipes = connection.Query<RecipeViewModel, IngredientViewModel, StepViewModel, RecipeViewModel>(
                "GetFavoriteRecipes", // Procedimiento almacenado
                (recipe, ingredient, step) =>
                {
                    if (!recipeDictionary.TryGetValue(recipe.Id, out var recipeEntry))
                    {
                        recipeEntry = recipe;
                        recipeEntry.Ingredients = new List<IngredientViewModel>();
                        recipeEntry.Steps = new List<StepViewModel>();
                        recipeDictionary.Add(recipe.Id, recipeEntry);
                    }

                    if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                    {
                        recipeEntry.Ingredients.Add(ingredient);
                    }

                    if (step != null && !string.IsNullOrWhiteSpace(step.Description))
                    {
                        recipeEntry.Steps.Add(step);
                    }

                    return recipeEntry;
                },
                new { UserId = userId },
                splitOn: "IngredientName,StepNumber",
                commandType: CommandType.StoredProcedure
            );

            return recipes.Distinct().ToList();
        }








    }
}
