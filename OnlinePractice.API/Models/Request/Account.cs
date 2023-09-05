using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlinePractice.API.Models.Request
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class Register
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ProfileImage is required")]
        public string ProfileImage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

    }
    public class ProfileImage
    {
        public IFormFile? Image { get; set; }
    }
    public class GetUserById
    {
        public Guid Id { get; set; }
    }
    public class GetUserEmail
    {
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateAdmin : CurrentUser
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string Location { get; set; }=string.Empty;
    }
    public class RemoveProfile : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class CheckNumberExist
    {
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class ForgotPassword
    {
        [Required]
        public string Email { get; set; } =string.Empty;
    }

    public class CurrentPassword
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
    public class ChangePassword : CurrentUser
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// NewPassword
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// ConfirmPassword
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// UserId
        /// </summary>

    }
}
