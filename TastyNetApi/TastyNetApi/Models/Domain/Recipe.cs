using System;
using System.Collections.Generic;

namespace TastyNetApi.Models.Domain
{
    public class Recipe
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public long CategoryId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedRecipes { get; set; }
    }
}
