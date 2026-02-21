using Admin.Models;
using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly AppDbContext _dbContext;

        
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
           
            var model = new HomeIndexViewModel
            {
                CategoryCount = _dbContext.Categories.Count(),
                UserCount = _dbContext.Users.Count(),
                ProductCount = _dbContext.Products.Count()
            };

           
            return View(model);
        }
    }
}
