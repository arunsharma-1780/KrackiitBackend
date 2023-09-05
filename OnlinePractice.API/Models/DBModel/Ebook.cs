using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Ebook:BaseModel
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public string EbookTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string EbookPdfUrl { get; set; } = string.Empty;
        public string EbookThumbnail { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
