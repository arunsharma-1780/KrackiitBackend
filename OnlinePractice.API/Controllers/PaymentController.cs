using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Models.RazorPay;
using Pay = OnlinePractice.API.Models.Payment.Request;
using PayRes = OnlinePractice.API.Models.Payment.Response;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using Razorpay.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OnlinePractice.API.Validator.Interfaces;
using System.Web;
using CCA.Util;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Models.Payment.Request;
using MailKit.Search;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentController> _logger;
        //   private const string CCAvenueOrderStatusApiEndpoint = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans?command=orderStatusTracker&request_type=JSON";
        //private const string CCAvenueStatusApiEndpoint = "https://api.ccavenue.com/apis/servlet/StatusUpdateServlet";
        //  private const string CCAvenueStatusApiEndpoint = "https://apitest.ccavenue.com/apis/servlet/StatusTransaction";
        //private const string CCAvenueOrderStatusApiEndpoint = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans";
        private const string CCAvenueStatusApiEndpoint = "https://secure.ccavenue.com/transaction/getITCStatus";
        private const string AccessCode = "AVBQ04KE89BA89QBAB";
        private const string MerchantId = "2446430";
        private const string CancelOrderUrl = "https://yourwebsite.com/cancel_order";
        private readonly ISwaggerProvider _swaggerProvider;


        public PaymentController(IConfiguration config, IPaymentRepository paymentRepository, ILogger<PaymentController> logger, ISwaggerProvider swaggerProvider)
        {
            _config = config;
            _paymentRepository = paymentRepository;
            _logger = logger;
            _swaggerProvider = swaggerProvider;
        }

        [HttpGet("download-swagger")]
        public IActionResult DownloadSwagger()
        {
            var swagger = _swaggerProvider.GetSwagger("v1"); // Replace "v1" with your actual API version


            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                var openApiWriter = new OpenApiJsonWriter(writer);
                swagger.SerializeAsV3(openApiWriter);
                writer.Flush();

                return File(stream.ToArray(), "application/json", "swagger.json");
            }


        }
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(string orderID)
        {
            CCACrypto cCACrypto = new CCACrypto();

            var apiUrl = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans";
            string merchantId = "2446430";
            string accessCode = "AVBQ04KE89BA89QBAB";
            string workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
            string postData = string.Format("access_code={0}&order_id={1}", accessCode, orderID);

            string encryptedData = cCACrypto.Encrypt(postData, workingKey);
            string encodedData = HttpUtility.UrlEncode(encryptedData);

            // Construct the request content
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
    {
                {"enc_request",encodedData},
        { "command", "orderStatusTracker"},
        { "access_code", accessCode},
        { "merchant_id", merchantId},
        { "order_no", orderID},
        { "request_type", "JSON"}
    });

            // Send the HTTP request and get the response
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(apiUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }
        public class CCAvenueStatusRequest
        {
            public string access_code { get; set; }
            public string order_id { get; set; }
            public string enc_request { get; set; }
        }
        [HttpGet("status1")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public async Task<IActionResult> GetStatus1(string orderID)
        {
            CCACrypto cCACrypto = new CCACrypto();
            string merchantId = "2446430";
            string accessCode = "AVBQ04KE89BA89QBAB";
            string workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
            var httpClient = new HttpClient();
            string postData = string.Format("access_code={0}&order_id={1}", accessCode, orderID);
            string encryptedData = cCACrypto.Encrypt(postData, workingKey);
            string encodedData = HttpUtility.UrlEncode(encryptedData);

            var encryptedRequest = EncryptData($"{orderID}|{accessCode}", workingKey);
            string CCAvenueOrderStatusApiEndpoint = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans?";
            var parameters = new
            {
                command = "orderStatusTracker",
                access_code = accessCode,
                request_type = "JSON",
                enc_request = encryptedData
            };

            var jsonPayload = JsonConvert.SerializeObject(parameters);
            var httpContent = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(CCAvenueOrderStatusApiEndpoint, httpContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return Ok(responseContent);
        }
        private string GetEncryptedRequest(string orderId)
        {
            CCACrypto cCACrypto = new CCACrypto();
            string accessCode = "AVBQ04KE89BA89QBAB";
            string workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
            string postData = string.Format("access_code={0}&order_id={1}", accessCode, orderId);
            string encryptedData = cCACrypto.Encrypt(postData, workingKey);
            return encryptedData;
        }
        private string ParseStatusResponse(string responseContent)
        {
            // Implement the logic to parse the response content and extract the order status
            // Return the order status
            return "Order Status";
        }
        private string EncryptData(string plainText, string encryptionKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor();
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }
        public class CCavenueStatusResponse
        {
            public string OrderId { get; set; } = string.Empty;
            public string TrackingId { get; set; } = string.Empty;
        }




        private string GetPaymentStatusFromResponse(string responseContent)
        {
            // Extract the payment status from the response content
            // Implement your logic based on the response format (JSON/XML)
            // Example:
            // var paymentStatus = JObject.Parse(responseContent)["paymentStatus"].ToString();
            // return paymentStatus;

            return string.Empty; // Placeholder, replace with actual logic
        }

        //[HttpGet("response")]
        //public async Task<IActionResult> Response()
        //{
        //    // Handle response from CCAvenue
        //}

        //[HttpGet("cancel")]
        //public IActionResult Cancel()
        //{
        //    // Handle cancellation
        //}

        //[HttpPost("request")]
        //public async Task<IActionResult> Request()
        //{
        //    // Generate payment request and redirect to CCAvenue
        //}
        //private readonly ILogger<PaymentController> _logger;
        //private readonly IPaymentRepository _paymentRepository;
        //private readonly IStudentRepository _studentRepository;
        //private IHttpContextAccessor _httpContextAccessor;
        //private readonly IPaymentValidation _validation;
        //private RazorpayClient _razorpayClient;
        //public PaymentController(ILogger<PaymentController> logger, IPaymentRepository paymentRepository, IHttpContextAccessor httpContextAccessor, IStudentRepository studentRepository, IPaymentValidation validation)
        //{
        //    _logger = logger;
        //    _paymentRepository = paymentRepository;
        //    _httpContextAccessor = httpContextAccessor;
        //    _razorpayClient = new RazorpayClient("rzp_test_Rf21KzTsM3qlgY", "Ak5ItEdieyW3l2XHLATB0RT7");
        //    _studentRepository= studentRepository;
        //    _validation = validation;
        //}

        //[HttpPost]
        //[Authorize]
        //[Route("createorder")]
        //public async Task<IActionResult> CreateOrder(Pay.OderAmount oderAmount)
        //{
        //    ErrorResponse? errorResponse;
        //    try
        //    {
        //        #region Check Login Token
        //        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        CurrentUser user = new()
        //        {
        //            UserId = this.UserId,
        //        };
        //        var oldToken = await _studentRepository.GetToken(user);
        //        if (token != oldToken)
        //        {
        //            return Unauthorized();
        //        }
        //        #endregion
        //        #region Validate Request Model
        //        var validation = await _validation.OderAmountValidator.ValidateAsync(oderAmount);
        //        errorResponse = CustomResponseValidator.CheckModelValidation(validation);
        //        if (errorResponse != null)
        //        {
        //            return BadRequest(errorResponse);
        //        }
        //        #endregion
        //        oderAmount.UserId = this.UserId;
        //        var studentData = await _paymentRepository.CreateOrder(oderAmount);
        //        if (studentData != null)
        //        {
        //            _logger.LogInformation("Student order generated Successfully!");
        //            return Ok(ResponseResult<PayRes.OrderResponse?>.Success("Student order generated Successfully!", studentData));
        //        }
        //        else
        //        {
        //            _logger.LogCritical("Student order not Found!");
        //            return Ok(ResponseResult<PayRes.OrderResponse?>.Failure("Student order not Found!", new PayRes.OrderResponse()));
        //        }
        //    }
        //    catch (DbUpdateException exp)
        //    {
        //        var ex = exp.InnerException as SqlException;
        //        errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createorder", ex);
        //        return BadRequest(errorResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createorder", ex);
        //        return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
        //    }
        //}

        //[HttpPost]
        //[Authorize]
        //[Route("recharegewallet")]
        //public async Task<IActionResult> PaymentStatus(Pay.PaymentStatus paymentStatus)
        //{
        //    ErrorResponse? errorResponse;
        //    try
        //    {
        //        #region Check Login Token
        //        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        CurrentUser user = new()
        //        {
        //            UserId = this.UserId,
        //        };
        //        var oldToken = await _studentRepository.GetToken(user);
        //        if (token != oldToken)
        //        {
        //            return Unauthorized();
        //        }
        //        #endregion
        //        #region Validate Request Model
        //        var validation = await _validation.PaymentStatusValidator.ValidateAsync(paymentStatus);
        //        errorResponse = CustomResponseValidator.CheckModelValidation(validation);
        //        if (errorResponse != null)
        //        {
        //            return BadRequest(errorResponse);
        //        }
        //        #endregion
        //        paymentStatus.UserId = this.UserId;
        //        var studentData = await _paymentRepository.PaymentStatus(paymentStatus);
        //        if (studentData)
        //        {
        //            _logger.LogInformation("Payment added Successfully!");
        //            return Ok(ResponseResult<bool>.Success("Payment added Successfully!", studentData));
        //        }
        //        else
        //        {
        //            _logger.LogCritical("Payment  not added!");
        //            return Ok(ResponseResult<bool>.Failure("Payment  not added!", studentData));
        //        }
        //    }
        //    catch (DbUpdateException exp)
        //    {
        //        var ex = exp.InnerException as SqlException;
        //        errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "paymentstatus", ex);
        //        return BadRequest(errorResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "paymentstatus", ex);
        //        return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
        //    }
        //}
    }
}

