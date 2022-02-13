using FSPBook.Data;
using FSPBook.Data.Entities;
using FSPBook.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FSPBook.Web.Models
{
    public class CreatePostViewModel
    {

        public List<Profile> Profiles { get; set; }

        public bool Success { get; set; }

        [BindProperty]
        [Required(ErrorMessage = ErrorMessages.RequriedAuthor)]
        [Range(1, 10000, ErrorMessage = ErrorMessages.RequriedAuthor)]
        public int ProfileId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = ErrorMessages.RequriedContent)]
        [MinLength(1, ErrorMessage = ErrorMessages.MinimumContent)]
        public string ContentInput { get; set; }

        
    }
}
