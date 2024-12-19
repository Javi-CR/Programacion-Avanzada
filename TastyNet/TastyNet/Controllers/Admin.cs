using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using TastyNet.Models;

namespace TastyNet.Controllers
{
    public class Admin : Controller
    {

        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _conf;

        public Admin(IHttpClientFactory http, IConfiguration conf) 
        {
            _http = http;
            _conf = conf;


        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllRecipes()
        {
            var datos = AllRecipe();
            return View(datos);
        }

        [HttpGet]
        public IActionResult DeleteUsers()
        {
            return View();
        }



        private List<RecipesALL> AllRecipe()
        {
            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Recetas/AllRecipes";

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    var datosContenido = JsonSerializer.Deserialize<List<RecipesALL>>((JsonElement)result.Contenido!);
                    return datosContenido!.ToList();
                }

                return new List<RecipesALL>();
            }
        }


    }
}
