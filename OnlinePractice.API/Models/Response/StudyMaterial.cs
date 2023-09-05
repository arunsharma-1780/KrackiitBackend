using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Response
{
    public class StudyMaterial
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public FormatType FormatType { get; set; }
        public double Price { get; set; }
        public Guid CourseId { get; set; }
        public Guid ExamId { get; set; }

    }
}
