using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.Request
{
    public class CreateStudent : CurrentUser
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; } = string.Empty;

    }

    public class StudentLogin
    { 
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        //[Required(ErrorMessage = "Email is required")]
        //public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class StudentLogout
    {
        public string Email { get; set; } = string.Empty;
    }


    public class Tokenlogin
    {
        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class SendOTP
    {
       public string MobileNumber { get; set; } =string.Empty;
        public string CallingUnit { get; set; } = string.Empty;
    }

    public class EditStudent:CurrentUser
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Password is required")]
        //public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public Guid SubcourseId { get; set; }
    }

    public class ForgotStudentPassword:CurrentUser
    {
        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;

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
    public class MobNumber : CurrentUser
    {
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class UpdateStudentProfile : CurrentUser
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Password is required")]
        //public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        //public string ProfileImage { get; set; } = string.Empty;
        //public Guid InstituteId { get; set; }
        //public Guid SubcourseId { get; set; }
    }

    public class AddFeedback:CurrentUser
    {
        public string StudentFeedback { get; set; } = string.Empty;
    }
}
