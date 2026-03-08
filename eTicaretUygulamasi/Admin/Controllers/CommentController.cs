using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _dbcontext;

        public CommentController(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IActionResult List()
        {
            var comments = _dbcontext.ProductComments
                .Include(x => x.User)
                .Include(x => x.Product)
                .ToList();

            return View(comments);
        }
        [HttpPost]
        public IActionResult Approve(int id )
        {
            var comment = _dbcontext.ProductComments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            comment.IsConfirmed = true;
            _dbcontext.SaveChangesAsync();
            return RedirectToAction("List");
        }
    }
}
