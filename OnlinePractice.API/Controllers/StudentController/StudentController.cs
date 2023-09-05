using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        public readonly IStudentRepository _studentRepository;
        public readonly IStudentValidation _validation;
        private readonly ILogger<StudentController> _logger;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IInstituteRepository _instituteRepository;

        public StudentController(IStudentRepository studentrepository,
            ILogger<StudentController> logger, IStudentValidation validation, IInstituteRepository instituteRepository, ISubCourseRepository subCourseRepository)
        {
            _studentRepository = studentrepository;
            _logger = logger;
            _validation = validation;
            _instituteRepository = instituteRepository;
            _subCourseRepository = subCourseRepository;
        }


        /// <summary>
        /// Create Student Contoller Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("studentsignup")]
        public async Task<IActionResult> StudentSignup([FromBody] CreateStudent student)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateStudentValidator.ValidateAsync(student);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                #region SubCourse Id Check
                //Req.SubCourseById subCourse = new()
                //{
                //    Id = student.SubcourseId
                //};

                //var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                //if (!isSubCourseExist)
                //{
                //    _logger.LogInformation("Invalid SubCourse Id!");
                //    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                //}
                //#endregion
                //#region Institute Id Check
                //Req.InstituteById institute = new()
                //{
                //    Id = student.InstituteId
                //};
                //var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                //if (!isInstituteExist)
                //{
                //    _logger.LogInformation("Invalid Institute Id!");
                //    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                //}
                #endregion

                var studentData = await _studentRepository.AddStudent(student);
                if (studentData.Result)
                {
                    _logger.LogInformation("Student Added Successfully!");
                    return Ok(ResponseResult<string>.Success(studentData.Message, studentData.Token));
                }
                else
                {
                    _logger.LogCritical("Student not Added!");
                    return Ok(ResponseResult<DBNull>.Failure(studentData.Message));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addstudent", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addstudent", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        ///Get Student Contoller Method
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getstudentprofile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.StudentById>))]
        public async Task<IActionResult> GetProfileById(GetUserById admin)
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
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model

                #endregion                //var validation = await _validation.GetUserByIdValidator.ValidateAsync(admin);
                //errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                //if (errorResponse != null)
                //{
                //    return BadRequest(errorResponse);
                //}
                var result = await _studentRepository.GetStudentById(admin);
                if (result != null)
                {
                    _logger.LogInformation("Student Details get successfully!");
                    return Ok(ResponseResult<Res.StudentById>.Success(" Student get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not found!");
                    return Ok(ResponseResult<Res.StudentById>.Failure("Student not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getStudentprofile", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getStudentprofile", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// Edit Student Contoller Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("editstudent")]
        public async Task<IActionResult> EditStudent([FromBody] EditStudent student)
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
                var validation = await _validation.EditStudentValidator.ValidateAsync(student);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                student.UserId = UserId;
                var studentData = await _studentRepository.EditStudent(student);
                if (studentData.Result)
                {
                    _logger.LogInformation("Student Added Successfully!");
                    return Ok(ResponseResult<string>.Success(studentData.Message, studentData.Token));
                }
                else
                {
                    _logger.LogCritical("Student not Added!");
                    return Ok(ResponseResult<DBNull>.Failure(studentData.Message));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstudent", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstudent", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Login Student Contoller Method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("studentlogin")]
        public async Task<IActionResult> Login([FromBody] StudentLogin model)
        {
            ErrorResponse? errorResponse;
            try
            {
              
                #region Validate Request Model
                if (string.IsNullOrEmpty(model.MobileNumber) && string.IsNullOrEmpty(model.Email))
                {
                    _logger.LogInformation("Either MobileNumber or Email requierd!");
                    return Ok(ResponseResult<DBNull>.Failure("Either MobileNumber or Email requierd!"));
                }

                if (!string.IsNullOrEmpty(model.MobileNumber))
                {
                    var validationMobile = await _validation.MobileValidatior.ValidateAsync(model);
                    errorResponse = CustomResponseValidator.CheckModelValidation(validationMobile);
                    if (errorResponse != null)
                    {
                        return BadRequest(errorResponse);
                    }
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var validationEmail = await _validation.EmailValidatior.ValidateAsync(model);
                    errorResponse = CustomResponseValidator.CheckModelValidation(validationEmail);
                    if (errorResponse != null)
                    {
                        return BadRequest(errorResponse);
                    }
                }
                var validation = await _validation.StudentLoginValidatior.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region
                var result = await _studentRepository.StudentLogin(model);
                //if (result != null && result.Token == "403")
                //{
                //    _logger.LogCritical("User is already login on another device!");
                //    return Ok(ResponseResult<string>.Failure("User is already login on another device!"));
                //}
                if (result != null)
                {
                    _logger.LogInformation("Logged in successfully!");
                    return Ok(ResponseResult<Tokens>.Success("Logged in successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Please enter valid credentials.");
                    return Ok(ResponseResult<string>.Failure("Please enter valid credentials."));
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "studentlogin", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Get token by Mobilenumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("gettokenbynumber")]
        public async Task<IActionResult> GetTokenByNumber([FromBody] Tokenlogin model)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetTokenByNumberValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region
                var result = await _studentRepository.GetTokenByNumber(model);
                if (result != null)
                {
                    _logger.LogInformation("token gets successfully!");
                    return Ok(ResponseResult<Tokens>.Success("token gets successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Please enter valid Mobile Number.");
                    return Ok(ResponseResult<string>.Failure("Please enter valid Mobile Number."));
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "login", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Send OTP Contoller Method
        /// </summary>
        /// <param name="mobilenumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendotp")]
        public async Task<IActionResult> SendOTP(SendOTP sendOTP)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.SendOTPValidator.ValidateAsync(sendOTP);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentRepository.SendOtp(sendOTP);
                if (result.isOtpSent)
                {
                    _logger.LogInformation("Otp sent successfully!");
                    return Ok(ResponseResult<Res.SendOtp>.Success("Otp sent successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Otp not sent!");
                    return Ok(ResponseResult<string>.Failure(result.MID));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "sendotp", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "sendotp", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Check SMS or OTP Balance Contoller Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("checksmsbalance")]
        public async Task<IActionResult> CheckBalance()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _studentRepository.CheckSMSBalance();
                if (result != null)
                {
                    _logger.LogInformation("Get SMS balance successfully!");
                    return Ok(ResponseResult<Res.SMSBalance>.Success("Get SMS balance successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Balance not fount!");
                    return Ok(ResponseResult<DBNull>.Failure("Balance not fount!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "checksmsbalance", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "checksmsbalance", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotStudentPassword password)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.ForgotPasswordValidator.ValidateAsync(password);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                if (password.NewPassword != password.ConfirmPassword)
                {
                    _logger.LogInformation("New Password and  Confirm Password do not match!");
                    return Ok(ResponseResult<DBNull>.Failure("New Password and  Confirm Password do not match!"));
                }
                password.UserId = UserId;
                var result = await _studentRepository.ForgotPassword(password);
                if (result)
                {
                    _logger.LogInformation("Password updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Password updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Password not updated!");
                    return Ok(ResponseResult<DBNull>.Failure("Password not updated!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "forgotpassword", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "forgotpassword", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Edit Student Contoller Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("editstudentprofile")]
        public async Task<IActionResult> EditStudentProfile([FromBody] Req.UpdateStudentProfile student)
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
                var validation = await _validation.EditStudentProfileValidator.ValidateAsync(student);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                student.UserId = UserId;
                var studentData = await _studentRepository.EditStudentProfile(student);
                if (studentData.Result)
                {
                    _logger.LogInformation("Student Edited Successfully!");
                    return Ok(ResponseResult<string>.Success(studentData.Message));
                }
                else
                {
                    _logger.LogCritical("Student not Found!");
                    return Ok(ResponseResult<DBNull>.Failure(studentData.Message));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstudentprofile", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "editstudentprofile", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }



        [HttpPost]
        [Authorize]
        [Route("deletestudent")]
        public async Task<IActionResult> DeleteStudent([FromBody] MobNumber number)
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
                var validation = await _validation.MobNumberValidator.ValidateAsync(number);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                number.UserId = UserId;
                var studentData = await _studentRepository.DeleteUser(number);
                if (studentData)
                {
                    _logger.LogInformation("User deleted Successfully!");
                    return Ok(ResponseResult<DBNull>.Success("User deleted Successfully!"));
                }
                else
                {
                    _logger.LogCritical("User not Found!");
                    return Ok(ResponseResult<DBNull>.Failure("User not Found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "deletestudent", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "deletestudent", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }



        [HttpPost]
        [Authorize]
        [Route("addfeedback")]
        public async Task<IActionResult> AddStudentFeedback([FromBody] AddFeedback addFeedback)
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
                var validation = await _validation.AddFeedbackValidator.ValidateAsync(addFeedback);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                addFeedback.UserId = UserId;
                var studentData = await _studentRepository.AddFeedback(addFeedback);
                if (studentData)
                {
                    _logger.LogInformation("Feedback Added Successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Feedback Added Successfully!"));
                }
                else
                {
                    _logger.LogCritical("Feedback not Added!");
                    return Ok(ResponseResult<DBNull>.Failure(" Feedback not Added!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addfeedback", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addfeedback", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Route("uploadimage")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.ProfileImage image)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.StudentProfileImageValidator.ValidateAsync(image);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _studentRepository.UploadImage(image);

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Profile Picture Updated successfully!");
                    return Ok(ResponseResult<string>.Success("Profile Picture Updated successfully!!", result));
                }
                else
                {
                    _logger.LogCritical("Profile Picture not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Profile Picture not uploaded!"));
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
                return Ok(ResponseResult<DBNull>.Exception("Institute Logo not created!"));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getstudentinstitute")]
        public async Task<IActionResult> GetStudentInstitue()
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
                var studentData = await _studentRepository.GetStudentInstitute(user);
                if (studentData != null)
                {
                    _logger.LogInformation("Student institute get Successfully!");
                    return Ok(ResponseResult<Res.StudentInstitueList>.Success("Student institute get Successfully!", studentData));
                }
                else
                {
                    _logger.LogCritical("Student institute not Found!");
                    return Ok(ResponseResult<Res.StudentInstitueList>.Failure("Student institute not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentinstitute", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getstudentinstitute", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}