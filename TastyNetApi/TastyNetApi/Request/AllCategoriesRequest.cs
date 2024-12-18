using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class AllCategoriesRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

}
