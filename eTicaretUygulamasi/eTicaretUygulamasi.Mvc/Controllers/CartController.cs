using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CartController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult AddProduct(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) 
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı.";
                return View();
            }

            var viewModel = new CartAddProductViewModel
            {
                ProductId = product.Id,
                Quantity = 1,
                ProductName = product.DDName,
                ProductPrice = product.Price,
                CategoryName = product.Category?.Name ?? "Bilinmiyor"
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddProduct ( CartAddProductViewModel model)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == model.ProductId);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı.";
                return View(model);
            }

            model.ProductName = product.DDName;
            model.ProductPrice = product.Price;
            model.CategoryName = product.Category?.Name ?? "Bilinmiyor";
          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = 1;

            var existingCartItem = _dbContext.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == model.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += model.Quantity;
            }
            else
            {
                var newCartItem = new CartItemEntity
                {
                    UserId = userId,
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.CartItems.Add(newCartItem);               

            }
            _dbContext.SaveChanges();

            ViewBag.SuccessMessage = "Ürün sepete eklendi.";
            return RedirectToAction("Edit");


        }


        public IActionResult Edit()
        {
            return View();
        }
    }
}
