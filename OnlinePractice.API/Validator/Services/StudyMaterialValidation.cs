using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class StudyMaterialValidation : IStudyMaterialValidation
    {
        public CreateStudyMaterialValidator CreateStudyMaterialValidator{ get; set; } = new();

    }
}
