
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProfileController : Controller
    {
       
        private readonly IDataRepository _repo;

        public ProfileController( IDataRepository repo)
        {
            
            _repo = repo;

        }

        public async Task<IActionResult> Details()
        {
            //var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            var user = await _repo.GetByIdWithIncludes<UserEntity>(1);
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
        public async Task<IActionResult> Edit()
        {
            //var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            var user = await _repo.GetByIdWithIncludes<UserEntity>(1);
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
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
                var user = await _repo.GetByIdWithIncludes<UserEntity>(1);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    await _repo.Update(user);


                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                    return RedirectToAction("Details");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = 1;
            //var orders = _dbContext.Orders
            //    .Where(o => o.UserId == userId)
            //    .Select(o => new MyOrdersViewModel
            //    {
            //        OrderId = o.Id,
            //        OrderDate = o.CreatedAt,
            //        TotalPrice = o.TotalPrice,
            //        Status = o.Status
            //    }).ToList();
            var orders = await _repo.GetWhere<OrderEntity>(o => o.UserId == userId);
            var viewModel = orders.Select(o => new MyOrdersViewModel
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status
            }).ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> MyProducts()
        {
            var sellerId = 1;
            //var products = _dbContext.Products
            //    .Where(p => p.SellerId == sellerId)
            //    .Select(p => new MyProductsViewModel
            //    {
            //        ProductId = p.Id,
            //        ProductName = p.DDName,
            //        Price = p.Price,
            //        StockQuantity = p.StockAmount
            //    }).ToList();
            var products = await _repo.GetWhere<ProductEntity>(p => p.SellerId == sellerId);

            var viewModel = products
                 .Select(p => new MyProductsViewModel
                 {
                     ProductId = p.Id,
                     ProductName = p.DDName,
                     Price = p.Price,
                     StockQuantity = p.StockAmount
                 }).ToList();


            return View(viewModel);

        }
    }
}