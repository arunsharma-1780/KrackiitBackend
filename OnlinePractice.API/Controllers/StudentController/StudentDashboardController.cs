using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Controllers;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Models;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentDashboardController : BaseController
    {

        private readonly ILogger<StudentDashboardController> _logger;
        public readonly IStudentRepository _studentRepository;
        private readonly IMockTestRepository _mockTestRepository;
        private readonly IStudentMockTestRepository _studentMockTestRepository;
        private readonly IStudentDashboardValidation _studentDashboardValidation;
        public StudentDashboardController(
         ILogger<StudentDashboardController> logger, IMockTestRepository mockTestRepository, IStudentMockTestRepository studentMockTestRepository,IStudentDashboardValidation studentDashboardValidation,
         IStudentRepository studentRepository)
        {
            _logger = logger;
            _mockTestRepository = mockTestRepository;
            _studentDashboardValidation = studentDashboardValidation;
            _studentRepository = studentRepository;
            _studentMockTestRepository = studentMockTestRepository;

        }
        [HttpPost]
        [Authorize]
        [Route("getmocktestlistbyinstitute")]
        public async Task<IActionResult> GetMocktestListByInstitute(Req.MockTestInstitute institute)
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
                var validation = await _studentDashboardValidation.InstituteMockTestValidator.ValidateAsync(institute);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                #region SubCourse Id Check

                #endregion
                institute.UserId = this.UserId;
                var result = await _studentMockTestRepository.GetMockTestListByInstitute(institute);
                if (result != null && result.InstituteMockTest.Any())
                {
                    _logger.LogInformation("MockTest list gets Successfully!");
                    return Ok(ResponseResult<Res.InstituteMockTestList>.Success("MockTest list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest not Found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest not Found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmocktestlistbyinstitute", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getmocktestlistbyinstitute", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
