using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentEbookController : BaseController
    {
        private readonly ILogger<StudentEbookController> _logger;
        private readonly IStudentEbookRespository _studentEbookRespository;
        private readonly IStudentRepository _studentRespository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IStudentEbookValidation _studentEbookValidation;
        private readonly ISubjectRepository _subjectRepository;

        public StudentEbookController(IStudentEbookRespository studentEbookRespository, IStudentRepository studentRespository,
            ILogger<StudentEbookController> logger, IInstituteRepository instituteRepository, ISubCourseRepository subCourseRepository,
            IStudentEbookValidation studentEbookValidation, ISubjectRepository subjectRepository)
        {
            _studentEbookRespository = studentEbookRespository;
            _studentRespository = studentRespository;
            _logger = logger;
            _instituteRepository = instituteRepository;
            _subCourseRepository = subCourseRepository;
            _studentEbookValidation = studentEbookValidation;
            _subjectRepository = subjectRepository;
        }



        /// <summary>
        /// Get Ebooks SubjectList by institute
        /// </summary>
        /// <param name="ebooksSubjects"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getsubjectlistbyinstitute")]
        public async Task<IActionResult> GetSubjectListByInstitute(Req.EbooksSubjects ebooksSubjects)
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
                var validation = await _studentEbookValidation.SubjectListByInstituteValidator.ValidateAsync(ebooksSubjects);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentEbookRespository.GetEbookSubjects(ebooksSubjects);
                if (result != null && result.ebookSubjects.Any())
                {
                    _logger.LogInformation("Subjects list gets Successfully!");
                    return Ok(ResponseResult<Res.EbookSubjectsList?>.Success("Subjects list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Subjects not Found!");
                    return Ok(ResponseResult<Res.EbookSubjectsList>.Failure("Subjects not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlistbyinstitute", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlistbyinstitute", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get Ebook list by subject id
        /// </summary>
        /// <param name="studentEbook"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getebookslistbysubject")]
        public async Task<IActionResult> GetEbooksListBySubject(Req.GetStudentEbook studentEbook)
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
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentEbookValidation.EbookListValidator.ValidateAsync(studentEbook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                studentEbook.UserId = this.UserId;
                var result = await _studentEbookRespository.GetStudentsEbook(studentEbook);
                if (result != null && result.StudentEbookList.Any())
                {
                    _logger.LogInformation("Ebook list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentEbooksList?>.Success("Ebook list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebook not Found!");
                    return Ok(ResponseResult<Res.StudentEbooksList?>.Failure("Ebook not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getebookslistbysubject", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getebookslistbysubject", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// getEbook Data by Id
        /// </summary>
        /// <param name="getEbook"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getebookbyid")]
        public async Task<IActionResult> GetEbookById(Req.GetEbookById getEbook)
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
                var validation = await _studentEbookValidation.GetEbookValidator.ValidateAsync(getEbook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                getEbook.UserId = this.UserId;
                var result = await _studentEbookRespository.GetEbook(getEbook);
                if (result != null)
                {
                    _logger.LogInformation("Ebook gets Successfully!");
                    return Ok(ResponseResult<Res.GetEbook?>.Success("Ebook gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebook not Found!");
                    return Ok(ResponseResult<Res.GetEbook?>.Failure("Ebook not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getebookbyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getebookbyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}
