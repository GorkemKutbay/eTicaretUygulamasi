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

        [HttpGet]
        public IActionResult Edit()
        {
            int userId = 1;

            var cartItems = _dbContext.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                    .ToList();
            TempData["itemCount"] = cartItems.Count();
            var viewModel = new CartEditViewModel
            {
                Items = cartItems.Select(c => new CartEditItemViewModel
                { 
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.DDName,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(CartEditViewModel model)
        {
           
            int userId = 1;

            if (!ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    var product = _dbContext.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.DDName;
                        item.Price = product.Price;
                    }
                }
                return View(model);
            }

            foreach (var item in model.Items)
            { 
                var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.Id == item.Id && c.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                }
            
            }
            _dbContext.SaveChanges();

            ViewBag.SuccessMessage = "Sepetiniz güncellendi!";

            var updatedCartItems = _dbContext.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .ToList();

            var updatedModel = new CartEditViewModel
            { 
             Items = updatedCartItems.Select(c => new CartEditItemViewModel
             {
                 Id = c.Id,
                 ProductId = c.ProductId,
                 ProductName = c.Product.DDName,
                 Price = c.Product.Price,
                 Quantity = c.Quantity
             }).ToList()
             };
            
            return View(updatedModel);
        }


        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            int userId = 1;

            var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                _dbContext.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Ürün sepetten kaldırıldı!";
            }

            return RedirectToAction("Edit");
        }        
    }
}
