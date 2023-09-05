using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Validator;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Validator.Interfaces;
using Org.BouncyCastle.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OnlinePractice.API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminWalletController : BaseController
    {
        private readonly ILogger<AdminWalletController> _logger;
        private readonly IAdminWalletValidation _validation;
        private readonly IAdminWalletRepository _adminWalletRepository;
        public AdminWalletController(
            ILogger<AdminWalletController> logger, IAdminWalletValidation validation, IAdminWalletRepository adminWalletRepository
            )
        {
            _logger = logger;
            _validation = validation;
            _adminWalletRepository = adminWalletRepository;
        }
        /// <summary>
        /// Get all transactions 
        /// </summary>
        /// <param name="adminWallet"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("getalltransactions")]
        public async Task<IActionResult> GetAllTransctions(Req.GetAdminWallet adminWallet)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAdminWalletValidator.ValidateAsync(adminWallet);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminWalletRepository.GetAllTransctions(adminWallet);
                if (result != null)
                {
                    _logger.LogInformation("Get all transactions successfully!");
                    return Ok(ResponseResult<Res.AdminWalletList>.Success("Get all transactions successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Transactions not found!");
                    return Ok(ResponseResult<Res.AdminWalletList>.Failure("Transactions not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltransactions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltransactions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get transaction details
        /// </summary>
        /// <param name="getTransaction"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("gettransactiondetails")]
        public async Task<IActionResult> GetTransctionDetails(Req.GetTransactionDetails getTransaction)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetTransactionDetailsValidator.ValidateAsync(getTransaction);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminWalletRepository.GetTransactionDetails(getTransaction);
                if (result != null)
                {
                    _logger.LogInformation("Transaction details get successfully!");
                    return Ok(ResponseResult<Res.TransactionDetails?>.Success("Transaction details get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Transaction details not found!");
                    return Ok(ResponseResult<Res.TransactionDetails>.Failure("Transaction details not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettransactiondetails", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettransactiondetails", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
