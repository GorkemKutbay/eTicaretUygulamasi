using Admin.Models;
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("data-api");
        public HomeController(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<IActionResult> Index()
        {

            var model = new HomeIndexViewModel
            {
                //CategoryCount = _dbContext.Categories.Count(),
                //UserCount = _dbContext.Users.Count(),
                //ProductCount = _dbContext.Products.Count()
                //CategoryCount = await _repo.Count<CategoryEntity>(),
                CategoryCount = await Client.GetFromJsonAsync<int>("api/home/GetCategoryCount"),
                //UserCount = await _repo.Count<UserEntity>(),
                UserCount = await Client.GetFromJsonAsync<int>("api/home/GetUserCount"),
                //ProductCount = await _repo.Count<ProductEntity>()
                ProductCount = await Client.GetFromJsonAsync<int>("api/home/GetProductCount")
            };


            return View(model);
        }
    }
}
