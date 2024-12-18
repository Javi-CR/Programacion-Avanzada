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
            string baseUrl = _conf.GetSection("Variables:RutaApi").Value;
            string loginUrl = $"{baseUrl}User/Login";
            string tokenCheckUrl = $"{baseUrl}User/CheckTokenExistence";

            model.Password = _comunes.Encrypt(model.Password);
            JsonContent datos = JsonContent.Create(model);

            var response = client.PostAsync(loginUrl, datos).Result;
            var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

            if (result != null && result.Codigo == 0)
            {
                var datosContenido = JsonSerializer.Deserialize<Users>((JsonElement)result.Contenido!);

                var tokenCheckResponse = client.GetAsync($"{tokenCheckUrl}?email={model.Email}").Result;
                var tokenCheckResult = tokenCheckResponse.Content.ReadFromJsonAsync<Respuesta>().Result;

                if (tokenCheckResult != null && tokenCheckResult.Codigo == 0)
                {
                    TempData["Email"] = model.Email;
                    return RedirectToAction("VerifyEmail", "Login");
                }

                HttpContext.Session.SetString("UserConsecutive", datosContenido!.Id.ToString());
                HttpContext.Session.SetString("UserName", datosContenido!.Name);
                HttpContext.Session.SetString("UserEmail", datosContenido!.Email);
                HttpContext.Session.SetString("UserToken", datosContenido!.Token);
                HttpContext.Session.SetString("UserRole", datosContenido!.RoleId.ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result!.Mensaje == "Su cuenta aun no esta activada. Ingrese a su correo y verifique su email")
                {
                    TempData["Email"] = model.Email;
                    return RedirectToAction("VerifyEmail", "Login");
                }
                ViewBag.Mensaje = result!.Mensaje;
                return View();
            }
        }
    }

    [HttpGet]
    public IActionResult VerifyEmail()
    {
        ViewBag.Email = TempData["Email"];
        return View();
    }

    [HttpPost]
    public IActionResult VerifyEmail(string email, string verificationToken)
    {
        using (var client = _http.CreateClient())
        {
            string url = _conf.GetSection("Variables:RutaApi").Value + "User/VerifyEmail";
            JsonContent datos = JsonContent.Create(new { Email = email, VerificationToken = verificationToken });

            var response = client.PostAsync(url, datos).Result;
            var result = response.Content.ReadFromJsonAsync<Respuesta>().Result;

            if (result != null && result.Codigo == 0)
            {
                ViewBag.Mensaje = "Correo verificado exitosamente. Ahora puedes iniciar sesión.";
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
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}