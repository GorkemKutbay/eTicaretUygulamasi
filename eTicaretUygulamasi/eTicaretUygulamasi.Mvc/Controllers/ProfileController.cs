
using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProfileController :  BaseController    
    {
       
        private readonly IDataRepository _repo;

        public ProfileController( IDataRepository repo)
        {
            
            _repo = repo;

        }

        [HttpGet]
        [Authorize("AllRoles")] 
        public async Task<IActionResult> Details()
        {
         
            int userId = GetCurrentUserId();

            if (userId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

           
            var user = await _repo.GetByIdWithIncludes<UserEntity>(userId);

            if (user == null) return NotFound();

            var viewModel = new ProfileDetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone ?? "Belirtilmemiş",
                Address = user.Address ?? "Belirtilmemiş"
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

        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId(); // Bu metodu, oturum açmış kullanıcının ID'sini almak için uygulamanızın kimlik doğrulama mekanizmasına göre implement edin !!!

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


        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> MyProducts()
        {
            var sellerId = GetCurrentUserId(); // Bu metodu, oturum açmış kullanıcının ID'sini almak için uygulamanızın kimlik doğrulama mekanizmasına göre implement edin !!!
           
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