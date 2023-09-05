    using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Services;

namespace OnlinePractice.API.Controllers
{
   // [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class CourseController : BaseController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseValidation _validation;
        public CourseController(ICourseRepository courseRepository, ILogger<CourseController> logger, ICourseValidation validation, IExamTypeRepository examTypeRepository)
        {
            _courseRepository = courseRepository;
            _logger = logger;
            _validation = validation;
            _examTypeRepository = examTypeRepository;
        }


        /// <summary>
        /// Create CourseData
        /// </summary>
        /// <param name="createCourse"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateCourse createCourse)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateCourseValidator.ValidateAsync(createCourse);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region 
                Req.ExamTypeById exam = new()
                {
                    Id =  createCourse.ExamTypeId
                };

                var isExamExist = await _examTypeRepository.IsExist(exam);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Exam Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Exam Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.CourseName course = new()
                {
                    Name = createCourse.CourseName,
                    ExamId = createCourse.ExamTypeId
                };

                var isDuplicate = await _courseRepository.IsDuplicate(course);
                if (isDuplicate)
                {
                    _logger.LogInformation("Coursename is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Coursename is already exist!"));
                }
                #endregion

                createCourse.UserId = this.UserId;
                var result = await _courseRepository.Create(createCourse);

                if (result)
                {
                    _logger.LogInformation("Course created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Course created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Course not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Course not created!"));
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
        /// GetAll Courses by ExamtypeID
        /// </summary>
        /// <param name="ExamTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getcoursesbyexamid")]
        public async Task<IActionResult> GetCoursesByExamId(Req.GetExamId examId)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetExamIdValidator.ValidateAsync(examId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region 
                Req.ExamTypeById exam = new()
                {
                    Id = examId.ExamId
                };

                var isExamExist = await _examTypeRepository.IsExist(exam);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Exam Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Exam Id!"));
                }
                #endregion

                var result = await _courseRepository.GetCoursesByExamId(examId);

                if (result.Courses.Count > 0)
                {
                    _logger.LogInformation("Course get successfully");
                    return Ok(ResponseResult<Res.CourseList>.Success("Courses get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Course not found!");
                    return Ok(ResponseResult<Res.CourseList>.Failure("Courses not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcoursesbyexamid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcoursesbyexamid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        [HttpGet]
        [Authorize]
        [Route("getallcourses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.CourseList>))]
        public async Task<IActionResult> GetAllCourses()
        {
            ErrorResponse? errorResponse;
            try
            {

                var result = await _courseRepository.GetAllCourses();

                if (result.Courses.Count > 0)
                {
                    _logger.LogInformation("Get all courses successfully");
                    return Ok(ResponseResult<Res.CourseList>.Success("Get all courses successfully", result));
                }
                else
                {
                    _logger.LogInformation("Course not found!");
                    return Ok(ResponseResult<Res.CourseList>.Failure("Courses not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallcourses", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallcourses", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        /// <summary>
        /// Edit Course Data
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditCourse course)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditCourseValidator.ValidateAsync(course);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region
                Req.ExamTypeById exam = new()
                {
                    Id = course.ExamTypeId
                };

                var isExamExist = await _examTypeRepository.IsExist(exam);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Exam Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Exam Id!"));
                }
                #endregion
                #region Check Dublicacy
                Req.CourseName courses = new()
                {
                    Name = course.CourseName,
                    ExamId = course.ExamTypeId
                };

                var isDublicate = await _courseRepository.IsDuplicate(courses);
                if (isDublicate)
                {
                    _logger.LogInformation("Course name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Course name is already exist!"));
                }
                #endregion
                course.UserId = this.UserId;
                var result = await _courseRepository.Edit(course);
                if (result)
                {
                    _logger.LogInformation("Course edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Course edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Course not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Course not edited!"));
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
        /// get product details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Courses>))]
        public async Task<IActionResult> GetById(Req.CourseById course)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetCourseByIdValidator.ValidateAsync(course);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _courseRepository.GetById(course);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.Courses>.Success("Course get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Course not found!");
                    return Ok(ResponseResult<Res.Courses>.Failure("Course not found!", new()));
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
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.CourseById course)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteCourseValidator.ValidateAsync(course);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                course.UserId = this.UserId;
                var result = await _courseRepository.DeleteAll(course);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Course deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Course not deleted!"));
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


        /// <summary>
        /// Get All Courses by insitituteId
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getcoursesbyinstitute")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.InstituteData>))]
        public async Task<IActionResult> GetCoursesByInstituteId(InstituteDataID institute)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                //var validation = await _validation.GetInstituteByIdValidator.ValidateAsync(institute);
                //errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                //if (errorResponse != null)
                //{
                //    return BadRequest(errorResponse);
                //}
                #endregion
                var result = await _courseRepository.GetInstituteCourseData(institute);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.InstituteData>.Success("Records get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.InstituteData>.Failure("Institute not found!", new()));
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


    }
}
