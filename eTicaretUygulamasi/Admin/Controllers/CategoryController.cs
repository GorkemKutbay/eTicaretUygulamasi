using Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class CategoryController : Controller
    {
       

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

            TempData["SuccessMessage"] = "Yeni kategori oluşturuldu!";
            return RedirectToAction("Create");
        }

    }
}
