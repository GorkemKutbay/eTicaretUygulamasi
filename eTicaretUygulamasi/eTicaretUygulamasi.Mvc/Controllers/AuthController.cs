using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        public IActionResult ForgotPasword()
        {
            return View();
        }
    }
}
