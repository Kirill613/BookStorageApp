using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BookStorageApp.Models;
using BookStorageApp.ModelsView;
using BookStorageApp.Controllers;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace BookStorageApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, NickName = model.NickName, ImageName = "NoImage.png" };

                if (model.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    model.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    user.ImageName = model.ImageName;
                }

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    //if (await _roleManager.RoleExistsAsync("user"))
                    //{
                    await _userManager.AddToRoleAsync(user, "user");
                    //}


                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction(nameof(Index), nameof(Book));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), nameof(Book));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), nameof(Book));
        }

        public async Task<IActionResult> Profile(string userName)
        {

            User currentUser = await _userManager.GetUserAsync(User);
            User user = await _userManager.FindByNameAsync(userName);

            var list = _context.UserBooks                           //G
                .Where(b => b.UserId == user.Id)
                .Select(b => b.BookId);


            var listBooks = _context.Books
                .Where(b => list.Contains(b.Id))
                .ToList();

            var listComments = _context.Comments                     //G
                .Where(b => b.UserId == user.Id)
                .ToList();

            for (int i = 0; i < listComments.Count; i++)
            {
                listComments[i].Book = await _context.Books.FindAsync(listComments[i].BookId);
                listComments[i].Chapter = await _context.Chapters.FindAsync(listComments[i].ChapterId);
                listComments[i].User = await _context.Users.FindAsync(listComments[i].UserId);
            }

            if (user != null)
            {
                ProfileViewModel model = new ProfileViewModel
                {
                    Name = user.UserName,
                    Email = user.Email,
                    ImageName = user.ImageName,
                    NickName = user.NickName,
                    UserBooks = listBooks,
                    UserComments = listComments,

                };

                ViewBag.PermitChanges = false;

                if (currentUser != null)
                {
                    if (user.Id == currentUser.Id)
                    {
                        ViewBag.PermitChanges = true;
                    }
                }

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            User currentUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (model.ImageFile != null)
                {
                    //Save image to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    model.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    //Insert record
                    currentUser.ImageName = model.ImageName;
                }
                currentUser.NickName = model.NickName;

                _context.Users.Update(currentUser);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Profile), new { userName = currentUser.UserName });

        }
    }
}

