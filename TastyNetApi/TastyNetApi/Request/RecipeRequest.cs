using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class RecipeRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public long UserId { get; set; }

        public ICollection<IngredientRequest> Ingredients { get; set; }
        public ICollection<RecipeStepRequest> RecipeSteps { get; set; }
    }

}
