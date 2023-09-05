
using System.Text.Json.Serialization;

namespace OnlinePractice.API.Models.Request
{
    public class AssignPermission : CurrentUser
    {
        public List<ModuleList> Modules { get; set; } = new();
    }
    public class EditPermission : CurrentUserDetail
    {

        public List<EditModuleList> Modules { get; set; } = new();
    }
    public class CurrentUserDetail : CurrentUser
    {
        [JsonIgnore]
        public Guid StaffUserId { get; set; }
    }
    public class EditModuleList
    {
        public Guid ModuleId { get; set; }

        public bool Value { get; set; }
    }
    public class ModuleList
    {
        public Guid ModuleId { get; set; }
    }
}
