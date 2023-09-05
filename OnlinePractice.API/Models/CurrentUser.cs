using System.Text.Json.Serialization;

namespace OnlinePractice.API.Models
{
    public class CurrentUser
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
