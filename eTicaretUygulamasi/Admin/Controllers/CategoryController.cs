using Admin.Models;

using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("data-api");
        public CategoryController(IHttpClientFactory http)
        {
            _http = http;
        }


        [HttpGet("/category/create")]
        [Authorize("Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("/category/create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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
            await Client.PostAsJsonAsync("/api/category/CreateCategory", entity);
            //await _repo.Add(entity);






            TempData["SuccessMessage"] = "Yeni kategori başarıyla oluşturuldu!";


            return RedirectToAction("Create");
        }


        [HttpGet("/category/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {

            //var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            //var entity = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
            var entity = await Client.GetFromJsonAsync<CategoryEntity>($"/api/category/GetCategoryById/{id}");
            //var entity = await Client.GetFromJsonAsync<CategoryEntity>($"/api/category/GetCategoryById?id={id}");

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
            //var entity = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
            var entity = await Client.GetFromJsonAsync<CategoryEntity>($"/api/category/GetCategoryById/{id}");

            if (entity == null)
            {
                return RedirectToAction("Index", "Home");
            }


            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;


            //await _repo.Update(entity);
            await Client.PutAsJsonAsync("/api/category/UpdateCategory", entity);

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";


            return RedirectToAction("Edit", new { id = entity.Id });
        }

        [Authorize(Roles = "AdminOrSeller")]
        [HttpGet]
        public async Task<IActionResult> ListAllCategory()
        {
            //ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Categories = await Client.GetFromJsonAsync<List<CategoryEntity>>("/api/category/GetAllCategories");
            return View();
        }
        [HttpGet]
        [Route("/delete/category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            //var category = await _repo.GetByIdWithIncludes<CategoryEntity>(id);
            var category = await Client.GetFromJsonAsync<CategoryEntity>($"/api/category/GetCategoryById/{id}");
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
            var category = await Client.GetFromJsonAsync<CategoryEntity>($"/api/category/GetCategoryById/{id}");
            if (category == null) return NotFound();


            //var categoryProducts = await _repo.GetWhere<ProductEntity>(p => p.CategoryId == id);
            var categoryProducts = await Client.GetFromJsonAsync<List<ProductEntity>>($"/api/category/GetCategoryProducts/{id}");

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


            //await _repo.DeleteRange(categoryProducts);
            await Client.DeleteAsync($"/api/category/DeleteCategoryProducts/{id}");
            //await _repo.Delete(category);
            await Client.DeleteAsync($"/api/category/DeleteCategory/{id}");

            ViewBag.SuccessMessage = $"{category.Name} ve bağlı ürünler başarıyla silindi.";
            return View();
        }
    }
}
