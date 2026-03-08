
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Details()
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            if (user == null) return NotFound();

            var viewModel = new ProfileDetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = "",
                Address = ""
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            if (user == null) return NotFound();

            var viewModel = new ProfileEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = "",
                Address = ""
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    _dbContext.Update(user);
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                    return RedirectToAction("Details");
                }
            }
            return View(model);
        }

        public IActionResult MyOrders()
        {
            var userId = 1;
            var orders = _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new MyOrdersViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status
                }).ToList();

            return View(orders);
        }

        public IActionResult MyProducts()
        {
            var sellerId = 1;
            var products = _dbContext.Products
                .Where(p => p.SellerId == sellerId)
                .Select(p => new MyProductsViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.DDName,
                    Price = p.Price,
                    StockQuantity = p.StockAmount
                }).ToList();

            return View(products);
        }
    }
}