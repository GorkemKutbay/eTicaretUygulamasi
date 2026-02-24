using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{code}")]
        public IActionResult Index(int code)
        {
            if (code == 404)
            {
                return View("404");
            }

            return View("Error");
        }
    }
}
