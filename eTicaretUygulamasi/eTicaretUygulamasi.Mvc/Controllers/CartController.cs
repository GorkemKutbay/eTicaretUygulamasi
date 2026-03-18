using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class CartController : BaseController
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
                TempData["ErrorMessage"] = "Aradığınız ürün sistemde bulunamadı.";
                return RedirectToAction("Index", "Home");
            }
            if (!product.Enabled || product.StockAmount == 0)
            {                
                TempData["ErrorMessage"] = $"Üzgünüz, '{product.DDName}' adlı ürün şu an satışta değil veya tükendi.";
                return RedirectToAction("Index", "Home");
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

            if (product == null || !product.Enabled || product.StockAmount == 0)
            {
                TempData["ErrorMessage"] = "Bu ürün artık mevcut değil.";
                return RedirectToAction("Index", "Home");
            }
            if (model.Quantity > product.StockAmount)
            {
                ViewBag.ErrorMessage = $"Üzgünüz, bu üründen en fazla {product.StockAmount} adet ekleyebilirsiniz.";
                return View(model);
            }
            if (model.Quantity <= 0)
            {
                ViewBag.ErrorMessage = "Bu üründen en az 1 adet eklemelisiniz.";
                return View(model);
            }

            model.ProductName = product.DDName;
            model.ProductPrice = product.Price;
            model.CategoryName = product.Category?.Name ?? "Bilinmiyor";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = GetCurrentUserId();
           

            var cartItems = await _repo.GetWhere<CartItemEntity>(c => c.UserId == userId && c.ProductId == model.ProductId);
            var existingCartItem = cartItems.FirstOrDefault();

            if (existingCartItem != null)
            {
                // Sepetteki mevcut miktar + yeni eklenmek istenen miktar stoktan fazla mı?
                if (existingCartItem.Quantity + model.Quantity > product.StockAmount)
                {
                    TempData["ErrorMessage"] = $"Sepetinizdeki miktar ({existingCartItem.Quantity}) ile yeni eklenen toplamı stoğu ({product.StockAmount}) aşıyor!";
                    return RedirectToAction("Edit");
                }

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
            // int userId = 1;
            int userId = GetCurrentUserId();


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
            // int userId = 1;

            int userId = GetCurrentUserId();
            if (!ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
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
                var cartItem = (await _repo.GetWhereWithIncludes<CartItemEntity>(
                    c => c.Id == item.Id && c.UserId == userId,
                    c => c.Product)).FirstOrDefault();

                if (cartItem != null)
                {                   
                    if (item.Quantity > cartItem.Product.StockAmount)
                    {
                        TempData["ErrorMessage"] = $"'{cartItem.Product.DDName}' ürünü için yeterli stok yok! (Mevcut Stok: {cartItem.Product.StockAmount})";
                       
                        return RedirectToAction("Edit");
                    }
                  
                    if (item.Quantity <= 0)
                    {
                        TempData["ErrorMessage"] = "Ürün miktarı en az 1 olmalıdır.";
                        return RedirectToAction("Edit");
                    }
                   
                    cartItem.Quantity = item.Quantity;
                    await _repo.Update(cartItem);
                }
            }
          
            TempData["SuccessMessage"] = "Sepetiniz başarıyla güncellendi!";           
            return RedirectToAction("Edit");
        }


        [HttpPost]
        [Authorize("BuyerOrSeller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int id)
        {
            //int userId = 1;
            int userId = GetCurrentUserId();

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
