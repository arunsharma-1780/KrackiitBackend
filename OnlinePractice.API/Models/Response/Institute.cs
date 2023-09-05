using OnlinePractice.API.Validator;

namespace OnlinePractice.API.Models.Response
{
    public class InstituteList
    {
        public int TotalRecords { get; set; }
        public List<InstituteInfo> Institutes { get; set; } = new();
    }

    public class InstituteInfo
    {
        public Guid Id { get; set; }
        public string InstituteCode { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteContactNo { get; set; } = string.Empty;
        public string InstituteContactPerson { get; set; } = string.Empty;
        public string InstituteEmail { get; set; } = string.Empty;
        public string InstituteCity { get; set; } = string.Empty;
        public string InstituteAddress { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
    public class Institute
    {
        public Guid Id { get; set; }
        public string InstituteCode { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteContactNo { get; set; } = string.Empty;
        public string InstituteContactPerson { get; set; } = string.Empty;
        public string InstituteEmail { get; set; } = string.Empty;
        public string InstituteCity { get; set; } = string.Empty;
        public string InstituteAddress { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public int TotalStudent { get; set; }
        public int TotalStaff { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }

    public class InstituteListV1
    {
        public int TotalRecords { get; set; }
        public List<InstituteV1> Institutes { get; set; } = new();
    }

    public class InstituteV1
    {
        public Guid Id { get; set; }
        public string InstituteCode { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
       
    }

    public class InstituteData
    { 
        public List<InstituteDataList> InstituteDataLists { get; set; } = new();
    }
    public class InstituteDataList
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
    }

    public class InstituteModuleData
    {
        public Guid CourseId { get; set; }
    }
}
