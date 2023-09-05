namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentVideoValidation
    {
        public SubjectListValidator SubjectListValidator { get; set; }
        public VideoListValidator VideoListValidator { get; set; }
        public GetVideoValidator GetVideoValidator { get; set; }
    }
}
