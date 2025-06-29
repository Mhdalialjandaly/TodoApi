using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Services.Response
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        public string UserName => FirstName + "" + LastName;
        [Required]
        public UserRole Role { get; set; } = UserRole.Guest; 

        public string PhoneNumber { get; set; }
    }
}
