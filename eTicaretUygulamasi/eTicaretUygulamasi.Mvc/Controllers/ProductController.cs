using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        
        private readonly IDataRepository _repo;

        public ProductController(IDataRepository repo)
        {
           
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _repo.GetAll<CategoryEntity>();
            ViewBag.Categories = categories;
            return View();
        }

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
                    SellerId = 1
                };

                await _repo.Add(newProduct);

                

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("Listing", "Home");
            }
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            return View(model);
        }

        // Product Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı!";
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
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
        {
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var existing =  await _repo.GetByIdWithIncludes<ProductEntity>(viewModel.Id);

            if (existing == null)
            {
                ViewBag.ErrorMessage = "Güncellenecek ürün bulunamadı!";
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

        // Product Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _dbContext.Products
            //    .Include(p => p.Category)
            //    .FirstOrDefault(p => p.Id == id);
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Ürün bulunamadı!";
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

      
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =  await  _repo.GetByIdWithIncludes<ProductEntity>(id);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Silinecek ürün bulunamadı!";
                return View("Delete");
            }

            //var comments = _dbContext.ProductComments.Where(c => c.ProductId == id).ToList();
            //_dbContext.ProductComments.RemoveRange(comments);
            var comment = await _repo.GetWhere<ProductCommentEntity>(c => c.ProductId == id);
            await _repo.DeleteRange(comment);

            //var images = _dbContext.ProductImages.Where(i => i.ProductId == id).ToList();
            //_dbContext.ProductImages.RemoveRange(images);
            var image = await _repo.GetWhere<ProductImageEntity>(i => i.ProductId == id);
            await _repo.DeleteRange(image);

            //var cartItems = _dbContext.CartItems.Where(c => c.ProductId == id).ToList();
            //_dbContext.CartItems.RemoveRange(cartItems);
            var cartItem = await _repo.GetWhere<CartItemEntity>(c => c.ProductId == id);
            await _repo.DeleteRange(cartItem);

            //var orderItems = _dbContext.OrderItemS.Where(o => o.ProductId == id).ToList();
            //_dbContext.OrderItemS.RemoveRange(orderItems);
            var orderItem = await _repo.GetWhere<OrderItemEntity>(o => o.ProductId == id);
            await _repo.DeleteRange(orderItem);

           
            

            string deletedName = product.DDName;
            await _repo.Delete(product);

            ViewBag.SuccessMessage = $"'{deletedName}' adlı ürün başarıyla silindi!";
            return View("Delete");
        }

        // Product Comment
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

        
        [HttpPost]
        public async Task<IActionResult> Comment(ProductCommentViewModel viewModel)
        {
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
                UserId = 1,
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
