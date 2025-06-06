using System.Collections.Generic;

namespace MiniBBS.Models
{
    public class AdminDashboardViewModel
    {
        public int UserCount { get; set; }
        public int ForumCount { get; set; }
        public int PostCount { get; set; }
        public int CommentCount { get; set; }
    }
}
