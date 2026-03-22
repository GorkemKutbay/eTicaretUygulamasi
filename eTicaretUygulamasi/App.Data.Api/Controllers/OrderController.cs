using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetOrdersByUser")]
        public async Task<IActionResult> GetOrdersByUser(int id)
        {
            var orders = _context.Orders.Where(o => o.UserId == id).ToList();
            return Ok(orders);
        }
        [HttpGet("GetOrdersWithCategory")]
        public async Task<IActionResult> GetOrdersWithCategory(int id)
        {
            var orders = await _context.CartItems.Where(o => o.UserId == id).Include(p => p.Product).ToListAsync();
            return Ok(orders);


        }
        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderEntity order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }
        [HttpDelete("DeleteRangeOrder")]
        public IActionResult DeleteRangeOrder(int id)
        {
            var cartItems = _context.CartItems.Where(c => c.UserId == id).ToList();
            _context.CartItems.RemoveRange(cartItems);
            return Ok(cartItems);
        }
        [HttpGet("GetOrderWithProduct")]
        public IActionResult GetOrderWithProduct(int id)
        {
            var orders = _context.OrderItems.Where(o => o.OrderId == id).Include(o => o.Product).ToList();
            return Ok(orders);
        }
        [HttpGet("TwoParameter")]
        public IActionResult TwoParameter(int userId, int id)
        {
            var order = _context.Orders.Where(o => o.UserId == userId && o.Id == id).FirstOrDefault();
            return Ok(order);

        }
    }
}
