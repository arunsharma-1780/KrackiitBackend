using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Validator;

namespace OnlinePractice.API.Models.DBModel
{
    public class StudentMockTest : BaseModel
    {
        public string MockTestName { get; set; } = string.Empty;
        public int TotalQuetsion { get; set; }
        public TimeSpan TimeDuration { get; set; }
        public QuestionType QuestionType { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid InstituteId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public string Language { get; set; } = string.Empty;
    }
}
