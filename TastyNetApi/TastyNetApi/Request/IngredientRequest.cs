using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class IngredientRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Quantity { get; set; }
    }

}
