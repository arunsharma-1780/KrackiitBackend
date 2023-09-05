using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class VideoThumbnailimage
    {
        public IFormFile? Image { get; set; }
    }

    public class VideoUrl
    {
        public IFormFile? VideoLinkUrl { get; set; }
    }

    public class CreateVideo : CurrentUser
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid SubjectCategory { get; set; }
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
    public class EditVideo : CreateVideo
    {
        public Guid Id { get; set; }
    }

    public class VideoById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllVideos
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid InstituteId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        [DefaultValue("All")]
        public string Language { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllVideoAuthors
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid InstituteId { get; set; }

    }
}
