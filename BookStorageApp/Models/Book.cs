using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Models
{
    public class Book
    {
        [Display(Name = "BookId")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название книги")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Краткое описание")]
        public string Info { get; set; }


        [Column(TypeName = "nvarchar(50)")]
        public string ImageTitle { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Image Name")]
        public string ImageName { get; set; }
        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile ImageFile { get; set; }


        [Display(Name = "Год релиза")]
        public int? ReleaseYear { get; set; }
        [Display(Name = "Имя автора")]
        public string AuthorName { get; set; }
        [Display(Name = "Количество глав")]
        public int? ChapterNumber { get; set; }
        [Display(Name = "Тэги")]
        public virtual ICollection<Tag> TagsOfBook { get; set; }
        [Display(Name = "Главы")]
        public ICollection<Chapter> ChaptersOfBook { get; set; }
        public ICollection<Comment> CommentsOfBook { get; set; }
        public Book()
        {
            TagsOfBook = new HashSet<Tag>();
            ChaptersOfBook = new HashSet<Chapter>();
            CommentsOfBook = new HashSet<Comment>();
        }
    }
}
