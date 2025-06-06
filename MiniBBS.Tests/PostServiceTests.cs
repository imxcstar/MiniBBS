using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;

namespace MiniBBS.Tests;

public class PostServiceTests
{
    private ForumDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ForumDbContext(options);
    }

    [Fact]
    public async Task CanCreatePost()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "Test", Description = "" });
        context.Users.Add(new User { Id = 1, UserName = "u" });
        await context.SaveChangesAsync();
        var service = new PostService(context);
        var post = await service.CreatePostAsync(new Post
        {
            Title = "t",
            Content = "c",
            ForumID = 1,
            UserID = 1,
            PostedTime = DateTime.UtcNow
        });
        Assert.NotEqual(0, post.PostID);

        var fetched = await service.GetPostByIdAsync(post.PostID);
        Assert.Equal("t", fetched.Title);
    }

    [Fact]
    public async Task CanUpdateAndDeletePost()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "Test", Description = "" });
        context.Users.Add(new User { Id = 1, UserName = "u" });
        var post = new Post { Title = "t", Content = "c", ForumID = 1, UserID = 1, PostedTime = DateTime.UtcNow };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var service = new PostService(context);
        post.Title = "new";
        await service.UpdatePostAsync(post);
        Assert.Equal("new", (await service.GetPostByIdAsync(post.PostID)).Title);

        await service.DeletePostAsync(post.PostID);
        Assert.Empty(await service.GetPostsByForumIdAsync(1));
    }
}
