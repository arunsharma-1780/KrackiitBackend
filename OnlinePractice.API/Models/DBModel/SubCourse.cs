using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class SubCourse:BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string SubCourseName { get; set; } = string.Empty;
        public Guid CourseID { get; set; }
    }
}
