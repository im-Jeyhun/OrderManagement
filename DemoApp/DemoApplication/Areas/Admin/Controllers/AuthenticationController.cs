﻿using DemoApplication.Areas.Admin.ViewModels.Authentication;
using DemoApplication.Contracts.Identity;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/auth")]
    public class AuthenticationController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IUserService _userService;

        public AuthenticationController(DataContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        #region Login and Logout

        [HttpGet("login", Name = "admin-auth-login")]
        public async Task<IActionResult> LoginAsync()
        {
           
            return View(new LoginViewModel());
        }

        [HttpPost("login", Name = "admin-auth-login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel? model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _userService.CheckPasswordAsync(model!.Email, model!.Password))
            {
                ModelState.AddModelError(String.Empty, "Email or password is not correct");
                return View(model);
            }

            if (await _dbContext.Users.AnyAsync(u => u.Email == model.Email && u.Role.Name == RoleNames.ADMIN))
            {
                await _userService.SignInAsync(model!.Email, model!.Password, RoleNames.ADMIN);
                return RedirectToRoute("admin-author-list");
            }

            if (!await _dbContext.Users.AnyAsync(u => u.Role.Name == RoleNames.ADMIN))
            {
                ModelState.AddModelError(String.Empty, "You are not admin");
                return View(model);
            }

            await _userService.SignInAsync(model.Email, model.Password, RoleNames.ADMIN);

            return RedirectToRoute("admin-author-list");
        }

        [HttpGet("logout", Name = "admin-auth-logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _userService.SignOutAsync();

            return RedirectToRoute("admin-auth-login");
        }

        #endregion
    }
}
