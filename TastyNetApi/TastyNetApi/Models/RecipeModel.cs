namespace TastyNetApi.Models
{
    public class Recipe
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }

        // Relaciones
        public Category Category { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
        public ICollection<RecipeStep> RecipeSteps { get; set; }
    }
}
