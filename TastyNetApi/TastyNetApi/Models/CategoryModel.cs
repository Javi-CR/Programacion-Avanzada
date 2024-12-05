using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // Relación con las recetas
        [JsonIgnore] // Evitar ciclos con Recipe -> Category -> Recipes
        public ICollection<Recipe> Recipes { get; set; }
    }
}
