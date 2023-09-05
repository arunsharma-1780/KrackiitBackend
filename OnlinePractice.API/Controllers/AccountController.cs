using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Newtonsoft.Json;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Org.BouncyCastle.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IAccountValidation _validation;
        private readonly ILogger<AccountController> _logger;


        public AccountController(
            IAccountRepository accountRepository,
             IAccountValidation validation,
            ILogger<AccountController> logger)
        {
            _accountRepository = accountRepository;
            _validation = validation;
            _logger = logger;
        }


        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Req.Login model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.LoginValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region
                var result = await _accountRepository.Login(model);
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
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "login", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Uplaod Admin Profile Picture
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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
                var validation = await _validation.ProfileImageValidator.ValidateAsync(image);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _accountRepository.UploadImage(image);

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
                return Ok(ResponseResult<DBNull>.Failure("Institute Logo not created!"));
            }
        }



        /// <summary>
        /// Api for create admin profile
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createadmin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CreateAdmin([FromBody] Req.Register register)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.RegisterModelValidator.ValidateAsync(register);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var User = await _accountRepository.AddAdmin(register);
                if (User)
                {
                    _logger.LogInformation("Admin created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Admin created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Admin not created!");
                    return Ok(ResponseResult<DBNull>.Success("Admin not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createadmin", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createadmin", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Api for update admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("updateadmin")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UpdateAdmin(Req.UpdateAdmin admin)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.UpdateAdminValidator.ValidateAsync(admin);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                admin.UserId = this.UserId;
                var result = await _accountRepository.UpdateAdmin(admin);
                if (result)
                {
                    _logger.LogInformation("Admin updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Admin updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Admin not updated!");
                    return Ok(ResponseResult<DBNull>.Failure("Admin not updated!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updateadmin", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updateadmin", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPut]
        [Authorize]
        [Route("removeprofileimage")]
        public async Task<IActionResult> Removeprofileimage(Req.RemoveProfile profile)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.RemoveProfileValidator.ValidateAsync(profile);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _accountRepository.RemoveImage(profile);
                if (result)
                {
                    _logger.LogInformation("Profile image remove successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Profile image remove successfully!"));
                }
                else
                {
                    _logger.LogCritical("Profile image not removed!");
                    return Ok(ResponseResult<DBNull>.Failure("Profile image not removed!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removeprofileimage", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removeprofileimage", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Api for get admin profile
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getadminprofile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.UserById>))]
        public async Task<IActionResult> GetProfileById(Req.GetUserById admin)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetUserByIdValidator.ValidateAsync(admin);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _accountRepository.GetUserById(admin);
                if (result != null)
                {
                    _logger.LogInformation(" Details get successfully!");
                    return Ok(ResponseResult<Res.UserById>.Success("Details get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Admin not found!");
                    return Ok(ResponseResult<Res.UserById>.Failure("Admin not found!", new Res.UserById()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getadminprofile", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getadminprofile", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// Api for for admin forget password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forgetpassword")]
        public async Task<IActionResult> ForgetPassword(Req.ForgotPassword password)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.ForgetPasswordValidator.ValidateAsync(password);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _accountRepository.ForgotPassWord(password);
                if (result)
                {
                    _logger.LogInformation("Email send successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Email send successfully!"));
                }
                else
                {
                    _logger.LogCritical("Email does not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Email does not exist!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "forgetpassword", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "forgetpassword", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Api for change password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(Req.ChangePassword password)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.ChangePasswordValidator.ValidateAsync(password);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Password
                Req.CurrentPassword passwordNew = new()
                {
                    Id = password.Id,
                    Password = password.CurrentPassword
                };
                var isPassWordExist = await _accountRepository.CheckPassword(passwordNew);
                if (!isPassWordExist)
                {
                    _logger.LogInformation("Invalid Current Password!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Current Password!"));
                }
                #endregion
                if (password.NewPassword != password.ConfirmPassword)
                {
                    _logger.LogInformation("New Password and  Confirm Password do not match!");
                    return Ok(ResponseResult<DBNull>.Failure("New Password and  Confirm Password do not match!"));
                }
                password.UserId = this.UserId;
                var result =  await _accountRepository.ChangePassWord(password);
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
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "changepassword", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "changepassword", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Api for resend password
        /// for staff
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resendpassword")]
        // [Authorize(Roles =UserRoles.Admin)]
        public async Task<IActionResult> ResendPassword(Req.ForgotPassword password)
        {
            try
            {
                ErrorResponse? errorResponse;
                #region Validate Request Model
                var validation = await _validation.ForgetPasswordValidator.ValidateAsync(password);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _accountRepository.ResendPassword(password);
                if (result)
                {
                    _logger.LogInformation("Password send successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Password send successfully!"));
                }
                else
                {
                    _logger.LogCritical("Email does not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Email does not exist!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "resendpassword", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Route("CheckVersionForDevPurpose")]
        // [Authorize(Roles =UserRoles.Admin)]
        public IActionResult CheckVersion()
        {
            return Ok("06/09/2023");
        }

        #region Logout

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            returnUrl = returnUrl == null ? Request.Scheme + "://" + Request.Host : returnUrl;
            if (User?.Identity?.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
            return Redirect(returnUrl);
        }
        #endregion
    }
}
