using OnlinePractice.API.Models.Request;
using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.AuthDB
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

    }

    public class ForgotPassword
    {
        [Required]
        public string? Email { get; set; }
    }
}
