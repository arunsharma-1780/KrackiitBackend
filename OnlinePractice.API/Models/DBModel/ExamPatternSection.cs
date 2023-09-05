namespace OnlinePractice.API.Models.DBModel
{
    public class ExamPatternSection:BaseModel
    {
        public Guid SubjectId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
        public Guid ExamPatternId { get; set; }
    }
}
