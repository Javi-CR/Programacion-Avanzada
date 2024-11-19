namespace TastyNet.Models
{
    public class RecipeDetailsViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<IngredientViewModel> Ingredients { get; set; } = new();
        public List<StepViewModel> Steps { get; set; } = new();
    }

    public class IngredientViewModel
    {
        public string Name { get; set; }
        public string? Quantity { get; set; }
    }

    public class StepViewModel
    {
        public int StepNumber { get; set; }
        public string Description { get; set; }
    }
}
