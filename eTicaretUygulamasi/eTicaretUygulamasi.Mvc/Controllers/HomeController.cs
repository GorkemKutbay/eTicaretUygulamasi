using System.Diagnostics;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class HomeController : Controller
    {

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
