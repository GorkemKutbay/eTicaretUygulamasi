using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _dbcontext;
        private readonly IDataRepository _repo;

        //Listele
        public UserController(AppDbContext dbContext,IDataRepository repo)
        {
            _dbcontext = dbContext;
            _repo = repo;
        }
        public async Task<IActionResult> List()
        {
            //var users = _dbcontext.Users
            //    .Include(x => x.Role)
            //    .ToList();
          
            var users = await _repo.GetWhereWithIncludes<UserEntity>( u => true,u => u.Role);

            return View(users);

            
        }

        public async Task<IActionResult> ApproveAsync(int id)
        {
            //var user = _dbcontext.Users.Find(id);

            var user = await _repo.GetByIdWithIncludes<UserEntity>(id);
            if (user == null)
            {
                return NotFound();
            }
            user.RoleId = 2;
            await _repo.Update(user);
            return RedirectToAction("List");
        }
    }
}
