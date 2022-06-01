using BookStorageApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db;
        private readonly UserManager<User> _userManager;
        public HomeController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

 /*       public async Task<IActionResult> HeaderPartial()
        {
            await _userManager.FindByNameAsync
                
            return PartialView("HeaderPartial", );
        }  */     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
