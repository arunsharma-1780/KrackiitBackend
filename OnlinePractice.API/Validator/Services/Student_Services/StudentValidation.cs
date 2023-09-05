using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentValidation : IStudentValidation
    {
        public CreateStudentValidator CreateStudentValidator { get; set; } = new();
        public EditStudentValidator EditStudentValidator { get; set; } = new();
        public StudentLoginValidatior StudentLoginValidatior { get; set; } = new();
        public SendOTPValidator SendOTPValidator { get; set; } = new();
        public GetTokenByNumberValidator GetTokenByNumberValidator { get; set; } = new();
        public ForgotPasswordValidator ForgotPasswordValidator { get; set; } = new();
        public MobNumberValidator MobNumberValidator { get; set; } = new();

        public EmailValidatior EmailValidatior { get; set; } = new();

        public MobileValidatior MobileValidatior { get; set; } = new();
        public EditStudentProfileValidator EditStudentProfileValidator { get; set; } = new();

        public StudentProfileImageValidator StudentProfileImageValidator { get; set; } = new();

        public AddFeedbackValidator AddFeedbackValidator { get; set; } = new();
    }
}
