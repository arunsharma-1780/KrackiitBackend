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
using MimeKit.Encodings;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Services;

namespace OnlinePractice.API.Controllers
{
    // [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class ExamTypeController : BaseController
    {
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILogger<ExamTypeController> _logger;
        private readonly IExamTypeValidation _validation;
        public ExamTypeController(IExamTypeRepository examTypeRepository, ILogger<ExamTypeController> logger, IExamTypeValidation validation, ISubjectRepository subjectRepository)
        {
            _examTypeRepository = examTypeRepository;
            _logger = logger;
            _validation = validation;
            _validation = validation;
            _subjectRepository = subjectRepository;
        }

        /// <summary>
        /// Master API for create exam
        /// </summary>
        /// <param name="examFlow"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("createexamflow")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CreateExamMaster(Req.CreateExamFlow examFlow)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateExamFlowValidator.ValidateAsync(examFlow);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check duplicacy
                Req.ExamName exam = new()
                {
                    Name = examFlow.ExamTypeName
                };

                var isDuplicate = await _examTypeRepository.IsDuplicate(exam);
                if (isDuplicate)
                {
                    _logger.LogInformation("ExamName is already exists!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamName is already exists!"));
                }
                #endregion
                #region Validate SubjectIds
                foreach (var item in examFlow.SubjectIds)
                {
                    Req.SubjectById subject = new()
                    {
                        Id = item.SubjectId
                    };
                    var isSubjectExist = await _subjectRepository.IsExist(subject);
                    if (!isSubjectExist)
                    {
                        _logger.LogInformation("Invalid Subject Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                    }
                }
                #endregion
                examFlow.UserId = this.UserId;
                var result = await _examTypeRepository.CreateExamFlow(examFlow);
                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType created successfully!"));
                }
                else
                {
                    _logger.LogCritical("ExamType not created!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType not created!"));
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
                return Ok(ResponseResult<DBNull>.Failure("Something went wrong!"));
            }
        }


        /// <summary>
        /// Create ExamType
        /// </summary>
        /// <param name="createExam"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateExamType createExam)
       {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateExamTypeValidator.ValidateAsync(createExam);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check duplicacy
                Req.ExamName exam = new()
                {
                    Name = createExam.ExamName
                };

                var isDuplicate = await _examTypeRepository.IsDuplicate(exam);
                if (isDuplicate)
                {
                    _logger.LogInformation("ExamName is already exists!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamName is already exists!"));
                }
                #endregion

                createExam.UserId = this.UserId;
                var result = await _examTypeRepository.Create(createExam);

                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType created successfully!"));
                }
                else
                {
                    _logger.LogCritical("ExamType not created!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamType not created!"));
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
        /// EditExamType 
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditExamType examType)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditExamTypeValidator.ValidateAsync(examType);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check dublicacy
                Req.ExamName exam = new()
                {
                    Name = examType.ExamName
                };

                var isDublicate = await _examTypeRepository.IsDuplicate(exam);
                if (isDublicate)
                {
                    _logger.LogInformation("Exam name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam name is already exist!"));
                }
                #endregion
                examType.UserId = this.UserId;
                var result = await _examTypeRepository.Edit(examType);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("ExamType not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamType not edited!"));
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
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// Get all exam list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.ExamTypeList>))]
        public async Task<IActionResult> GetAllExamType()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _examTypeRepository.GetAllExamType();

                if (result.ExamType.Count > 0)
                {
                    _logger.LogInformation("record get successfully");
                    return Ok(ResponseResult<Res.ExamTypeList>.Success("ExamTypes gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("ExamTypes not found!");
                    return Ok(ResponseResult<Res.ExamTypeList>.Failure("ExamTypes not found!",new()));
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


        [HttpGet]
        [Authorize]
        [Route("getexamlistwithcourses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.ExamList>))]
        public async Task<IActionResult> GetExamListwithCourses()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _examTypeRepository.GetExamListWithCourse();
                

                if (result.ExamType.Any())
                {
                    _logger.LogInformation("record get successfully");
                    return Ok(ResponseResult<Res.ExamList>.Success("ExamTypes gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("ExamTypes not found!");
                    return Ok(ResponseResult<Res.ExamList>.Failure("ExamTypes not found!",new()));
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
        /// Get exam detail by id
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.ExamType>))]
        public async Task<IActionResult> GetById(Req.ExamTypeById examType)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetExamTypeByIdValidator.ValidateAsync(examType);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _examTypeRepository.GetById(examType);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.ExamType>.Success("ExamType get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.ExamType>.Failure("ExamType not found!",new()));
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
        /// Delete delete exam by id
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deleteold")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(Req.ExamTypeById examType)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteExamTypeValidator.ValidateAsync(examType);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                examType.UserId = this.UserId;
                var result = await _examTypeRepository.Delete(examType);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamType not deleted!"));
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
        /// Delete all exam by id
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("delete")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteAll(Req.ExamTypeById examType)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteExamTypeValidator.ValidateAsync(examType);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                examType.UserId = this.UserId;
                var result = await _examTypeRepository.DeleteAll(examType);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("ExamType deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("ExamType not deleted!"));
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
