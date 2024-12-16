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

        public UserController(IHttpClientFactory http, IConfiguration conf)
        {
            _http = http;
            _conf = conf;
      
        }

        [HttpGet]
        [Route("Profile")]
        public IActionResult Profile() => View();

        [HttpGet]
        [Route("EditProfile")]
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
        public IActionResult EditProfile(Users model)
        {
            return View();
        }


        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword() => View();


    }
}
