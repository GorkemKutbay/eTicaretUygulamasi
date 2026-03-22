
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    [Authorize("BuyerOrSeller")]
    public class OrderController : BaseController
    {
        private readonly IHttpClientFactory _http;
        private HttpClient Client => _http.CreateClient("Api");
        public OrderController(IHttpClientFactory http)
        {
            _http = http;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = GetCurrentUserId();

            //var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(c => c.UserId == userId, c => c.Product);
            //var cartItems = await Client.GetAsync($"api/order/GetOrdersWithCategory/{userId}");
            var cartItems = await Client.GetFromJsonAsync<List<CartItemEntity>>($"api/order/GetOrdersWithCategory/{userId}");


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

            //var cartItems = await _repo.GetWhereWithIncludes<CartItemEntity>(c => c.UserId == userId, c => c.Product);
            var cartItems = await Client.GetFromJsonAsync<List<CartItemEntity>>($"api/order/GetOrdersWithCategory/{userId}");
            if (!cartItems.Any())
            {
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

            //await _repo.Add(order);
            await Client.PostAsJsonAsync("api/order/AddOrder", order);




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
                //await _repo.Add(orderItem);
                await Client.PostAsJsonAsync("api/order/AddOrder", orderItem);

            }

            //await _repo.DeleteRange(cartItems);
            await Client.DeleteAsync($"api/order/DeleteRangeOrder/{userId}");




            TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu!";
            return RedirectToAction("Details", new { id = order.Id });
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int userId = GetCurrentUserId();

            //var order = (await _repo.GetWhere<OrderEntity>(o => o.Id == id && o.UserId == userId)).FirstOrDefault();
            var order = await Client.GetFromJsonAsync<OrderEntity>($"api/order/GetOrderById/{id}/{userId}");

            if (order == null)
            {
                ViewBag.ErrorMessage = "Sipariş bulunamadı!";
                return View();
            }

            //var orderItems = await _repo.GetWhereWithIncludes<OrderItemEntity>(oi => oi.OrderId == order.Id, oi => oi.Product);
            var orderItems = await Client.GetFromJsonAsync<List<OrderItemEntity>>($"api/order/GetOrderWithProduct/{order.Id}");

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
