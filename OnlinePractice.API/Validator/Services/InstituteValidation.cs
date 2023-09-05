using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class InstituteValidation: IInstituteValidation
    {
        public InstituteLogoValidator instituteLogoValidation { get; set; } = new();
        public CreateInstituteValidator CreateInstituteValidator { get; set; } = new();
        public EditInstituteValidator EditInstituteValidator { get; set; } = new();

        public GetInstituteByIdValidator GetInstituteByIdValidator { get; set; } = new();
        public DeleteInstituteValidator DeleteInstituteValidator { get; set; } = new();
    }
}
