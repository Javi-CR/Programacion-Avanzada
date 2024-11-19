using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Models.DTOs
{
    public class CreateRecipeDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public long UserId { get; set; }

        public List<IngredientDto> Ingredients { get; set; } = new();
        public List<StepDto> Steps { get; set; } = new();
    }


}
