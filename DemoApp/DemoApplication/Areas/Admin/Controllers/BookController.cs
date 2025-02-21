﻿using DemoApplication.Areas.Admin.ViewModels.Book;
using DemoApplication.Areas.Admin.ViewModels.Book.Add;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/book")]
    [Authorize(Roles = "admin")]
    public class BookController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<BookController> _logger;
        private readonly IFileService _fileService;

        public BookController(DataContext dataContext, ILogger<BookController> logger, IFileService fileService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _fileService = fileService;
        }


        #region List

        [HttpGet("list", Name = "admin-book-list")]
        public async Task<IActionResult> ListAsync()
        {
            var model = await _dataContext.Books
                .Select(b => new ListItemViewModel(
                        b.Id,
                        b.Title,
                        b.Price,
                        $"{b.Author.FirstName} {b.Author.LastName}",
                        b.CreatedAt,
                        b.BookCategories
                            .Select(bc => bc.Category)
                                .Select(c => new ListItemViewModel.CategoryViewModeL(c.Title, c.Parent.Title)).ToList()))
                .ToListAsync();

            return View(model);
        }

        #endregion

        #region Add

        [HttpGet("add", Name = "admin-book-add")]
        public IActionResult Add()
        {
            var model = new AddViewModel
            {
                Authors = _dataContext.Authors
                    .Select(a => new AuthorListItemViewModel(a.Id, $"{a.FirstName} {a.LastName}"))
                    .ToList(),
                Categories = _dataContext.Categories
                    .Select(c => new CategoryListItemViewModel(c.Id, c.Title))
                    .ToList(),
            };

            return View(model);
        }

        [HttpPost("add", Name = "admin-book-add")]
        public async Task<IActionResult> Add(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return GetView(model);
            }

            if (!_dataContext.Authors.Any(a => a.Id == model.AuthorId))
            {
                ModelState.AddModelError(string.Empty, "Author is not found");
                return GetView(model);
            }

            foreach (var categoryId in model.CategoryIds)
            {
                if (!_dataContext.Categories.Any(c => c.Id == categoryId))
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong");
                    _logger.LogWarning($"Category with id({categoryId}) not found in db ");
                    return GetView(model);
                }

            }

            //uplaod file to os

            var book = await AddBookAsync();
            await AddBookImage();
            await _dataContext.SaveChangesAsync();


            return RedirectToRoute("admin-book-list");




            IActionResult GetView(AddViewModel model)
            {
                model.Authors = _dataContext.Authors
                    .Select(a => new AuthorListItemViewModel(a.Id, $"{a.FirstName} {a.LastName}"))
                    .ToList();

                model.Categories = _dataContext.Categories
                   .Select(c => new CategoryListItemViewModel(c.Id, c.Title))
                   .ToList();

                return View(model);
            }

            async Task<Book> AddBookAsync()
            {
                var book = new Book
                {
                    Title = model.Title,
                    Price = model.Price,
                    CreatedAt = DateTime.Now,
                    AuthorId = model.AuthorId,

                };

                await _dataContext.Books.AddAsync(book);

                foreach (var categoryId in model.CategoryIds)
                {
                    var bookCategory = new BookCategory
                    {
                        CategoryId = categoryId,
                        Book = book,
                    };

                   await _dataContext.BookCategories.AddAsync(bookCategory);
                }

                return book;

            }

            async Task AddBookImage()
            {
                foreach (IFormFile imageName in model.Images)
                {
                    var imageNameInSystem = await _fileService.UploadAsync(imageName, UploadDirectory.Book);

                    var image = new Image
                    {
                        ImageName = imageName.FileName,
                        ImageNameInFileSystem = imageNameInSystem,
                        Book = book
                    };
                   await _dataContext.Images.AddAsync(image);
                }
            }
        }


        #endregion

       

        [HttpGet("update/{id}", Name = "admin-book-update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id)
        {
            var book = await _dataContext.Books.Include(b => b.BookCategories).FirstOrDefaultAsync(b => b.Id == id);
            if (book is null)
            {
                return NotFound();
            }


            var model = new UpdateViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                AuthorId = book.AuthorId,
                Authors = _dataContext.Authors
                    .Select(a => new AuthorListItemViewModel(a.Id, $"{a.FirstName} {a.LastName}"))
                    .ToList(),
                Categories = _dataContext.Categories
                    .Select(c => new CategoryListItemViewModel(c.Id, c.Title))
                    .ToList(),
                CategoryIds = book.BookCategories.Select(bc => bc.CategoryId).ToList(),

                //ImageUrl = _fileService.GetFileUrl(book.Images.)

            };

            return View(model);
        }

        //[HttpPost("update/{id}", Name = "admin-book-update")]
        //public async Task<IActionResult> UpdateAsync(AddViewModel model)
        //{
        //    var book = await _dataContext.Books.Include(b => b.BookCategories).FirstOrDefaultAsync(b => b.Id == model.Id);
        //    if (book is null)
        //    {
        //        return NotFound();
        //    }

        //    if(model.Image is null)
        //    {

        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return GetView(model);
        //    }

        //    if (!_dataContext.Authors.Any(a => a.Id == model.AuthorId))
        //    {
        //        ModelState.AddModelError(string.Empty, "Author is not found");
        //        return GetView(model);
        //    }

        //    foreach (var categoryId in model.CategoryIds)
        //    {
        //        if (!_dataContext.Categories.Any(c => c.Id == categoryId))
        //        {
        //            ModelState.AddModelError(string.Empty, "Something went wrong");
        //            _logger.LogWarning($"Category with id({categoryId}) not found in db ");
        //            return GetView(model);
        //        }

        //    }

        //    await _fileService.DeleteAsync(book.ImageNameInFileSystem, UploadDirectory.Book);
        //    var imageFileNameInSystem = await _fileService.UploadAsync(model.Image, UploadDirectory.Book);

        //    await UpdateBookAsync(model.Image.FileName, imageFileNameInSystem);

        //    return RedirectToRoute("admin-book-list");




        //    IActionResult GetView(AddViewModel model)
        //    {
        //        model.Authors = _dataContext.Authors
        //            .Select(a => new AuthorListItemViewModel(a.Id, $"{a.FirstName} {a.LastName}"))
        //            .ToList();

        //        model.Categories = _dataContext.Categories
        //           .Select(c => new CategoryListItemViewModel(c.Id, c.Title))
        //           .ToList();

        //        model.CategoryIds = book.BookCategories.Select(bc => bc.CategoryId).ToList();
        //        model.ImageUrl = _fileService.GetFileUrl(book.ImageNameInFileSystem, UploadDirectory.Book);

        //        return View(model);

        //    }

        //    async Task UpdateBookAsync(string imageName, string imageNameInFileSystem)
        //    {
        //        book.Title = model.Title;
        //        book.AuthorId = model.AuthorId;
        //        book.Price = model.Price;
        //        book.UpdatedAt = DateTime.Now;
        //        book.ImageName = imageName;
        //        book.ImageNameInFileSystem = imageNameInFileSystem;

        //        var categoriesInDb = book.BookCategories.Select(bc => bc.CategoryId).ToList();
        //        var categoriesToRemove = categoriesInDb.Except(model.CategoryIds).ToList();
        //        var categoriesToAdd = model.CategoryIds.Except(categoriesInDb).ToList();

        //        book.BookCategories.RemoveAll(bc => categoriesToRemove.Contains(bc.CategoryId));

        //        foreach (var categoryId in categoriesToAdd)
        //        {
        //            var bookCategory = new BookCategory
        //            {
        //                CategoryId = categoryId,
        //                Book = book,
        //            };

        //            _dataContext.BookCategories.Add(bookCategory);
        //        }

        //        _dataContext.SaveChanges();
        //    }
        //}

        //#endregion

        //#region Delete

        //[HttpPost("delete/{id}", Name = "admin-book-delete")]
        //public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        //{
        //    var book = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        //    if (book is null)
        //    {
        //        return NotFound();
        //    }

        //    await _fileService.DeleteAsync(book.ImageNameInFileSystem, UploadDirectory.Book);

        //    _dataContext.Books.Remove(book);
        //    await _dataContext.SaveChangesAsync();

        //    return RedirectToRoute("admin-book-list");
        //}

        //#endregion
    }
}
