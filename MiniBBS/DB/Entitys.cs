using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniBBS.DB
{
    public class User : IdentityUser<int>
    {
        public DateTime RegistrationDate { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    public class Forum
    {
        [Key]
        public int ForumID { get; set; }

        [Required]
        public string ForumName { get; set; }

        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PostedTime { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int ForumID { get; set; }
        public Forum Forum { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PostedTime { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int PostID { get; set; }
        public Post Post { get; set; }
    }
}
