using Anteproyecto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Anteproyecto.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("AccountRecovery")]
        public IActionResult AccountRecovery()
        [Route("RecoverAccount")]
        public IActionResult RecoverAccount()
        {
            return View();
        }

    }

}