using MiniBBS.DB;

namespace MiniBBS.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Forum> Forums { get; set; }
        public Forum SelectedForum { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
        public bool IsUserLoggedIn { get; set; }
    }


    public class PostViewModel
    {
        public int PostID { get; set; }
        public string Title { get; set; }
        public DateTime PostedTime { get; set; }
        public string Username { get; set; }
        public int CommentCount { get; set; }
    }
}
