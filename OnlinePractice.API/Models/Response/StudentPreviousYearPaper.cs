using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Response
{
    public class StudentPYPList
    {
        public List<StudentPreviousYearPaperData> previousYearPapers { get; set; } = new();
        public int TotalRecord { get; set; }
    }
    public class StudentPreviousYearPaperData
    {
        public Guid PaperId { get; set; }
        public string PaperTitle { get; set; } = string.Empty;
        public string PaperPdfUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Language { get; set; } = string.Empty;
        public int Year { get; set; }
        [DefaultValue(false)]
        public bool IsPurchased { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
