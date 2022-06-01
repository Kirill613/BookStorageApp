using BookStorageApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.ModelsView
{
    public class FilerViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public SelectList SortOrder { get; set; }
        public string SearchString { get; set; }
    }
}
