using MiniBBS.DB;

namespace MiniBBS.Service
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> ValidateUserAsync(string username, string password);
    }

    public interface IForumService
    {
        Task<IEnumerable<Forum>> GetAllForumsAsync();
        Task<Forum> GetForumByIdAsync(int id);
        Task<Forum> CreateForumAsync(Forum forum);
        Task<Forum> UpdateForumAsync(int id, Forum forum);
        Task<bool> DeleteForumAsync(int id);
    }

    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsByForumIdAsync(int forumId);
        Task<Post?> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(Post post);
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(int postId);
    }

    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);
    }
}
