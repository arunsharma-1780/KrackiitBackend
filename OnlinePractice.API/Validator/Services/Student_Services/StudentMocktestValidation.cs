using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentMocktestValidation: IStudentMocktestValidation
    {
        public StudentGetMockTestsValidator StudentGetMockTestsValidator { get; set; } = new();
        public CustomeStudentMockTestValidator CustomeStudentMockTestValidator { get; set; } = new();
        public StudentAutomaticMockTestQuestionValidator StudentAutomaticMockTestQuestionValidator { get; set; } = new();
        public GetStudentQuestionPanelValidator GetStudentQuestionPanelValidator { get; set; } =new();
        public StudentQuestionResponseValidator StudentQuestionResponseValidator { get; set; } = new();
        public StudentAnwersPanelValidator StudentAnwersPanelValidator { get; set; } =new();
        public StudentMockTestIdValidator StudentMockTestIdValidator { get; set; } = new();
        public StudentMockTestStatusValidator StudentMockTestStatusValidator { get; set; } = new();
        public GetStudentResultValidator GetStudentResultValidator { get; set; } = new();
        public GetResultValidator GetResultValidator { get; set; } = new();
        public MarkAsSeenValidator MarkAsSeenValidator { get; set; } = new();
        public ReviewAnswerValidator ReviewAnswerValidator { get; set; } = new();
        public RemoveAnswerValidator RemoveAnswerValidator { get; set; } = new();
        public ResumeMockTestValidator ResumeMockTestValidator { get; set; } = new();
        public GetStudentQuestionSolutionValidator GetStudentQuestionSolutionValidator { get; set; } = new();
    }
}
