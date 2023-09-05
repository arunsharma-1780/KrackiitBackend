using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentVideoValidation:IStudentVideoValidation
    {
        public SubjectListValidator SubjectListValidator { get; set; } = new();
        public VideoListValidator VideoListValidator { get; set; } = new();
        public GetVideoValidator GetVideoValidator { get; set; } = new();
    }
}
