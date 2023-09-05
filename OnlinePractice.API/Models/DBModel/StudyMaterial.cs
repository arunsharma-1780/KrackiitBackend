using System.ComponentModel.DataAnnotations.Schema;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class StudyMaterial : BaseModel
    {
        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; } = string.Empty;
        public string AutherName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; } = string.Empty;
        public FormatType FormatType { get; set; }
        public double Price { get; set; }
        public virtual Course Course { get; set; } = new();
        public string Path { get; set; } = string.Empty;
    }
}
