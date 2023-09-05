namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentPYPValidation
    {
        public PYPListByInstituteValidator PYPListByInstituteValidator { get; set; }
        public PapersDataByFilterValidator PapersDataByFilterValidator { get; set; }
    }
}
