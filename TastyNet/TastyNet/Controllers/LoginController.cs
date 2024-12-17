using TastyNet.Models;
using TastyNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        string url = _conf.GetSection("Variables:RutaApi").Value + "User/Register";

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
                string url = _conf.GetSection("Variables:RutaApi").Value + "User/Login";

                model.Password = _comunes.Encrypt(model.Password);
                JsonContent datos = JsonContent.Create(model);

                var response = client.PostAsync(url, datos).Result;
                var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (result != null && result.Codigo == 0)
                {
                   var datosContenido = JsonSerializer.Deserialize<Users>((JsonElement)result.Contenido!);

                    HttpContext.Session.SetString("UserConsecutive", datosContenido!.Id.ToString());
                    HttpContext.Session.SetString("UserName", datosContenido!.Name);
                    HttpContext.Session.SetString("UserEmail", datosContenido!.Email);
                    HttpContext.Session.SetString("UserToken", datosContenido!.Token);
                    HttpContext.Session.SetString("UserRole", datosContenido!.RoleId.ToString());


                return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Mensaje = result!.Mensaje;
                    return View();
                }
            }
        }
        
        [HttpGet]
        public IActionResult RecoverAccount()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult RecoverAccount(Users model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensaje = "El correo electrónico proporcionado no es válido.";
                return View(model); // Se asegura de devolver la vista con el modelo actual
            }

            try
            {
                using (var client = _http.CreateClient())
                {
                    string url = _conf.GetSection("Variables:RutaApi").Value + "User/RecoverAccount";

                    JsonContent datos = JsonContent.Create(new { Email = model.Email }); // Solo se envía la propiedad necesaria
                    var response = client.PostAsync(url, datos).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

                        if (result != null && result.Codigo == 0)
                        {
                            TempData["SuccessMessage"] = "Se ha enviado una contraseña temporal a su correo electrónico.";
                            return RedirectToAction("Login", "Login");
                        }
                        else
                        {
                            ViewBag.Mensaje = result?.Mensaje ?? "Ocurrió un error desconocido en la respuesta de la API.";
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = "Error al comunicarse con el servidor. Inténtelo más tarde.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Ocurrió un error: {ex.Message}";
            }

            return View(model); // Devuelve la vista con el modelo para mostrar el mensaje de error
        }

        
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}