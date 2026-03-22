using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("data-api");
        public UserController(IHttpClientFactory http)
        {
            _http = http;
        }
        public async Task<IActionResult> List()
        {


            //var users = await _repo.GetWhereWithIncludes<UserEntity>(u => true, u => u.Role);


            return View(users);


        }

        public async Task<IActionResult> Approve(int id)
        {


            //var user = await _repo.GetByIdWithIncludes<UserEntity>(id);
            var user = await Client.GetFromJsonAsync<UserEntity>($"/api/user/GetUserById/{id}");
            if (user == null) return NotFound();

            user.RoleId = 2;       // Satıcı rolü (Örn: 2)
            user.Request = false;  // İstek tamamlandığı için false'a çekiyoruz

            //await _repo.Update(user);
            await Client.PutAsJsonAsync($"/api/user/UpdateUserRole", user);

            return RedirectToAction("List");
        }


    }
}
