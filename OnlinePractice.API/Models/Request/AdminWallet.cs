using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class GetAdminWallet
    {
        public Guid InstituteId { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }

    }

    public class GetTransactionDetails
    {
        public Guid OrderId { get; set; }
    }
}
