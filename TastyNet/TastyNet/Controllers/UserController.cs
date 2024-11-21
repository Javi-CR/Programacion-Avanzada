using Microsoft.AspNetCore.Mvc;

namespace TastyNet.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        [Route("Profile")]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        [Route("EditProfile")]
        public IActionResult EditProfile()
        {
            return View();
        }

        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }


    }
}
