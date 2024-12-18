using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class RecipesByCategoryRequest
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long CategoryId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public DateTime CreatedRecipes { get; set; }

        //public string FormattedDate => CreatedRecipes.ToString("dd/MM/yyyy HH:mm:ss");


    }

}
