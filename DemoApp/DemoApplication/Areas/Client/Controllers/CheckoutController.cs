using DemoApplication.Areas.Client.ViewModels.Checkout;
using DemoApplication.Contracts.Order;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("checkout")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public CheckoutController(DataContext dataContext, IUserService userService, IOrderService orderService)
        {
            _dataContext = dataContext;
            _userService = userService;
            _orderService = orderService;
        }
        [HttpGet("checkoutlist", Name = "client-checkout-checkoutlist")]
        public async Task<IActionResult> Checkoutlist()
        {
            var model = new ProductListItemViewModel
            {

                Products = await _dataContext.BasketProducts.Include(bp => bp.Book)
                 .Select(bp => new ProductListItemViewModel.ListItem(bp.BookId, bp.Book.Title, bp.Quantity, bp.Book.Price, bp.Book.Price * bp.Quantity))
                 .ToListAsync()
            };
            return View(model);
        }

        [HttpPost("place-order", Name = "client-checkout-place-order")]
        public async Task<IActionResult> PlaceOrder()
        {
            var pasketProducts = _dataContext.BasketProducts.Include(bp => bp.Book).Select(bp => new
            ProductListItemViewModel.ListItem(bp.BookId, bp.Book.Title, bp.Quantity, bp.Book.Price, bp.Book.Price * bp.Quantity)).ToList();

            var createOrder = await CreateOrder();

            foreach (var basketProduct in pasketProducts)
            {
                var orderProduct = new OrderProduct
                {
                    BookId = basketProduct.Id,
                    Quantity = basketProduct.Quantity,
                    OrderId = createOrder.Id

                };

                _dataContext.OrderProducts.Add(orderProduct);
            }

            await DeleteBasketProducts();

            _dataContext.SaveChanges();

            async Task<Order> CreateOrder()
            {
                var order = new Order
                {
                    Id = _orderService.GenerateId(),
                    UserId = _userService.CurrentUser.Id,
                    Status = (int)Status.Created,
                    SumTotalPrice = _dataContext.BasketProducts.
                    Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id).Sum(bp => bp.Book.Price * bp.Quantity)

                };

                await _dataContext.Orders.AddAsync(order);

                return order;

            }

            async Task DeleteBasketProducts()
            {
                var removedBasketProducts = await _dataContext.BasketProducts
                       .Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id).ToListAsync();

                removedBasketProducts.ForEach(bp => _dataContext.BasketProducts.Remove(bp));
            }

            return RedirectToRoute("client-checkout-checkoutlist");
        }

    }





}
