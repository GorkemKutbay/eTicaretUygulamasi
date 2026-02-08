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

        [HttpPost("/category/{id}/edit")]
        public IActionResult Edit(int id)
        {
            return View();
        }


        // Delete
        [HttpGet("/category/{id}/delete")]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost("/category/{id}/delete")]
        public IActionResult Delete(int id)
        {
            return View();

        }

    }
}
