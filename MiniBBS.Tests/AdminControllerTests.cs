using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniBBS.Controllers;
using MiniBBS.DB;
using MiniBBS.Models;
using Moq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MiniBBS.Tests;

public class AdminControllerTests
{
    private static ForumDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ForumDbContext(options);
    }

    private static UserManager<User> CreateUserManager(ForumDbContext context)
    {
        var store = new UserStore<User, IdentityRole<int>, ForumDbContext, int>(context);
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        return new UserManager<User>(store, options.Object, new PasswordHasher<User>(),
            [new UserValidator<User>()], [new PasswordValidator<User>()], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, new Mock<ILogger<UserManager<User>>>().Object);
    }

    [Fact]
    public async Task Users_ReturnsViewWithModel()
    {
        using var context = GetContext();
        context.Users.Add(new User { UserName = "u", RegistrationDate = DateTime.UtcNow });
        context.SaveChanges();
        var manager = CreateUserManager(context);
        var controller = new AdminController(context, manager);

        var result = await controller.Users() as ViewResult;
        var model = Assert.IsAssignableFrom<IEnumerable<AdminUserViewModel>>(result?.Model);
        Assert.Single(model);
    }
}
