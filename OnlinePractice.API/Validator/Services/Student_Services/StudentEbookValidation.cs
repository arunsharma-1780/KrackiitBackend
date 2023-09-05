using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentEbookValidation:IStudentEbookValidation
    {
        public SubjectListByInstituteValidator SubjectListByInstituteValidator { get; set; } = new();
        public EbookListValidator EbookListValidator { get; set; } = new();
        public GetEbookValidator GetEbookValidator { get; set; } = new();
    }
}
