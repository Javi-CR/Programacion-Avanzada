namespace TastyNetApi.Models.DTOs
{
    public class RecipeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
