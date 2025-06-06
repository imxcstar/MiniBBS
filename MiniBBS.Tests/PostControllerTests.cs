using Microsoft.AspNetCore.Mvc;
using MiniBBS.Controllers;
using MiniBBS.DB;
using MiniBBS.Models;
using MiniBBS.Service;
using Moq;

namespace MiniBBS.Tests;

public class PostControllerTests
{
    [Fact]
    public async Task Details_ReturnsViewModel()
    {
        var post = new Post { PostID = 1, Title = "t", Content = "c", PostedTime = DateTime.UtcNow, User = new User { UserName = "u" } };
        var comments = new List<Comment> { new() { CommentID = 1, Content = "cc", PostedTime = DateTime.UtcNow, User = new User { UserName = "u" } } };

        var postService = new Mock<IPostService>();
        postService.Setup(p => p.GetPostByIdAsync(1)).ReturnsAsync(post);
        var commentService = new Mock<ICommentService>();
        commentService.Setup(c => c.GetCommentsByPostIdAsync(1)).ReturnsAsync(comments);
        var forumService = new Mock<IForumService>();

        var controller = new PostController(postService.Object, commentService.Object, forumService.Object);
        var result = await controller.Details(1) as ViewResult;
        var model = Assert.IsType<PostDetailsViewModel>(result?.Model);

        Assert.Equal("t", model.Title);
        Assert.Single(model.Comments);
    }
}
