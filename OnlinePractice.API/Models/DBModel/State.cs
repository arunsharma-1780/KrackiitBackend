using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.DBModel
{
    public class State:BaseModel
    {
        public string StateName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
    }
}
