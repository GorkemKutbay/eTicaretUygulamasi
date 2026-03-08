using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _dbcontext;

        //Listele
        public UserController(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IActionResult List()
        {
            var users = _dbcontext.Users
                .Include(x => x.Role)
                .ToList();

            return View(users);
        }

        public IActionResult Approve(int id)
        {
            var user = _dbcontext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.RoleId = 2; 
            _dbcontext.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
