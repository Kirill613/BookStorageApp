using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Models
{
    public class Tag
    {
        [Display(Name = "TagId")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название тэга")]
        public string Name { get; set; } 
        public virtual ICollection<Book> BooksOfTag { get; set; }
        public Tag()
        {
            BooksOfTag = new HashSet<Book>();
        }
    }
}
