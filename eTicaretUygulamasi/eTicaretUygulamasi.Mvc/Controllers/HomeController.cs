using App.Data;
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

        private readonly IDataRepository _repo;

        public HomeController(IDataRepository repo)
        {

            _repo = repo;
        }
        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            var products = await _repo.GetWhere<ProductEntity>(x => x.Enabled == true && x.StockAmount > 0);
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p => p.DDName.ToLower().Contains(searchTerm) ||
                                             (p.Details != null && p.Details.ToLower().Contains(searchTerm)))
                                   .ToList();
            }
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchTerm = searchTerm;






            //var products = await _repo.GetWhereWithIncludes<ProductEntity>(
            //    p => p.Enabled,               // Filtre: Sadece aktif ürünler
            //    p => p.Category               // İlişki: Kategori bilgilerini de getir
            //);

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
            //var products = _dbContext.Products.Include(p => p.Category).ToList();
            var products = await _repo.GetWhereWithIncludes<ProductEntity>(
                p => true,                    // Filtre: Tüm ürünler
                p => p.Category               // İlişki: Kategori bilgilerini de getir
            );
            return View(products);

        }
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> ProductDetail(int id)
        {
            //var product = _dbContext.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefault(p => p.Id == id);

            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category, p => p.Seller);
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
