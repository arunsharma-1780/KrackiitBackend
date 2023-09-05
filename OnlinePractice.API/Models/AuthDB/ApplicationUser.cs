using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace OnlinePractice.API.Models.AuthDB
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }   = string.Empty;
        public string Password { get; set; } =string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public Guid? CreatorUserId { get; set; } = null;
        public DateTime? CreationDate { get; set; }
        public Guid? LastModifierUserId { get; set; } = null;
        public DateTime? LastModifyDate { get; set; } = null;
        public Guid? DeleterUserId { get; set; } = null;
        public DateTime? DeletionDate { get; set; } = null;
        [DefaultValue(false)]
        public bool IsVerified { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public double Balance { get; set; }


    }
}
