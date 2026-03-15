using Admin.Models;
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class CategoryController : Controller
    {

        private readonly IDataRepository _repo;

        public CategoryController(IDataRepository repo)
        {
            _repo = repo;
        }


        [HttpGet("/category/create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("/category/create")]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
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


            //_dbContext.Categories.Add(entity);
            await _repo.Add(entity);





            TempData["SuccessMessage"] = "Yeni kategori başarıyla oluşturuldu!";


            return RedirectToAction("Create");
        }


        [HttpGet("/category/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {

            //var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            var entity = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
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
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            //var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            var entity = await _repo.GetByIdWithIncludes<CategoryEntity>(id);

            if (entity == null)
            {
                return RedirectToAction("Index", "Home");
            }


            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;


            await _repo.Update(entity);

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";


            return RedirectToAction("Edit", new { id = entity.Id });
        }
        [HttpGet]
        public async Task<IActionResult> ListAllCategory()
        {
            //ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Categories = await _repo.GetAll<CategoryEntity>();
            return View();
        }
        [HttpGet]
        [Route("/delete/category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            var category = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
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
            var category = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
            if (category == null) return NotFound();

           
            var categoryProducts = await _repo.GetWhere<ProductEntity>(p => p.CategoryId == id);

            if (categoryProducts.Any() && !forceDelete)
            {
                ViewBag.Warning = "Bu kategoriye ait ürünler var. Silerseniz bağlı ürünler de silinecek.";
                ViewBag.HasProducts = true;

                var model = new CategoryDeleteViewModel
                {
                    Id = category.Id,
                    CategoryName = category.Name,
                    DateTime = category.CreatedAt
                };
                return View(model);
            }

           
            await _repo.DeleteRange(categoryProducts);
            await _repo.Delete(category);

            ViewBag.SuccessMessage = $"{category.Name} ve bağlı ürünler başarıyla silindi.";
            return View();
        }
    }
}
