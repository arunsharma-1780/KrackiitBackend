namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IStudentRegistrationValidation
    {
        public GetAllStudentValidator GetAllStudentValidator { get; set; }
        public AddStudentValidator AddStudentValidator { get; set; }
        public UpdateStudentValidator UpdateStudentValidator { get; set; }
        public GetStudentByIdValidator GetStudentByIdValidator { get; set; }
        public AddBulkUploadStudentValidator AddBulkUploadStudentValidator { get; set; }
    }
}
