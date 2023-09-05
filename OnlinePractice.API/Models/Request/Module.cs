using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class CreateModule : CurrentUser
    {
        public string ModuleName { get; set; } = string.Empty;
        public ModuleType ModuleType { get; set; }

    }
    public class EditModule : CurrentUser
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;

    }

    public class GetModule : CurrentUser
    {
        public Guid Id { get; set; }
    }


    public class CheckModuleType
    {
        public ModuleType ModuleType { get; set; }
    }
}
