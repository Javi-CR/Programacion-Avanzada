using System.ComponentModel.DataAnnotations.Schema;

namespace TastyNetApi.Models
{


    [NotMapped]
    public class RecipesALL
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedRecipes { get; set; }

    }
}

