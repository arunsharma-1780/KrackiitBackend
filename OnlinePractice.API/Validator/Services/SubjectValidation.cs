using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class SubjectValidation : ISubjectValidation
    {
        public CreateSubjectValidator CreateSubjectValidator { get; set; } = new();
        public EditSubjectValidator EditSubjectValidator { get; set; } = new();

        public GetSubjectByIdValidator GetSubjectByIdValidator { get; set; } = new();
        public DeleteSubjectValidator DeleteSubjectValidator { get; set; } = new();
        public GetAllSubjectBySubCourseIdValidator GetAllSubjectBySubCourseIdValidator { get; set; } = new();
        public CreateSubjectCategoryValidator CreateSubjectCategoryValidator { get; set; } = new();
        public EditSubjectCategoryValidator EditSubjectCategoryValidator { get; set; } = new();
    }
}
