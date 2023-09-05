using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using OnlinePractice.API.Validator.Services.Student_Services;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers.StudentController
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class MyPurchasedController : BaseController
    {
        private readonly ILogger<MyPurchasedController> _logger;
        public readonly IMyPurchasedRespository _purchasedRespository;
        public readonly IStudentRepository _studentRepository;
        public readonly IMyPurchasedValidation _validation;
        public readonly ISubjectRepository _subjectRepository;
        public readonly ITopicRepository topicRepository;



        public MyPurchasedController(ILogger<MyPurchasedController> logger, IStudentRepository studentRepository, IMyPurchasedRespository purchasedRespository,
            IMyPurchasedValidation validation,ISubjectRepository subjectRepository, ITopicRepository topicRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _purchasedRespository = purchasedRespository;
            _validation = validation;
            _subjectRepository = subjectRepository;
            this.topicRepository = topicRepository;
        }


        /// <summary>
        /// Create myPurchased Data
        /// </summary>
        /// <param name="myPurchased"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("createmypurchased")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> Create(Req.CreateMyPurchased myPurchased)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateMyPurchasedValidator.ValidateAsync(myPurchased);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                myPurchased.UserId = this.UserId;
                var result = await _purchasedRespository.CreateMyPurchased(myPurchased);

                if (result)
                {
                    _logger.LogInformation("Purchased created successfully!");
                    return Ok(ResponseResult<bool>.Success("Purchased created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Purchased not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Purchased not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createmypurchased", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createmypurchased", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }



        /// <summary>
        /// Get Student Purchased Modules List
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getmodulelist")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetStudentModules()
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
                //var validation = await _validation.GetMyPurchasedMocktestValidator.ValidateAsync(sti);
                //errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                //if (errorResponse != null)
                //{
                //    return BadRequest(errorResponse);
                //}
                #endregion

                var result =  _purchasedRespository.GetStudentModules(user);
                if (result != null && result.studentModules.Any())
                {
                    _logger.LogInformation("Modules list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentModulesList?>.Success("Modules list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not Found!");
                    return Ok(ResponseResult<Res.StudentModulesList?>.Failure("Modules not Found!", new()));
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


        /// <summary>
        /// Get Student Purchase mocktest
        /// </summary>
        /// <param name="purchasedMocktest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getpurchasedmocktest")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPurchasedMocktest(Req.MyPurchasedMocktest purchasedMocktest)
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
                purchasedMocktest.UserId= this.UserId;
                var result = await _purchasedRespository.GetPurchasedMocktest(purchasedMocktest);
                if (result != null && result.MyPurchasedMockTests.Any())
                {
                    _logger.LogInformation("Purchased Mocktest list gets Successfully!");
                    return Ok(ResponseResult<Res.MyPurchasedMockTestsLists?>.Success("Purchased Mocktest list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not Found!");
                    return Ok(ResponseResult<Res.StudentModulesList?>.Failure("Modules not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedmocktest", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedmocktest", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get purchased Ebooks 
        /// </summary>
        /// <param name="myPurchasedEbook"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getpurchasedebooks")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPurchasedEbooks(Req.MyPurchasedEbook myPurchasedEbook)
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
                var validation = await _validation.GetMyPurchasedEbooksValidator.ValidateAsync(myPurchasedEbook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                myPurchasedEbook.UserId= this.UserId;
                var result = await _purchasedRespository.GetPurchasedEbooks(myPurchasedEbook);
                if (result != null && result.MyPurchasedEbooks.Any())
                {
                    _logger.LogInformation("Purchased Ebooks list gets Successfully!");
                    return Ok(ResponseResult<Res.MyPurchasedEbooksLists?>.Success("Purchased Ebooks list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not Found!");
                    return Ok(ResponseResult<Res.MyPurchasedEbooksLists?>.Failure("Modules not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedebooks", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedebooks", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// GetMy Purchased Video
        /// </summary>
        /// <param name="myPurchasedVideo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
       // [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getpurchasedvideos")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPurchasedVideos(Req.MyPurchasedVideo myPurchasedVideo)
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
                var validation = await _validation.GetMyPurchasedVideosValidator.ValidateAsync(myPurchasedVideo);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                myPurchasedVideo.UserId = this.UserId;
                var result = await _purchasedRespository.GetPurchasedVideos(myPurchasedVideo);
                if (result != null && result.MyPurchasedVideos.Any())
                {
                    _logger.LogInformation("Purchased Videos list gets Successfully!");
                    return Ok(ResponseResult<Res.MyPurchasedVideosLists?>.Success("Purchased Videos list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not Found!");
                    return Ok(ResponseResult<Res.MyPurchasedVideosLists?>.Failure("Modules not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedvideos", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedvideos", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// My purchased PYP
        /// </summary>
        /// <param name="myPurchasedPreviousYearPAper"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getpurchasedpreviousyearpaper")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPurchasedPreviousYearPaper(Req.MyPurchasedPreviousYearPAper myPurchasedPreviousYearPAper)
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
                var validation = await _validation.GetMyPurchasedPreviousYearPaperValidator.ValidateAsync(myPurchasedPreviousYearPAper);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                myPurchasedPreviousYearPAper.UserId = this.UserId;
                var result = await _purchasedRespository.GetPurchasedPreviousYearPaper(myPurchasedPreviousYearPAper);
                if (result != null && result.MyPurchasedPYPs.Any())
                {
                    _logger.LogInformation("Purchased Papers list gets Successfully!");
                    return Ok(ResponseResult<Res.MyPurchasedPYPLists?>.Success("Purchased Papers list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Modules not Found!");
                    return Ok(ResponseResult<Res.MyPurchasedPYPLists?>.Failure("Modules not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedpreviousyearpaper", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getpurchasedpreviousyearpaper", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
