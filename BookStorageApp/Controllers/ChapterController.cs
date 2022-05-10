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
    public class ChapterController : Controller
    {
        private readonly AppDbContext _context;

        public ChapterController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Chapters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chapters.ToListAsync());
        }

        // GET: Chapters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.Include(x => x.Book)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        //[HttpPost]
        //public IActionResult ChangeText(int id, int fontSize, int textColor, int backgroundColor)
        //{
        //    ViewBag.FontSize = fontSize;
        //    ViewBag.TextColor = textColor;
        //    ViewBag.BackgroundColor = backgroundColor;

        //    return RedirectToAction(nameof(Details), nameof(Chapter), new { id = id });
        //}

        public async Task<IActionResult> NextChapter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters
                                .Include(x => x.Book)
                                .ThenInclude(x => x.ChaptersOfBook)
                                .FirstOrDefaultAsync(m => m.Id == id);

            var chapterIds = chapter.Book.ChaptersOfBook
                                    .OrderBy(chapter => chapter.VolumeNumber)
                                    .ThenBy(chapter => chapter.ChapterNumber)
                                    .Select(chapter => chapter.Id)
                                    .ToArray();

            for (int i = 0; i < chapterIds.Length; i++)
            {
                if (chapterIds[i] == id)
                {
                    if (i + 1 < chapterIds.Length)
                        return RedirectToAction(nameof(Details), nameof(Chapter), new { id = chapterIds[i + 1] });
                    if (i + 1 == chapterIds.Length)
                        return RedirectToAction(nameof(Details), nameof(Book), new { id = chapter.Book.Id });
                }
            }
            return RedirectToAction(nameof(Details), nameof(Chapter), new { id = id});
        }

        public async Task<IActionResult> PreviousChapter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters
                                .Include(x => x.Book)
                                .ThenInclude(x => x.ChaptersOfBook)
                                .FirstOrDefaultAsync(m => m.Id == id);

            var chapterIds = chapter.Book.ChaptersOfBook
                                    .OrderBy(chapter => chapter.VolumeNumber)
                                    .ThenBy(chapter => chapter.ChapterNumber)
                                    .Select(chapter => chapter.Id)
                                    .ToArray();

            for (int i = 0; i < chapterIds.Length; i++)
            {
                if (chapterIds[i] == id)
                {
                    if (i - 1 >= 0)
                        return RedirectToAction(nameof(Details), nameof(Chapter), new { id = chapterIds[i - 1] });
                    else
                        return RedirectToAction(nameof(Details), nameof(Book), new { id = chapter.Book.Id });                 
                }
            }
            return RedirectToAction(nameof(Details), nameof(Chapter), new { id = id });
        }

        // GET: Chapters/Create

        public IActionResult Create(int id)
        {
            ViewBag.BookId = id;
            return View();
        }

        // POST: Chapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChapterNumber,VolumeNumber,Text,Name")] Chapter chapter, int bookId)
        {
            if (ModelState.ErrorCount <= 1)
            {
                chapter.Book = await _context.Books.FindAsync(bookId);
                _context.Add(chapter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), nameof(Book), new { id = bookId });
            }
            return View(chapter);
        }

        // GET: Chapters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.Include(x => x.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        // POST: Chapters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,ChapterNumber,VolumeNumber,Text,Name")] Chapter chapter, int bookId)
        {
            if (ModelState.ErrorCount <= 1)
            {
                try
                {
                    chapter.Book = await _context.Books.FindAsync(bookId);
                    _context.Update(chapter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterExists(chapter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), nameof(Book), new { id = chapter.Book.Id });
            }
            return View(chapter);
        }

        // GET: Chapters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.Include(x => x.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chapter = await _context.Chapters.Include(x => x.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), nameof(Book), new { id = chapter.Book.Id });
        }

        private bool ChapterExists(int id)
        {
            return _context.Chapters.Any(e => e.Id == id);
        }
    }
}
