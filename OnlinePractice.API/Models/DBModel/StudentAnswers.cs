using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class StudentAnswers : BaseModel
    {
        public Guid MockTestId { get; set; }
        public Guid StudentId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }

        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string StudentAnswer { get; set; } = string.Empty;
        public bool IsCorrectA { get; set; }
        public bool IsCorrectB { get; set; }
        public bool IsCorrectC { get; set; }
        public bool IsCorrectD { get; set; }
        public bool IsMarkReview { get; set;}
        public bool IsAnswered { get; set; }
        public bool IsVisited { get; set; }
    }
}
