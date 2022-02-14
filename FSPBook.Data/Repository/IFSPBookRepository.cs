using FSPBook.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSPBook.Data.Repository
{
    /// <summary>
    /// Repository for Retrieving, Creating posts
    /// </summary>
    public interface IFSPBookRepository
    {
        List<Profile> GetProfiles();

        List<Post> GetPosts();

        Post CreatePost(Post post);

        List<Post> GetProfilePostsById(int id);
    }
}
