

namespace TastyNetApi.Models.Domain
{
    public class Ingredient
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public string Name { get; set; }
        public string? Quantity { get; set; }
    }
}
