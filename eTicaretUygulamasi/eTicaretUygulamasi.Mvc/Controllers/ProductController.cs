using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IDataRepository _repo;

        public ProductController(IDataRepository repo)
        {
            _repo = repo;
        }

        // ============ SELLER ACTIONS ============

        [Authorize(Roles = "seller")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _repo.GetAll<CategoryEntity>();
            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Roles = "seller")] 
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

                await _repo.Add(newProduct);

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("Listing", "Home");
            }
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            return View(model);
        }

        // Product Edit
        [Authorize(Roles = "seller")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int sellerId = GetCurrentUserId(); 

            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);

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

            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            return View(viewModel);
        }

        [Authorize(Roles = "seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
        {
            int sellerId = GetCurrentUserId();
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var existing = await _repo.GetByIdWithIncludes<ProductEntity>(viewModel.Id);

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

            await _repo.Update(existing);

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi!";
            return View(viewModel);
        }

       
        [Authorize(Roles = "seller")] 
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int sellerId = GetCurrentUserId();

            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category);

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

        [Authorize(Roles = "seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int sellerId = GetCurrentUserId();

            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);

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

            var comment = await _repo.GetWhere<ProductCommentEntity>(c => c.ProductId == id);
            await _repo.DeleteRange(comment);

            var image = await _repo.GetWhere<ProductImageEntity>(i => i.ProductId == id);
            await _repo.DeleteRange(image);

            var cartItem = await _repo.GetWhere<CartItemEntity>(c => c.ProductId == id);
            await _repo.DeleteRange(cartItem);

            var orderItem = await _repo.GetWhere<OrderItemEntity>(o => o.ProductId == id);
            await _repo.DeleteRange(orderItem);

            string deletedName = product.DDName;
            await _repo.Delete(product);

            ViewBag.SuccessMessage = $"'{deletedName}' adlı ürün başarıyla silindi!";
            return View("Delete");
        }

        // ============ BUYER ACTIONS ============

        // Product Comment
       [Authorize(Roles = "BuyerOrSeller")]
        [HttpGet]
        public async Task<IActionResult> Comment(int id)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);

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

        [Authorize(Roles = "BuyerOrSeller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(ProductCommentViewModel viewModel)
        {
            int userId = GetCurrentUserId();

            var product = await _repo.GetByIdWithIncludes<ProductEntity>(viewModel.ProductId);
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

            await _repo.Add(comment);

            ViewBag.SuccessMessage = "Yorumunuz başarıyla gönderildi!";

            ModelState.Clear();
            var freshModel = new ProductCommentViewModel
            {
                ProductId = viewModel.ProductId,
                StarCount = 3
            };
            return View(freshModel);
        }
    }
}