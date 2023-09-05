using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Video : BaseModel
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoThumbnail { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
