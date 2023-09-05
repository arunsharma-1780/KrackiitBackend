using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlinePractice.API.Models.DBModel
{
    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string TableName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
    }
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public Guid? UserId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public AuditLog ToAudit()
        {
            var audit = new AuditLog
            {
                UserId = UserId,
                TableName = TableName,
                DateTime =  DateTime.UtcNow,
                OldValues = OldValues.Count == 0 ? string.Empty : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ? string.Empty : JsonConvert.SerializeObject(NewValues)
            };
            return audit;
        }
    }
}
