using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Ingredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long RecipeId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Quantity { get; set; } = string.Empty;

        // Relación con Recipe
        [JsonIgnore] // Evitar ciclos con Recipe -> Ingredients -> Recipe
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }
}
