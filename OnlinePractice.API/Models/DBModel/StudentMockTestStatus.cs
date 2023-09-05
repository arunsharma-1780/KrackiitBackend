namespace OnlinePractice.API.Models.DBModel
{
    public class StudentMockTestStatus : BaseModel
    {
        public Guid MockTestId { get; set; }
        public Guid StudentId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set;}
        public TimeSpan RemainingDuration { get; set; }
        public int RemainingAttempts { get; set; }
        public bool IsCustom { get; set; }
    }

}
