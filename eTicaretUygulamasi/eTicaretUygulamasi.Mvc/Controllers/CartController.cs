
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

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("data-api");
        public CartController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> AddProduct(int id)
        {
            var url = $"api/home/getbyid/{id}?includes=Category";

            var response = await Client.GetAsync(url);

            var product = await response.Content.ReadFromJsonAsync<ProductEntity>();

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
            var url = $"api/home/getbyid/{model.ProductId}?includes=Category";

            var response = await Client.GetAsync(url);

            var product = await response.Content.ReadFromJsonAsync<ProductEntity>();

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


            //var cartItems = await _repo.GetWhere<CartItemEntity>(c => c.UserId == userId && c.ProductId == model.ProductId);
            var url2 = $"api/cart/getbyuserandproduct?userId={userId}&productId={model.ProductId}";

            var response1 = await Client.GetAsync(url2);

            var cartItems = await response1.Content.ReadFromJsonAsync<List<CartItemEntity>>();

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
                var url3 = "api/cart/update";

                // Nesneyi JSON formatında gövdeye (body) koyarak PUT isteği atıyoruz
                var response3 = await Client.PutAsJsonAsync(url3, existingCartItem);
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
                var url4 = "api/cart/add";

                // Nesneyi JSON formatında gövdeye (body) koyarak PUT isteği atıyoruz
                var response4 = await Client.PostAsJsonAsync(url4, newCartItem);
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


            //var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(
            //    c => c.UserId == userId,       // Filtre: Sadece bu kullanıcının sepeti
            //    c => c.Product,                // İlişki 1: Ürün bilgilerini getir
            //    c => c.Product.Category        // İlişki 2: Ürünün kategorisini de getir
            //);
            var url5 = $"api/cart/getbyuser/{userId}";

            // API'ye GET isteği atıyoruz
            var response = await Client.GetAsync(url5);
            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemEntity>>();


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
                  //var product = (await _repo.GetWhere<ProductEntity>(p => p.Id == item.ProductId)).FirstOrDefault();
                  var url6 = $"api/product/getbyid/{item.ProductId}";

                  // API'ye GET isteği atıyoruz
                  var response = await Client.GetAsync(url6);
                  var product = await response.Content.ReadFromJsonAsync<ProductEntity>();

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
              var url7 = $"api/cart/getbyuserandproduct?userId={userId}&productId={item.ProductId}";
              var response = await Client.GetAsync(url7);
              var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemEntity>>();
              var cartItem = cartItems.FirstOrDefault();

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
                  
                  var updateResponse = await Client.PutAsJsonAsync("api/cart/update", cartItem);
                  if (!updateResponse.IsSuccessStatusCode)
                  {
                      TempData["ErrorMessage"] = "Güncelleme sırasında teknik bir hata oluştu.";
                      return RedirectToAction("Edit");
                  }
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
            //var cartItem = (await _repo.GetWhere<CartItemEntity>(c => c.Id == id && c.UserId == userId)).FirstOrDefault();
            var response = await Client.GetAsync($"api/home/getbyid/{id}");
            var cartItem = await response.Content.ReadFromJsonAsync<CartItemEntity>();

            if (cartItem != null)
            {
                //await _repo.Delete(cartItem);
                

                // Nesneyi JSON formatında gövdeye (body) koyarak PUT isteği atıyoruz
                var response4 = await Client.PutAsJsonAsync("api/cart/delete", cartItem);
                TempData["SuccessMessage"] = "Ürün sepetten kaldırıldı!";
            }

            return RedirectToAction("Edit");
        }
    }
}
