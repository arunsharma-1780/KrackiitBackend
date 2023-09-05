using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Validator;
using Org.BouncyCastle.Ocsp;
using OnlinePractice.API.Repository.Services.StudentServices;
using FluentValidation;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentPYPController : BaseController
    {
        private readonly ILogger<StudentPYPController> _logger;
        private readonly IStudentRepository _studentRespository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IStudentPYPRepository _studentPYPRepository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly IStudentPYPValidation _studentPYPValidation;

        public StudentPYPController(ILogger<StudentPYPController> logger, IStudentRepository studentRepository, ISubCourseRepository subCourseRepository
            , IInstituteRepository instituteRepository, IStudentPYPValidation studentPYPValidation, IStudentPYPRepository studentPYPRepository)
        {
            _logger = logger;
            _studentRespository = studentRepository;
            _subCourseRepository = subCourseRepository;
            _instituteRepository = instituteRepository;
            _studentPYPValidation = studentPYPValidation;
            _studentPYPRepository = studentPYPRepository;
        }


        /// <summary>
        /// Get All PreviousYearPaper Data
        /// </summary>
        /// <param name="pypInstitutes"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getpyplistbyinstitute")]
        public async Task<IActionResult> GetPreviousYearPaperListByInstitute(Req.PYPInstitutes pypInstitutes)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token.
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentPYPValidation.PYPListByInstituteValidator.ValidateAsync(pypInstitutes);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = pypInstitutes.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = pypInstitutes.SubcourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion

                var result = await _studentPYPRepository.GetPreviousYearPaper(pypInstitutes);
                if (result != null && result.previousYearPapers.Any())
                {
                    _logger.LogInformation("Previous Year Paper list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentPYPList>.Success("Previous Year Paper list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Previous Year Paper not Found!");
                    return Ok(ResponseResult<DBNull>.Failure("Previous Year Paper not Found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpyplistbyinstitute", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpyplistbyinstitute", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get All PreviousYear  Data by Filter
        /// </summary>
        /// <param name="studentPreviousYear"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getpyplistbyfilters")]
        public async Task<IActionResult> GetPreviousYearPaperListByFilters(Req.StudentPreviousYearPaperFilter studentPreviousYear)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token.
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentPYPValidation.PapersDataByFilterValidator.ValidateAsync(studentPreviousYear);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = studentPreviousYear.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = studentPreviousYear.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                studentPreviousYear.UserId = this.UserId;
                var result = await _studentPYPRepository.GetPapersDataByFilter(studentPreviousYear);
                if (result != null && result.previousYearPapers.Any())
                {
                    _logger.LogInformation("Previous Year Paper list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentPYPList?>.Success("Previous Year Paper list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Previous Year Paper not Found!");
                    return Ok(ResponseResult<Res.StudentPYPList?>.Failure("Previous Year Paper not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpyplistbyfilters", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpyplistbyfilters", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getpricewisesort")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetPriceWiseSort()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _studentPYPRepository.GetPriceWiseSort();
                if (result != null)
                {
                    _logger.LogInformation("Pricing wise sort list get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("Pricing wise sort list get get successfully!", result));
                }
                else
                {
                    _logger.LogCritical(" Pricing wise sort not found!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Failure("Pricing wise sort not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpricewisesort", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpricewisesort", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
