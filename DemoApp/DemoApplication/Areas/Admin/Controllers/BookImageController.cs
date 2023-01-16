using DemoApplication.Areas.Admin.ViewModels.BookImage;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/books")]
    [Authorize(Roles = "admin")]
    public class BookImageController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IFileService _fileService;

        public BookImageController(
            DataContext dataContext,
            IFileService fileService)
        {
            _dataContext = dataContext;
            _fileService = fileService;
        }

        [HttpGet("{bookId}/image/list", Name = "admin-book-image-list")]
        public async Task<IActionResult> ListAsync([FromRoute] int bookId)
        {
            var book = await _dataContext.Books.Include(b => b.Images).FirstOrDefaultAsync();

            if (book == null) return NotFound();

            var model = new BookImagesViewModel
            {
                BookName = book.Title,
                BookId = book.Id,

            };

            model.Images = book.Images.Select(bi => new BookImagesViewModel.ListItem
            {
                Id = bi.Id,
                ImageUrL = _fileService.GetFileUrl(bi.ImageNameInFileSystem, UploadDirectory.Book)
            }).ToList();

            return View(model);
        }

        [HttpGet("{bookId}/image/add", Name = "admin-book-image-add")]
        public async Task<IActionResult> AddAsync()
        {
            return View(new AddViewModel());
        }

        [HttpPost("{bookId}/image/add", Name = "admin-book-image-add")]
        public async Task<IActionResult> AddAsync([FromRoute] int bookId, AddViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var book = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null) return NotFound();

            var bookImageNameInFileSystem = await _fileService.UploadAsync(model.Image, UploadDirectory.Book);

            AddBookImage(model.Image.FileName, bookImageNameInFileSystem);

            await _dataContext.SaveChangesAsync();


            void AddBookImage(string bookImageName, string bookImageNameInFileSystem)
            {
                var bookImage = new Image
                {
                    ImageName = bookImageName,
                    ImageNameInFileSystem = bookImageNameInFileSystem,
                    Book = book,
                    Order = model.Order
                };

                _dataContext.AddAsync(bookImage);
            }

            return RedirectToRoute("admin-book-image-list", new { bookId = book.Id });
        }

        [HttpGet("{bookId}/image/{bookImageId}/update", Name = "admin-book-image-update")]
        public async Task<IActionResult> UpdateAsyn([FromRoute] int bookId, int bookImageId)
        {
            var bookImage = await _dataContext.Images
                .FirstOrDefaultAsync(bi => bi.Id == bookImageId && bi.BookId == bookId);

            var model = new UpdateViewModel
            {
                BookId = bookImage.BookId,
                BookImageId = bookImage.Id,
                ImageUrl = _fileService.GetFileUrl(bookImage.ImageName, UploadDirectory.Book),

            };

            return View(model);
        }

        [HttpPost("{bookId}/image/{bookImageId}/update", Name = "admin-book-image-update")]
        public async Task<IActionResult> UpdateAsyn([FromRoute] int bookId, int bookImageId, UpdateViewModel model)
        {

            if (!ModelState.IsValid)
            {
                GetView(model);
            }
            var bookImage = await _dataContext.Images
                  .FirstOrDefaultAsync(bi => bi.Id == bookImageId && bi.BookId == bookId);


            if(model.Image is not null)
            {
                await _fileService.DeleteAsync(bookImage.ImageNameInFileSystem, UploadDirectory.Book);
                var imageFileNameInSystem = await _fileService.UploadAsync(model.Image, UploadDirectory.Book);
                await UpdateBookImageAsync(model.Image.FileName, imageFileNameInSystem);
            }
            
            UpdateBook();
            



            async Task UpdateBookImageAsync(string bookImageName, string bookImageNameInFileSystem)
            {
                bookImage.ImageName = bookImageName;
                bookImage.ImageNameInFileSystem = bookImageNameInFileSystem;
                bookImage.Order = model.Order;

                _dataContext.SaveChangesAsync();
            };

            void UpdateBook()
            {
                bookImage.Order = model.Order;
            }

            IActionResult GetView(UpdateViewModel model)
            {
                
                return View(model);
            }

            return RedirectToRoute("admin-book-image-list");
        }



        [HttpPost("{bookId}/image/{bookImageId}/delete", Name = "admin-book-image-delete")]
        public async Task<IActionResult> DeleteAsync(int bookId, int bookImageId)
        {
            var bookImage = await _dataContext.Images
                .FirstOrDefaultAsync(bi => bi.Id == bookImageId && bi.BookId == bookId);
            if (bookImage is null)
            {
                return NotFound();
            }

            await _fileService.DeleteAsync(bookImage.ImageNameInFileSystem, UploadDirectory.Book);

            _dataContext.Images.Remove(bookImage);
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-book-image-list", new { bookId = bookId });
        }

    }
}
