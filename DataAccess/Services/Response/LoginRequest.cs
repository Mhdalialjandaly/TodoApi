using System.ComponentModel.DataAnnotations;

namespace DataAccess.Services.Response
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false; // لتحديد مدة صلاحية الـ Token
    }
}
