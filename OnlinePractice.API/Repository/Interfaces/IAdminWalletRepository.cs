using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IAdminWalletRepository
    {
        public Task<Res.AdminWalletList?> GetAllTransctions(Req.GetAdminWallet adminWallet);
        public Task<Res.TransactionDetails?> GetTransactionDetails(Req.GetTransactionDetails getTransaction);
    }
}
