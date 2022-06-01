using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStorageApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookStorageApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        /*        // GET: Comments/Create
                public IActionResult Create()
                {
                    return View();
                }
        */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,BookId")] Comment comment)
        {
            if (comment.Text != null)
            {
                User currentUser = await _userManager.GetUserAsync(User);

                string text = comment.Text;

                comment.Text = text;
                comment.UserId = currentUser.Id;
                comment.Id = Guid.NewGuid();

                _context.Comments.Add(comment);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), nameof(Book), new { id = comment.BookId, menuItemSelected = "Comments" });
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForChapter([Bind("Text,BookId,ChapterId")] Comment comment)
        {
            if (comment.Text!= null)
            {
                User currentUser = await _userManager.GetUserAsync(User);

                string text = comment.Text;

                comment.Text = text;
                comment.UserId = currentUser.Id;
                comment.Id = Guid.NewGuid();

                _context.Comments.Add(comment);

                await _context.SaveChangesAsync();
            }         

            return RedirectToAction(nameof(Details), nameof(Chapter), new { id = comment.ChapterId });
        }



        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), nameof(Book), new { id = comment.BookId, menuItemSelected = "Comments" });
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("DeleteForChapter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForChapter(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), nameof(Chapter), new { id = comment.ChapterId });
        }

        [HttpPost, ActionName("DeleteForProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForProfile(Guid id)
        {
            User currentUser = await _userManager.GetUserAsync(User);

            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Account", new { userName = currentUser.UserName });
        }

        private bool CommentExists(Guid id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
