using Microsoft.AspNetCore.Mvc;

namespace TastyNet.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        [Route("Profile")]
        public IActionResult Profile() => View();

        [HttpGet]
        [Route("EditProfile")]
        public IActionResult EditProfile() => View();

        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword() => View();


    }
}
