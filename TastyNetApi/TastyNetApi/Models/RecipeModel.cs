using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Image { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedRecipes { get; set; } = DateTime.Now;


        // Uso de la cualidad virutal para remplazar variables o objetos sin problemas 

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [JsonIgnore] // Evitar ciclos con Favorites -> Recipe
        public virtual ICollection<Favorite>? Favorites { get; set; }

        public virtual ICollection<Ingredient>? Ingredients { get; set; }

        public virtual ICollection<RecipeStep>? RecipeSteps { get; set; }
    }
}
