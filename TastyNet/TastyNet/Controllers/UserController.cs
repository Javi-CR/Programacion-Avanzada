using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using TastyNet.Models;
using TastyNet.Servicios;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using TastyNet.Services;

namespace TastyNet.Controllers
{
    [Authorize(Roles = "2")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;
        private readonly IHashService _comunes;

        public UserController(IHttpClientFactory http, IConfiguration conf, IHostEnvironment env, IHashService comunes)
        {
            _http = http;
            _conf = conf;
            _env = env;
            _comunes = comunes;
        }

        [HttpGet]
        [Route("Profile")]
        public IActionResult Profile()
        {
            // Obtener el consecutivo del usuario desde la sesión
            var consecutivo = HttpContext.Session.GetString("UserConsecutive");

            if (string.IsNullOrEmpty(consecutivo))
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                using (var client = _http.CreateClient())
                {
                    // Llamar a la API para obtener las recetas favoritas del usuario
                    string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/GetFavoriteRecipes/" + consecutivo;

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                        if (result != null && result.Codigo == 0)
                        {
                            var favoriteRecipes = JsonSerializer.Deserialize<List<FavoriteRecipeResponse>>(result.Contenido.ToString());
                            ViewBag.FavoriteRecipes = favoriteRecipes;
                            return View();
                        }
                        else
                        {
                            ViewBag.Mensaje = result?.Mensaje ?? "No se encontraron recetas favoritas.";
                          
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = "Error al obtener las recetas favoritas desde el servidor.";
                    
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error en la solicitud: {ex.Message}";
            }

            // Retornar la vista con los datos del usuario
            return View();
        }




        [HttpGet]
        public IActionResult EditProfile()
        
        {
            using (var client = _http.CreateClient())
            {
                var consecutivo = HttpContext.Session.GetString("UserConsecutive");
                string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/CheckUser?Consecutivo=" + consecutivo;

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    var datosContenido = JsonSerializer.Deserialize<Users>((JsonElement)result.Contenido!);
                    return View(datosContenido);
                }

                return View(new Users());
            }
        }

        [HttpPost]
        public IActionResult EditProfile(EditProfile model)
        {
            model.Id = long.Parse(HttpContext.Session.GetString("UserConsecutive")!);

            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/UpdateProfile";

                JsonContent datos = JsonContent.Create(model);

                
                var response = client.PutAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    HttpContext.Session.SetString("UserName", model.Name);
                    HttpContext.Session.SetString("UserEmail", model.Email);

                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }

        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePassword model)
        {
            model.Password = _comunes.Encrypt(model.Password);
            model.ConfirmPassword = _comunes.Encrypt(model.ConfirmPassword);

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Mensaje = "Your password confirmation does not match";
                return View();
            }

            model.Id = long.Parse(HttpContext.Session.GetString("UserConsecutive")!);

            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/ChangePass";

                JsonContent datos = JsonContent.Create(model);

                
                var response = client.PutAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    ViewBag.Mensaje = "Password changed!";
                    return View();
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }


        


    }
}
