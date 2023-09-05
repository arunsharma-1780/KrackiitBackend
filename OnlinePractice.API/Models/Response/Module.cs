using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Response
{
    public class ModulesList
    {
        public Guid ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public List<ModuleDataList> ModuleData { get; set; } = new();

    }

    public class Module
    {
        public List<ModulesList> ModulesList { get; set; } = new();
    }
    public class ModuleDataList
    {
        public Guid Id { get; set; }
        public string Headings { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ModuleCategory { get; set; } = new();
        //public List<string> Language { get; set; } = new();
        //public List<string> MockTestType { get; set; } = new();

        //public string Status { get; set; } = string.Empty;
        public DateTime? LastModifiedDate { get; set; }

    }

    public class ModuleDetailsList
    {
        public List<ModuleDetails> ModuleDetails { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class ModuleDetails
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public ModuleType ModuleType { get; set; }
    }

}
