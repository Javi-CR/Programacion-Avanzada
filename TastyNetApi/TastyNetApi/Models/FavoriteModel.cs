using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Favorite
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }

        [JsonIgnore]
        public Recipe Recipe { get; set; }
    }
}
