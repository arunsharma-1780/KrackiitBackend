using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentDashboardValidation
    {
        public InstituteMockTestValidator InstituteMockTestValidator { get; set; }
    }
}
