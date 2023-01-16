using DemoApplication.Areas.Client.ViewModels.Account;
using DemoApplication.Contracts.Order;
using DemoApplication.Database;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;

        public AccountController(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
        }

        [HttpGet("dashboard", Name = "client-account-dashboard")]
        public IActionResult Dashboard()
        {
            var user = _userService.CurrentUser;
            var user2 = _userService.CurrentUser;

            return View();
        }

        [HttpGet("orders", Name = "client-account-orders")]
        public async Task<IActionResult> Orders()
        {
            var model = await _dataContext.Orders.Select(o => new OrderListItemViewModel
            {
                Id = o.Id,
                OrderStatus = StatusStatusCode.GetStatusCode((Status)o.Status),
                OrderSum = o.SumTotalPrice,
                OrderTime = o.CreatedAt,
                OrderProducts = _dataContext.OrderProducts.Where(op => op.OrderId == o.Id).ToList()
            }).ToListAsync();

            return View(model);
        }
    }
}
