using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create()
        {
            return View();
        }

        // Product Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

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

            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ProductEditViewModel viewModel)
        {
            ViewBag.Categories = _dbContext.Categories.ToList();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var existing = _dbContext.Products.FirstOrDefault(p => p.Id == viewModel.Id);

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

            _dbContext.SaveChanges();

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi!";
            return View(viewModel);
        }

        // Product Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

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
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Silinecek ürün bulunamadı!";
                return View("Delete");
            }

            var comments = _dbContext.ProductComments.Where(c => c.ProductId == id).ToList();
            _dbContext.ProductComments.RemoveRange(comments);

            var images = _dbContext.ProductImages.Where(i => i.ProductId == id).ToList();
            _dbContext.ProductImages.RemoveRange(images);

            var cartItems = _dbContext.CartItems.Where(c => c.ProductId == id).ToList();
            _dbContext.CartItems.RemoveRange(cartItems);

            var orderItems = _dbContext.OrderItemS.Where(o => o.ProductId == id).ToList();
            _dbContext.OrderItemS.RemoveRange(orderItems);
           
            string deletedName = product.DDName;
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            ViewBag.SuccessMessage = $"'{deletedName}' adlı ürün başarıyla silindi!";
            return View("Delete");
        }

        // Product Comment
        [HttpGet]
        public IActionResult Comment(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

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
        public IActionResult Comment(ProductCommentViewModel viewModel)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == viewModel.ProductId);
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

            _dbContext.ProductComments.Add(comment);
            _dbContext.SaveChanges();

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
