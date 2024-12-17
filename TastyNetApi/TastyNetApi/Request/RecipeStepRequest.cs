using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class RecipeStepRequest
    {
        [Required]
        public int StepNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
    }

}
