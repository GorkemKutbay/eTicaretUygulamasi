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
        private readonly AppDbContext _dbcontext;
        private readonly IDataRepository _repo;

        //Listele
        public UserController(AppDbContext dbContext, IDataRepository repo)
        {
            _dbcontext = dbContext;
            _repo = repo;
        }
        public async Task<IActionResult> List()
        {


            var users = await _repo.GetWhereWithIncludes<UserEntity>(u => true, u => u.Role);

            return View(users);


        }

        public async Task<IActionResult> Approve(int id)
        {


            var user = await _repo.GetByIdWithIncludes<UserEntity>(id);
            if (user == null) return NotFound();

            user.RoleId = 2;       // Satıcı rolü (Örn: 2)
            user.Request = false;  // İstek tamamlandığı için false'a çekiyoruz

            await _repo.Update(user);

            return RedirectToAction("List");
        }


    }
}
