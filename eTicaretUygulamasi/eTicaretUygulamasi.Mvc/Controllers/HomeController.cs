using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eTicaretUygulamasi.Mvc.Controllers
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
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();

        }
        public IActionResult Contact()
        {
            return View();

        }
        public IActionResult Listing()
        {
            var products = _dbContext.Products.Include(p => p.Category).ToList();

            return View(products);

        }
        public IActionResult ProductDetail(int id)
        {
            var product = _dbContext.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.DDName,
                Price = product.Price,
                CategoryName = product.Category?.Name ?? "Kategori bulunamdı"
            };

            return View(viewModel);

        }


    }
}
