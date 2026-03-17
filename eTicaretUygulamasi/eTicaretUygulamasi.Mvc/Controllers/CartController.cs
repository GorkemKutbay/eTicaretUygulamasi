using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class CartController : Controller
    {
        
        private readonly IDataRepository _repo;

        public CartController( IDataRepository repo)
        {
           
            _repo = repo;
        }

        [HttpGet]
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> AddProduct(int id)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category);

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
        [Authorize("BuyerOrSeller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(CartAddProductViewModel model)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(model.ProductId, p => p.Category);

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

            //var existingCartItem = _dbContext.CartItems
            //    .FirstOrDefault(c => c.UserId == userId && c.ProductId == model.ProductId);

            var cartItems = await _repo.GetWhere<CartItemEntity>(c => c.UserId == userId && c.ProductId == model.ProductId);
            var existingCartItem = cartItems.FirstOrDefault();

            if (existingCartItem != null)
            {

                existingCartItem.Quantity += model.Quantity;


                await _repo.Update(existingCartItem);
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
                await _repo.Add(newCartItem);
            }

            TempData["SuccessMessage"] = "Ürün sepete eklendi.";

            return RedirectToAction("Edit");

        }

        [HttpGet]
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> Edit()
        {
            int userId = 1;

            //var cartItems = _dbContext.CartItems
            //        .Where(c => c.UserId == userId)
            //        .Include(c => c.Product)
            //        .ThenInclude(p => p.Category)
            //        .ToList();

            var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(
                c => c.UserId == userId,       // Filtre: Sadece bu kullanıcının sepeti
                c => c.Product,                // İlişki 1: Ürün bilgilerini getir
                c => c.Product.Category        // İlişki 2: Ürünün kategorisini de getir
            );


            ViewBag.items = cartItems.Count();
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
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(viewModel);
        }

        [HttpPost]
        [Authorize("BuyerOrSeller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CartEditViewModel model)
        {

            int userId = 1;

            if (!ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    //var product = _dbContext.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    var product = (await _repo.GetWhere<ProductEntity>(p => p.Id == item.ProductId)).FirstOrDefault();
                    if (product is not null)
                    {
                        item.ProductName = product.DDName;
                        item.Price = product.Price;
                    }
                }
                return View(model);
            }

            foreach (var item in model.Items)
            {
                //var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.Id == item.Id && c.UserId == userId);
                var cartItem = (await _repo.GetWhere<CartItemEntity>(c => c.Id == item.Id && c.UserId == userId)).FirstOrDefault();
                if (cartItem is not null)
                {
                    cartItem.Quantity = item.Quantity;
                    await _repo.Update(cartItem);
                }

            }
            

            ViewBag.SuccessMessage = "Sepetiniz güncellendi!";

            //var updatedCartItems = _dbContext.CartItems
            //    .Include(c => c.Product)
            //    .ThenInclude(p => p.Category)
            //    .Where(c => c.UserId == userId)
            //    .ToList();
            var updatedCartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(
                c => c.UserId == userId,       // Filtre: Sadece bu kullanıcının sepeti
                c => c.Product,                // İlişki 1: Ürün bilgilerini getir
                c => c.Product.Category        // İlişki 2: Ürünün kategorisini de getir
            );
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
        [Authorize("BuyerOrSeller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int id)
        {
            int userId = 1;

            //var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            var cartItem = (await _repo.GetWhere<CartItemEntity>(c => c.Id == id && c.UserId == userId)).FirstOrDefault();

            if (cartItem != null)
            {
                await _repo.Delete(cartItem);
                TempData["SuccessMessage"] = "Ürün sepetten kaldırıldı!";
            }

            return RedirectToAction("Edit");
        }
    }
}
