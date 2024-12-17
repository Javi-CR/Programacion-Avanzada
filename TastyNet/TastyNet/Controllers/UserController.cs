using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TastyNet.Models;
using TastyNet.Servicios;
using static System.Net.WebRequestMethods;

namespace TastyNet.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;

        public UserController(IHttpClientFactory http, IConfiguration conf, IHostEnvironment env)
        {
            _http = http;
            _conf = conf;
            _env = env;
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
        public async Task<IActionResult> EditProfile(IFormFile Imagen, Users model)
        {
            var consecutivo = HttpContext.Session.GetString("UserConsecutive");

            var ext = string.Empty;
            var folder = string.Empty;
            model.ProfilePicture = string.Empty;

            if (!string.IsNullOrEmpty(consecutivo))
            {
                model.Id = long.Parse(consecutivo);
            }
            else
            {
                return View();
            }

            if (Imagen != null)
            {
                ext = Path.GetExtension(Path.GetFileName(Imagen.FileName));
                folder = Path.Combine(_env.ContentRootPath, "wwwroot\\profile");
                model.ProfilePicture = "/products/";

                if (ext.ToLower() != ".png")
                {
                    ViewBag.Mensaje = "La imagen debe ser .png";
                    return View();
                }
            }

            try
            {
                using (var client = _http.CreateClient())
                {
                    string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/UpdateProfile";

                    // Serializar el modelo a JSON
                    JsonContent datos = JsonContent.Create(model);

                    // Hacer la solicitud HTTP asincrónicamente
                    var response = await client.PostAsync(url, datos);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<Respuesta>();

                        if (result != null && result.Codigo == 0)
                        {
                            if (Imagen != null)
                            {
                                string archivo = Path.Combine(folder, result.Mensaje + ext);
                                using (Stream fs = new FileStream(archivo, FileMode.Create))
                                {
                                    Imagen.CopyTo(fs);
                                }
                            }

                            return RedirectToAction("Profile", "User");
                        }
                        else
                        {
                            ViewBag.Mensaje = result?.Mensaje ?? "Error desconocido.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = "Error al conectar con el servidor.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                // Captura excepciones relacionadas con la solicitud HTTP
                ViewBag.Mensaje = $"Error en la solicitud: {ex.Message}";
                return View();
            }
        }


        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword() => View();


        


    }
}
