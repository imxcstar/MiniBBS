using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;

namespace MiniBBS.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                return result.Succeeded;
            }

            return false;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                return result.Succeeded;
            }

            return false;
        }
    }


}
