using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : BaseController
    {
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IAdminDashboardValidation _validation;
        private readonly IAdminDashboardRepository _adminDashboardRepository;
        public AdminDashboardController(ILogger<AdminDashboardController> logger, 
            IAdminDashboardValidation validation,
            IAdminDashboardRepository adminDashboardRepository)
        {
            _logger = logger;
            _validation = validation;
            _adminDashboardRepository = adminDashboardRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("getdurationfilterlist")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetDurationFilter()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _adminDashboardRepository.GetDurationFilter();
                if (result != null)
                {
                    _logger.LogInformation("Duration filter get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("Duration filter get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Duration filter not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Duration filter not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getdurationfilterlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getdurationfilterlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("gettotalsales")]
        public async Task<IActionResult> GetTotalSales(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetTotalSale(model);
                if (result != null)
                {
                    _logger.LogInformation("Admin sales get successfully!");
                    return Ok(ResponseResult<Res.TotalSaleDetails>.Success("Admin sales get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Admin sales not found!");
                    return Ok(ResponseResult<Res.TotalSaleDetails>.Failure("Admin sales not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalsales", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalsales", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("gettotalenrollment")]
        public async Task<IActionResult> GetTotalEnrollment(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetTotalEnrollment(model);
                if (result != null)
                {
                    _logger.LogInformation("Admin enrollment get successfully!");
                    return Ok(ResponseResult<Res.TotalEnrollmentDetails>.Success("Admin  enrollment get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Admin  enrollment not found!");
                    return Ok(ResponseResult<Res.TotalEnrollmentDetails>.Failure("Admin  enrollment not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalenrollment", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalenrollment", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = false)]
        [Authorize(Roles = "Admin")]
        [Route("getinstitutestudent")]
        public async Task<IActionResult> GetInstituteStudent(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetInstituteStudentCourse(model);
                if (result != null)
                {
                    _logger.LogInformation("Institute student get successfully!");
                    return Ok(ResponseResult<Res.InstituteStudent>.Success("Institute student get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Institute student not found!");
                    return Ok(ResponseResult<Res.InstituteStudent>.Failure("Institute student not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getinstitutestudent", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getinstitutestudent", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = false)]
        [Authorize(Roles = "Admin")]
        [Route("getrecenttransactionandtimeline")]
        public async Task<IActionResult> GetRecentTransaction()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _adminDashboardRepository.GetRecentTransactionsandTimeLineList();
                if (result != null)
                {
                    _logger.LogInformation("Recent transactions get successfully!");
                    return Ok(ResponseResult<Res.RecentTransactionandTimeLineList>.Success("Recent transactions get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Recent transactions not found!");
                    return Ok(ResponseResult<Res.RecentTransactionandTimeLineList>.Failure("Recent transactions not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getrecenttransactionandtimeline", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getrecenttransactionandtimeline", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Admin")]
        [Route("gettimeline")]
        public async Task<IActionResult> GetTimeLine()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _adminDashboardRepository.GetTimeLine();
                if (result != null && result.TimeLines.Any())
                {
                    _logger.LogInformation("Time line get successfully!");
                    return Ok(ResponseResult<Res.TimeLineList>.Success("Time line get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Time line not found!");
                    return Ok(ResponseResult<Res.TimeLineList>.Failure("Time line not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettimeline", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettimeline", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = false)]
        [Authorize(Roles = "Admin")]
        [Route("getstudentenrollmentgraph")]
        public async Task<IActionResult> GetEnrollmentGraph(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetStudentEnrollmentGraph(model);
                if (result != null)
                {
                    _logger.LogInformation("Get student  enroll graph successfully!");
                    return Ok(ResponseResult<List<KeyValuePair<int, int>>?>.Success("Get student  enroll graph successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Graph not found!");
                    return Ok(ResponseResult<List<KeyValuePair<int, int>>?>.Failure("Graph not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("gettotalsalesgraph")]
        public async Task<IActionResult> GetTotalSalesGraph(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetTotalSalesGraph(model);
                if (result != null)
                {
                    _logger.LogInformation("Get student total sales graph successfully!");
                    return Ok(ResponseResult<List<KeyValuePair<int, double>>>.Success("Get student total sales graph successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Graph not found!");
                    return Ok(ResponseResult<List<KeyValuePair<int, double>>>.Failure("Graph not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalsalesgraph", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "gettotalsalesgraph", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [Route("V1")]
        public async Task<IActionResult> GetEnrollmentGraphV1(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetStudentEnrollmentGraphV1(model);
                if (result != null)
                {
                    _logger.LogInformation("Get student  enroll graph successfully!");
                    return Ok(ResponseResult<List<KeyValuePair<DateTime, int>>>.Success("Get student  enroll graph successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Graph not found!");
                    return Ok(ResponseResult<List<KeyValuePair<DateTime, int>>>.Failure("Graph not found!", new List<KeyValuePair<DateTime, int>>()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [Route("V2")]
        public async Task<IActionResult> GetEnrollmentGraphV2(Req.FilterModel model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.FilterModelValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _adminDashboardRepository.GetTotalSalesGraphV2(model);
                if (result != null)
                {
                    _logger.LogInformation("Get student  enroll graph successfully!");
                    return Ok(ResponseResult<List<KeyValuePair<DateTime, double>>>.Success("Get student  enroll graph successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Graph not found!");
                    return Ok(ResponseResult<List<KeyValuePair<DateTime, double>>>.Failure("Graph not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentenrollmentgraph", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
