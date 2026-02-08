using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
       // Delete
        [HttpGet("/product/{id}/delete")]
        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost("/product/{id}/delete")]
        public IActionResult Delete(int id)
        {
            return View();
        }

    }
}
