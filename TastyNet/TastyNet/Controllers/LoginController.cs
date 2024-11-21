using TastyNet.Models;
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

public LoginController(IHttpClientFactory http, IConfiguration conf)
{
    _http = http;
    _conf = conf;
   // _comunes = comunes;
}

[HttpGet]
public IActionResult CrearCuenta()
{
    return View();
}

[HttpPost]
public IActionResult CrearCuenta(Users model)
{
    using (var client = _http.CreateClient())
    {
        string url = _conf.GetSection("Variables:RutaApi").Value + "Login/CrearCuenta";

        model.Contrasenna = _comunes.Encrypt(model.Contrasenna);
        JsonContent datos = JsonContent.Create(model);

        var response = client.PostAsync(url, datos).Result;
        var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

        if (result != null && result.Codigo == 0)
        {
            return RedirectToAction("IniciarSesion", "Login");
        }
        else
        {
            ViewBag.Mensaje = result!.Mensaje;
            return View();
        }
    }
}

[HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(Users model)
        {
            using (var client = _http.CreateClient())
            {
                string url = _conf.GetSection("Variables:RutaApi").Value + "Login/IniciarSesion";

                model.Contrasenna = _comunes.Encrypt(model.Contrasenna);
                JsonContent datos = JsonContent.Create(model);

                var response = client.PostAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                    var datosContenido = JsonSerializer.Deserialize<Users>((JsonElement)result.Contenido!);

                    HttpContext.Session.SetString("ConsecutivoUsuario", datosContenido!.Consecutivo.ToString());
                    HttpContext.Session.SetString("NombreUsuario", datosContenido!.Nombre);
                    HttpContext.Session.SetString("TokenUsuario", datosContenido!.Token);
                    HttpContext.Session.SetString("RolUsuario", datosContenido!.ConsecutivoRol.ToString());

                    return RedirectToAction("Inicio", "Home");
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }
}