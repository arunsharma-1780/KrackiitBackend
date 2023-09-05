using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentMocktestValidation
    {
        public StudentGetMockTestsValidator StudentGetMockTestsValidator { get; set; }
        public CustomeStudentMockTestValidator CustomeStudentMockTestValidator { get; set; }
        public StudentAutomaticMockTestQuestionValidator StudentAutomaticMockTestQuestionValidator { get; set; }
        public GetStudentQuestionPanelValidator GetStudentQuestionPanelValidator { get; set; }
        public StudentQuestionResponseValidator StudentQuestionResponseValidator { get; set; }
        public StudentAnwersPanelValidator StudentAnwersPanelValidator { get; set; }
        public StudentMockTestIdValidator StudentMockTestIdValidator { get; set; }
        public StudentMockTestStatusValidator StudentMockTestStatusValidator { get; set; }
        public GetStudentResultValidator GetStudentResultValidator { get; set; }
        public GetResultValidator GetResultValidator { get; set; }
        public MarkAsSeenValidator MarkAsSeenValidator { get; set; }
        public ReviewAnswerValidator ReviewAnswerValidator { get; set; }
        public RemoveAnswerValidator RemoveAnswerValidator { get; set; }
        public ResumeMockTestValidator ResumeMockTestValidator { get; set; }
        public GetStudentQuestionSolutionValidator GetStudentQuestionSolutionValidator { get; set; }

    }
}
