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

                
                connection.Execute(
                    "EXEC InsertFavorite @UserId, @RecipeId",
                    new { UserId = 1, RecipeId = recipeId }, 
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }







    }
}
