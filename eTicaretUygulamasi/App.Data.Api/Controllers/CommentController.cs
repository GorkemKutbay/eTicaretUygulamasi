using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetComments1")]
        public async Task<IActionResult> Get()
        {
            var comments = await _context.ProductComments
                .Include(x => true)
                .Include(y=>y.User)
                .Include(z=>z.Product)
                .Include(x=>x.User.Role)
                .ToListAsync();
            return Ok(comments);
        }
        [HttpGet("GetCommentById/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var comment = await _context.ProductComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }
        [HttpPut("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromBody] ProductCommentEntity comment)
        {
            if (comment == null)
            {
                return BadRequest();
            }

            _context.ProductComments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("GetCommentsByProductId/{id}")]
        public IActionResult GetComment(int id )
        {
            var comment =  _context.ProductComments.FindAsync(id);
            return Ok(comment);
        }
    }
}
