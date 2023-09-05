using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Response
{
    public class EbookSubjectsList
    {
        public List<EbookSubjects> ebookSubjects { get; set; } = new();
    }
    public class EbookSubjects
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }

    public class StudentEbooksList
    {
        public List<StudentEbook> StudentEbookList { get; set; } = new();
        public int TotalRecord { get; set; }

    }
    public class StudentEbook
    {
        public Guid EbookId { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string EbookTitle { get; set; } = string.Empty;
        public double Price { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string EbookThumbnail { get; set; } = string.Empty;
        public string EbookPdfURL { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsPurchased { get; set; }
        public DateTime? CreationDate { get; set; }
    }
    public class GetEbook
    {
        public Guid EbookId { get; set; }
        public string EbookThumbnail { get; set; } = string.Empty;
        public string EbookPdfURL { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string EbookTitle { get; set; } = string.Empty;
        public double Price { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPurchased { get; set; }

    }


}
