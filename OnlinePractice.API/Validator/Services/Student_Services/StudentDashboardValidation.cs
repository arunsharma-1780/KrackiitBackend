using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentDashboardValidation : IStudentDashboardValidation
    {
        public InstituteMockTestValidator InstituteMockTestValidator { get; set; } = new();
    }
}
