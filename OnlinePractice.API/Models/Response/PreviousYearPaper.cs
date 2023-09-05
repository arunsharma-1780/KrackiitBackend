namespace OnlinePractice.API.Models.Response
{

    public class PreviousYearPaperList
    {
        public int TotalRecords { get; set; }
        public List<PreviousYearPaperV1> PreviousYearPapers { get; set; } = new();
    }

    public class PreviousYearPaperV1
    {
        public Guid Id { get; set; }
        public string PaperTitle { get; set; } = string.Empty;
        public int Year { get; set; }
        public Guid SubCourseId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string? AddedBy { get; set; } 
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
        public List<string> Language { get; set; } = new();
        
    }

    public class PreviousYearPaper
    {
        public Guid Id { get; set; }
        public Guid ExamTypeId { get; set; }
        public string ExamTypeName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid SubCourseId { get; set; }
        public string SubCourseName { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PaperTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string PaperPdfUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
        public int TotalPurchase { get; set; }

    }
    public class VideoAuthorList
    {
        public List<VideoAuthors> VideoAuthors { get; set; } = new();
    }

    public class VideoAuthors
    {
        public string FacultyName { get; set; } = string.Empty;
    }
}
