using OnlinePractice.API.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class ExamType : BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string ExamName { get; set; }=string.Empty;
    }
}
