namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IAdminWalletValidation
    {
        public GetAdminWalletValidator GetAdminWalletValidator { get; set; }
        public GetTransactionDetailsValidator GetTransactionDetailsValidator { get; set; }
    }
}
