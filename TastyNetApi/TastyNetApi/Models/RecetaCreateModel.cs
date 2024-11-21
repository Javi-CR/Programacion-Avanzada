namespace TastyNetApi.Models
{
    public class RecetaCreateModel
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public List<IngredientModel> Ingredients { get; set; }
        public List<RecipeStepModel> Steps { get; set; }
    }

    public class IngredientModel
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
    }

    public class RecipeStepModel
    {
        public int StepNumber { get; set; }
        public string Description { get; set; }
    }
}
