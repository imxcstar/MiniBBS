namespace MiniBBS.Models
{
    public class AdminPostViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string ForumName { get; set; }
        public string Username { get; set; }
        public DateTime PostedTime { get; set; }
    }
}
