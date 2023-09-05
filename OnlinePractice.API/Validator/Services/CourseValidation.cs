using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class CourseValidation:ICourseValidation
    {
        public CreateCourseValidator CreateCourseValidator { get; set; } = new();
        public EditCourseValidator EditCourseValidator { get; set; } = new();
        public GetCourseByIdValidator GetCourseByIdValidator { get; set; } = new();
        public DeleteCourseValidator DeleteCourseValidator { get; set; } = new();
        public GetExamIdValidator GetExamIdValidator { get; set; } = new();
    }
}
