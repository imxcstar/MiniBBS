namespace MiniBBS.Models
{
    public class PostDetailsViewModel
    {
        public int PostID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }
        public string Username { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }
        public string Username { get; set; }
    }

}
