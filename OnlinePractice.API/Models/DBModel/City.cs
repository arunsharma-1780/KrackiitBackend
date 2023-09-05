using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.DBModel
{
    public class City:BaseModel
    {
        public string CityName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid StateId { get; set; }
    }
}
