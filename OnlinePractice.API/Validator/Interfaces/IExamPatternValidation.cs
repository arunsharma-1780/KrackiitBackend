using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IExamPatternValidation
    {
        public CreateExamPatternValidator CreateExamPatternValidator { get; set; }
        public GetByExamPatternIdValidator GetByExamPatternIdValidator { get; set; }
        public EditExamPatternValidator EditExamPatternValidator { get; set; }
        public EditGeneralInstructionExamPatternValidator EditGeneralInstructionExamPatternValidator { get; set; }
        public DeleteExamPatternValidator DeleteExamPatternValidator { get; set; }
        public GetAllExamPatternValidator GetAllExamPatternValidator { get; set; }
        public GetSectionListValidator GetSectionListValidator { get; set; }
    }
}
