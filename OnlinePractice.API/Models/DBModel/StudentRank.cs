namespace OnlinePractice.API.Models.DBModel
{
    public class StudentRank : BaseModel
    {
        public Guid StudentId { get; set; }
        public Guid InstituteId { get; set; }
        public Guid SubCourseId { get; set; }
        public double TotalMark { get; set; }
        public double TotalObtainMark { get; set; }
        public int TotalMockTest { get; set; }
        public double AveragePercentage { get; set; }
        public int Rank { get; set; }
    }
}
