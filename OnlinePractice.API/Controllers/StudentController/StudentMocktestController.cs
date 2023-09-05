using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Models;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Services;
using FluentValidation;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Services.StudentServices;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentMocktestController : BaseController
    {
        private readonly ILogger<StudentDashboardController> _logger;
        private readonly IStudentMockTestRepository _studentMockTestRepository;
        private readonly IMockTestRepository _mockTestRepository;
        public readonly IStudentRepository _studentRepository;
        private readonly IStudentMocktestValidation _studentMocktestValidation;
        public StudentMocktestController(
         ILogger<StudentDashboardController> logger, IMockTestRepository mockTestRepository, IStudentMockTestRepository studentMockTestRepository, IStudentMocktestValidation studentMocktestValidation,
         IStudentRepository studentRepository
         )
        {
            _logger = logger;
            _studentMockTestRepository = studentMockTestRepository;
            _studentMocktestValidation = studentMocktestValidation;
            _studentRepository = studentRepository;
            _mockTestRepository = mockTestRepository;
        }
        /// <summary>
        /// Get mocktest list on behalf 
        /// of filters
        /// </summary>
        /// <param name="mockTest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentmocktestbyfilters")]
        public async Task<IActionResult> GetMocktestListByInstitute(Req.StudentMockTest mockTest)
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
                var validation = await _studentMocktestValidation.StudentGetMockTestsValidator.ValidateAsync(mockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Language

                var language = _studentRepository.CheckStudentLanguage(mockTest.LanguageFilter.ToString());

                if (!language)
                {
                    _logger.LogInformation("Invalid Language!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Language!"));
                }
                #endregion

                mockTest.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetMockTestListByFilters(mockTest);
                if (result != null && result.StudentMockTests.Any())
                {
                    _logger.LogInformation("MockTest list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentMockTestList>.Success("MockTest list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not Found!");
                    return Ok(ResponseResult<Res.StudentMockTestList>.Failure("MockTest not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentmocktestbyfilters", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentmocktestbyfilters", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Get Mocktest language list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getallmocktestlanguage")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllMockTestLanguage()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _studentMockTestRepository.GetMockTestLanguage();
                if (result != null)
                {
                    _logger.LogInformation("MockTest Language get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("MockTest Language get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest Language not found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Language not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestlanguage", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestlanguage", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get all mocktest status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getallmockteststatus")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllMockTestStatus()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _studentMockTestRepository.GetMockTestStatus();
                if (result != null)
                {
                    _logger.LogInformation("MockTest status get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("MockTest status get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest status not found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest status not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmockteststatus", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmockteststatus", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Get all custom mocktest
        /// list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getallcustomemockteststatus")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetCustomeMockTestStatus()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _studentMockTestRepository.GetCustomeMockTestStatus();
                if (result != null)
                {
                    _logger.LogInformation("MockTest status get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("MockTest status get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest status not found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest status not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallcustomemockteststatus", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallcustomemockteststatus", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get all pricing filter list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getallmocktestpricing")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllMockTestPricing()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _studentMockTestRepository.GetMockTestPricing();
                if (result != null)
                {
                    _logger.LogInformation("MockTest Pricing filter get successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("MockTest Pricing filter get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest Pricing filter not found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Pricing filter not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestpricing", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestpricing", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        ///API for Generate custome mocktest
        /// </summary>
        /// <param name="mockTest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("generatestudentmocktest")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GenerateStudentMockTest(Req.StudentAutomaticMockTestQuestion mockTest)
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
                var validation = await _studentMocktestValidation.StudentAutomaticMockTestQuestionValidator.ValidateAsync(mockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Language

                var language = _studentRepository.CheckStudentLanguage(mockTest.Language.ToString());

                if (!language)
                {
                    _logger.LogInformation("Invalid Language!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Language!"));
                }
                #endregion
                #region CheckNameDuplicacy
                Req.CustomeMockTestNameCheck mockTestName = new()
                {
                    MockTestName = mockTest.MockTestName,
                    UserId = this.UserId
                };
                var isDuplicate = await _studentMockTestRepository.IsCustomeDuplicate(mockTestName);
                if (isDuplicate)
                {
                    _logger.LogInformation("MockTest name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest name is already exist!"));
                }
                #endregion

                mockTest.UserId = this.UserId;
                var result = await _studentMockTestRepository.GenerateAutomaticMockTestForStudent(mockTest);
                if (result != null && result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<bool>.Success(result.Message, result.Result));
                }
                else if (result != null && !result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<bool>.Failure(result.Message));
                }
                else
                {
                    _logger.LogCritical("MockTest not created!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "generatestudentmocktest", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "generatestudentmocktest", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// API for get all custom
        /// mocktest list
        /// </summary>
        /// <param name="mockTest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getcustomemocktestlist")]
        public async Task<IActionResult> GetCustomeMocktestListByInstitute(Req.CustomeStudentMockTest mockTest)
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
                var validation = await _studentMocktestValidation.CustomeStudentMockTestValidator.ValidateAsync(mockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                mockTest.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetCustomeMockTestListByFilter(mockTest);
                if (result != null && result.StudentMockTests.Any())
                {
                    _logger.LogInformation("MockTest list gets Successfully!");
                    return Ok(ResponseResult<Res.CustomeStudentMockTestList>.Success("MockTest list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not Found!");
                    return Ok(ResponseResult<Res.CustomeStudentMockTestList>.Failure("MockTest not Found!", new Res.CustomeStudentMockTestList()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcustomemocktestlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcustomemocktestlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for get student question 
        /// panel list
        /// </summary>
        /// <param name="questionPanel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentquestionpanel")]
        public async Task<IActionResult> GetStudentQuestionPanel(Req.GetStudentQuestionPanel questionPanel)
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
                var validation = await _studentMocktestValidation.GetStudentQuestionPanelValidator.ValidateAsync(questionPanel);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                questionPanel.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetStudentQuestionPanel(questionPanel);
                if (result != null)
                {
                    _logger.LogInformation("MockTest question list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentMocktestPanel>.Success("MockTest question list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not Found!");
                    return Ok(ResponseResult<Res.StudentMocktestPanel>.Failure("MockTest not Found!", new Res.StudentMocktestPanel()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentquestionpanel", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentquestionpanel", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for update student
        /// visited the question
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("markasseen")]
        public async Task<IActionResult> SaveStudentAnswer(Req.MarkAsSeen response)
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
                var validation = await _studentMocktestValidation.MarkAsSeenValidator.ValidateAsync(response);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                response.UserId = this.UserId;
                var result = await _studentMockTestRepository.MarkAsSeen(response);
                if (result)
                {
                    _logger.LogInformation("Mark as seen save Successfully!");
                    return Ok(ResponseResult<bool>.Success("Mark as seen save Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mark as seen not saved!");
                    return Ok(ResponseResult<bool>.Failure("Mark as seen not saved!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "markasseen", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "markasseen", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// API for saving student
        /// responses
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("savestudentanswers")]
        public async Task<IActionResult> SaveStudentAnswer(Req.StudentQuestionResponse response)
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
                var validation = await _studentMocktestValidation.StudentQuestionResponseValidator.ValidateAsync(response);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                response.UserId = this.UserId;
                var result = await _studentMockTestRepository.SaveStudentResponses(response);
                if (result != null)
                {
                    _logger.LogInformation("Student answer save Successfully!");
                    return Ok(ResponseResult<Res.StudentQuestionResponseV2>.Success("Student answer save  Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Answer not saved!");
                    return Ok(ResponseResult<Res.StudentQuestionResponseV2>.Failure("Answer not saved!", new Res.StudentQuestionResponseV2()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "savestudentanswers", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "savestudentanswers", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// API for get all student 
        /// answer list
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentanswers")]
        public async Task<IActionResult> GetStudentAnswers(Req.StudentAnwersPanel response)
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
                var validation = await _studentMocktestValidation.StudentAnwersPanelValidator.ValidateAsync(response);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                response.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetStudentAnswerPanel(response);
                if (result != null && result.StudentQuestionResponse.Any())
                {
                    _logger.LogInformation("Student answer get Successfully!");
                    return Ok(ResponseResult<Res.StudentAnswersPanelList>.Success("Student answer get  Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Answers not found!");
                    return Ok(ResponseResult<Res.StudentMocktestPanelList>.Failure("Answer not found!", new Res.StudentMocktestPanelList())) ;
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentanswers", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentanswers", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API to get student correct
        /// answer for review
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("reviewstudentanswers")]
        public async Task<IActionResult> GetStudentAnswers(Req.ReviewAnswer response)
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
                var validation = await _studentMocktestValidation.ReviewAnswerValidator.ValidateAsync(response);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                response.UserId = this.UserId;
                var result = await _studentMockTestRepository.ReviewStudentAnswer(response);
                if (result != null)
                {
                    _logger.LogInformation("Student answer get Successfully!");
                    return Ok(ResponseResult<Res.StudentQuestionResponseV2>.Success("Student answer get  Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Answers not found!");
                    return Ok(ResponseResult<Res.StudentQuestionResponseV2>.Failure("Answer not found!", new Res.StudentQuestionResponseV2()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "reviewstudentanswers", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "reviewstudentanswers", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// API for remove student 
        /// answer
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("removestudentanswers")]
        public async Task<IActionResult> RemoveStudentAnswers(Req.RemoveAnswer response)
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
                var validation = await _studentMocktestValidation.RemoveAnswerValidator.ValidateAsync(response);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                response.UserId = this.UserId;
                var result = await _studentMockTestRepository.RemoveStudentAnser(response);
                if (result)
                {
                    _logger.LogInformation("Student answer removed Successfully!");
                    return Ok(ResponseResult<bool>.Success("Student answer removed  Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student answer not found!");
                    return Ok(ResponseResult<bool>.Failure("Student answer not found!",result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removestudentanswers", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removestudentanswers", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for get mocktest
        /// general instructions
        /// </summary>
        /// <param name="mockTestId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getgeneralinstructions")]
        public async Task<IActionResult> GetGeneralInstructions(Req.StudentMockTestId mockTestId)
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
                var validation = await _studentMocktestValidation.StudentMockTestIdValidator.ValidateAsync(mockTestId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestIdCheckExists
                Req.CheckMockTestById checkMockTest = new()
                {
                    MockTestSettingId = mockTestId.MockTestId
                };

                var IsMockTestSettingIdExists = await _mockTestRepository.CheckMockTestById(checkMockTest);
                if (!IsMockTestSettingIdExists && !mockTestId.IsCustom)
                {
                    _logger.LogInformation("Invalid MockTest Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid MockTest Id!"));
                }
                #endregion

                mockTestId.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetGeneralInstructions(mockTestId);
                if (result != null && !string.IsNullOrEmpty(result.GeneralInstruction))
                {
                    _logger.LogInformation("General instructions get Successfully!");
                    return Ok(ResponseResult<Res.GeneralInstructions>.Success("General instructions get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("General instructions not found!");
                    return Ok(ResponseResult<DBNull>.Failure("General instructions not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getgeneralinstrictions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getgeneralinstrictions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for update student
        /// mocktest status
        /// </summary>
        /// <param name="studentMockTestStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("savemockteststatus")]
        public async Task<IActionResult> SaveMockTestStatus(Req.StudentMockTestStatus studentMockTestStatus)
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
                var validation = await _studentMocktestValidation.StudentMockTestStatusValidator.ValidateAsync(studentMockTestStatus);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                studentMockTestStatus.UserId = this.UserId;
                var result = await _studentMockTestRepository.SaveMockTestStatus(studentMockTestStatus);
                if (result != null && result.Id != Guid.Empty && !result.IsCompleted)
                {
                    _logger.LogInformation("MockTest status saved Successfully!");
                    return Ok(ResponseResult<DBNull>.Success("MockTest status saved Successfully!!"));
                }
                if (result != null && result.Id != Guid.Empty && result.IsCompleted)
                {
                    _logger.LogInformation("MockTest status saved Successfully!");
                    return Ok(ResponseResult<Guid>.Success("MockTest status saved Successfully!!", result.Id));
                }
                else
                {
                    _logger.LogCritical("MockTest status not added!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest status not added!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "savemockteststatus", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "savemockteststatus", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// API for get student result
        /// </summary>
        /// <param name="mockTestId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentresult")]  
        public async Task<IActionResult> GetStudentResults(Req.GetResult mockTestId)
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
                var validation = await _studentMocktestValidation.GetResultValidator.ValidateAsync(mockTestId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                mockTestId.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetStudentResults(mockTestId);
                if (result != null)
                {
                    _logger.LogInformation("Student result get Successfully!");
                    return Ok(ResponseResult<Res.StudentResults>.Success("Student result get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<Res.StudentResults>.Failure("Result not found!", new Res.StudentResults()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentresult", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentresult", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for get student all results
        /// </summary>
        /// <param name="mockTestId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentallpreviousresult")]
        public async Task<IActionResult> GetStudentPreviousResults(Req.GetStudentResult mockTestId)
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
                var validation = await _studentMocktestValidation.GetStudentResultValidator.ValidateAsync(mockTestId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestIdCheckExists
                #endregion

                mockTestId.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetStudentPreviousResults(mockTestId);
                if (result != null)
                {
                    _logger.LogInformation("Student result get Successfully!");
                    return Ok(ResponseResult<Res.StudentResultList>.Success("Student result get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Result not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentallpreviousresult", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentallpreviousresult", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getstudentquestionsolution")]
        public async Task<IActionResult> GetStudentQuestionSolution(Req.GetStudentQuestionSolution questionPanel)
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
                var validation = await _studentMocktestValidation.GetStudentQuestionSolutionValidator.ValidateAsync(questionPanel);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                questionPanel.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetStudentQuestionSolution(questionPanel);
                if (result != null)
                {
                    _logger.LogInformation("MockTest question solution list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentQuestionSolution>.Success("MockTest question solution list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not Found!");
                    return Ok(ResponseResult<Res.StudentMocktestPanel>.Failure("MockTest not Found!", new Res.StudentMocktestPanel()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentquestionpanel", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentquestionpanel", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}
