using DemoApplication.Database;
using DemoApplication.Services.Abstracts;

namespace DemoApplication.Services.Concretes
{
    public class OrderService : IOrderService
    {
        private const int MIN_RANDOM_NUMBER = 10000;
        private const int MAX_RANDOM_NUMBER = 100000;
        private const string PREFIX = "OR";

        private readonly DataContext _dataContext;

        public OrderService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private string GenerateNumber()
        {
            Random randomId = new Random();
            return randomId.Next(MIN_RANDOM_NUMBER, MAX_RANDOM_NUMBER).ToString();
        }

        public string GenerateId()
        {
            var Id = $"{PREFIX}{GenerateNumber()}";

            while(_dataContext.Orders.Any(o => o.Id == Id))
            {
                Id = $"{PREFIX}{GenerateNumber()}";
            }

            return Id;

        }
    }
}
