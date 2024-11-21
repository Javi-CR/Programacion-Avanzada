using TastyNet.Models;
using TastyNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TastyNet.Services;

namespace TastyNet.Controllers;

public class LoginController : Controller
{
private readonly IHttpClientFactory _http;
private readonly IConfiguration _conf;
private readonly IHashService _comunes;

public LoginController(IHttpClientFactory http, IConfiguration conf, IHashService comunes)
{
    _http = http;
    _conf = conf;
    _comunes = comunes;
}

[HttpGet]
public IActionResult Register()
{
    return View();
}

[HttpPost]
public IActionResult Register(Users model)
{
    using (var client = _http.CreateClient())
    {
        string url = _conf.GetSection("Variables:RutaApi").Value + "Login/Register";

        model.Password = _comunes.Encrypt(model.Password);
        JsonContent datos = JsonContent.Create(model);

        var response = client.PostAsync(url, datos).Result;
        var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

        if (result != null && result.Codigo == 0)
        {
            return RedirectToAction("Login", "Login");
        }
        else
        {
            ViewBag.Mensaje = result!.Mensaje;
            return View();
        }
    }
}

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Users model)
        {
            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Login/Login";

                model.Password = _comunes.Encrypt(model.Password);
                JsonContent datos = JsonContent.Create(model);

                var response = client.PostAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    var datosContenido = JsonSerializer.Deserialize<Users>((JsonElement)result.Contenido!);

                   // HttpContext.Session.SetString("ConsecutivoUsuario", datosContenido!.Consecutivo.ToString());
                   // HttpContext.Session.SetString("NombreUsuario", datosContenido!.Nombre);
                   // HttpContext.Session.SetString("TokenUsuario", datosContenido!.Token);
                   // HttpContext.Session.SetString("RolUsuario", datosContenido!.ConsecutivoRol.ToString());

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }
}