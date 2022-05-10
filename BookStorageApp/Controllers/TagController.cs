using BookStorageApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Controllers
{
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.BookTag.ToListAsync());
        //}
    }
}
