using Admin.Models;
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class HomeController : Controller
    {


        private readonly IDataRepository _repo;

        public HomeController(IDataRepository repo)
        {

            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {

            var model = new HomeIndexViewModel
            {
                //CategoryCount = _dbContext.Categories.Count(),
                //UserCount = _dbContext.Users.Count(),
                //ProductCount = _dbContext.Products.Count()
                CategoryCount = await _repo.Count<CategoryEntity>(),
                UserCount = await _repo.Count<UserEntity>(),
                ProductCount = await _repo.Count<ProductEntity>()
            };


            return View(model);
        }
    }
}
