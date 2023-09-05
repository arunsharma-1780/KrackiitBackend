using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class ExamPatternValidator: IExamPatternValidation
    {     
        public CreateExamPatternValidator CreateExamPatternValidator { get; set; } = new();
        public GetByExamPatternIdValidator GetByExamPatternIdValidator { get; set; } = new();
        public EditExamPatternValidator EditExamPatternValidator { get; set; } = new();
        public EditGeneralInstructionExamPatternValidator EditGeneralInstructionExamPatternValidator { get; set; } = new();
        public DeleteExamPatternValidator DeleteExamPatternValidator { get; set; } = new();
        public GetAllExamPatternValidator GetAllExamPatternValidator { get; set; } = new();
        public GetSectionListValidator GetSectionListValidator { get; set; } = new();
    }
}
