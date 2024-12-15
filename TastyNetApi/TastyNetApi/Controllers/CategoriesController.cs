using Microsoft.AspNetCore.Mvc;
using TastyNetApi.Repositories;

namespace TastyNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoriesController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("GetCategoriesWithSubcategories")]
        public IActionResult GetCategoriesWithSubcategories()
        {
            var categories = _categoryRepository.GetCategoriesWithSubcategories();
            return Ok(categories);
        }
    }
}
