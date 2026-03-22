using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("CreateCategory")]
        public async Task<ActionResult> CreateCategory(CategoryEntity category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }
        [HttpGet("GetCategoryById/{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _context.Categories.Find(id);
            return Ok(category);
        }
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> Update(CategoryEntity category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }
        [HttpGet("GetAllCategories")]
        public IActionResult GetAll()
        {
            var categories = _context.Categories.ToList();
            return Ok (categories);
        }
        [HttpGet("GetCategoryProducts/{id}")]
        public IActionResult GetProducts(int id )
        {
            var categoryProducts = _context.Products.Where(p => p.CategoryId == id).ToList();
            return Ok(categoryProducts);
        }
        [HttpDelete("DeleteCategoryProducts/{id}")]
        public async Task<IActionResult> Delete(int id )
        {
            var categoryProducts = _context.Products.Where(p => p.CategoryId == id).ToList();
            _context.Products.RemoveRange(categoryProducts);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id )
        {
            var category = await _context.Categories.FindAsync(id); 
             _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok();
        }



    }
}
