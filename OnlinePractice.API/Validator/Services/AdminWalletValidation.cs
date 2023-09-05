using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class AdminWalletValidation : IAdminWalletValidation
    {
        public GetAdminWalletValidator GetAdminWalletValidator { get; set; } = new();
        public GetTransactionDetailsValidator GetTransactionDetailsValidator { get; set; } = new();
    }
}
