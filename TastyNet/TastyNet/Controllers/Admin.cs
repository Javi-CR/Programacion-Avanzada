using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using TastyNet.Models;
using System.Net.Http.Headers;

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
        public async Task<IActionResult> DeleteRecipes()
        {
            var recipes = new List<RecipesALL>();

            try
            {
                var client = _http.CreateClient();
                var apiUrl = $"{_conf["Variables:RutaApi"]}Recetas/AllRecipes";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonSerializer.Deserialize<Respuesta>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (respuesta != null && respuesta.Codigo == 1)
                    {
                        recipes = JsonSerializer.Deserialize<List<RecipesALL>>(respuesta.Contenido.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "Error al obtener las recetas. Por favor, intente de nuevo.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return View(recipes);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteUsers()
        {
            var users = new List<Users>();

            try
            {
                var client = _http.CreateClient();
                var apiUrl = $"{_conf["Variables:RutaApi"]}User/ConsultarUsuarios";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonSerializer.Deserialize<Respuesta>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (respuesta != null && respuesta.Codigo == 1)
                    {
                        users = JsonSerializer.Deserialize<List<Users>>(respuesta.Contenido.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "Error al obtener usuarios. Por favor, intente de nuevo.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return View(users);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var client = _http.CreateClient();
                var apiUrl = $"{_conf["Variables:RutaApi"]}User/DeleteAccount?userId={id}";

                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Usuario eliminado exitosamente.";
                    return RedirectToAction("DeleteUsers", "Admin");
                }
                else
                {
                    ViewBag.Message = "Error al eliminar el usuario. Por favor, intente nuevamente.";
                    return RedirectToAction("DeleteUsers", "Admin");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



    }
}
