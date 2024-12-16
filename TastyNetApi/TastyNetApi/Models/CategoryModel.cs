using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Relación con las recetas
        [JsonIgnore] // Evitar ciclos con Recipe -> Category -> Recipes 
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
