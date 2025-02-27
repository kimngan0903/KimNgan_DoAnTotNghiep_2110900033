using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email không để trống")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không để trống")]
        public string PasswordHash { get; set; }
        public bool Remember { get; set; }
        public int UserId { get; internal set; }
    }
}
