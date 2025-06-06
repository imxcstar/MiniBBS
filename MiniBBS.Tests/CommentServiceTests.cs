using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;

namespace MiniBBS.Tests;

public class CommentServiceTests
{
    private ForumDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ForumDbContext(options);
    }

    [Fact]
    public async Task CanAddComment()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "f", Description = "" });
        context.Users.Add(new User { Id = 1, UserName = "u" });
        context.Posts.Add(new Post { PostID = 1, Title = "t", Content = "c", ForumID = 1, UserID = 1 });
        await context.SaveChangesAsync();

        var service = new CommentService(context);
        var comment = await service.AddCommentAsync(new Comment
        {
            Content = "cc",
            PostID = 1,
            UserID = 1,
            PostedTime = DateTime.UtcNow
        });

        var fetched = await service.GetCommentByIdAsync(comment.CommentID);
        Assert.Equal("cc", fetched.Content);
    }
}
