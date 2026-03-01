using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.Models; 
using eTicaretUygulamasi.Mvc.App.Data.Entities; 
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _dbContext.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var newProduct = new ProductEntity
                {
                    DDName = model.Name,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Details = model.Details?? "",
                    SellerId = 1
                };

                _dbContext.Products.Add(newProduct);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("Listing", "Home");
            }
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(model);
        }
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

        public IActionResult Comment()
        {
            return View();
        }
    }
}
