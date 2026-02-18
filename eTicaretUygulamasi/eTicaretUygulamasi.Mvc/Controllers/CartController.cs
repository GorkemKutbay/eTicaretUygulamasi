using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CartController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
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
