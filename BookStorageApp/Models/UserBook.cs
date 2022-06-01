using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorageApp.Models
{
    public class UserBook
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public int BookId { get; set; }
    }
}
