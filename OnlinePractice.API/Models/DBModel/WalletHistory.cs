using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class WalletHistory : BaseModel
    {
        public Guid StudentId { get; set; }
        public Guid ProductId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public double CreditAmount { get; set; }
        public double DebitAmount { get; set; }
        public double RemainingAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
