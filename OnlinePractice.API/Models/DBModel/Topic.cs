using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Topic:BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string TopicName { get; set; }=string.Empty;

        public Guid SubjectCategoryId { get; set; }
    }
}
