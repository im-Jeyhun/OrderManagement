namespace DemoApplication.Areas.Admin.ViewModels.BookImage
{
    public class UpdateViewModel
    {
        public int BookId { get; set; }
        public int BookImageId { get; set; }

        public string ImageUrl { get; set; }
        public int Order { get; set; }
        public IFormFile? Image { get; set; }
    }
}
