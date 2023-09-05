using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class Module : BaseModel
    {
        public string ModuleName { get; set; } = String.Empty;
        public ModuleType ModuleType { get; set; }
    }
}
