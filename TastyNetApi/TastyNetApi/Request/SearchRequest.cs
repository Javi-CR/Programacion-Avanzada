using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class SearchRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

    }


}
