namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IExamTypeValidation
    {
        public CreateExamTypeValidator CreateExamTypeValidator { get; set; }
        public EditExamTypeValidator EditExamTypeValidator { get; set; }
        public GetExamTypeByIdValidator GetExamTypeByIdValidator { get; set; }
        public DeleteExamTypeValidator DeleteExamTypeValidator { get; set; }
        public CreateExamFlowValidator CreateExamFlowValidator { get; set; }

    }
}
