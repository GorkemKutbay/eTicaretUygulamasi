using Admin.Models;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("/category/create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("/category/create")]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var entity = new CategoryEntity
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass,
                CreatedAt = DateTime.UtcNow
            };


            _dbContext.Categories.Add(entity);


            _dbContext.SaveChanges();


            TempData["SuccessMessage"] = "Yeni kategori başarıyla oluşturuldu!";


            return RedirectToAction("Create");
        }


        [HttpGet("/category/edit/{id}")]
        public IActionResult Edit(int id)
        {

            var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (entity == null)
            {

                return RedirectToAction("Index", "Home");
            }


            var model = new CategoryEditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Color = entity.Color,
                IconCssClass = entity.IconCssClass
            };

            return View(model);
        }


        [HttpPost("/category/edit/{id}")]
        public IActionResult Edit(int id, CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (entity == null)
            {
                return RedirectToAction("Index", "Home");
            }


            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;


            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";


            return RedirectToAction("Edit", new { id = entity.Id });
        }
        [HttpGet]
        public IActionResult ListAllCategory()
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View();
        }
        [HttpGet]
        [Route("/delete/category/{id}")]
        public IActionResult Delete(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var model = new CategoryDeleteViewModel()
            {
                Id = category.Id,
                CategoryName = category.Name,
                DateTime = category.CreatedAt,

            };

            return View(model);
        }
        [HttpPost]
        [Route("/delete/category/{id}")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmAsync(int id, bool forceDelete = false)
        {
            var category = await _dbContext.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            var hasProducts = _dbContext.Products
                .Any(p => p.CategoryId == id);

            
            if (hasProducts && !forceDelete)
            {
                ViewBag.Warning =
                    "Bu kategoriye ait ürünler var. Silerseniz bağlı ürünler de silinecek.";

                var model = new CategoryDeleteViewModel
                {
                    Id = category.Id,
                    CategoryName = category.Name,
                    DateTime = category.CreatedAt
                };

                ViewBag.HasProducts = true;

                return View(model); 
            }

            
            var products = _dbContext.Products
                .Where(p => p.CategoryId == id);

            _dbContext.Products.RemoveRange(products);
            _dbContext.Categories.Remove(category);

            await _dbContext.SaveChangesAsync();

            ViewBag.SuccessMessage =
                $"{category.Name} ve bağlı ürünler başarıyla silindi.";

            return View(); 
        }
    }
}
