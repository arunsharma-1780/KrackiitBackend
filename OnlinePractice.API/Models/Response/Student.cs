using Stripe;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Response
{
    public class StudentById
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfileImage { get; set; }
        public string InstituteId { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public string SubcourseId { get; set; } = string.Empty;
        public string SubcourseName { get; set; } = string.Empty;
        public string courseId { get; set; } = string.Empty;
        public string courseName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public double Balance { get; set; }
    }
    public class SendOtp
    {
        public string? otp { get; set; }
        public string MID { get; set; } = string.Empty;
        public bool isOtpSent { get; set; }

        [DefaultValue(false)]
        public bool IsForget { get; set; }
    }

    public class SMSBalance
    {
        public int Balance { get; set; }
    }
    public class StudentInstitueList
    {
        public List<StudentInstitute> StudentInstitutes { get; set; } = new();
        public int TotalRecord { get; set; }
    }

    public class StudentInstitute
    {
        public Guid InstitueId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
    }

}
