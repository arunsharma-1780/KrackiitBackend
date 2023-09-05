using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlinePractice.API.Models
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? CreatorUserId { get; set; } = null;
        public DateTime? CreationDate { get; set; }
        public Guid? LastModifierUserId { get; set; } = null;
        public DateTime? LastModifyDate { get; set; } = null;
        public Guid? DeleterUserId { get; set; } = null;
        public DateTime? DeletionDate { get; set; } = null;
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(true)] 
        public bool IsActive { get; set; }
    }
}
