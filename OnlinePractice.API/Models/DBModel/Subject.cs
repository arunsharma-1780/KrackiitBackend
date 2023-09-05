using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Subject:BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string SubjectName { get; set; } = string.Empty;
    }
}
