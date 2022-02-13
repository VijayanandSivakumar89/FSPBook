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

        public HomeController(IConfiguration configuration, IFSPBookRepository bookRepository )
        {
                  
            _configuration = configuration;
            _repository = bookRepository;
        }
       

        public async Task<ActionResult> Index()
        {
            var articles = await GetTechnologyNewsAsync();
            var viewModel = new HomeViewModel()
            {
                Posts = _repository.GetPosts(),
                Articles = articles.Take(5).ToList()
            };
            HttpContext.Session.SetObject("Articles", viewModel.Articles);
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult CreatePost()
        {
            var viewModel = new CreatePostViewModel();
            viewModel.Profiles = _repository.GetProfiles();
            return View(viewModel);
        }

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

        [HttpGet]
        public IActionResult Profile(int id)
        {
            var viewModel = new ProfileViewModel();
            viewModel.Posts = _repository.GetProfilePostsById(id);
            viewModel.Articles = HttpContext.Session.GetObject<List<Article>>("Articles");
            viewModel.Profile = viewModel.Posts[0].Profile;
            return View(viewModel);
        }

        public async Task<List<Article>> GetTechnologyNewsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, _configuration["NewsAPIURL"] + "top-headlines" + "?" + "country=us&category=technology");

                httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration["NewsAPIKey"]);

                var httpResponse = await httpClient.SendAsync(httpRequest);

                var json = await httpResponse.Content?.ReadAsStringAsync();
                var newsArticles = new List<Article>();
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
    }
}
