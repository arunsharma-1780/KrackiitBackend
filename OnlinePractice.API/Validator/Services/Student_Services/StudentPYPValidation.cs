using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentPYPValidation : IStudentPYPValidation
    {
        public PYPListByInstituteValidator PYPListByInstituteValidator { get; set; } = new();
        public PapersDataByFilterValidator PapersDataByFilterValidator { get; set; } = new();
    }
}
