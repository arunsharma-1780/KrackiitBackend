using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Com = OnlinePractice.API.Models.Common;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using PayRes = OnlinePractice.API.Models.Payment.Response;
using Pay = OnlinePractice.API.Models.Payment.Request;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]

    public class StudentWalletController : BaseController
    {
        private readonly ILogger<StudentWalletController> _logger;
        private readonly IStudentWalletRepository _studentWalletRepository;
        public readonly IStudentRepository _studentRepository;
        public readonly IStudentWalletValidation _validation;


        public StudentWalletController(ILogger<StudentWalletController> logger, IStudentWalletRepository studentWalletRepository, IStudentRepository studentRepository, IStudentWalletValidation validation)
        {
            _logger = logger;
            _studentWalletRepository = studentWalletRepository;
            _studentRepository = studentRepository;
            _validation = validation;
        }
        /// <summary>
        /// Not used now
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [Route("manualrechargewallet")]
        public async Task<IActionResult> ReChargeewallet(Req.CreateStudentWallet wallet)
        {
            #region Check Login Token
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            CurrentUser user = new()
            {
                UserId = this.UserId,
            };
            var oldToken = await _studentRepository.GetToken(user);
            if (token != oldToken)
            {
                return Unauthorized();
            }
            #endregion
            ErrorResponse? errorResponse;
            #region Validate Request Model
            var validation = await _validation.CreateStudentWalletValidator.ValidateAsync(wallet);
            errorResponse = CustomResponseValidator.CheckModelValidation(validation);
            if (errorResponse != null)
            {
                return BadRequest(errorResponse);
            }
            #endregion
            try
            {

                wallet.UserId = this.UserId;
                var result = await _studentWalletRepository.ReChargeWallet(wallet);
                if (result)
                {
                    _logger.LogInformation("Wallet created successfully!");
                    return Ok(ResponseResult<bool>.Success("Wallet created successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Wallet not added!");
                    return Ok(ResponseResult<bool>.Failure("Wallet not added!!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createwallet", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createwallet", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Create order in cashfree 
        /// </summary>
        /// <param name="oderAmount"></param>
        /// <returns></returns>
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
        //        var studentData = await _studentWalletRepository.CreateOrder(oderAmount);
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
        [HttpPost]
        [Authorize]
        [Route("createorder")]
        public async Task<IActionResult> Post([FromBody] Pay.PaymentRequest paymentRequest)
        {
            #region Check Login Token
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            CurrentUser user = new()
            {
                UserId = this.UserId,
            };
            var oldToken = await _studentRepository.GetToken(user);
            if (token != oldToken)
            {
                return Unauthorized();
            }
            #endregion
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.PaymentRequestValidator.ValidateAsync(paymentRequest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                paymentRequest.UserId = this.UserId;
                var result = await _studentWalletRepository.CreateOrder(paymentRequest);
                if (result != null)
                {
                    _logger.LogInformation("Order created successfully!");
                  return Ok(ResponseResult<PayRes.PaymentResponse>.Success("Order created successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Order not created!");
                    return Ok(ResponseResult<PayRes.PaymentResponse>.Failure("Order not created!", new PayRes.PaymentResponse()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createorder", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createorder", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Add Amount in student wallet
        /// </summary>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("recharegewallet")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> PaymentStatus(Pay.PaymentStatus paymentStatus)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRepository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.PaymentStatusValidator.ValidateAsync(paymentStatus);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                paymentStatus.UserId = this.UserId;
                var studentData = await _studentWalletRepository.PaymentStatus(paymentStatus);
                if (studentData)
                {
                    _logger.LogInformation("Payment added Successfully!");
                    return Ok(ResponseResult<bool>.Success("Payment added Successfully!", studentData));
                }
                else
                {
                    _logger.LogCritical("Payment  not added!");
                    return Ok(ResponseResult<bool>.Failure("Payment  not added!", studentData));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "paymentstatus", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "paymentstatus", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("getorder")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> GetOrder(Req.CreateStudentWallet paymentStatus)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateStudentWalletValidator.ValidateAsync(paymentStatus);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                paymentStatus.UserId = this.UserId;
                var studentData = await _studentWalletRepository.GetOrder(paymentStatus);
                if (studentData)
                {
                    _logger.LogInformation("Order status is paid!");
                    return Ok(ResponseResult<bool>.Success("Order status is paid!", studentData));
                }
                else
                {
                    _logger.LogCritical("Order is not completed!");
                    return Ok(ResponseResult<bool>.Failure("Order is not completed!", studentData));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getorder", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getorder", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(Req.Checkout wallet)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRepository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CheckoutValidator.ValidateAsync(wallet);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                wallet.UserId = this.UserId;
                var result = await _studentWalletRepository.CheckOut(wallet);
                if (result)
                {
                    _logger.LogInformation("Checkout items successfully!");
                    return Ok(ResponseResult<bool>.Success("Checkout items successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Checkout items not added!");
                    return Ok(ResponseResult<bool>.Failure("Checkout items not added!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "checkout", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "checkout", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getstudentbalance")]
        public async Task<IActionResult> GetStudentBalance()
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRepository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion


                var result = await _studentWalletRepository.GetStudentBalance(user);
                if (result != null)
                {
                    _logger.LogInformation("Get student balance successfully!");
                    return Ok(ResponseResult<Res.StudentBalance?>.Success("Get student balance successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student balance not found!");
                    return Ok(ResponseResult<Res.StudentBalance?>.Failure("Student balance not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentbalance", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentbalance", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getwallethistory")]
        public async Task<IActionResult> PurchaseFromWallet(Req.GetWalletHistory wallet)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRepository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetWalletHistoryValidator.ValidateAsync(wallet);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                wallet.UserId = this.UserId;
                var result = await _studentWalletRepository.GetWalletHistory(wallet);
                if (result != null)
                {
                    _logger.LogInformation("Get wallet history successfully!");
                    return Ok(ResponseResult<Res.StudentWallet?>.Success("Get wallet history successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Wallet history not found!");
                    return Ok(ResponseResult<Res.StudentWallet?>.Failure("Wallet history not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getwallethistory", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getwallethistory", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
