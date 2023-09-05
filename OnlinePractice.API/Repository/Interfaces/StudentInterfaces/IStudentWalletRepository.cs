using OnlinePractice.API.Models;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using PayRes = OnlinePractice.API.Models.Payment.Response;
using Pay = OnlinePractice.API.Models.Payment.Request;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentWalletRepository
    {
        public Task<PayRes.PaymentResponse> CreateOrder(Pay.PaymentRequest paymentRequest);
        public Task<bool> ReChargeWallet(Req.CreateStudentWallet createwallet);
        public Task<bool> PurchaseFromWallet(Req.PurchaseFromWallet wallet);
        public Task<Res.StudentWallet?> GetWalletHistory(Req.GetWalletHistory wallet);
        public Task<bool> CheckOut(Req.Checkout checkout);
        public Task<Res.StudentBalance> GetStudentBalance(CurrentUser user);
       // public Task<PayRes.OrderResponse?> CreateOrder(Pay.OderAmount oderAmount);
        public Task<bool> PaymentStatus(Pay.PaymentStatus payment);
        public Task<bool> GetOrder(Req.CreateStudentWallet createwallet);
    }
}
