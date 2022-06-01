using BookStorageApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.ModelsView
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public List<Book> UserBooks { get; set; }
        public List<Comment> UserComments { get; set; }
        public int Id { get; set; } //ерунда
        public string ImageName { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }   
    }
}
