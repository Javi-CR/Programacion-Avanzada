using Microsoft.AspNetCore.Mvc;

namespace TastyNet.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult MostrarError()
        {
            return View("Error");
        }
    }
}
