using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string PasswordHash { get; set; }

        public bool Remember { get; set; }

        public int CustomerId { get; internal set; }
    }
}
