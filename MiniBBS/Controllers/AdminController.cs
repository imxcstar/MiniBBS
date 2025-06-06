using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MiniBBS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ForumDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ForumDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = new AdminDashboardViewModel
            {
                UserCount = _context.Users.Count(),
                ForumCount = _context.Forums.Count(),
                PostCount = _context.Posts.Count(),
                CommentCount = _context.Comments.Count()
            };
            return View(model);
        }

        public async Task<IActionResult> Users()
        {
            var list = new List<AdminUserViewModel>();
            foreach (var user in _context.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add(new AdminUserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    RegistrationDate = user.RegistrationDate,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Forums()
        {
            var forums = await _context.Forums.ToListAsync();
            var model = forums.Select(f => new AdminForumViewModel
            {
                ForumId = f.ForumID,
                ForumName = f.ForumName,
                Description = f.Description ?? string.Empty
            });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForum(string forumName, string description)
        {
            if (!string.IsNullOrWhiteSpace(forumName))
            {
                _context.Forums.Add(new Forum { ForumName = forumName, Description = description ?? string.Empty });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Forums));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForum(int id)
        {
            var forum = await _context.Forums.FindAsync(id);
            if (forum != null)
            {
                _context.Forums.Remove(forum);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Forums));
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _context.Posts.Include(p => p.User).Include(p => p.Forum).ToListAsync();
            var model = posts.Select(p => new AdminPostViewModel
            {
                PostId = p.PostID,
                Title = p.Title,
                ForumName = p.Forum?.ForumName ?? string.Empty,
                Username = p.User?.UserName ?? string.Empty,
                PostedTime = p.PostedTime
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Posts));
        }
    }
}
