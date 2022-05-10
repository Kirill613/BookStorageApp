using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStorageApp.Models;

namespace BookStorageApp.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.Include(x => x.TagsOfBook).ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                            .Include(x => x.TagsOfBook)
                            .Include(x => x.ChaptersOfBook)
                            .FirstOrDefaultAsync(m => m.Id == id);

            List<Tag> tempTagList = new List<Tag>();

            foreach (Tag _tag in book.TagsOfBook)
            {
                tempTagList.Add(_tag);
            }

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Info,ReleaseYear,AuthorName,ChapterNumber")] Book book, int[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                if (selectedCourses != null)
                {
                    foreach (var c in _context.Tags.Where(co => selectedCourses.Contains(co.Id)))
                    {
                        book.TagsOfBook.Add(c);
                    }
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(x => x.TagsOfBook).FirstOrDefaultAsync(m => m.Id == id);
            //var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.Tags = _context.Tags.ToList();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Info,ReleaseYear,AuthorName,ChapterNumber")] Book book, int[] selectedCourses)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var data = await _context.Books.Include(x => x.TagsOfBook).FirstOrDefaultAsync(m => m.Id == id);
                    data.TagsOfBook.Clear();
                    _context.Entry(data).State = EntityState.Detached;
                    _context.SaveChanges();

                    if (selectedCourses != null)
                    {
                        foreach (var c in _context.Tags.Where(co => selectedCourses.Contains(co.Id)))
                        {
                            book.TagsOfBook.Add(c);
                        }
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), nameof(Book), new { id = book.Id });
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(x => x.TagsOfBook)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
