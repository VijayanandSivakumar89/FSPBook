using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSPBook.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DateTimePosted {get;set;}
        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
