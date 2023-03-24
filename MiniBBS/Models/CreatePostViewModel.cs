namespace MiniBBS.Models
{
    public class CreatePostViewModel
    {
        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
