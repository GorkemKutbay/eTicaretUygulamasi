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
    }
}
