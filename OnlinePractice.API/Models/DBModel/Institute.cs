using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class Institute : BaseModel
    {
        public string InstituteCode { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteContactNo { get; set; } = string.Empty;
        public string InstituteContactPerson { get; set; } = string.Empty;
        public string InstituteEmail { get; set; } = string.Empty;
        public string InstituteCity { get; set; } = string.Empty;
        public string InstituteAddress { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
    }
}
