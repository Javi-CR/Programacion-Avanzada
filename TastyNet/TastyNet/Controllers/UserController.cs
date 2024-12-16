using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TastyNet.Models;
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
        public IActionResult Profile() => View();

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
        public IActionResult EditProfile(IFormFile Imagen, Users model)
        {
            var ext = string.Empty;
            var folder = string.Empty;
            model.ProfilePicture = string.Empty;

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

            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Profile/UpdateProfile";

                JsonContent datos = JsonContent.Create(model);

                var response = client.PostAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

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

                    return RedirectToAction("ConsultarProductos", "Producto");
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }


        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword() => View();


    }
}
