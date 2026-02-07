using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class CartController : Controller
    {
        public IActionResult AddProduct(int id)
        {
            return RedirectToAction("Index","Home");
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
