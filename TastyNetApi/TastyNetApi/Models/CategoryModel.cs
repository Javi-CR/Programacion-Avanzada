namespace TastyNetApi.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // Relación con las recetas
        public ICollection<Recipe> Recipes { get; set; }
    }
}
