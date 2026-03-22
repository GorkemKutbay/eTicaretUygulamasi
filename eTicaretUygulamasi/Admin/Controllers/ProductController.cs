using Admin.Models;
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("data-api");

        public ProductController(IHttpClientFactory http)
        {
            _http = http;
        }

        // Delete
        [HttpGet("/product/{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            //var product= _context.Products
            //    .Include(p => p.Category)
            //    .FirstOrDefault(p => p.Id == id);
            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id, p => p.Category);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"/api/product/GetWithCategory/{id}");

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var product = _context.Products.FirstOrDefault(p => p.Id == id);
            //var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            var product = await Client.GetFromJsonAsync<ProductEntity>($"/api/product/GetById/{id}");
            if (product == null)
            {
                ViewBag.ErrorMessage = "Product not found.";
                return View("Delete");

            }

            //var comments = _context.ProductComments.Where(c => c.ProductId == id).ToList();
            //_context.ProductComments.RemoveRange(comments);

            //var images = _context.ProductImages.Where(i => i.ProductId == id).ToList();
            //_context.ProductImages.RemoveRange(images);

            //var cartItems = _context.CartItems.Where(ci => ci.ProductId == id).ToList();
            //_context.CartItems.RemoveRange(cartItems);

            //var orderItems = _context.OrderItemS.Where(oi => oi.ProductId == id).ToList();
            //_context.OrderItemS.RemoveRange(orderItems);

            string DeletedName = product.DDName;
            //_context.Products.Remove(product);
            //_context.SaveChanges();
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




            ViewBag.SuccessMessage = $"Product '{DeletedName}' has been deleted successfully.";

            return View("Delete");


        }
        public async Task<IActionResult> ListAllProducts()
        {
            ViewBag.Products =await _repo.GetAll<ProductEntity>();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Pasive(int id)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Enabled = false;
            await _repo.Update(product);
            return RedirectToAction("ListAllProducts");
        }
        [HttpPost]
        public async Task<IActionResult> Active(int id)
        {
            var product = await _repo.GetByIdWithIncludes<ProductEntity>(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Enabled = true;
            await _repo.Update(product);
            return RedirectToAction("ListAllProducts");
        }
    }
}
