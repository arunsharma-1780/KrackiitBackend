namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentValidation
    {
        public CreateStudentValidator CreateStudentValidator { get; set; }
        public EditStudentValidator EditStudentValidator { get; set; }
        public StudentLoginValidatior StudentLoginValidatior { get; set; }
        public GetTokenByNumberValidator GetTokenByNumberValidator { get; set; }
        public ForgotPasswordValidator ForgotPasswordValidator { get; set; }
        public SendOTPValidator SendOTPValidator { get; set; }
        public MobNumberValidator MobNumberValidator { get; set; }
        public EmailValidatior EmailValidatior { get; set; }
        public MobileValidatior MobileValidatior { get; set; }
        public EditStudentProfileValidator EditStudentProfileValidator { get; set; }
        public StudentProfileImageValidator StudentProfileImageValidator { get; set; }
        public AddFeedbackValidator AddFeedbackValidator { get; set; }
    }
}
