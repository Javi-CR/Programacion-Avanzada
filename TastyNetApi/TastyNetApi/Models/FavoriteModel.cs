using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TastyNetApi.Models
{
    public class Favorite
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long RecipeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedFavorites { get; set; } = DateTime.Now; 

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        [JsonIgnore]
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }
}
