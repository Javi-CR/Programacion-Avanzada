using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Ingredient
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }

        // Relación con Recipe
        [JsonIgnore] // Evitar ciclos con Recipe -> Ingredients -> Recipe
        public Recipe? Recipe { get; set; }
    }
}
