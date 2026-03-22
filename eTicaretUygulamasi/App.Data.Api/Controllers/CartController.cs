using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetByIdWithIncludes(int id, [FromQuery] string[] includes)
        {
            // Sorguyu başlatıyoruz
            IQueryable<ProductEntity> query = _context.Products;

            // Eğer istemci "Category", "Seller" gibi stringler gönderdiyse Include ediyoruz
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    // EF Core string tabanlı include desteği sağlar
                    query = query.Include(include);
                }
            }

            var product = await query.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("getbyuserandproduct")]
        public async Task<IActionResult> GetByUserAndProduct([FromQuery] string userId, [FromQuery] int productId)
        {
            // Repository veya Context üzerinden sorgunuzu yapın
            //var cartItems = await _repo.GetWhere<CartItemEntity>(c => c.UserId == userId && c.ProductId == productId);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId.ToString() == userId && c.ProductId == productId)
                .ToListAsync();

            // GetWhere genellikle bir liste döner (IEnumerable/List). 
            // Eğer sepet boşsa NotFound veya boş liste dönebilirsiniz.
            if (cartItems == null || !cartItems.Any())
            {
                // Boş liste de dönebilirsiniz: return Ok(new List<CartItemEntity>());
                return NotFound();
            }

            return Ok(cartItems);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemEntity cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Güncellenecek veri boş olamaz.");
            }

            // Repository üzerinden güncelleme işlemini yapıyoruz
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            // Güncelleme başarılı olduğunda genellikle 200 OK veya 204 NoContent dönülür
            return Ok();
        }
        [HttpPost("Add")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemEntity cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Eklenecek veri boş olamaz.");
            }

            // Repository üzerinden ekleme işlemini yapıyoruz
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            // Ekleme başarılı olduğunda genellikle 200 OK veya 204 NoContent dönülür
            return Ok();
        }
        [HttpGet("getbyuser/{userId}")]
        public async Task<IActionResult> GetUserCartItems(string userId)
        {
            // İlişkili tablolarla birlikte kullanıcının sepetini çekiyoruz
            //var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(
            //    c => c.UserId == userId,
            //    c => c.Product,
            //    c => c.Product.Category
            //);
            var cartItems = await _context.CartItems
                .Where(c => c.UserId.ToString() == userId)
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                // Sepet boşsa boş bir liste dönebilirsiniz
                return Ok(new List<CartItemEntity>());
            }

            return Ok(cartItems);
        }
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            // Repository üzerinden ID'ye göre ürünü çekip ilkini alıyoruz
            //var product = (await _repo.GetWhere<ProductEntity>(p => p.Id == id)).FirstOrDefault();
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // 404 döner
            }

            return Ok(product); // 200 OK ile ürünü JSON olarak döner
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCartItem([FromBody] CartItemEntity cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Silinicek veri boş olamaz.");
            }

            // Repository üzerinden silme işlemini yapıyoruz
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Güncelleme başarılı olduğunda genellikle 200 OK veya 204 NoContent dönülür
            return Ok();
        }

    }
}
