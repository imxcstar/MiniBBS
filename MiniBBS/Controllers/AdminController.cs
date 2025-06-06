using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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
    }
}
