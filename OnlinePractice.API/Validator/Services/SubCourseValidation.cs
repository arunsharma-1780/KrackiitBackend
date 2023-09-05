using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class SubCourseValidation:ISubCourseValidation
    {
        public CreateSubCourseValidator CreateSubCourseValidator { get; set; } = new();
        public EditSubCourseValidator EditSubCourseValidator { get; set; } = new();

        public GetSubCourseByIdValidator GetSubCourseByIdValidator { get; set; } = new();
        public DeleteSubCourseValidator DeleteSubCourseValidator { get; set; } = new();
        public GetAllSubCourseByCourseIdValidator GetAllSubCourseByCourseIdValidator { get; set; } = new();
        public EditMultipleSubCourseValidator EditMultipleSubCourseValidator { get; set; } = new();
    }
}
