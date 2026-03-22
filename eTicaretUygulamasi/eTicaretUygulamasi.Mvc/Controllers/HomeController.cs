using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient Client => _clientFactory.CreateClient("data-api");
        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            
            var response = await Client.GetAsync("api/home/get");
            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunu ele al
                throw new InvalidOperationException(response.StatusCode.ToString());
            }
            var allProducts = await response.Content.ReadFromJsonAsync<List<ProductEntity>>() ?? throw new InvalidOperationException("No products found");
            var products = allProducts.Where(x => x.Enabled && x.StockAmount > 0).ToList() ;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p => p.DDName.ToLower().Contains(searchTerm) ||
                                             (p.Details != null && p.Details.ToLower().Contains(searchTerm)))
                                   .ToList();
            }
            ViewBag.Categories = await Client.GetFromJsonAsync<List<CategoryEntity>>("api/category/get");
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchTerm = searchTerm;







            return View(products);
        }


        public IActionResult AboutUs()
        {
            return View();

        }
        public IActionResult Contact()
        {
            return View();

        }
        public async Task<IActionResult> Listing()
        {
            
            //var products = await _repo.GetWhereWithIncludes<ProductEntity>(
            //    p => true,                    // Filtre: Tüm ürünler
            //    p => p.Category               // İlişki: Kategori bilgilerini de getir
            //);
            var products = await Client.GetFromJsonAsync<List<ProductEntity>>("api/home/GetProductsForListing");

            return View(products);

        }
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> ProductDetail(int id)
        {
            //var product = _dbContext.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefault(p => p.Id == id);

            var url = $"api/home/getbyid/{id}?includes=Category&includes=Seller";

            var response = await Client.GetAsync(url);

            var product = await response.Content.ReadFromJsonAsync<ProductEntity>();




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
