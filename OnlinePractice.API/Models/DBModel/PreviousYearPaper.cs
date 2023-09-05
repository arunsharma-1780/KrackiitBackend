namespace OnlinePractice.API.Models.DBModel
{
    public class PreviousYearPaper :BaseModel
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
        public int Year { get; set; }
        public string PaperTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string PaperPdfUrl { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
