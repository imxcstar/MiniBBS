using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniBBS.Controllers;
using MiniBBS.DB;
using MiniBBS.Models;
using MiniBBS.Service;
using Moq;
using System.Security.Claims;
using System.Collections.Generic;

namespace MiniBBS.Tests;

public class HomeControllerTests
{
    private static UserManager<User> CreateUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        return new UserManager<User>(store.Object, options.Object, new PasswordHasher<User>(), [], [], new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), null!, new Mock<ILogger<UserManager<User>>>().Object);
    }

    private static SignInManager<User> CreateSignInManager(bool signedIn)
    {
        var userManager = CreateUserManager();
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var logger = new Mock<ILogger<SignInManager<User>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<User>>();

        var managerMock = new Mock<SignInManager<User>>(userManager, contextAccessor.Object, claimsFactory.Object,
            options.Object, logger.Object, schemes.Object, confirmation.Object);
        managerMock.Setup(m => m.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(signedIn);
        return managerMock.Object;
    }

    [Fact]
    public async Task Index_ReturnsViewWithModel()
    {
        var forums = new List<Forum> { new() { ForumID = 1, ForumName = "f" } };
        var posts = new List<Post> { new() { PostID = 1, Title = "t", PostedTime = DateTime.UtcNow, User = new User { UserName = "u" }, Comments = new List<Comment>() } };

        var forumService = new Mock<IForumService>();
        forumService.Setup(f => f.GetAllForumsAsync()).ReturnsAsync(forums);
        forumService.Setup(f => f.GetForumByIdAsync(1)).ReturnsAsync(forums[0]);

        var postService = new Mock<IPostService>();
        postService.Setup(p => p.GetPostsByForumIdAsync(1)).ReturnsAsync(posts);

        var controller = new HomeController(forumService.Object, postService.Object, CreateSignInManager(false));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Index(1) as ViewResult;
        var model = Assert.IsType<IndexViewModel>(result?.Model);

        Assert.Single(model.Forums);
        Assert.Equal("f", model.SelectedForum.ForumName);
        var post = Assert.Single(model.Posts);
        Assert.Equal("t", post.Title);
    }
}
