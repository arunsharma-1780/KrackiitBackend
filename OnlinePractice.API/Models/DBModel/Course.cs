using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Course:BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string CourseName { get; set; } = string.Empty;
        public Guid ExamTypeID { get; set; }
    }
}
