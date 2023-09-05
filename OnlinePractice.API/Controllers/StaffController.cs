using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using System.Text;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;


namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : BaseController
    {
        public readonly IStaffRepository _staffRepository;
        public readonly IStaffValidation _validation;
        public readonly IModuleRepository _moduleRepository;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IStaffRepository staffRepository,
            ILogger<StaffController> logger,
            IStaffValidation validation,
            IModuleRepository moduleRepository)
        {
            _staffRepository = staffRepository;
            _logger = logger;
            _validation = validation;
            _moduleRepository = moduleRepository;
        }
        /// <summary>
        /// Api for create staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("createstaff")]
        public async Task<IActionResult> CreateStaff([FromBody] Req.CreateStaff staff)
        {
            ErrorResponse? errorResponse;
            try
            {

                #region Validate Request Model
                var validation = await _validation.CreateStaffValidator.ValidateAsync(staff);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Module
                if (!staff.Permission.Modules.Any())
                {
                    _logger.LogInformation("Atleast one module permission is required!");
                    return Ok(ResponseResult<DBNull>.Failure("Atleast one module permission is required!"));
                }
                var isModule = await _moduleRepository.CheckModule(staff.Permission.Modules);
                if (!isModule)
                {
                    _logger.LogInformation("Module is not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Module is not exist!"));
                }
                #endregion
                #region Check InstituteCodeDuplicacy
                Req.InstituteCheck instituteCheck = new()
                {
                    InstituteId = staff.InstituteId
                };

                var isDuplicate = await _staffRepository.IsDuplicate(instituteCheck);
                if (isDuplicate)
                {
                    _logger.LogInformation("Institute is not Exists!");
                    return Ok(ResponseResult<DBNull>.Failure("Institute is not Exists!"));
                }
                #endregion
 
                staff.UserId = this.UserId;
                var result = await _staffRepository.AddStaff(staff);
                if (result != null && result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Success(result.Message,result));
                }
                if (result != null && !result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Failure(result.Message,result));
                }
                else
                {
                    _logger.LogCritical("Staff not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Staff not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createstaff", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createstaff", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// APi for get all staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getallstaff")]
        public async Task<IActionResult> GetAllStaff(Req.GetAllStaff staff)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllStaffValidator.ValidateAsync(staff);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                staff.UserId = this.UserId;
                var result = await _staffRepository.GetAllStaff(staff);
                if (result.StaffList.Count > 0)
                {
                    _logger.LogInformation("Get all staff successfully!");
                    return Ok(ResponseResult<Res.StaffData>.Success("Get all staff successfully!", result));
                }
                else
                {
                    _logger.LogCritical("staff not found!");
                    return Ok(ResponseResult<Res.StaffData>.Failure("Staff not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallstaff", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallstaff", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        [HttpGet]
        [Authorize]
        [Route("getallstaffandadmin")]
        public async Task<IActionResult> GetAllStaffandAdmin()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _staffRepository.GetAllStaffAndAdmin();
                if (result.StaffList.Count > 0)
                {
                    _logger.LogInformation("Get all staff and admin successfully!");
                    return Ok(ResponseResult<Res.StaffAdminData>.Success("Get all staff and admin successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Record not found!");
                    return Ok(ResponseResult<Res.StaffAdminData>.Failure("Record not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallstaffandadmin", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallstaffandadmin", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Api for get staff detail
        /// by id
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstaffbyid")]
        public async Task<IActionResult> GetStafffById(Req.GetByIdStaff staff)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetByIdStaffValidator.ValidateAsync(staff);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _staffRepository.GetStaffById(staff);
                if (result != null)
                {
                    _logger.LogInformation("Get staff details successfully!");
                    return Ok(ResponseResult<Res.StaffById>.Success("Get staff details successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Staff not found!");
                    return Ok(ResponseResult<Res.StaffById>.Failure("Staff not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstaffbyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstaffbyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Api for edit staff details
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("editstaff")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateStaff([FromBody] Req.EditStaff staff)
        {
            ErrorResponse? errorResponse;
            try
            {
      
                #region Validate Request Model
                var validation = await _validation.EditStaffValidator.ValidateAsync(staff);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Module

                var isModule = await _moduleRepository.CheckEditModule(staff.Permission.Modules);
                if (!isModule)
                {
                    _logger.LogInformation("Module is not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Module is not exist!"));
                }
                #endregion
                #region Permission Check
                bool permissionCheck = false;
                foreach (var permission in staff.Permission.Modules)
                {
                    if (permission.Value)
                    {
                        permissionCheck = true;
                    }   
                }
                if (!permissionCheck)
                {
                    _logger.LogInformation("Atleast one module permission is required!");
                    return Ok(ResponseResult<DBNull>.Failure("Atleast one module permission is required!"));
                }
#endregion
                staff.UserId = this.UserId;
                var result = await _staffRepository.UpdateStaff(staff);
                if (result)
                {
                    _logger.LogInformation("Staff updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Staff updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Staff not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Staff not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstaff", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstaff", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        ///Api for Delete staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deletestaff")]
        public async Task<IActionResult> RemoveStaff(Req.GetByIdStaff staff)
        {
            ErrorResponse? errorResponse;
            try
            {

                #region Validate Request Model
                var validation = await _validation.GetByIdStaffValidator.ValidateAsync(staff);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                staff.UserId = this.UserId;
                var result = await _staffRepository.RemoveStaff(staff);
                if (result)
                {
                    _logger.LogInformation("Staff deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Staff deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Staff not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Staff not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "deletestaff", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "deletestaff", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Api for get all module list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getmodulelist")]
        public async Task<IActionResult> GetModuleList()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _staffRepository.GetModuleList();
                if (result != null)
                {
                    _logger.LogInformation("Get all modules successfully!");
                    return Ok(ResponseResult<Res.Module>.Success("Get all modules successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not found!");
                    return Ok(ResponseResult<Res.Module>.Failure("Modules not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmodulelist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmodulelist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
