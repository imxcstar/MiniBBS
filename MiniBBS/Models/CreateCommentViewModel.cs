using MiniBBS.DB;

namespace MiniBBS.Models
{
    public class CreateCommentViewModel
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
