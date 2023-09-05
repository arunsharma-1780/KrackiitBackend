using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Payment.Request;
using Req = OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.RazorPay;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Org.BouncyCastle.Ocsp;
using Razorpay.Api;
using System.Text;
using Pay = OnlinePractice.API.Models.Payment.Request;
using PayRes = OnlinePractice.API.Models.Payment.Response;
using OnlinePractice.API.Models.Common;
using CCA.Util;
using static OnlinePractice.API.Controllers.PaymentController;
using System.Web;
using static OnlinePractice.API.Controllers.CCAvenueController;

namespace OnlinePractice.API.Repository.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
      public  PaymentRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }



        public async Task<PayRes.PaymentResponse> PaymentStatus(Pay.PaymentRequest paymentRequest)
        {
            var userInfo = await _userManager.FindByIdAsync(paymentRequest.UserId.ToString());

            CCACrypto cCACrypto = new CCACrypto();
            string merchantId = "2446430";
            string accessCode = "AVBQ04KE89BA89QBAB";
            string workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
            string orderId = Guid.NewGuid().ToString();
            string amount = paymentRequest.Amount.ToString("F");
            string redirectUrl = "https://kractitt.vercel.app/dashboard";
            string cancelUrl = "https://learn.microsoft.com/en-us/dotnet/api/system.guid.empty?view=net-7.0";
            string currency = "INR";
            string billingName = userInfo != null ? userInfo.DisplayName : string.Empty;
            string billingTel = userInfo != null ? userInfo.PhoneNumber : string.Empty;
            string billingEmail = userInfo != null ? userInfo.Email : string.Empty;
            string billingCountry = "India";

            string postData = string.Format("merchant_id={0}&order_id={1}&amount={2}&currency={3}&redirect_url={4}&cancel_url={5}&billing_name={6}&billing_tel={7}&billing_email={8}&billing_country={9}", merchantId, orderId, amount, currency, redirectUrl, cancelUrl, billingName, billingTel, billingEmail, billingCountry);
            string encryptedData = cCACrypto.Encrypt(postData, workingKey);
            string encodedData = HttpUtility.UrlEncode(encryptedData);

            PayRes.PaymentResponse paymentResponse = new PayRes.PaymentResponse
            {
                OrderId = orderId,
                RedirectUrl = "https://test.ccavenue.com/bnk/servlet/processNbkReq?gtwID=AVN&requestType=PAYMENT" + encodedData + "&access_code=" + accessCode
            };
            return paymentResponse;
        }
    }
}
