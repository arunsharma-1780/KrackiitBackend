using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class ExamTypeValidation : IExamTypeValidation
    {
        public CreateExamTypeValidator CreateExamTypeValidator { get; set; } = new();
        public EditExamTypeValidator EditExamTypeValidator { get; set; } = new();
        public GetExamTypeByIdValidator GetExamTypeByIdValidator { get; set; } = new();
        public DeleteExamTypeValidator DeleteExamTypeValidator { get; set; } = new();
        public CreateExamFlowValidator CreateExamFlowValidator { get; set; } = new();
    }
}
