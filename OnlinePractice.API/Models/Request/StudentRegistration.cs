using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.Request
{
    public class AddStudent : CurrentUser
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
    public class AddBulkUploadStudent : CurrentUser
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
        [DefaultValue(false)]
        public bool IsBulkUpload { get; set; }
        public string Amount { get; set; } =string.Empty;

    }
    public class UpdateStudent : CurrentUser
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
    public class GetStudentById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllStudent
    {
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class StudentBulkUpload
    {
        public IFormFile? file { get; set; }
    }

    public class BulkUpload :CurrentUser
    {
        public IFormFile? file { get; set; }
    }
}
