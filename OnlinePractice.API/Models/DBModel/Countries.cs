using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Countries:BaseModel
    {
        public string CountryName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;

    }
}
