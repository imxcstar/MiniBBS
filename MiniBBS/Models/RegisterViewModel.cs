using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MiniBBS.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }
    }
}
