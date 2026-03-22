
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IHttpClientFactory _http;

        private HttpClient Client => _http.CreateClient("data-api");
        public ProductController(IHttpClientFactory http)
        {
            _http = http;
        }

        // ============ SELLER ACTIONS ============

        [Authorize(Policy = "seller")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var categories = await _repo.GetAll<CategoryEntity>();
            var response = await Client.GetAsync("api/product/get");
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.StatusCode.ToString());
            }
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryEntity>>();


            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Policy = "seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newProduct = new ProductEntity
                {
                    DDName = model.Name,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Details = model.Details ?? "",
                    SellerId = GetCurrentUserId()
                };

                var response = await Client.PostAsJsonAsync("api/product/addproduct", newProduct);


                TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("Listing", "Home");
            }
            //ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            ViewBag.Categories = await Client.GetFromJsonAsync<List<CategoryEntity>>("api/product/get");
            return View(model);
        }

        // Product Edit
        [Authorize("seller")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int sellerId = GetCurrentUserId();

            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);

            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");

            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı!";
                return View();
            }


            if (product.SellerId != sellerId)
            {
                ViewBag.ErrorMessage = "Bu ürünü düzenleme yetkiniz yok!";
                return View();
            }

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                DDName = product.DDName,
                Price = product.Price,
                Details = product.Details,
                StockAmount = product.StockAmount,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId
            };

            ViewBag.Categories = await Client.GetFromJsonAsync<List<CategoryEntity>>("api/product/get");
            return View(viewModel);
        }

        [Authorize("seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
        {
            int sellerId = GetCurrentUserId();
            ViewBag.Categories = await Client.GetFromJsonAsync<List<CategoryEntity>>("api/product/get");

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var existing = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{viewModel.Id}");

            if (existing == null)
            {
                ViewBag.ErrorMessage = "Güncellenecek ürün bulunamadı!";
                return View(viewModel);
            }


            if (existing.SellerId != sellerId)
            {
                ViewBag.ErrorMessage = "Bu ürünü düzenleme yetkiniz yok!";
                return View(viewModel);
            }

            existing.DDName = viewModel.DDName;
            existing.Price = viewModel.Price;
            existing.Details = viewModel.Details;
            existing.StockAmount = viewModel.StockAmount;
            existing.CategoryId = viewModel.CategoryId;

            if (existing.StockAmount == 0)
            {
                existing.Enabled = false;
            }
            //await _repo.Update(existing);
            await Client.PutAsJsonAsync("api/product/update", existing);

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi!";
            return View(viewModel);
        }


        [Authorize(Policy = "seller")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int sellerId = GetCurrentUserId();

            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");

            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı!";
                return View();
            }

            if (product.SellerId != sellerId)
            {
                ViewBag.ErrorMessage = "Bu ürünü silme yetkiniz yok!";
                return View();
            }

            var viewModel = new ProductDeleteViewModel
            {
                Id = product.Id,
                DDName = product.DDName,
                Price = product.Price,
                CategoryName = product.Category != null ? product.Category.Name : "Bilinmiyor",
                StockAmount = product.StockAmount
            };

            return View(viewModel);
        }

        [Authorize("seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int sellerId = GetCurrentUserId();

            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");

            if (product == null)
            {
                ViewBag.ErrorMessage = "Silinecek ürün bulunamadı!";
                return View("Delete");
            }


            if (product.SellerId != sellerId)
            {
                ViewBag.ErrorMessage = "Bu ürünü silme yetkiniz yok!";
                return View("Delete");
            }

            //var comment = await _repo.GetWhere<ProductCommentEntity>(c => c.ProductId == id);
            var comment = await Client.GetFromJsonAsync<List<ProductCommentEntity>>($"api/product/getcommentbyid/{id}");
            //await _repo.DeleteRange(comment);
            await Client.DeleteAsync($"api/product/deletecomment/{id}");

            //var image = await _repo.GetWhere<ProductImageEntity>(i => i.ProductId == id);
            //await _repo.DeleteRange(image);
            var image = await Client.GetFromJsonAsync<List<ProductImageEntity>>($"api/product/getimagebyid/{id}");
            await Client.DeleteAsync($"api/product/deleteimage/{id}");

            //var cartItem = await _repo.GetWhere<CartItemEntity>(c => c.ProductId == id);
            //await _repo.DeleteRange(cartItem);
            var cartItem = await Client.GetFromJsonAsync<List<CartItemEntity>>($"api/product/getcartitembyid/{id}");
            await Client.DeleteAsync($"api/product/deletecartitem/{id}");

            //var orderItem = await _repo.GetWhere<OrderItemEntity>(o => o.ProductId == id);
            //await _repo.DeleteRange(orderItem);

            string deletedName = product.DDName;
            //await _repo.Delete(product);
            await Client.DeleteAsync($"api/product/deleteproduct/{product.Id}");
            //await Client.PutAsJsonAsync("api/product/update", existing);


            ViewBag.SuccessMessage = $"'{deletedName}' adlı ürün başarıyla silindi!";
            return View("Delete");
        }

        // ============ BUYER ACTIONS ============

        // Product Comment
        [Authorize("BuyerOrSeller")]
        [HttpGet]
        public async Task<IActionResult> Comment(int id)
        {
            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");


            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı!";
                return View();
            }

            ViewBag.ProductName = product.DDName;

            var viewModel = new ProductCommentViewModel
            {
                ProductId = id,
                StarCount = 3
            };


            return View(viewModel);
        }

        [Authorize("BuyerOrSeller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(ProductCommentViewModel viewModel)
        {
            int userId = GetCurrentUserId();

            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(viewModel.ProductId);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{viewModel.ProductId}");
            if (product != null)
            {
                ViewBag.ProductName = product.DDName;
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var comment = new ProductCommentEntity
            {
                ProductId = viewModel.ProductId,
                UserId = userId,
                Text = viewModel.Text,
                StarCount = viewModel.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            //await _repo.Add(comment);
            await Client.PostAsJsonAsync("api/product/addcomment", comment);

            ViewBag.SuccessMessage = "Yorumunuz başarıyla gönderildi!";

            ModelState.Clear();
            var freshModel = new ProductCommentViewModel
            {
                ProductId = viewModel.ProductId,
                StarCount = 3
            };
            return View(freshModel);
        }
        [Authorize("seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Pasive(int id)
        {
            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");
            if (product == null)
            {
                return NotFound();
            }
            product.Enabled = false;
            //await _repo.Update(product);
            await Client.PutAsJsonAsync("api/product/update", product);
            return RedirectToAction("MyProducts", "Profile");
        }
        [Authorize("seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Active(int id)
        {
            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"api/product/getbyid/{id}");
            if (product == null)
            {
                return NotFound();
            }
            if (product.StockAmount == 0)
            {
                TempData["ErrorMessage"] = "Stoğu 0 olan bir ürünü aktif hale getiremezsiniz. Lütfen önce stok ekleyin.";
                return RedirectToAction("MyProducts", "Profile");
            }
            product.Enabled = true;
            //await _repo.Update(product);
            await Client.PutAsJsonAsync("api/product/update", product);
            return RedirectToAction("MyProducts", "Profile");
        }

    }
}