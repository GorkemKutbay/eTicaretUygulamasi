using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("Get")]
        public IActionResult Get()
        {
            var users = _dbContext.Users;
            return Ok(users);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("GetProductsForListing")]
        public IActionResult GetProductsForListing()
        {
            var products = _dbContext.Products
                .Include(x => x.Category)
                .ToList();

            return Ok(products);


        }
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetByIdWithIncludes(int id, [FromQuery] string[] includes)
        {
            // Sorguyu başlatıyoruz
            IQueryable<ProductEntity> query = _dbContext.Products;

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
        [HttpGet("GetCategoryCount")]
        public IActionResult GetCategoryCount()
        {
            var count = _dbContext.Categories.Count();
            return Ok(count);

        }
        [HttpGet("GetUserCount")]
        public IActionResult GetUserCount()
        {
            var count = _dbContext.Users.Count();
            return Ok(count);

        }
        [HttpGet("GetProductCount")]
        public IActionResult GetProductCount()
        {
            var count = _dbContext.Products.Count();
            return Ok(count);

        }
    }
}
