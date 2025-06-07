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
        options.Setup(o => o.Value).Returns(new IdentityOptions
        {
            Password = new PasswordOptions
            {
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
                RequireNonAlphanumeric = false
            }
        });
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

    [Fact]
    public async Task Forums_ReturnsViewWithModel()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumName = "F", Description = "D" });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        var result = await controller.Forums() as ViewResult;
        var model = Assert.IsAssignableFrom<IEnumerable<AdminForumViewModel>>(result?.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Posts_ReturnsViewWithModel()
    {
        using var context = GetContext();
        context.Users.Add(new User { Id = 1, UserName = "u" });
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "F", Description = "D" });
        context.Posts.Add(new Post { Title = "t", Content = "c", ForumID = 1, UserID = 1, PostedTime = DateTime.UtcNow });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        var result = await controller.Posts() as ViewResult;
        var model = Assert.IsAssignableFrom<IEnumerable<AdminPostViewModel>>(result?.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task CreateForum_AddsForum()
    {
        using var context = GetContext();
        var controller = new AdminController(context, CreateUserManager(context));
        await controller.CreateForum("New", "Desc");
        Assert.Single(context.Forums);
    }

    [Fact]
    public async Task EditForum_UpdatesForum()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "Old", Description = "D" });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        var model = new AdminForumViewModel { ForumId = 1, ForumName = "New", Description = "D2" };
        await controller.EditForum(model);
        Assert.Equal("New", context.Forums.First().ForumName);
    }

    [Fact]
    public async Task CreateUser_AddsUser()
    {
        using var context = GetContext();
        var controller = new AdminController(context, CreateUserManager(context));
        await controller.CreateUser(new AdminUserViewModel { Username = "u", Email = "e" });
        Assert.Single(context.Users);
    }

    [Fact]
    public async Task EditUser_UpdatesUser()
    {
        using var context = GetContext();
        var manager = CreateUserManager(context);
        var user = new User { UserName = "u", RegistrationDate = DateTime.UtcNow };
        await manager.CreateAsync(user);
        context.SaveChanges();
        var controller = new AdminController(context, manager);

        var model = new AdminUserViewModel { UserId = user.Id, Username = "new", Email = "e" };
        await controller.EditUser(model);
        Assert.Equal("new", context.Users.First().UserName);
    }

    [Fact]
    public async Task CreateAndEditPost()
    {
        using var context = GetContext();
        context.Users.Add(new User { Id = 1, UserName = "u" });
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "F", Description = "d" });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        await controller.CreatePost(new AdminPostFormViewModel { Title = "t", Content = "c", ForumId = 1, UserId = 1 });
        var post = context.Posts.First();
        Assert.Equal("t", post.Title);

        await controller.EditPost(new AdminPostFormViewModel { PostId = post.PostID, Title = "n", Content = "n", ForumId = 1, UserId = 1 });
        Assert.Equal("n", context.Posts.First().Title);
    }

    [Fact]
    public async Task PostComments_ReturnsViewWithModel()
    {
        using var context = GetContext();
        context.Users.Add(new User { Id = 1, UserName = "u" });
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "F", Description = "d" });
        context.Posts.Add(new Post { PostID = 1, Title = "t", Content = "c", ForumID = 1, UserID = 1, PostedTime = DateTime.UtcNow });
        context.Comments.Add(new Comment { CommentID = 1, Content = "cc", PostID = 1, UserID = 1, PostedTime = DateTime.UtcNow });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        var result = await controller.PostComments(1) as ViewResult;
        var model = Assert.IsAssignableFrom<IEnumerable<AdminCommentViewModel>>(result?.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task DeleteComment_RemovesComment()
    {
        using var context = GetContext();
        context.Users.Add(new User { Id = 1, UserName = "u" });
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "F", Description = "d" });
        context.Posts.Add(new Post { PostID = 1, Title = "t", Content = "c", ForumID = 1, UserID = 1, PostedTime = DateTime.UtcNow });
        context.Comments.Add(new Comment { CommentID = 1, Content = "cc", PostID = 1, UserID = 1, PostedTime = DateTime.UtcNow });
        context.SaveChanges();
        var controller = new AdminController(context, CreateUserManager(context));

        await controller.DeleteComment(1, 1);
        Assert.Empty(context.Comments);
    }
}
