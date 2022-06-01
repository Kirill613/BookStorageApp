using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Models
{
    public class Chapter
    {
        [Display(Name = "Id главы")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Книга")]
        public Book Book { get; set; }
        [Required]
        [Display(Name = "Номер главы")]
        public int ChapterNumber { get; set; }
        [Required]
        [Display(Name = "Номер тома")]
        public int VolumeNumber { get; set; }
        [Required]
        [Display(Name = "Содержание")]
        public string Text { get; set; }
        [Required]
        [Display(Name = "Название главы")]
        public string Name { get; set; }
        public ICollection<Comment> CommentOfChapter { get; set; }
        public Chapter()
        {
            CommentOfChapter = new HashSet<Comment>();
        }
    }
}
