namespace OnlinePractice.API.Validator.Interfaces
{
    public interface ICourseValidation
    {
        public CreateCourseValidator CreateCourseValidator { get; set; }      
        public EditCourseValidator EditCourseValidator { get; set; }
        public GetCourseByIdValidator GetCourseByIdValidator { get; set; }
        public DeleteCourseValidator DeleteCourseValidator { get; set; }
        public GetExamIdValidator GetExamIdValidator { get; set; }
    }
}
