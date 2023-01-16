using DemoApplication.Areas.Admin.ViewModels.Order;
using DemoApplication.Contracts.Order;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/order")]
    [Authorize(Roles = "admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private const string ORDER_MESSAGE_TITLE = "Your oder current stage";
        public OrderController(DataContext dataContext, IUserService userService, IEmailService emailService)
        {
            _dataContext = dataContext;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet("order-list",Name ="admin-order-list")]
        public async Task<IActionResult> ListAsync ()
        {
            var model = await _dataContext.Orders.Include(o => o.User).Select(o => new OrderListItemViewModel
            {
                Id = o.Id,
                OrderStatus = StatusStatusCode.GetStatusCode((Status)o.Status),
                OrderSum = o.SumTotalPrice,
                OrderTime = o.CreatedAt,
                Client = $"{o.User.FirstName}{o.User.LastName}"
            }).ToListAsync();

            return View(model);
        }

        [HttpGet("order-update/{orderId}", Name = "admin-order-update")]
        public async Task<IActionResult> UpdateAsync([FromRoute]string orderId)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null) return NotFound();

            var model = new OrderUpdateViewModel
            {
                OrderId = order.Id,
                OrderStatus = order.Status
            };

            return View(model);
        }

        [HttpPost("order-update/{orderId}", Name = "admin-order-update")]
        public async Task<IActionResult> UpdateAsync(string orderId , OrderUpdateViewModel model )
        {
            if(!ModelState.IsValid) return View(model);

            var order = await _dataContext.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null) return NotFound();

            order.Status = model.OrderStatus;

            var message = new Message(order.User.Email, ORDER_MESSAGE_TITLE,
                StatusMessage.GetStatusMessage((Status)model.OrderStatus,order.User,order.Id));

            _emailService.Send(message);

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-order-list");

        }

        [HttpPost("order-delete/{orderId}", Name = "admin-order-delete")]
        public async Task<IActionResult> DeleteAsync(string orderId)
        {

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null) return NotFound();

            _dataContext.Orders.Remove(order);

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-order-list");

        }
    }
}
