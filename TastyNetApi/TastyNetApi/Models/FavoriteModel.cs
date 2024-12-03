namespace TastyNetApi.Models
{
    public class Favorite
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
