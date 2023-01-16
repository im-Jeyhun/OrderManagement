using DemoApplication.Database.Models;

namespace DemoApplication.Areas.Admin.ViewModels.Order
{
    public class OrderListItemViewModel
    {
        public string Id { get; set; }

        public string Client { get; set; }
        public DateTime OrderTime { get; set; }

        public string OrderStatus { get; set; }

        public decimal OrderSum { get; set; }

    }
}
