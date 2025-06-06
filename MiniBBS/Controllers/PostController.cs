using Microsoft.AspNetCore.Mvc;
using MiniBBS.DB;
using MiniBBS.Models;
using MiniBBS.Service;
using System.Security.Claims;

namespace MiniBBS.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IForumService _forumService;


        public PostController(IPostService postService, ICommentService commentService, IForumService forumService)
        {
            _postService = postService;
            _commentService = commentService;
            _forumService = forumService;
        }

        public async Task<IActionResult> Details(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var comments = await _commentService.GetCommentsByPostIdAsync(postId);

            var postDetailsViewModel = new PostDetailsViewModel
            {
                PostID = post.PostID,
                Title = post.Title,
                Content = post.Content,
                PostedTime = post.PostedTime,
                Username = post.User?.UserName ?? "",
                Comments = comments.Select(c => new CommentViewModel
                {
                    CommentID = c.CommentID,
                    Content = c.Content,
                    PostedTime = c.PostedTime,
                    Username = c.User?.UserName ?? ""
                }).ToList()
            };

            return View(postDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int forumId)
        {
            var forum = await _forumService.GetForumByIdAsync(forumId);
            var viewModel = new CreatePostViewModel
            {
                ForumId = forum.ForumID,
                ForumName = forum.ForumName
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    ForumID = model.ForumId,
                    UserID = int.Parse(userId),
                    PostedTime = DateTime.UtcNow
                };

                await _postService.CreatePostAsync(post);
                return RedirectToAction("Index", "Home", new { forumId = model.ForumId });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                PostID = postId,
                Content = content,
                UserID = int.Parse(userId),
                PostedTime = DateTime.Now
            };

            await _commentService.AddCommentAsync(comment);

            return RedirectToAction("Details", "Post", new { postId = postId });
        }
    }
}
