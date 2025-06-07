namespace MiniBBS.Models
{
    public class AdminCommentViewModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public DateTime PostedTime { get; set; }
    }
}
