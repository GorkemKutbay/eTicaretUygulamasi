using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    [Authorize(Roles = "buyer")]
    public class OrderController  : BaseController
    {        private readonly IDataRepository _repo;

        public OrderController(IDataRepository repo)
        {
            
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = GetCurrentUserId();

            //var cartItems= _dbContext.CartItems
            //    .Where(c=> c.UserId == userId)
            //    .Include(c => c.Product)
            //    .ToList();
            var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(c => c.UserId == userId, c => c.Product);

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
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            int userId = GetCurrentUserId();

            //var cartItems = _dbContext.CartItems
            //    .Where(c => c.UserId == userId)
            //    .Include(c => c.Product)
            //    .ToList(); 
            var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(c => c.UserId == userId, c => c.Product);
            if (!cartItems.Any())
            {
                //ViewBag.ErrorMessage = "Sepetinizde ürün bulunmamaktadır!";
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır!";
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

            //_dbContext.Orders.Add(order);
            await _repo.Add(order);


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
                //_dbContext.OrderItemS.Add(orderItem);
                await _repo.Add(orderItem);
            }

            //_dbContext.CartItems.RemoveRange(cartItems);
            //_dbContext.SaveChanges();
            await _repo.DeleteRange(cartItems);

            TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu!";
            return RedirectToAction("Details", new { id = order.Id });
        }


        [HttpGet]
        [Authorize("BuyerOrSeller")]
        public async Task<IActionResult> Details(int id)
        {
            int userId = GetCurrentUserId();

            //var order = _dbContext.Orders
            //    .FirstOrDefault(o => o.Id == id && o.UserId == userId);
            var order = (await _repo.GetWhere<OrderEntity>(o => o.Id == id && o.UserId == userId)).FirstOrDefault();

            if (order == null)
            {
                ViewBag.ErrorMessage = "Sipariş bulunamadı!";
                return View();
            }

            //var orderItems = _dbContext.OrderItemS
            //    .Where(oi => oi.OrderId == order.Id)
            //    .Include(oi => oi.Product)
            //    .ToList();
            var orderItems = await _repo.GetWhereWithIncludes<OrderItemEntity>(oi => oi.OrderId == order.Id, oi => oi.Product);

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
