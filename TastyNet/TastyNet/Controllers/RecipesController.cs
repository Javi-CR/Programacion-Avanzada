using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TastyNet.Controllers
{
    public class RecipesController : Controller
    {

        // Quick & Easy
        public IActionResult ThirtyMinuteMeals() => View();

        public IActionResult FiveIngredientsOrLess() => View();

        public IActionResult OnePotDishes() => View();

        // Comfort Food
        public IActionResult CasserolesAndBakes() => View();

        public IActionResult SoupsAndStews() => View();

        public IActionResult FamilyFavorites() => View();

        // World Flavors
        public IActionResult Italian() => View();

        public IActionResult Asian() => View();

        public IActionResult Mexican() => View();

        // Cakes & Cupcakes
        public IActionResult CakesAndCupcakes() => View();

        public IActionResult CookiesAndBars() => View();

        public IActionResult PiesAndDesserts() => View();

        // Healthy Choices
        public IActionResult LowCarb() => View();

        public IActionResult VeganAndVegetarian() => View();

        public IActionResult GlutenFree() => View();

        // Weekend Meals
        public IActionResult RoastsAndGrills() => View();

        public IActionResult BrunchFavorites() => View();

        public IActionResult PartyPlates() => View();

    }
   
}
