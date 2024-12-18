using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class SubCategoriesByCategoryRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

}
