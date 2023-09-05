using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using DM = OnlinePractice.API.Models.DBModel;
using PayRes = OnlinePractice.API.Models.Payment.Response;
using Pay = OnlinePractice.API.Models.Payment.Request;
using Enum = OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;

using MailKit.Search;
using System.Text;
using Newtonsoft.Json;
using System.Security.Policy;
using CCA.Util;
using static OnlinePractice.API.Controllers.CCAvenueController;
using System.Web;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentWalletRepository : IStudentWalletRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMyCartRepository _myCartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public StudentWalletRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMyCartRepository myCartRepository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _myCartRepository = myCartRepository;
            _configuration = configuration;
        }
        public async Task<PayRes.PaymentResponse> CreateOrder(Pay.PaymentRequest paymentRequest)
        {
            var userInfo = await _userManager.FindByIdAsync(paymentRequest.UserId.ToString());
            CCACrypto cCACrypto = new CCACrypto();
            string merchantId = "2446430";
            string accessCode;
            string workingKey;
            switch (paymentRequest.ServerType)
            {
                case ServerType.UAT:
                    accessCode = "AVHT64KE73AY44THYA";
                    workingKey = "1A519C5244263A3846066A550187739A";
                    break;
                case ServerType.Production:
                    accessCode = "AVYV83KF77CK49VYKC";
                    workingKey = "3729D938F24A418576CA6A26176F2C7E";
                    break;
                default:
                    accessCode = "AVBQ04KE89BA89QBAB";
                    workingKey = "1B4BA83F33CEA6BEC8AEBD1720192BAF";
                    break;
            }
            string orderId = Guid.NewGuid().ToString();
            string amount = paymentRequest.Amount.ToString("F");
            string pData = string.Format("encResp={0}&orderNo={1}", "encResp", "orderNo");
            Pay.ResData resData = new Pay.ResData();
            string cancelUrl;
            string redirectUrl;
            if (paymentRequest.IsMobile)
            {
                switch (paymentRequest.ServerType)
                {
                    case ServerType.UAT:
                        cancelUrl = "https://krackitt.com/mobilePayment?status=cancel";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders?" + resData;
                        break;
                    case ServerType.Production:
                        cancelUrl = "https://krackitt.com/mobilePayment?status=cancel";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders?" + resData;
                        break;
                    default:
                        cancelUrl = "https://krackitt.com/mobilePayment?status=cancel";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders/local?" + resData;
                        // redirectUrl = "https://localhost:7010/api/CCAvenue/payment/orders/local?" + resData;
                        break;
                }
            }
            else
            {
                switch (paymentRequest.ServerType)
                {
                    case ServerType.UAT:
                        cancelUrl = "https://krackitt.com/Wallet?status=Aborted";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders?" + resData;
                        // redirectUrl = "https://localhost:7010/api/CCAvenue/payment/orders?" + resData;
                        break;
                    case ServerType.Production:
                        cancelUrl = "https://krackitt.com/Wallet?status=Aborted";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders?" + resData;
                        break;
                    default:
                        cancelUrl = "http://localhost:3000/Wallet?status=Aborted";
                        redirectUrl = "https://backend.krackitt.com/api/CCAvenue/payment/orders/local?" + resData;
                       //  redirectUrl = "https://localhost:7010/api/CCAvenue/payment/orders/local?" + resData;
                        break;
                }
            }

            string currency = "INR";
            string billingName = userInfo != null ? userInfo.DisplayName : string.Empty;
            string billingTel = userInfo != null ? userInfo.PhoneNumber : string.Empty;
            string billingEmail = userInfo != null ? userInfo.Email : string.Empty;
            string billingCountry = "India";
            string billing_city = "Indore";
            string billing_zip = "N/A";
            string billing_address = "N/A";
            string billing_state = "Madhya Pradesh";
            string is_mobile = paymentRequest.IsMobile == true ? "Mobile" : "Web";
            string serverType = paymentRequest.ServerType.ToString();
            string postData = string.Format("merchant_id={0}&order_id={1}&amount={2}&currency={3}&redirect_url={4}&cancel_url={5}&billing_name={6}&billing_tel={7}&billing_email={8}&billing_country={9}&merchant_param1={10}&merchant_param2={11}&billing_city={12}&billing_zip={13}&billing_state={14}&billing_address={15}", merchantId, orderId, amount, currency, redirectUrl, cancelUrl, billingName, billingTel, billingEmail, billingCountry, is_mobile, serverType, billing_city, billing_zip, billing_state, billing_address);
            string encryptedData = cCACrypto.Encrypt(postData, workingKey);
            string encodedData = HttpUtility.UrlEncode(encryptedData);
            PayRes.PaymentResponse paymentResponse = new();
            switch (paymentRequest.ServerType)
            {
                case ServerType.Production:
                    paymentResponse.OrderId = orderId;
                    paymentResponse.RedirectUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + encodedData + "&access_code=" + accessCode;
                    break;
                case ServerType.UAT:

                    paymentResponse.OrderId = orderId;
                    paymentResponse.RedirectUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + encodedData + "&access_code=" + accessCode;

                    break;
                default:

                    paymentResponse.OrderId = orderId;
                    paymentResponse.RedirectUrl = "https://test.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + encodedData + "&access_code=" + accessCode;
                    break;
            }
            return paymentResponse;
        }
        //if ()
        //PayRes.PaymentResponse paymentResponse = new PayRes.PaymentResponse
        //{
        //    OrderId = orderId,
        //    RedirectUrl = "https://test.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + encodedData + "&access_code=" + accessCode
        //};


        //public async Task<PayRes.OrderResponse?> CreateOrder(Pay.OderAmount oderAmount)
        //{
        //    var userInfo = await _userManager.FindByIdAsync(oderAmount.UserId.ToString());


        //    Pay.Order order = new();
        //    order.order_id = Guid.NewGuid().ToString();
        //    order.order_currency = "INR";
        //    order.order_amount = oderAmount.Amount;
        //    order.customer_details = new()
        //    {
        //        customer_email = userInfo != null ? userInfo.Email : string.Empty,
        //        customer_name = userInfo != null ? userInfo.DisplayName : string.Empty,
        //        customer_phone = userInfo != null ? userInfo.PhoneNumber : string.Empty,
        //        customer_id = userInfo != null ? userInfo.Id : Guid.Empty.ToString(),
        //    };
        //    //Uri returnUrl = new Uri("https://onlinepractice-omega.vercel.app/pattern?order_id={order_id}");
        //    //if(oderAmount.Mode == Enum.Mode.Local)
        //    //{
        //    //    returnUrl = new Uri("https://onlinepractice-omega.vercel.app/pattern?order_id={order_id}");
        //    //}
        //    //else if(oderAmount.Mode == Enum.Mode.UAT)
        //    //{
        //    //    returnUrl = new Uri("https://onlinepractice-omega.vercel.app/pattern?order_id={order_id}");
        //    //}
        //    //else if(oderAmount.Mode == Enum.Mode.Production)
        //    //{
        //    //    returnUrl = new Uri("https://onlinepractice-omega.vercel.app/pattern?order_id={order_id}");
        //    //}
        //    order.order_meta = new()
        //    {
        //        notify_url = "https://test.cashfree.com"
        //       // return_url = returnUrl.ToString()
        //    };
        //    string json = JsonConvert.SerializeObject(order);

        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage();
        //    request.RequestUri = new Uri("https://sandbox.cashfree.com/pg/orders");
        //    request.Method = System.Net.Http.HttpMethod.Post;
        //    request.Headers.Add("Accept", "*/*");
        //    request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");
        //    request.Headers.Add("x-client-id", _configuration["CashFreeTest:ClientId"]);
        //    request.Headers.Add("x-client-secret", _configuration["CashFreeTest:ClientSecret"]);
        //    request.Headers.Add("x-api-version", "2022-09-01");
        //    request.Headers.Add("x-request-id", "Nitesh Singh");

        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    request.Content = content;
        //    var response = await client.SendAsync(request);
        //    var result = await response.Content.ReadAsStringAsync();
        //    var final = JsonConvert.DeserializeObject<PayRes.OrderResponse>(result);
        //    if(final != null)
        //    {
        //        DM.Payment payment = new()
        //        {
        //            OrderId = Guid.Parse(final.order_id),
        //            StudentId = Guid.Parse(final.customer_details.customer_id),
        //            OrderAmount = final.order_amount,
        //            OrderStatus = final.order_status,
        //            IsActive = true,
        //            IsDeleted = false,
        //            CreationDate = DateTime.UtcNow,
        //            CreatorUserId = Guid.Parse(final.customer_details.customer_id)
        //        };
        //        await _unitOfWork.Repository<DM.Payment>().Insert(payment);
        //    }
        //    return final;
        //}

        public async Task<bool> PaymentStatus(Pay.PaymentStatus payment)
        {
            bool result = false;
            var userInfo = await _userManager.FindByIdAsync(payment.UserId.ToString());
            Req.CreateStudentWallet wallet = new()
            {
                UserId = payment.UserId,
                OrderId = payment.OrderId
            };
            if (payment.IsSuccess)
            {
                result = await ReChargeWallet(wallet);
            }
            else
            {
                return false;
            }
            return result;
        }
        public async Task<bool> ReChargeWallet(Req.CreateStudentWallet createwallet)
        {
            var updatedWallet = await _userManager.FindByEmailAsync(createwallet.Email);
            string json = JsonConvert.SerializeObject(createwallet.OrderId);


            if (updatedWallet != null && createwallet.Amount > 0 && createwallet.Status == "Success")
            {
                createwallet.UserId = Guid.Parse(updatedWallet.Id);
                DM.WalletHistory walletHistory = new()
                {
                    TransactionId = createwallet.OrderId,
                    StudentId = createwallet.UserId,
                    CreditAmount = createwallet.Amount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createwallet.UserId
                };
                await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                DM.Payment payment = new()
                {
                    OrderId = Guid.Parse(createwallet.OrderId),
                    StudentId = createwallet.UserId,
                    OrderAmount = createwallet.Amount,
                    OrderStatus = "Success",
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createwallet.UserId
                };
                await _unitOfWork.Repository<DM.Payment>().Insert(payment);
                updatedWallet.Balance = updatedWallet.Balance + walletHistory.CreditAmount;
                var result = await _userManager.UpdateAsync(updatedWallet);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
        //public async Task<bool> ReChargeWallet(Req.CreateStudentWallet createwallet)
        //{
        //    var updatedWallet = await _userManager.FindByIdAsync(createwallet.UserId.ToString());
        //    string json = JsonConvert.SerializeObject(createwallet.OrderId);

        //    HttpClient client = new HttpClient();
        //    var request = new HttpRequestMessage();
        //    request.Method = System.Net.Http.HttpMethod.Get;
        //    request.RequestUri = new Uri("https://sandbox.cashfree.com/pg/orders/"+createwallet.OrderId);
        //    request.Headers.Add("Accept", "*/*");
        //    request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");
        //    request.Headers.Add("x-client-id", _configuration["CashFreeTest:ClientId"]);
        //    request.Headers.Add("x-client-secret", _configuration["CashFreeTest:ClientSecret"]);
        //    request.Headers.Add("x-api-version", "2022-09-01");
        //    request.Headers.Add("x-request-id", "Nitesh Singh");
        //    var response = await client.SendAsync(request);
        //    var final = await response.Content.ReadAsStringAsync();
        //    var orderDetail = JsonConvert.DeserializeObject<PayRes.OrderResponse>(final);


        //    if (updatedWallet != null && orderDetail != null && orderDetail.order_amount > 0  && orderDetail.order_status =="PAID")
        //    {

        //        DM.WalletHistory walletHistory = new()
        //        {      
        //            TransactionId = createwallet.OrderId,
        //            StudentId = createwallet.UserId,
        //            CreditAmount = orderDetail.order_amount,
        //            IsActive = true,
        //            IsDeleted = false,
        //            CreationDate = DateTime.UtcNow,
        //            CreatorUserId = createwallet.UserId
        //        };
        //        await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
        //        var payment = await _unitOfWork.Repository<DM.Payment>().GetSingle(x => x.OrderId == Guid.Parse(orderDetail.order_id) && x.StudentId == createwallet.UserId && !x.IsDeleted);
        //        if(payment != null)
        //        {
        //            payment.OrderStatus = orderDetail.order_status;
        //            payment.LastModifierUserId = createwallet.UserId;
        //            payment.LastModifyDate = DateTime.UtcNow;
        //            await _unitOfWork.Repository<DM.Payment>().Update(payment);
        //        }
        //        updatedWallet.Balance = updatedWallet.Balance + walletHistory.CreditAmount;
        //        var result = await _userManager.UpdateAsync(updatedWallet);
        //        if (result.Succeeded)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public async Task<bool> GetOrder(Req.CreateStudentWallet createwallet)
        {
            bool IsSuccess = false;
            PayRes.OrderDetail order = new();
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage();
            request.Method = System.Net.Http.HttpMethod.Get;
            request.RequestUri = new Uri("https://sandbox.cashfree.com/pg/orders/" + createwallet.OrderId);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");
            request.Headers.Add("x-client-id", _configuration["CashFreeTest:ClientId"]);
            request.Headers.Add("x-client-secret", _configuration["CashFreeTest:ClientSecret"]);
            request.Headers.Add("x-api-version", "2022-09-01");
            request.Headers.Add("x-request-id", "Nitesh Singh");
            var response = await client.SendAsync(request);
            var final = await response.Content.ReadAsStringAsync();
            var orderDetail = JsonConvert.DeserializeObject<PayRes.OrderResponse>(final);
            if (orderDetail != null && orderDetail.order_status == "PAID")
            {
                IsSuccess = true;
                return IsSuccess;
            }
            return false;
        }
        public async Task<bool> PurchaseFromWallet(Req.PurchaseFromWallet wallet)
        {
            int result = 0;
            var updatedWallet = await _userManager.FindByIdAsync(wallet.UserId.ToString());
            if (updatedWallet != null && updatedWallet.Balance < wallet.Amount)
            {
                return false;
            }
            if (updatedWallet != null)
            {
                DM.WalletHistory walletHistory = new()
                {
                    StudentId = wallet.UserId,
                    DebitAmount = wallet.Amount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = wallet.UserId
                };
                await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                updatedWallet.Balance = updatedWallet.Balance - wallet.Amount;
                await _userManager.UpdateAsync(updatedWallet);
                return result > 0;
            }
            return false;
        }
        public async Task<bool> CheckOut(Req.Checkout checkout)
        {
            var checkWallet = await _userManager.FindByIdAsync(checkout.UserId.ToString());
            if (checkWallet != null && checkWallet.Balance < checkout.TotalAmount)
            {
                return false;
            }
            if (checkWallet != null)
            {
                checkWallet.Balance = checkWallet.Balance - checkout.TotalAmount;
                var result = await _userManager.UpdateAsync(checkWallet);
                if (result.Succeeded)
                {
                    if (checkout.CheckOutItems.Count > 0)
                    {
                        var transactionId = Guid.NewGuid().ToString();
                        foreach (var item in checkout.CheckOutItems)
                        {
                            DM.MyPurchased myPurchased = new()
                            {
                                StudentId = checkout.UserId,
                                ProductCategory = item.ProductCategory,
                                Price = item.Price,
                                ProductId = item.ProductId,
                                IsActive = true,
                                IsDeleted = false,
                                CreationDate = DateTime.UtcNow,
                                CreatorUserId = checkout.UserId
                            };
                            await _unitOfWork.Repository<DM.MyPurchased>().Insert(myPurchased);
                            var myCart = await _unitOfWork.Repository<DM.MyCart>().GetSingle(x => x.StudentId == checkout.UserId && x.ProductId == item.ProductId && !x.IsDeleted && !x.IsPurchased);
                            if (myCart != null)
                            {
                                myCart.IsPurchased = true;
                                myCart.LastModifierUserId = checkout.UserId;
                                myCart.LastModifyDate = DateTime.UtcNow;
                                await _unitOfWork.Repository<DM.MyCart>().Update(myCart);
                            }

                            DM.WalletHistory walletHistory = new()
                            {
                                ProductId = item.ProductId,
                                ProductCategory = item.ProductCategory,
                                StudentId = checkout.UserId,
                                DebitAmount = item.Price,
                                IsActive = true,
                                IsDeleted = false,
                                CreationDate = DateTime.UtcNow,
                                CreatorUserId = checkout.UserId,
                                TransactionId = transactionId
                            };
                            await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public async Task<Res.StudentWallet?> GetWalletHistory(Req.GetWalletHistory wallet)
        {
            Res.StudentWallet studentWallet = new();
            var checkWallet = await _userManager.FindByIdAsync(wallet.UserId.ToString());
            studentWallet.StudentId = checkWallet != null ? Guid.Parse(checkWallet.Id) : Guid.Empty;
            studentWallet.Balance = checkWallet != null ? checkWallet.Balance : 0;

            List<Res.StudentWalletHistory> studentWalletHistories = new();

            var studentWalletHistory = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => x.StudentId == wallet.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate));

            foreach (var item in studentWalletHistory)
            {
                Res.StudentWalletHistory walletHistory = new();
                walletHistory.ProductId = item.ProductId;
                var details = await _myCartRepository.GetProductCategoryName(item.ProductCategory, item.ProductId);
                walletHistory.ProductCategory = item.ProductCategory;
                walletHistory.ProductName = details != null ? details.ProductName : string.Empty;
                walletHistory.CreditAmount = item.CreditAmount;
                walletHistory.DebitAmount = item.DebitAmount;
                walletHistory.StudentName = checkWallet != null ? checkWallet.DisplayName : string.Empty;
                walletHistory.CreationDate = item.CreationDate;
                studentWalletHistories.Add(walletHistory);
            }
            var result = studentWalletHistories;
            var resultV1 = result.Page(wallet.PageNumber, wallet.PageSize);
            studentWallet.StudentWalletHistory = resultV1.ToList();
            studentWallet.TotalRecords = studentWalletHistories.Count;
            if (studentWallet.TotalRecords > 0)
            {
                return studentWallet;
            }
            return null;
        }
        public async Task<Res.StudentBalance> GetStudentBalance(CurrentUser user)
        {
            Res.StudentBalance studentWallet = new();
            var checkWallet = await _userManager.FindByIdAsync(user.UserId.ToString());
            studentWallet.Balance = checkWallet != null ? checkWallet.Balance : 0;
            return studentWallet;
        }


    }
}
