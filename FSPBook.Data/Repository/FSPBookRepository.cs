using FSPBook.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSPBook.Data.Repository
{
    public class FSPBookRepository : IFSPBookRepository
    {

        private readonly Context _context;

        public FSPBookRepository(Context dbContext)
        {
            _context = dbContext;
        }

        /// <summary>
        /// Method to save a new post to FSP DB
        /// </summary>
        /// <param name="post"></param>
        /// <returns>Saved Post</returns>
        public Post CreatePost(Post post)
        {
            _context.Post.Add(post);
            _context.SaveChangesAsync();
            return post;
        }
        
        /// <summary>
        /// Method to retrieve all posts from the DB
        /// </summary>
        /// <returns>List of posts including its author details</returns>
        public List<Post> GetPosts()
        {
            var posts = _context.Post.ToList();
            foreach(var post in posts)
            {
                post.Profile = _context.Profile.Where(x => x.Id == post.ProfileId).FirstOrDefault();
            }
            return posts.OrderByDescending(x => x.DateTimePosted).ToList();
        }

        /// <summary>
        /// Method to retrieve all author details from DB
        /// </summary>
        /// <returns>list of author information</returns>
        public List<Profile> GetProfiles()
        {
            return _context.Profile.ToList();
        }

        /// <summary>
        /// Method to retrieve information about a author based on their Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of posts created by the author with author details</returns>
        public List<Post>  GetProfilePostsById(int id)
        {
            var profile = _context.Profile.Where(x => x.Id == id).FirstOrDefault();
            var posts = _context.Post.Where(x => x.ProfileId == profile.Id).OrderByDescending(d => d.DateTimePosted).ToList();
            foreach(var post in posts)
            {
                post.Profile = profile;
            }
            return posts;
        }
    }
}
