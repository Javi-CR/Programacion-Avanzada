namespace TastyNetApi.Models
{
    public class Recipe
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public long UserId { get; set; } // Relación con User

        // Relación con Category
        public Category Category { get; set; }

        // Relación con Ingredients
        public ICollection<Ingredient> Ingredients { get; set; }

        // Relación con RecipeSteps
        public ICollection<RecipeStep> RecipeSteps { get; set; }

        // Relación con Favorites
        public ICollection<Favorite> Favorites { get; set; }
    }
}
