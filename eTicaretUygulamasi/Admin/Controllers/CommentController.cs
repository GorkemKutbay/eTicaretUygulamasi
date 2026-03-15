using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class CommentController : Controller
    {
        
        private readonly IDataRepository _repo;

        public CommentController( IDataRepository repo)
        {
            
            _repo = repo;
        }
        public async Task<IActionResult> List()
        {
            //var comments = _dbcontext.ProductComments
            //    .Include(x => x.User)
            //    .Include(x => x.Product)
            //    .ToList();
            var comments = await _repo.GetWhereWithIncludes<ProductCommentEntity>(x => !x.IsConfirmed, x => x.User, x => x.Product);

            return View(comments);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            //var comment = _dbcontext.ProductComments.Find(id);
            var comment = await _repo.GetByIdWithIncludes<ProductCommentEntity>(id);
            if (comment == null)
            {
                return NotFound();
            }
            comment.IsConfirmed = true;
            await _repo.Update(comment);
            return RedirectToAction("List");
        }
    }
}
