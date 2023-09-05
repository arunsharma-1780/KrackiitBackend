using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Org.BouncyCastle.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OnlinePractice.API.Models.Request;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using OfficeOpenXml;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]

    public class AdminResultController : BaseController
    {
        private readonly ILogger<AdminResultController> _logger;
        private readonly IAdminResultRepository _adminResultRepository;
        private readonly IAdminResultValidation _validation;
        private readonly IStaffRepository _staffRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        public AdminResultController
            (
            ILogger<AdminResultController> logger,
            IAdminResultRepository adminResultRepository,
            IAdminResultValidation validation,
         IStaffRepository staffRepository,
     ISubCourseRepository subCourseRepository
            )
        {
            _logger = logger;
            _adminResultRepository = adminResultRepository;
            _validation = validation;
            _staffRepository = staffRepository;
            _subCourseRepository = subCourseRepository;
        }
        [HttpPost]
        [Authorize]
        [Route("getallmocktest")]
        public async Task<IActionResult> GetAllResults(Req.GetMockTestList mockTest)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetMockTestListValidator.ValidateAsync(mockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Validate SubCourse
                if (mockTest.SubCourseId != Guid.Empty)
                {
                    Req.SubCourseById subCourse = new()
                    {
                        Id = mockTest.SubCourseId
                    };
                    var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                    if (!isSubCourseExist)
                    {
                        _logger.LogInformation("Invalid SubCourse Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                    }
                }
                #endregion
                #region Check InstituteCodeDuplicacy
                if (mockTest.InstituteId != Guid.Empty)
                {
                    Req.InstituteCheck instituteCheck = new()
                    {
                        InstituteId = mockTest.InstituteId
                    };

                    var isDuplicate = await _staffRepository.IsDuplicate(instituteCheck);
                    if (isDuplicate)
                    {
                        _logger.LogInformation("Institute is not Exists!");
                        return Ok(ResponseResult<DBNull>.Failure("Institute is not Exists!"));
                    }
                }
                #endregion
                var result = await _adminResultRepository.GetAllMockTest(mockTest);
                if (result != null && result.MockTestInfoModels.Any())
                {
                    _logger.LogInformation("Get all mocktest successfully!");
                    return Ok(ResponseResult<Res.MockTestList>.Success("Get all mocktest successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not found!");
                    return Ok(ResponseResult<Res.MockTestList>.Failure("MockTest not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktest", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktest", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get all results by subcourse and institute
        /// </summary>
        /// <param name="adminResult"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("getallresults")]
        public async Task<IActionResult> GetAllResults(Req.GeAdminResult adminResult)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GeAdminResultValidator.ValidateAsync(adminResult);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Validate SubCourse
                if (adminResult.SubCourseId != Guid.Empty)
                {
                    Req.SubCourseById subCourse = new()
                    {
                        Id = adminResult.SubCourseId
                    };
                    var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                    if (!isSubCourseExist)
                    {
                        _logger.LogInformation("Invalid SubCourse Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                    }
                }
                #endregion
                #region Check InstituteCodeDuplicacy
                if (adminResult.InstituteId != Guid.Empty)
                {
                    Req.InstituteCheck instituteCheck = new()
                    {
                        InstituteId = adminResult.InstituteId
                    };

                    var isDuplicate = await _staffRepository.IsDuplicate(instituteCheck);
                    if (isDuplicate)
                    {
                        _logger.LogInformation("Institute is not Exists!");
                        return Ok(ResponseResult<DBNull>.Failure("Institute is not Exists!"));
                    }
                }
                #endregion
                adminResult.UserId = this.UserId;
                var result = await _adminResultRepository.GetAllResults(adminResult);
                if (result != null && result.StudentResults.Any())
                {
                    _logger.LogInformation("Get all student results successfully!");
                    return Ok(ResponseResult<Res.AdminStudentResultList>.Success("Get all student results successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Results not found!");
                    return Ok(ResponseResult<Res.AdminStudentResultList>.Failure("Results not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallresults", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallresults", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get mocktest wise student rank
        /// </summary>
        /// <param name="resultByMockTestId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("getresultbymocktestid")]
        public async Task<IActionResult> GetResultByMocktTestId(Req.GeResultByMockTestId resultByMockTestId)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GeResultByMockTestIdValidator.ValidateAsync(resultByMockTestId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                resultByMockTestId.UserId = this.UserId;
                var result = await _adminResultRepository.GetResultByMockTestId(resultByMockTestId);
                if (result != null)
                {
                    _logger.LogInformation("Get student details successfully!");
                    return Ok(ResponseResult<Res.StudentResultDetail>.Success("Get student details successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<Res.StudentResultDetail>.Failure("Result not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultbymocktestid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultbymocktestid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getallresultanalysis")]
        public async Task<IActionResult> GetResultStudentAnalysis(Req.GeAdminResult adminResult)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GeAdminResultValidator.ValidateAsync(adminResult);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                adminResult.UserId = this.UserId;
                var result = await _adminResultRepository.GetResultStudentAnalysis(adminResult);
                if (result != null)
                {
                    _logger.LogInformation("Get student details successfully!");
                    return Ok(ResponseResult<Res.ResultAnalysisList>.Success("Get student details successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<Res.ResultAnalysisList>.Failure("Result not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultstudentanalysis", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultstudentanalysis", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("studentresultanalysisdetails")]
        public async Task<IActionResult> GetResultAnalysisDetails(Req.GeResultAnalysisDetail resultAnalysisDetail)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GeResultAnalysisDetailValidator.ValidateAsync(resultAnalysisDetail);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                resultAnalysisDetail.UserId = this.UserId;
                var result = await _adminResultRepository.GetResultAnalysisDetails(resultAnalysisDetail);
                if (result != null)
                {
                    _logger.LogInformation("Get student details successfully!");
                    return Ok(ResponseResult<Res.ResultAnalysisDetail>.Success("Get student details successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<Res.ResultAnalysisDetail>.Failure("Result not found!", new Res.ResultAnalysisDetail()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultanalysisdetails", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultanalysisdetails", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}

