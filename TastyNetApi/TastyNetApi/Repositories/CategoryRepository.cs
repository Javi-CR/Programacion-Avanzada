using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace TastyNetApi.Repositories
{
    public class CategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<CategoryWithSubcategories> GetCategoriesWithSubcategories()
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"EXEC GetCategoriesWithSubcategories";
            var categories = connection.Query<CategoryWithSubcategories, Subcategory, CategoryWithSubcategories>(
                query,
                (category, subcategory) =>
                {
                    category.Subcategories ??= new List<Subcategory>();
                    if (subcategory != null)
                        category.Subcategories.Add(subcategory);

                    return category;
                },
                splitOn: "SubcategoryId");

            return categories.Distinct().ToList();
        }
    }

    public class CategoryWithSubcategories
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Subcategory> Subcategories { get; set; }
    }

    public class Subcategory
    {
        public long SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
    }
}
