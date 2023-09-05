namespace OnlinePractice.API.Validator.Interfaces
{
    public interface ISubCourseValidation
    {
        public CreateSubCourseValidator CreateSubCourseValidator { get; set; }
        public EditSubCourseValidator EditSubCourseValidator { get; set; }
        public GetSubCourseByIdValidator GetSubCourseByIdValidator { get; set; }
        public DeleteSubCourseValidator DeleteSubCourseValidator { get; set; }
        public GetAllSubCourseByCourseIdValidator GetAllSubCourseByCourseIdValidator { get; set; }
        public EditMultipleSubCourseValidator EditMultipleSubCourseValidator { get; set; }
    }
}
