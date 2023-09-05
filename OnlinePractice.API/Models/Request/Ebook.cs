using OnlinePractice.API.Models.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.Request
{
    public class CreateEbook:CurrentUser
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid SubjectCategory { get; set; }
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
    public class EbookThumbnailimage
    {
        public IFormFile? Image { get; set; }
    }
    public class EbookPdfUrl
    {
        public IFormFile? PdfUrl { get; set; }
    }
    public class EditEbook:CreateEbook
    {
      public Guid Id { get; set; }
    }

    public class DeleteEbook :CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class EbookById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class UploadMedia
    {
        public IFormFile? MediaFile { get; set; }
        public string MediaFor { get; set; } = string.Empty;
    }
    public class GetAllEbook : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
    public class GetAllEbookV1
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid InstituteId { get; set; }
        public String WriterName { get; set; } = string.Empty;
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class GetAllAuthors
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }

    }

}
