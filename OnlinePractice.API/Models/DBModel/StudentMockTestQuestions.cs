namespace OnlinePractice.API.Models.DBModel
{
    public class StudentMockTestQuestions : BaseModel
    {
        public Guid StudentMockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public Guid SubjectId { get; set; }
        public int Marks { get; set; }
        public int NegativeMark { get; set; }
    }
}
