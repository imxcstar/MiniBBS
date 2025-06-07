using MiniBBS.DB;
using System.Collections.Generic;

namespace MiniBBS.Models
{
    public class NavBarViewModel
    {
        public IEnumerable<Forum>? Forums { get; set; }
        public Forum? SelectedForum { get; set; }
    }
}
