using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class RecipeStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long RecipeId { get; set; }

        [Required]
        public int StepNumber { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;


        // Relación con Recipe
        [JsonIgnore] // Evitar ciclos con Recipe -> RecipeSteps -> Recipe
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

    }
}
