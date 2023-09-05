namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentEbookValidation
    {
        public SubjectListByInstituteValidator SubjectListByInstituteValidator { get; set; }
        public EbookListValidator EbookListValidator { get; set; }
        public GetEbookValidator GetEbookValidator { get; set; }
    }
}
