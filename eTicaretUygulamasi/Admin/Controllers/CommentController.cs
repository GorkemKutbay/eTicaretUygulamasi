using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
        public IActionResult Approve()
        {
            return View();
        }
    }
}
