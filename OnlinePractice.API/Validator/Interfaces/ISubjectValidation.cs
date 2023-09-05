namespace OnlinePractice.API.Validator.Interfaces
{
    public interface ISubjectValidation
    {
        public CreateSubjectValidator CreateSubjectValidator { get; set; }
        public EditSubjectValidator EditSubjectValidator { get; set; }
        public GetSubjectByIdValidator GetSubjectByIdValidator { get; set; }
        public DeleteSubjectValidator DeleteSubjectValidator { get; set; }
        public GetAllSubjectBySubCourseIdValidator GetAllSubjectBySubCourseIdValidator { get; set; }
        public CreateSubjectCategoryValidator CreateSubjectCategoryValidator { get; set; }
        public EditSubjectCategoryValidator EditSubjectCategoryValidator { get; set; }
    }
}
