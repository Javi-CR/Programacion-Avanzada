namespace TastyNet.Models
{
    public class Recipe
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
    }

    public class Ingredient
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }

    public class RecipeStep
    {
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
