using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _dbContext;

        public OrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Create()
        {
            int userId = 1;

            var cartItems= _dbContext.CartItems
                .Where(c=> c.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            if (!cartItems.Any())
            { 
                ViewBag.ErrorMessage = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction("Edit", "Cart");
            }
            var viewModel = new OrderCreateViewModel
            {
                Items = cartItems.Select(c => new OrderCreateItemViewModel
                {
                    ProductName = c.Product.DDName,
                    UnitPrice = c.Product.Price,
                    Quantity = c.Quantity
                }).ToList()
            };

            return View(viewModel);
        }



        [HttpPost]
        public IActionResult Create(OrderCreateViewModel model)
        {
            int userId = 1;

            var cartItems = _dbContext.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList(); 
            if (!cartItems.Any())
            {
                ViewBag.ErrorMessage = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction("Edit", "Cart");
            }

            model.Items = cartItems.Select(c => new OrderCreateItemViewModel
            { 
                ProductName = c.Product.DDName,
                UnitPrice = c.Product.Price,
                Quantity = c.Quantity
            
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string orderCode = "ORD-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + userId;  // siparis kodu oluşturma

            decimal totalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);

            var order = new OrderEntity
            {
                UserId = userId,
                OrderCode = orderCode,
                TotalPrice = totalPrice,
                Status = "Hazırlanıyor",
                Address = model.Address,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();


            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.OrderItemS.Add(orderItem);
            }

            _dbContext.CartItems.RemoveRange(cartItems);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu!";
            return RedirectToAction("Details", new { id = order.Id });
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            int userId = 1;

            var order = _dbContext.Orders
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                ViewBag.ErrorMessage = "Sipariş bulunamadı!";
                return View();
            }

            var orderItems = _dbContext.OrderItemS
                .Where(oi => oi.OrderId == order.Id)
                .Include(oi => oi.Product)
                .ToList();

            var viewModel = new OrderDetailsViewModel
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                Address = order.Address,
                OrderDate = order.CreatedAt,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                Items = orderItems.Select(oi => new OrderDetailsItemViewModel
                {
                    ProductName = oi.Product.DDName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
