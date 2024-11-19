using Microsoft.AspNetCore.Mvc;
using TastyNet.Models;
using System.Threading.Tasks;
using TastyNet.Models;

public class RecipesController : Controller
{
    private readonly ApiService _apiService;

    public RecipesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? search, long? category)
    {
        var recipes = await _apiService.GetRecipesAsync(search, category);
        return View(recipes);
    }

    public async Task<IActionResult> Details(long id)
    {
        var recipe = await _apiService.GetRecipeDetailsAsync(id);
        return View(recipe);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRecipeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _apiService.CreateRecipeAsync(model);
        return RedirectToAction("Index");
    }
}
