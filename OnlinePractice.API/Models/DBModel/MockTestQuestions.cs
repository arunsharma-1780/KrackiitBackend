namespace OnlinePractice.API.Models.DBModel
{
    public class MockTestQuestions : BaseModel
    {
        public Guid SectionId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public Guid SubjectId { get; set; }
        public Guid MockTestSettingId { get; set; }
        public int Marks { get; set; }
        public int NegativeMark { get; set; }
    }
}
