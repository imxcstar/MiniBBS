using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniBBS.DB;
using MiniBBS.Service;
using Moq;

namespace MiniBBS.Tests;

public class UserServiceTests
{
    private ForumDbContext GetContext()
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
        var userManager = new UserManager<User>(store, options.Object, new PasswordHasher<User>(),
            [new UserValidator<User>()], [new PasswordValidator<User>()], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, new Mock<ILogger<UserManager<User>>>().Object);
        return userManager;
    }

    private static SignInManager<User> CreateSignInManager(UserManager<User> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var logger = new Mock<ILogger<SignInManager<User>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<User>>();
        return new SignInManager<User>(userManager, contextAccessor.Object, claimsFactory.Object,
            options.Object, logger.Object, schemes.Object, confirmation.Object);
    }

    [Fact]
    public async Task CanCreateUpdateAndDeleteUser()
    {
        using var context = GetContext();
        var userManager = CreateUserManager(context);
        var signInManager = CreateSignInManager(userManager);
        var service = new UserService(userManager, signInManager);

        var user = new User { UserName = "u" };
        var created = await service.CreateUserAsync(user);
        Assert.NotNull(created);

        user.UserName = "new";
        var updated = await service.UpdateUserAsync(user);
        Assert.Equal("new", updated?.UserName);

        var deleted = await service.DeleteUserAsync(user.Id);
        Assert.True(deleted);
    }

    [Fact]
    public async Task ValidateUser_ReturnsTrue()
    {
        var userStore = new Mock<IUserStore<User>>();
        var userManager = new Mock<UserManager<User>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManager.Setup(m => m.FindByNameAsync("u")).ReturnsAsync(new User());

        var signInManagerMock = new Mock<SignInManager<User>>(userManager.Object, new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object, new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<User>>>().Object, new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<User>>().Object);
        signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(It.IsAny<User>(), "p", false))
            .ReturnsAsync(SignInResult.Success);

        var service = new UserService(userManager.Object, signInManagerMock.Object);
        var result = await service.ValidateUserAsync("u", "p");
        Assert.True(result);
    }
}
