namespace TastyNet.Models
{
    public class RecipeViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public List<IngredientViewModel> Ingredients { get; set; }
        public List<StepViewModel> Steps { get; set; }
    }

    public class IngredientViewModel
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
    }

    public class StepViewModel
    {
        public int StepNumber { get; set; }
        public string Description { get; set; }
    }
}

