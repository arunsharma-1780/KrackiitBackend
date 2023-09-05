using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class CreateInstitute : CurrentUser
    {
        public string InstituteCode { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteContactNo { get; set; } = string.Empty;
        public string InstituteContactPerson { get; set; } = string.Empty;
        public string InstituteEmail { get; set; } = string.Empty;

        [DefaultValue("Indore")]
        public string InstituteCity { get; set; } = string.Empty;
        public string InstituteAddress { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
    }

    public class LogoImage
    {
        public IFormFile? Image { get; set; } 
    }
    public class EditInstitute: CreateInstitute
    {
        public Guid Id { get; set; }
    }
    public class InstituteById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllInstitute : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class CodeCheck
    {
        public string InstituteCode { get; set; } = string.Empty;
    }
    public class EditCodeCheck
    {
        public Guid Id { get; set; }
        public string InstituteCode { get; set; } = string.Empty;
    }


    public class InstituteDataID
    {
        public Guid InstituteId { get; set; }
    }
}
