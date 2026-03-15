using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
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
            
            _repo=repo;
        }
        public async Task<IActionResult> Index()
        {
            
            //var products = await _dbContext.Products
            //    .Include(p => p.Category)
            //    .Where(p => p.Enabled)
            //    .OrderByDescending(p => p.CreatedAt)
            //    .Take(8) 
            //    .ToListAsync();
            var products = await _repo.GetWhereWithIncludes<ProductEntity>(
                p => p.Enabled,               // Filtre: Sadece aktif ürünler
                p => p.Category               // İlişki: Kategori bilgilerini de getir
            );

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
