namespace OnlinePractice.API.Models.Response
{
    public class EbookList
    {
        public int TotalRecords { get; set; }
        public List<Ebook> Ebooks { get; set; } = new();
    }
    public class Ebook
    {
        public Guid EbookId { get; set; }
        public Guid ExamTypeId { get; set; }

        public string ExamTypeName { get; set; } =string.Empty;
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid SubCourseId { get; set; }

        public string SubCourseName { get; set; } = string.Empty;

        public Guid SubjectCategoryId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public Guid TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string EbookTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string EbookPdfUrl { get; set; } = string.Empty;
        public string EbookThumbnail { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
        public int TotalPurchase { get; set; }
    }
    public class EbookListV1
    {
        public int TotalRecords { get; set; }
        public List<EbookV1> Ebooks { get; set; } = new();
    }
    public class AutherList
    {
        public List<EbookAuthers> EbookAuthers { get; set; } =new();
    }

    public class EbookAuthers
    {
        public string AutherName { get; set; } =string.Empty;
    }
    public class EbookListV2
    {
        public List<Ebooks> Ebooks { get; set; } = new();
    }
    public class Ebooks
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class EbookV1
    {
        public Guid Id { get; set; }
        public string EbookPdfUrl { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string EbookTitle { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string EbookThumbnail { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
    }

    public class AuthorAndLanguage
    {
        public string Language { get; set; } = string.Empty;
        public string authorName { get; set; } = string.Empty;
    }
}
