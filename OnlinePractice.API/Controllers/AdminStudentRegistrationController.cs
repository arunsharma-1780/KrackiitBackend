using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Validator.Interfaces;
using OnlinePractice.API.Models.Common;
using IronXL;
using OfficeOpenXml;
using System.Data;
using Microsoft.Extensions.Logging;
using OnlinePractice.API.Models.AuthDB;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminStudentRegistrationController : BaseController
    {
        private readonly ILogger<AdminStudentRegistrationController> _logger;
        private readonly IStudentRegistrationRepository _studentRegistrationRepository;
        private readonly IStudentRegistrationValidation _validation;
        private readonly IStaffRepository _staffRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IFileRepository _fileRepository;
        public AdminStudentRegistrationController
            (
            ILogger<AdminStudentRegistrationController> logger,
            IStudentRegistrationRepository studentRegistrationRepository,
            IStudentRegistrationValidation validation,
            IStaffRepository staffRepository,
            ISubCourseRepository subCourseRepository,
            IFileRepository fileRepository
            )
        {
            _logger = logger;
            _studentRegistrationRepository = studentRegistrationRepository;
            _validation = validation;
            _staffRepository = staffRepository;
            _subCourseRepository = subCourseRepository;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// Add student from Admin side
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("addstudent")]
        public async Task<IActionResult> Create([FromBody] Req.AddStudent model)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.AddStudentValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentRegistrationRepository.AddStudent(model);
                if (result != null && result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Success(result.Message, result));
                }
                else if (result != null && !result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Failure(result.Message, result));
                }
                else
                {
                    _logger.LogCritical("Student not added!");
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Failure("Student not added!", new Com.ResultMessageAdmin()));
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
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Upload excel for adding student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("bulkupload")]
        public async Task<IActionResult> BulkUpload([FromForm] Req.StudentBulkUpload student)
        {

            ErrorResponse? errorResponse;
            try
            {
                Req.BulkUpload bulkUpload = new()
                {
                    file = student.file,
                    UserId = this.UserId
                };
                var result = await _studentRegistrationRepository.BulkUpload(bulkUpload);
                if (result)
                {
                    _logger.LogInformation("Student uploaded successfully!");
                    return Ok(ResponseResult<bool>.Success("Student uploaded successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not added!");
                    return Ok(ResponseResult<bool>.Failure("Student not added!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "bulkupload", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "bulkupload", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// Get sample excel file
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getsampleexcel")]
        public IActionResult DownloadExcelFile()
        {
            // create a sample data table
            DataTable dt = new DataTable("SampleDataTable");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Full Name"), new DataColumn("Email"), new DataColumn("Mobile Number"), new DataColumn("Password") ,new DataColumn("Amount") });
            // create an Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SampleWorksheet");
                worksheet.Cells.LoadFromDataTable(dt, true);
                // convert the Excel package to a byte array
                var fileBytes = package.GetAsByteArray();
                // set the content type and file name
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "StudentBulkUpload.xlsx";
                // create a file stream result
                var stream = new MemoryStream(fileBytes);
                var fileStreamResult = new FileStreamResult(stream, contentType);
                // set the file download name
                fileStreamResult.FileDownloadName = fileName;
                return fileStreamResult;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getsampleexcelurl")]
        public IActionResult GetExcelFile()
        {
            ErrorResponse? errorResponse;
            try
            {
                
                var result =  _fileRepository.GetSampleExcelUrl();
                if (result != null)
                {
                    _logger.LogInformation("Get sample file  url successfully!");
                    return Ok(ResponseResult<string>.Success("Get sample file  url successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Url not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Url not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsampleexcelurl", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsampleexcelurl", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Update student details by Admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("updatestudent")]
        public async Task<IActionResult> Update(Req.UpdateStudent model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.UpdateStudentValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentRegistrationRepository.EditStudent(model);
                if (result != null && result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Success(result.Message, result));
                }
                else if (result != null && !result.Result)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Failure(result.Message, result));
                }               
                else
                {
                    _logger.LogCritical("Student not updated!");
                    return Ok(ResponseResult<Com.ResultMessageAdmin>.Failure("Student not updated!", new Com.ResultMessageAdmin()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updatestudent", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updatestudent", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get all  student list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("getall")]
        public async Task<IActionResult> GetAllStudent(Req.GetAllStudent model)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllStudentValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Validate SubCourse
                if (model.SubCourseId != Guid.Empty)
                {
                    Req.SubCourseById subCourse = new()
                    {
                        Id = model.SubCourseId
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
                if (model.InstituteId != Guid.Empty)
                {
                    Req.InstituteCheck instituteCheck = new()
                    {
                        InstituteId = model.InstituteId
                    };

                    var isDuplicate = await _staffRepository.IsDuplicate(instituteCheck);
                    if (isDuplicate)
                    {
                        _logger.LogInformation("Institute is not Exists!");
                        return Ok(ResponseResult<DBNull>.Failure("Institute is not Exists!"));
                    }
                }
                #endregion
                var result = await _studentRegistrationRepository.GetAllStudent(model);
                if (result != null && result.StudentDetails.Any())
                {
                    _logger.LogInformation("Get all students  successfully!");
                    return Ok(ResponseResult<Res.StudentList>.Success("Get all students  successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not found!");
                    return Ok(ResponseResult<Res.StudentList>.Failure("Student not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get student details by Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("getbyid")]
        public async Task<IActionResult> GetById(Req.GetStudentById model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetStudentByIdValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentRegistrationRepository.GetStudentById(model);
                if (result != null)
                {
                    _logger.LogInformation("Get student details successfully!");
                    return Ok(ResponseResult<Res.StudentByIdInfo>.Success("Get student details successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not found!");
                    return Ok(ResponseResult<Res.StudentByIdInfo>.Failure("Student not found!", new()));
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
        /// Delete student by Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.GetStudentById model)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetStudentByIdValidator.ValidateAsync(model);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _studentRegistrationRepository.RemoveStudent(model);
                if (result)
                {
                    _logger.LogInformation("Student deleted successfully!");
                    return Ok(ResponseResult<bool>.Success("Student deleted successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not deleted!");
                    return Ok(ResponseResult<bool>.Failure("Student not deleted!", result));
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

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("updaterecords")]
        public async Task<IActionResult> UpdateRecords()
        {
            ErrorResponse? errorResponse;
            try
            {

                var result = await _studentRegistrationRepository.UpdateRecords();
                if (result)
                {
                    _logger.LogInformation("Student deleted successfully!");
                    return Ok(ResponseResult<bool>.Success("Student deleted successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Student not deleted!");
                    return Ok(ResponseResult<bool>.Failure("Student not deleted!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updaterecords", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updaterecords", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        [HttpPost]
        [Route("uploadsample")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.ProfileImage image)
        {
            ErrorResponse? errorResponse;
            try
            {

                
                string result = await _fileRepository.UploadExcel(image.Image);

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Excel uploaded successfully!");
                    return Ok(ResponseResult<string>.Success("Excel uploaded successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Excel not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Excel not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadsample", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadsample", ex);
                return Ok(ResponseResult<DBNull>.Failure("Something went wrong!"));
            }
        }

        [HttpPost]
        [Route("removesample")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveImage()
        {
            ErrorResponse? errorResponse;
            try
            {
                bool result =  await _fileRepository.RemoveSampleExcel();

                if (result)
                {
                    _logger.LogInformation("Remove excel successfully!");
                    return Ok(ResponseResult<bool>.Success("Remove excel successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Excel not removed!");
                    return Ok(ResponseResult<DBNull>.Failure("Excel not removed!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removesample", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removesample", ex);
                return Ok(ResponseResult<DBNull>.Failure("Institute Logo not created!"));
            }
        }

        [HttpPost]
        [Route("removestudentexcel")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveExcel()
        {
            ErrorResponse? errorResponse;
            try
            {
                bool result = await _fileRepository.RemoveStudentExcel();

                if (result)
                {
                    _logger.LogInformation("Remove excel successfully!");
                    return Ok(ResponseResult<bool>.Success("Remove excel successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Excel not removed!");
                    return Ok(ResponseResult<DBNull>.Failure("Excel not removed!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removestudentexcel", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removestudentexcel", ex);
                return Ok(ResponseResult<DBNull>.Failure("Institute Logo not created!"));
            }
        }
    }
}
