using Admin.Models;
using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Delete
        [HttpGet("/product/{id}/delete")]
        public IActionResult Delete(int id)
        {
            var product= _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            { 
                ViewBag.ErrorMessage = "Product not found.";
                return View();
            }

            var viewModel = new ProductDeleteViewModel
            {
                Id = product.Id,
                DDName = product.DDName,
                Price = product.Price,
                CategoryName = product.Category.Name,
                StockAmount = product.StockAmount
            };
            return View(viewModel);


        }
        [HttpPost("/product/{id}/delete")]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                ViewBag.ErrorMessage = "Product not found.";
                return View("Delete");

            }

            var comments = _context.ProductComments.Where(c => c.ProductId == id).ToList();
            _context.ProductComments.RemoveRange(comments);

            var images = _context.ProductImages.Where(i => i.ProductId == id).ToList();
            _context.ProductImages.RemoveRange(images);

            var cartItems = _context.CartItems.Where(ci => ci.ProductId == id).ToList();
            _context.CartItems.RemoveRange(cartItems);

            var orderItems = _context.OrderItemS.Where(oi => oi.ProductId == id).ToList();
            _context.OrderItemS.RemoveRange(orderItems);

            string DeletedName = product.DDName;
            _context.Products.Remove(product);
            _context.SaveChanges();

            ViewBag.SuccessMessage = $"Product '{DeletedName}' has been deleted successfully.";

            return View("Delete");


        }
        public IActionResult ListAllProducts()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }
    }
}
