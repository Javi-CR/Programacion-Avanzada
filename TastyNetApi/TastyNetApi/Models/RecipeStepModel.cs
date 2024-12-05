using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class RecipeStep
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; }

        // Relación con Recipe
        [JsonIgnore] // Evitar ciclos con Recipe -> RecipeSteps -> Recipe
        public Recipe? Recipe { get; set; }
    }
}
