namespace OnlinePractice.API.Models.Request
{
    public class CreatePreviousYearPaper : CurrentUser
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
    public class PaperPdfUrl
    {
        public IFormFile? PdfUrl { get; set; }
    }
    public class EditPreviousYearPaper : CreatePreviousYearPaper
    {
        public Guid Id { get; set; }
    }
    public class GetPaperPdf : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllPapers : CurrentUser
    {
        public Guid SubCourseId { get; set; }
        public int Year { get; set; }
        public Guid InstituteId { get; set; }
        public Guid CreationUserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetAllFacultyList
    {
        public Guid SubCourseId { get; set; }
        public Guid AddedBy { get; set; }
    }

    public class Faculties
    {
        public string FacultyName { get; set; } = string.Empty;
    }

}
