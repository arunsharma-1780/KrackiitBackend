using OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Models.Response
{
    public class StaffData
    {
        public List<StaffList> StaffList { get; set; } = new();

        public int TotalRecords { get; set; }
    }
    public class StaffList
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? MobileNumber { get; set; }

        public string? InstituteName { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public List<ModulesList>? Modules { get; set; } = new();
    }
    public class StaffAdminList
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
    public class StaffAdminData
    {
        public List<StaffAdminList> StaffList { get; set; } = new();

    }
    public class StaffById
    {
        public Guid Id { get; set; }
        public Guid InstituteId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? InstituteName { get; set; }
        public List<ModulesList> Modules { get; set; } = new();
    }
}
