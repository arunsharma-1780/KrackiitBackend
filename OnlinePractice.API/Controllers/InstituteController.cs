using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Microsoft.AspNetCore.Authorization;

namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class InstituteController : BaseController
    {
        private readonly IInstituteRepository _instituteRepostiory;
        private readonly ILogger<InstituteController> _logger;
        private readonly IInstituteValidation _validation;
        public InstituteController(IInstituteRepository instituteRepostiory, ILogger<InstituteController> logger, IInstituteValidation validation)
        {
            _instituteRepostiory = instituteRepostiory;
            _logger = logger;
            _validation = validation;
            _validation = validation;
        }

        /// <summary>
        /// Upload Image Contoller
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("uploadimage")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.LogoImage image)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.instituteLogoValidation.ValidateAsync(image);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _instituteRepostiory.UploadImage(image);

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Institute Logo upload successfully!");
                    return Ok(ResponseResult<string>.Success("Institute Logo upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Institute Logo not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Institute Logo not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadimage", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadimage", ex);
                return Ok(ResponseResult<DBNull>.Failure("Institute Logo not created!"));
            }
        }

        /// <summary>
        /// create product
        /// </summary>
        /// <param name="createProduct"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateInstitute createInstitute)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateInstituteValidator.ValidateAsync(createInstitute);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check InstituteCodeDuplicacy
                Req.CodeCheck codeCheck= new()
                {
                    InstituteCode = createInstitute.InstituteCode
                };

                var isDuplicate = await _instituteRepostiory.IsDuplicate(codeCheck);
                if (isDuplicate)
                {
                    _logger.LogInformation("InstituteCode is already exists!");
                    return Ok(ResponseResult<DBNull>.Failure("InstituteCode is already exists!"));
                }
                #endregion


                createInstitute.UserId = this.UserId;
                var result = await _instituteRepostiory.Create(createInstitute);

                if (result)
                {
                    _logger.LogInformation("Institute created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Institute created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Institute not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Institute not created!"));
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
                return Ok(ResponseResult<DBNull>.Failure("Institute not created!"));
            }
        }


        /// <summary>
        /// Get All institutes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.InstituteList>))]
        public async Task<IActionResult> GetAllInstitutes(Req.GetAllInstitute institute)
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _instituteRepostiory.GetAllInstitutes(institute);

                if (result.Institutes.Any())
                {
                    _logger.LogInformation("record get successfully");
                    return Ok(ResponseResult<Res.InstituteList>.Success("Institutes gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("Institutes not found!");
                    return Ok(ResponseResult<Res.InstituteList>.Failure("Institutes not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallinstitutes", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallinstitutes", ex);
                return Ok(ResponseResult<DBNull>.Failure("Institutes not Found!"));
            }
        }

        /// <summary>
        /// getall instituteCode
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getallinstitutecode")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.InstituteListV1>))]
        public async Task<IActionResult> GetAllInstitutesV1()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _instituteRepostiory.GetAllInstitutesV1();

                if (result.Institutes.Any())
                {
                    _logger.LogInformation("record get successfully");
                    return Ok(ResponseResult<Res.InstituteListV1>.Success("Institutes gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("Institutes not found!");
                    return Ok(ResponseResult<Res.InstituteListV1>.Failure("Institutes not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallinstitutes", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallinstitutes", ex);
                return Ok(ResponseResult<DBNull>.Failure("Institutes not Found!"));
            }
        }


        /// <summary>
        /// edit product
        /// </summary>
        /// <param name="editProduct"></param>
        /// <returns></returns>
       // [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [Authorize]
        [Route("edit")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(Req.EditInstitute editInstitute)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditInstituteValidator.ValidateAsync(editInstitute);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check InstituteCodeDuplicacy
                Req.EditCodeCheck codeCheck = new()
                {
                    Id = editInstitute.Id,
                    InstituteCode = editInstitute.InstituteCode
                };

                var isDuplicate = await _instituteRepostiory.IsEditDuplicate(codeCheck);
                if (isDuplicate)
                {
                    _logger.LogInformation("InstituteCode is already exists!");
                    return Ok(ResponseResult<DBNull>.Failure("InstituteCode is already exists!"));
                }
                #endregion
                editInstitute.UserId = this.UserId;
                var result = await _instituteRepostiory.Edit(editInstitute);
                if (result)
                {
                    _logger.LogInformation("Institute edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Institute edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Institute not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Institute not edited!"));
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
                return Ok(ResponseResult<DBNull>.Failure("Institute not edited"));
            }

        }


        /// <summary>
        /// get Institute details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Institute>))]
        public async Task<IActionResult> GetById(Req.InstituteById institute)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetInstituteByIdValidator.ValidateAsync(institute);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _instituteRepostiory.GetById(institute);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.Institute>.Success("Institute get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.Institute>.Failure("Institute not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Delete InstituteById
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.InstituteById institute)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteInstituteValidator.ValidateAsync(institute);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                institute.UserId = this.UserId;
                var result = await _instituteRepostiory.Delete(institute);
                if (result)
                {
                    _logger.LogInformation("Institute deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Institute deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Institute not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Institute not deleted!"));
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
