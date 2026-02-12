using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            return View();
        }

        public IActionResult MyProducts()
        {
            return View();
        }

    }
}
