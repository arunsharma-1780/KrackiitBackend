namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IInstituteValidation
    {
        public InstituteLogoValidator instituteLogoValidation { get; set; }
        public CreateInstituteValidator CreateInstituteValidator { get; set; }
        public EditInstituteValidator EditInstituteValidator { get; set; }
        public GetInstituteByIdValidator GetInstituteByIdValidator { get; set; }
        public DeleteInstituteValidator DeleteInstituteValidator { get; set; }

    }
}
