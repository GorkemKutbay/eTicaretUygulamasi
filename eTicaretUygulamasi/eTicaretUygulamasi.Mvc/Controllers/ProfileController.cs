

using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("ApiClient");
        public ProfileController(IHttpClientFactory http)
        {
            _http = http;
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


            //var user = await _repo.GetByIdWithIncludes<UserEntity>(userId);
            var user = await Client.GetFromJsonAsync<UserEntity>($"api/Profile/GetUser/{userId}");

            if (user == null) return NotFound();

            var viewModel = new ProfileDetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone ?? "Belirtilmemiş",
                Address = user.Address ?? "Belirtilmemiş",
                RoleId = user.RoleId
            };
            ViewBag.Id = user.Id;
            ViewBag.request = user.Request;
            return View(viewModel);
        }

        [HttpGet]
        [Authorize("AllRoles")]
        public async Task<IActionResult> Edit()
        {
            //var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            int userId = GetCurrentUserId();


            if (userId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }
            //var user = await _repo.GetByIdWithIncludes<UserEntity>(userId);
            var response = await Client.GetAsync($"api/Profile/GetUser/{userId}");
            var user = await response.Content.ReadFromJsonAsync<UserEntity>();

            if (user == null)
                return NotFound();

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
        [Authorize("AllRoles")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
                //var user = await _repo.GetByIdWithIncludes<UserEntity>(GetCurrentUserId());
                var response = await Client.GetAsync($"api/Profile/GetUser/{GetCurrentUserId()}");
                var user = await response.Content.ReadFromJsonAsync<UserEntity>();
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    //await _repo.Update(user);
                    await Client.PutAsJsonAsync("api/Profile/UpdateUser", user);


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


            //var orders = await _repo.GetWhere<OrderEntity>(o => o.UserId == userId);
            var response = await Client.GetAsync($"api/Orders/GetOrdersByUserId/{userId}");
            var orders = await response.Content.ReadFromJsonAsync<List<OrderEntity>>();
            var viewModel = orders.Select(o => new MyOrdersViewModel
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status
            }).ToList();
            return View(viewModel);
        }


        [Authorize(Policy = "seller")]

        public async Task<IActionResult> MyProducts()
        {
            var sellerId = GetCurrentUserId(); // Bu metodu, oturum açmış kullanıcının ID'sini almak için uygulamanızın kimlik doğrulama mekanizmasına göre implement edin !!!

            //var products = await _repo.GetWhere<ProductEntity>(p => p.SellerId == sellerId);
            var products = await Client.GetFromJsonAsync<List<ProductEntity>>($"api/product/GetProductsById/{sellerId}");

            var viewModel = products
                 .Select(p => new MyProductsViewModel
                 {
                     ProductId = p.Id,
                     ProductName = p.DDName,
                     Price = p.Price,
                     StockQuantity = p.StockAmount,
                     Enabledd = p.Enabled

                 }).ToList();


            return View(viewModel);

        }

        [Authorize("buyer")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SellerRequest(int id)
        {
            //var user = await _repo.GetByIdWithIncludes<UserEntity>(id);
            var user = await Client.GetFromJsonAsync<UserEntity>($"api/Profile/GetUser/{id}");

            if (user == null)
                return NotFound();


            user.Request = true;

            //await _repo.Update(user);
            await Client.PutAsJsonAsync("api/Profile/UpdateUser", user);

            return RedirectToAction(nameof(Details));
        }
    }
}