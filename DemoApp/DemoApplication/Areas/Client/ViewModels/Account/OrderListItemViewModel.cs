using DemoApplication.Database.Models;

namespace DemoApplication.Areas.Client.ViewModels.Account
{
    public class OrderListItemViewModel
    {
        public string Id { get; set; }

        public DateTime OrderTime { get; set; }

        public string OrderStatus { get; set; }

        public decimal OrderSum { get; set; }

        public List<OrderProduct> OrderProducts { get; set; }
    }
}
