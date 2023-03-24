using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniBBS.DB;
using MiniBBS.Models;
using MiniBBS.Service;
using System.Diagnostics;

namespace MiniBBS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IForumService _forumService;
        private readonly IPostService _postService;
        private readonly SignInManager<User> _signInManager;

        public HomeController(IForumService forumService, IPostService postService, SignInManager<User> signInManager)
        {
            _forumService = forumService;
            _postService = postService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(int forumId = 1)
        {
            var forums = await _forumService.GetAllForumsAsync();
            var selectedForum = await _forumService.GetForumByIdAsync(forumId);
            var posts = await _postService.GetPostsByForumIdAsync(forumId);

            var postViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                postViewModels.Add(new PostViewModel
                {
                    PostID = post.PostID,
                    Title = post.Title,
                    PostedTime = post.PostedTime,
                    Username = post.User.UserName,
                    CommentCount = post.Comments.Count
                });
            }

            var model = new IndexViewModel
            {
                Forums = forums,
                SelectedForum = selectedForum,
                Posts = postViewModels,
                IsUserLoggedIn = _signInManager.IsSignedIn(User)
            };


            return View(model);
        }
    }
}