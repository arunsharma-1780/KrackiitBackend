namespace OnlinePractice.API.Models.DBModel
{
    public class StudentFeedbacks:BaseModel
    {
        public Guid StudentId { get; set; }
        public string StudentFeedback { get; set; } = string.Empty;
    }
}
