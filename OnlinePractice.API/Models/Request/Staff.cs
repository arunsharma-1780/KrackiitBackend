using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.Request
{
    public class CreateStaff : CurrentUser
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

        [Required(ErrorMessage = "InstituteId is required")]
        public Guid InstituteId { get; set; }
        public AssignPermission Permission { get; set; } = new();
    }

    public class EditStaff :CurrentUser
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "InstituteId is required")]
        public Guid InstituteId { get; set; }
        public EditPermission Permission { get; set; } = new();

    }

    public class GetAllStaff :CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    } 

    public class GetByIdStaff : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class InstituteCheck
    {
        public Guid InstituteId { get; set; }
    }

}
