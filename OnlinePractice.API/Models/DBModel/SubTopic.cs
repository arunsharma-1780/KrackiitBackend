using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class SubTopic:BaseModel
    {
        [Column(TypeName = "nvarchar(150)")]
        public string SubTopicName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SubTopicDescription { get; set; } = string.Empty;

        public Guid TopicId { get; set; }
    }
}
