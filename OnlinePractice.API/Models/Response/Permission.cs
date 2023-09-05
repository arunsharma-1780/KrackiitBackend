namespace OnlinePractice.API.Models.Response
{
    public class Permission
    {
        public string ModuleName { get; set; } = string.Empty;
    }


    public class PermissionList
    {
        public List<Permission> Permissions { get; set; } = new();
    }
}
