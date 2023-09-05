using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Common;

using Microsoft.AspNetCore.Authorization;

namespace OnlinePractice.API.Controllers
{
    // [ApiExplorerSettings(IgnoreApi = true)]
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class SubCourseController : BaseController
    {
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<SubCourseController> _logger;
        private readonly ISubCourseValidation _validation;
        public SubCourseController(ISubCourseRepository subcourseRepository, ILogger<SubCourseController> logger, ISubCourseValidation validation, ICourseRepository courseRepository)
        {
            _subCourseRepository = subcourseRepository;
            _logger = logger;
            _validation = validation;
            _courseRepository = courseRepository;
        }


        /// <summary>
        /// Create SubCourseData
        /// </summary>
        /// <param name="createSubCourse"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateSubCourse createSubCourse)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateSubCourseValidator.ValidateAsync(createSubCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region 
                Req.CourseById course = new()
                {
                    Id = createSubCourse.CourseId
                };

                var isCourseExist = await _courseRepository.IsExist(course);
                if (!isCourseExist)
                {
                    _logger.LogInformation("Invalid Course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Course Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.SubCourseName subCourse = new()
                {
                    Name = createSubCourse.SubCourseName,
                    CourseId = createSubCourse.CourseId
                };

                var isDublicate = await _subCourseRepository.IsDuplicate(subCourse);
                if (isDublicate)
                {
                    _logger.LogInformation("SubCourse name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("SubCourse name is already exist!"));
                }
                #endregion

                createSubCourse.UserId = this.UserId;
                var result = await _subCourseRepository.Create(createSubCourse);

                if (result)
                {
                    _logger.LogInformation("SubCourse created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubCourse created successfully!"));
                }
                else
                {
                    _logger.LogCritical("SubCourse not created!");
                    return Ok(ResponseResult<DBNull>.Failure("SubCourse not created!"));
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
        /// Get All SubCourses by CourseId
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getsubcoursesbycourseid")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubCourseDataList>))]
        public async Task<IActionResult> GetAllSubCoursesByCourseId(Req.CourseById courseId)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllSubCourseByCourseIdValidator.ValidateAsync(courseId);
            errorResponse = CustomResponseValidator.CheckModelValidation(validation);
            if (errorResponse != null)
            {
                return BadRequest(errorResponse);
            }
            #endregion
            #region 
            Req.CourseById course = new()
            {
                Id = courseId.Id
            };

            var isCourseExist = await _courseRepository.IsExist(course);
            if (!isCourseExist)
            {
                _logger.LogInformation("Invalid Course Id!");
                return Ok(Result<DBNull>.Failure("Invalid Course Id!"));
            }
            #endregion
           
                var result = await _subCourseRepository.GetSubCoursesByCourseId(courseId);

                if (result != null && result.CourseID != Guid.Empty )
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubCourseDataList>.Success("SubCourses get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.SubCourseDataList>.Failure("SubCourses not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubcoursesbycourseid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubcoursesbycourseid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// Edit SubCourse Data
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Authorize]
        public async Task<IActionResult> Edit(Req.EditSubCourse subCourse)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditSubCourseValidator.ValidateAsync(subCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region 
                Req.CourseById course = new()
                {
                    Id = subCourse.CourseId
                };

                var isCourseExist = await _courseRepository.IsExist(course);
                if (!isCourseExist)
                {
                    _logger.LogInformation("Invalid Course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Course Id!"));
                }
                #endregion
                subCourse.UserId = this.UserId;
                var result = await _subCourseRepository.Edit(subCourse);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubCourse edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("SubCourse not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("SubCourse not edited!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        /// <summary>
        /// EditMultiple SubCourse
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editmultiple")]
        [Authorize]
        public async Task<IActionResult> EditMultiple(Req.EditMultipleSubCourse subCourse)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditMultipleSubCourseValidator.ValidateAsync(subCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region 
                Req.CourseById course = new()
                {
                    Id = subCourse.CourseID
                };

                var isCourseExist = await _courseRepository.IsExist(course);
                if (!isCourseExist)
                {
                    _logger.LogInformation("Invalid Course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Course Id!"));
                }
                #endregion
                subCourse.UserId = this.UserId;
                var result = await _subCourseRepository.EditMultiple(subCourse);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubCourse edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("SubCourse not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("SubCourse not edited!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// Get subcourses by Id
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubCourse>))]
        public async Task<IActionResult> GetById(Req.SubCourseById subCourse)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetSubCourseByIdValidator.ValidateAsync(subCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _subCourseRepository.GetById(subCourse);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubCourse>.Success("SubCourse get successfully", result));
                }
                else
                {
                    _logger.LogInformation("SubCourse not found!");
                    return Ok(ResponseResult<Res.SubCourse>.Failure("SubCourse not found!",new()));
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
        /// Delete Course
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> Delete(Req.SubCourseById subCourse)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteSubCourseValidator.ValidateAsync(subCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                subCourse.UserId = this.UserId;
                var result = await _subCourseRepository.DeleteAll(subCourse);
                if (result)
                {
                    _logger.LogInformation("SubCourse deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubCourse deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("SubCourse not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("SubCourse not deleted!"));
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

    }
}
