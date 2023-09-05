namespace OnlinePractice.API.Models.DBModel
{
    public class Permission : BaseModel
    {
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; } =string.Empty;
        public Guid StaffUserId { get; set; }
    }
}
