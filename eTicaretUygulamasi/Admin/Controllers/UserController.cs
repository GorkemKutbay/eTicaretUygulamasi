using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
       //Listele
       public IActionResult List()
       {
           return View();
       }

       public IActionResult Approve (int id)
       {
           return View();
       }
    }
}
