using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStorageApp.Models;
using BookStorageApp.ModelsView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BookStorageApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly AppDbContext _context;

        public BookController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString, string sortOrder, int[] selectedTags)
        {
            IQueryable<Book> books = _context.Books.Include(p => p.TagsOfBook);

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            var tagList = new List<Tag> { };
            foreach (var c in _context.Tags.Where(co => selectedTags.Contains(co.Id)))
            {
                tagList.Add(c);
            }

            foreach (var _tag in tagList)
            {
                books = books.Where(s => s.TagsOfBook.Contains(_tag));
            }

            switch (sortOrder)
            {
                case "Название":
                    books = books.OrderBy(s => s.Title);
                    break;
                case "Количество глав":
                    books = books.OrderBy(s => s.ChapterNumber);
                    break;
                case "Дата выхода":
                    books = books.OrderBy(s => s.ReleaseYear);
                    break;
                default:
                    books = books.OrderBy(s => s.Title);
                    break;
            }

            FilerViewModel filerViewModel = new FilerViewModel
            {
                Books = books,
                SortOrder = new SelectList(new List<string>()
                {
                    "Название",
                    "Дата выхода",
                    "Количество глав",
                }),
                SearchString = ""
            };
            ViewBag.asd = "asd";
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.CheckedTags = tagList;

            return View(filerViewModel);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id, string menuItemSelected = "Chapters")
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                            .Include(x => x.TagsOfBook)
                            .Include(x => x.ChaptersOfBook)
                            .Include(x => x.CommentsOfBook)
                            .FirstOrDefaultAsync(m => m.Id == id);


            for (int i = 0; i < book.CommentsOfBook.Count; i++)
            {
                book.CommentsOfBook.ToList()[i].Book = await _context.Books.FindAsync(book.CommentsOfBook.ToList()[i].BookId);
                book.CommentsOfBook.ToList()[i].Chapter = await _context.Chapters.FindAsync(book.CommentsOfBook.ToList()[i].ChapterId);
                book.CommentsOfBook.ToList()[i].User = await _context.Users.FindAsync(book.CommentsOfBook.ToList()[i].UserId);
            }


            List<Tag> tempTagList = new List<Tag>();

            foreach (Tag _tag in book.TagsOfBook)
            {
                tempTagList.Add(_tag);
            }

            if (book == null)
            {
                return NotFound();
            }
            ViewBag.Current = menuItemSelected;

            return View(book);
        }      

        // GET: Books/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Info,ReleaseYear,AuthorName,ChapterNumber,ImageFile")] Book book, int[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                if (book.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                    string extension = Path.GetExtension(book.ImageFile.FileName);
                    book.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }
                }             

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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Info,ReleaseYear,AuthorName,ChapterNumber,ImageFile")] Book book, int[] selectedCourses)
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
                    if (data.ImageName != null)
                    {
                        book.ImageName = data.ImageName;
                    }

                    if (book.ImageFile != null)
                    {
                        //delete prev
                        if (data.ImageName != null && data.ImageName != "NoImage.png")
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image", data.ImageName);
                            if (System.IO.File.Exists(imagePath))
                                System.IO.File.Delete(imagePath);
                        }
                        //Save image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                        string extension = Path.GetExtension(book.ImageFile.FileName);
                        book.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await book.ImageFile.CopyToAsync(fileStream);
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book.ImageName != "NoImage.png")
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", book.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }
          

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Filter(string sortOrder)
        {
            var books = from s in _context.Books
                        select s;


            switch (sortOrder)
            {
                case "Название":
                    books = books.OrderBy(s => s.Title);
                    break;
                case "Количество глав":
                    books = books.OrderBy(s => s.ChapterNumber);
                    break;
                case "Дата выхода":
                    books = books.OrderBy(s => s.ReleaseYear);
                    break;
                default:
                    books = books.OrderBy(s => s.Title);
                    break;
            }
            return RedirectToAction(nameof(Index), nameof(Book), await books.ToListAsync());
        }
    }
}
