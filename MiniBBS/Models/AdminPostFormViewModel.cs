namespace MiniBBS.Models
{
    public class AdminPostFormViewModel
    {
        public int PostId { get; set; }
        public int ForumId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
