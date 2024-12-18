using System.ComponentModel.DataAnnotations.Schema;

namespace TastyNetApi.Request
{
    public class FavoriteRecipeRequest
    {

        public long RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string CategoryName { get; set; }
        public string IngredientName { get; set; }
        public string IngredientQuantity { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; }

    }
}
