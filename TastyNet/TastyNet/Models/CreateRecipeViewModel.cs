namespace TastyNet.Models
{
    public class CreateRecipeViewModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public long CategoryId { get; set; }
        public List<IngredientViewModel> Ingredients { get; set; } = new();
        public List<StepViewModel> Steps { get; set; } = new();
    }
}
