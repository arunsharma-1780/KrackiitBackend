using CCA.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Org.BouncyCastle.Ocsp;
using System.Collections.Specialized;
using System.Web;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Pay = OnlinePractice.API.Models.Payment.Request;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CCAvenueController : ControllerBase
    {
        private readonly ILogger<CCAvenueController> _logger;
        private readonly IStudentWalletRepository _studentWalletRepository;
        public CCAvenueController(ILogger<CCAvenueController> logger, IStudentWalletRepository studentWalletRepository)
        {
            _logger = logger;
            _studentWalletRepository = studentWalletRepository;
        }
        [HttpPost]
        [Route("payment/orders/local")]
        public async Task<IActionResult> ConfirmationLocal([FromForm] Pay.ResData resData)
        {
            string workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
            CCACrypto cCACrypto = new CCACrypto();
            var decrypt = cCACrypto.Decrypt(resData.encResp, workingKey);
            // Separate the URL component
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(decrypt);

            string? orderstatus = queryParameters["order_status"];
            string? amount = queryParameters["amount"];
            string? order_id = queryParameters != null ? queryParameters["order_id"] : string.Empty;
            string? email = queryParameters != null ? queryParameters["billing_email"] : string.Empty;
            string? IsMobile = queryParameters != null ? queryParameters["merchant_param1"] : string.Empty;
            string? ServerType = queryParameters != null ? queryParameters["merchant_param2"] : string.Empty;

            if (orderstatus == "Success")
            {
                Req.CreateStudentWallet createStudent = new()
                {
                    OrderId = !string.IsNullOrEmpty(order_id) ? order_id: string.Empty,
                    Amount = Convert.ToDouble(amount),
                    Status = orderstatus,
                    Email = !string.IsNullOrEmpty(email)? email:string.Empty,
                };
                _ = await _studentWalletRepository.ReChargeWallet(createStudent);
            }
            // Update the order status, display confirmation, etc.
            if (IsMobile == "Mobile")
            {
                return ServerType switch
                {
                    "UAT" => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                    "Production" => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                    _ => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                };
            }
            else
            {
                return ServerType switch
                {
                    "UAT" => Redirect("https://krackitt.com/Wallet?status=" + orderstatus),
                    "Production" => Redirect("https://krackitt.com/Wallet?status=" + orderstatus),
                    _ => Redirect("http://localhost:3000/Wallet?status=" + orderstatus),
                };
            }

        }

        [HttpPost]
        [Route("payment/orders")]
        public async Task<IActionResult> ConfirmationUAT([FromForm] Pay.ResData resData)
        {
            string workingKey = "3729D938F24A418576CA6A26176F2C7E";
            CCACrypto cCACrypto = new CCACrypto();
            var decrypt = cCACrypto.Decrypt(resData.encResp, workingKey);
            // Separate the URL component
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(decrypt);
            string? orderstatus = queryParameters["order_status"];
            string? amount = queryParameters["amount"];
            string? order_id = queryParameters != null ? queryParameters["order_id"] : string.Empty;
            string? email = queryParameters != null ? queryParameters["billing_email"] : string.Empty;
            string? IsMobile = queryParameters != null ? queryParameters["merchant_param1"] : string.Empty;
            string? ServerType = queryParameters != null ? queryParameters["merchant_param2"] : string.Empty;

            if (orderstatus == "Success")
            {
                Req.CreateStudentWallet createStudent = new()
                {
                    OrderId = !string.IsNullOrEmpty(order_id) ? order_id : string.Empty,
                    Amount = Convert.ToDouble(amount),
                    Status = orderstatus,
                    Email = !string.IsNullOrEmpty(email) ? email : string.Empty,
                };
                _ = await _studentWalletRepository.ReChargeWallet(createStudent);
            }
            // Update the order status, display confirmation, etc.
            if (IsMobile == "Mobile")
            {
                return ServerType switch
                {
                    "UAT" => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                    "Production" => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                    _ => Redirect("https://krackitt.com/mobilePayment?status=" + orderstatus),
                };
            }
            else
            {
                return ServerType switch
                {
                    "UAT" => Redirect("https://krackitt.com/Wallet?status=" + orderstatus),
                    "Production" => Redirect("https://krackitt.com/Wallet?status=" + orderstatus),
                    _ => Redirect("http://localhost:3000/Wallet?status="+orderstatus),
                };
            }

        }
    }
}
