using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Recipe
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public long UserId { get; set; }

        public Category? Category { get; set; } 

        [JsonIgnore] 
        public ICollection<Favorite>? Favorites { get; set; }
        public List<Ingredient>? Ingredients { get; set; }
        public List<RecipeStep>? RecipeSteps { get; set; } 
    }
}

