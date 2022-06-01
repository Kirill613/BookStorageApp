using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStorageApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;


namespace BookStorageApp.Controllers
{
    public class UserBooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserBooksController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserBooks
        public async Task<IActionResult> GetAllUserBooks()
        {
            return View(await _context.UserBooks.ToListAsync());
        }

        // GET: UserBooks/Details/5
        public async Task<IActionResult> AddBookToUser(int bookId, string returnUrl)
        {
            User currentUser = await _userManager.GetUserAsync(User);

            if(_context.UserBooks.Where(i => (i.UserId == currentUser.Id) && (i.BookId == bookId)).Count() == 0)
            {
                UserBook userBook = new UserBook { BookId = bookId, UserId = currentUser.Id, Id = Guid.NewGuid() };

                await _context.UserBooks.AddAsync(userBook);
                await _context.SaveChangesAsync();
            }

            return Redirect(returnUrl);
            //return RedirectToAction(nameof(Index), nameof(Book));
        }

        public async Task<IActionResult> RemoveBookToUser(int bookId, string returnUrl)
        {
            User currentUser = await _userManager.GetUserAsync(User);

            var entity = await _context.UserBooks.FirstOrDefaultAsync(x => (x.BookId == bookId) && (x.UserId == currentUser.Id));
            _context.UserBooks.Remove(entity);
            await _context.SaveChangesAsync();

            return Redirect(returnUrl);
         //   return RedirectToAction(nameof(Index), nameof(Book));
        }
    }
}
