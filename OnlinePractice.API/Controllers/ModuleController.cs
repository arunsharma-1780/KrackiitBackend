using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ModuleController : BaseController
    {
        private readonly ILogger<ModuleController> _logger;
        private readonly IModuleRepository _moduleRepository;
        private readonly IModuleValidation _validation;
        public ModuleController(ILogger<ModuleController> logger, IModuleRepository moduleRepository, IModuleValidation validation) { _logger = logger; _moduleRepository = moduleRepository; _validation = validation; }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateModule module)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateModuleValidator.ValidateAsync(module);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Module Type
                Req.CheckModuleType moduleType = new()
                {
                    ModuleType = module.ModuleType
                };
                var checkModuleType = await _moduleRepository.CheckModuleType(moduleType);
                if (!checkModuleType)
                {
                    _logger.LogInformation("Module Type already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Module Type already exist!"));
                }
                #endregion
                var result = await _moduleRepository.Create(module);
                if (result)
                {
                    _logger.LogInformation("Module added successfully!");
                    return Ok(ResponseResult<bool>.Success("Module added successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Module not added!");
                    return Ok(ResponseResult<DBNull>.Failure("Module not added!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditModule module)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditModuleValidator.ValidateAsync(module);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _moduleRepository.Edit(module);
                if (result)
                {
                    _logger.LogInformation("Module updated successfully!");
                    return Ok(ResponseResult<bool>.Success("Module updated successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Module not updated!");
                    return Ok(ResponseResult<DBNull>.Failure("Module not updated!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.GetModule module)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetModuleValidator.ValidateAsync(module);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _moduleRepository.Delete(module);
                if (result)
                {
                    _logger.LogInformation("Module deleted successfully!");
                    return Ok(ResponseResult<bool>.Success("Module deleted successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Module not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Module not deleted!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("getall")]
        public async Task<IActionResult> GetAll()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _moduleRepository.GetAll();
                if (result != null && result.ModuleDetails.Any())
                {
                    _logger.LogInformation("Module list get successfully!");
                    return Ok(ResponseResult<Res.ModuleDetailsList>.Success("Module list get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Module not found!");
                    return Ok(ResponseResult<Res.ModuleDetailsList>.Failure("Module not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }




    }
}
