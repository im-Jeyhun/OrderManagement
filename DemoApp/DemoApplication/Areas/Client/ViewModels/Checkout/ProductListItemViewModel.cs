namespace DemoApplication.Areas.Client.ViewModels.Checkout
{
    public class ProductListItemViewModel
    {

        public int BookId { get; set; }

        public List<ListItem>? Products { get; set; }

        public class ListItem
        {

            public ListItem(int id , string title, int quantity, decimal price, decimal total)
            {
                Title = title;
                Quantity = quantity;
                Price = price;
                Total = total;
                Id = id;
            }

            public int Id { get; set; }
            public string? Title { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
        }

    }
}
