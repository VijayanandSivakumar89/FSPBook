using FSPBook.Data.Entities;
using FSPBook.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSPBook.Web.Models
{
    public class ProfileViewModel
    {
        public Profile Profile { get; set; }
        public List<Post> Posts { get; set; }

        public List<Article> Articles { get; set; }
    }
}
