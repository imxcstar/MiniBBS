using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBBS.DB;
using MiniBBS.Models;

namespace MiniBBS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ForumDbContext _context;

        public AdminController(ForumDbContext context)
        {
            _context = context;
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
    }
}
