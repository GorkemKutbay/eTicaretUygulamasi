using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
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
            return View();

        }

        public IActionResult ProductDetail()
        {
            return View();

        }


    }
}
