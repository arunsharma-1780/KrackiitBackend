using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Validator;
using Org.BouncyCastle.Ocsp;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]

    public class StudentResultController : BaseController
    {
        private readonly ILogger<StudentResultController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentResultRepository _studentResultRepository;
        

        public StudentResultController(ILogger<StudentResultController> logger, IStudentResultRepository studentResultRepository, IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _studentResultRepository = studentResultRepository;
        }
        /// <summary>
        /// Get student results
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getoverallresultanalysis")]
        public async Task<IActionResult> GetResultAnalysis()
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
                    #endregion
 
                    var result = await _studentResultRepository.GetResultAnalysis(user);
                    if (result != null)
                    {
                        _logger.LogInformation("Student overall result analysis get Successfully!");
                        return Ok(ResponseResult<Res.StudentResultAnalysis>.Success("Student overall result analysis get Successfully!", result));
                    }
                    else
                    {
                        _logger.LogCritical("Result not found!");
                        return Ok(ResponseResult<Res.StudentResultAnalysis>.Failure("Result not found!", new()));
                    }
                }
                catch (DbUpdateException exp)
                {
                    var ex = exp.InnerException as SqlException;
                    errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                    _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultanalysis", ex);
                    return BadRequest(errorResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getresultanalysis", ex);
                    return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
                }
                       
        }
        /// <summary>
        /// Get student existing mocktest list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getexistingmocktestlist")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetExistingMocktestList()
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
                #endregion

                var result = await _studentResultRepository.GetExistingMockTestList(user);
                if (result != null)
                {
                    _logger.LogInformation("Student existing mocktest list get Successfully!");
                    return Ok(ResponseResult<Res.ExistingMockTestList>.Success("Student existing mocktest list get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest not found!");
                    return Ok(ResponseResult<Res.ExistingMockTestList>.Failure("Mocktest not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getexistingmocktestlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getexistingmocktestlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        [HttpGet]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = false)]
        [Route("getcompletedmocktestlist")]
        public async Task<IActionResult> GetCompletedExistingMocktestList()
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
                #endregion

                var result = await _studentResultRepository.GetCompletedMockTestList(user);
                if (result != null)
                {
                    _logger.LogInformation("Student completed mocktest list get Successfully!");
                    return Ok(ResponseResult<Res.MocktestDataResultList>.Success("Student completed mocktest list get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest not found!");
                    return Ok(ResponseResult<Res.MocktestDataResultList>.Failure("Mocktest not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcompletedexistingmocktestlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcompletedexistingmocktestlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getmocktestwiseperformance")]
        public async Task<IActionResult> GettMocktestWisePerformance(Req.StudentResultMockTestId studentResultMockTest)
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
                #endregion
                studentResultMockTest.UserId = this.UserId;
                var result = await _studentResultRepository.GetMockTestWisePerformance(studentResultMockTest);
                if (result != null)
                {
                    _logger.LogInformation("Student mocktest wise result analysis get Successfully!");
                    return Ok(ResponseResult<Res.StudentMockTestWiseResultAnalysis>.Success("Student mocktest wise result analysis get Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Result not found!");
                    return Ok(ResponseResult<Res.StudentMockTestWiseResultAnalysis>.Failure("Result not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmocktestwiseperformance", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmocktestwiseperformance", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
    }
}
