using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class RecipeDetailsRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Ingredients { get; set; }

        public IEnumerable<string> Steps { get; set; }


    }


}

