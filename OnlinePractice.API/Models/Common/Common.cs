using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Common
{
    public class CheckPermission : CurrentUser
    {
        public ModuleType Module { get; set; }
    }
}
