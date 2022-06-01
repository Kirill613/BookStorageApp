using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Models
{
    public class Comment
    {
        [Required]
        [Display(Name = "Id комментария")]
        public Guid Id { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст комментария")]
        public string Text { get; set; }
        [Required]
        [Display(Name = "Id аккаунта")]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Дата и время создания")]
        public DateTime DateTime { get; set; }

        [Display(Name = "Id родителя комментария")]
        public int? ParentId { get; set; }
        [Required]
        [Display(Name = "Id связанной книги")]
        public int BookId { get; set; }
        [Display(Name = "Id связанной главы")]
        public int? ChapterId { get; set; }
        [NotMapped]
        public Chapter Chapter { get; set; }
        [NotMapped]
        public Book Book { get; set; }
        [NotMapped]
        public User User { get; set; }

    }
}
