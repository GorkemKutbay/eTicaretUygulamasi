using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("Get")]
        public IActionResult GetAll()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductEntity product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductEntity product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        [HttpGet("GetWithCategory")]
        public IActionResult GetWithCategory(int id)
        {
            var product = _context.Products.Where(p => p.Id == id).Include(p => p.Category);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpGet("GetCommentById")]
        public IActionResult GetComment(int id)
        {
            var comment = _context.ProductComments.Where(c => c.Id == id);
            return Ok(comment);

        }
        [HttpPost("DeleteComment")]
        public async Task<IActionResult> DeleteComment(ProductCommentEntity comment)
        {
            _context.ProductComments.RemoveRange(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetImageById")]
        public IActionResult GetImage(int id)
        {
            var image = _context.ProductImages.Where(i => i.Id == id);
            return Ok(image);

        }
        [HttpPost("DeleteImage")]
        public async Task<IActionResult> DeleteImage(ProductImageEntity image)
        {
            _context.ProductImages.RemoveRange(image);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetCartItemById")]
        public IActionResult GetCartItem(int id)
        {
            var cartItem = _context.CartItems.Where(c => c.Id == id);
            return Ok(cartItem);
        }
        [HttpPost("DeleteCartItem")]
        public async Task<IActionResult> DeleteCartItem(CartItemEntity cartItem)
        {
            _context.CartItems.RemoveRange(cartItem);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetOrderItemById")]
        public IActionResult GetOrderItem(int id)
        {
            var orderItem = _context.OrderItems.Where(o => o.Id == id);
            return Ok(orderItem);
        }
        [HttpPost("DeleteOrderItem")]
        public async Task<IActionResult> DeleteOrderItem(OrderItemEntity orderItem)
        {
            _context.OrderItems.RemoveRange(orderItem);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("AddCommment")]
        public async Task<IActionResult> AddComment([FromBody] ProductCommentEntity comment)
        {
            _context.ProductComments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetProductsById")]
        public IActionResult GetProductsById(int id)
        {
            var products = _context.Products.Where(p => p.SellerId == id).ToList();
            return Ok(products);
        }
    }
}
