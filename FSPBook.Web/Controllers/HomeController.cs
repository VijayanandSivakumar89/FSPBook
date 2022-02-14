using FSPBook.Data.Entities;
using FSPBook.Data.Repository;
using FSPBook.Web.Models;
using FSPBook.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSPBook.Web.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IFSPBookRepository _repository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Controller constructor injected with the repository and configuration objects instanciated in middleware
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="bookRepository"></param>
        public HomeController(IConfiguration configuration, IFSPBookRepository bookRepository )
        {
                  
            _configuration = configuration;
            _repository = bookRepository;
        }
       
        /// <summary>
        /// Landing Page of the site to display the latest posts from authors along with Technical News
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            var articles = new List<Article>();
            articles = await GetTechnologyNewsAsync(); //Call to get latest Tech news from News API
            var fakeArticle = new List<Article>() { new Article() { Title = "FakeNews", Url = "https://ichef.bbci.co.uk/news/1024/branded_news/83B3/production/_115651733_breaking-large-promo-nc.png" } };
            articles = (articles.Count > 0) ? articles.Take(5).ToList() : fakeArticle;
            var viewModel = new HomeViewModel()
            {
                Posts = _repository.GetPosts(),
                Articles = articles
            };
            HttpContext.Session.SetObject("Articles", viewModel.Articles);
            return View(viewModel);
        }

       /// <summary>
       /// Screen to create new posts
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        public IActionResult CreatePost()
        {
            var viewModel = new CreatePostViewModel();
            viewModel.Profiles = _repository.GetProfiles();
            return View(viewModel);
        }

        /// <summary>
        /// Saves the new posts to DB
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreatePost(CreatePostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var post = new Post()
            {
                ProfileId = viewModel.ProfileId,
                Content = viewModel.ContentInput,
                DateTimePosted = DateTimeOffset.Now
            };
            var result = _repository.CreatePost(post);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Screen to view individual author profile and their posts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Profile(int id)
        {
            var viewModel = new ProfileViewModel();
            viewModel.Posts = _repository.GetProfilePostsById(id);
            viewModel.Articles = HttpContext.Session.GetObject<List<Article>>("Articles");
            viewModel.Profile = viewModel.Posts[0].Profile;
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// News API call to fetch latest Technology news
        /// </summary>
        /// <returns></returns>
        public async Task<List<Article>> GetTechnologyNewsAsync()
        {
            var newsArticles = new List<Article>();
            try
            {

                using (var httpClient = new HttpClient())
                {
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, _configuration["NewsAPIURL"] + "top-headlines" + "?" + "country=us&category=technology");

                    httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration["NewsAPIKey"]);

                    var httpResponse = await httpClient.SendAsync(httpRequest);

                    var json = await httpResponse.Content?.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        // convert the json to an obj
                        dynamic apiResponse = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

                        foreach (var item in (IEnumerable<dynamic>)apiResponse.articles)
                        {
                            var article = new Article()
                            {
                                Title = item.title,
                                Url = item.urlToImage
                            };
                            newsArticles.Add(article);
                        }

                    }
                    return newsArticles;
                }
            }
            catch (Exception e)
            {
                return newsArticles;
            }
                
            
        }
    }
}
