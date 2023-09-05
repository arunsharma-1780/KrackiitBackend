using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Microsoft.AspNetCore.Authorization;

namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class SubjectController : BaseController
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly ILogger<SubjectController> _logger;
        private readonly ISubjectValidation _validation;
        public SubjectController(ISubjectRepository subjectRepository, ISubCourseRepository subCourseRepository, ILogger<SubjectController> logger, ISubjectValidation validation)
        {
            _subjectRepository = subjectRepository;
            _subCourseRepository = subCourseRepository;
            _logger = logger;
            _validation = validation;
        }


        /// <summary>
        /// CreateSubjectData
        /// </summary>
        /// <param name="createSubject"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> Create(Req.CreateSubject createSubject)
        {
            ErrorResponse? errorResponse;
            try
            {
                createSubject.UserId = this.UserId;

                #region Validate Request Model
                var validation = await _validation.CreateSubjectValidator.ValidateAsync(createSubject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Duplicacy
                Req.SubjectName subjectName = new()
                {
                    Name = createSubject.SubjectName
                };

                var isDublicate = await _subjectRepository.IsDuplicate(subjectName);
                if (isDublicate)
                {
                    _logger.LogInformation("Coursename is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Coursename is already exist!"));
                }
                #endregion

                var result = await _subjectRepository.Create(createSubject);

                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject not created!"));
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

        [HttpPost]
        [Route("createsubjectcategory")]
        [Authorize]
        public async Task<IActionResult> CreateSubjectCategory(Req.CreateSubjectCategory createSubject)
        {
            ErrorResponse? errorResponse;
            try
            {
                createSubject.UserId = this.UserId;

                #region Validate Request Model
                var validation = await _validation.CreateSubjectCategoryValidator.ValidateAsync(createSubject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.SubCourseById subCourse = new()
                {
                    Id = createSubject.SubCourseId
                };
                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Validate SubjectIds
                foreach (var item in createSubject.SubjectIds)
                {
                    Req.SubjectById subject = new()
                    {
                        Id = item.Id
                    };
                    var isSubjectExist = await _subjectRepository.IsExist(subject);
                    if (!isSubjectExist)
                    {
                        _logger.LogInformation("Invalid Subject Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                    }
                }
                #endregion

                var result = await _subjectRepository.CreateSubjectCategory(createSubject);

                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject category created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject category not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createsubjectcategory", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createsubjectcategory", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        [HttpPost]
        [Route("updateubjectcategory")]
        [Authorize]
        public async Task<IActionResult> UpdateSubjectCategory(Req.EditSubjectCategory editSubjectCategory)
        {
            ErrorResponse? errorResponse;
            try
            {
                editSubjectCategory.UserId = this.UserId;

                #region Validate Request Model
                var validation = await _validation.EditSubjectCategoryValidator.ValidateAsync(editSubjectCategory);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.SubCourseById subCourse = new()
                {
                    Id = editSubjectCategory.SubCourseId
                };
                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Validate SubjectIds
                foreach (var item in editSubjectCategory.SubjectIds)
                {
                    Req.SubjectById subject = new()
                    {
                        Id = item.Id
                    };
                    var isSubjectExist = await _subjectRepository.IsExist(subject);
                    if (!isSubjectExist)
                    {
                        _logger.LogInformation("Invalid Subject Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                    }
                }
                #endregion

                var result = await _subjectRepository.UpdatesubjectCategory(editSubjectCategory);

                if (result)
                {
                    _logger.LogInformation("Subject category updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject category updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Subject category not updated successfully!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject category not updated!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updateubjectcategory", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "updateubjectcategory", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// DeleteAllDataMethod
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> DeleteAll(Req.SubjectById subject)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteSubjectValidator.ValidateAsync(subject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                subject.UserId = this.UserId;
                var result = await _subjectRepository.DeleteAll(subject);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject not deleted!"));
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

        [HttpPost]
        [Route("getallsubjectscategorybysubcoursesid")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubjectCategoryList>))]
        public async Task<IActionResult> GetAllSubjectsBySubCourseId(Req.GetSubjects subject)
        {
            ErrorResponse? errorResponse;
            #region Validate Request Model
            #endregion
            try
            {
                var result = await _subjectRepository.GetAllSubjectsbySubCourseId(subject);

                if (result.SubjectCategories.Count > 0)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubjectCategoryList>.Success("Subjects get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.SubjectCategoryList>.Failure("Subjects not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getSubjectsbySubCourseid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getSubjectsbySubCourseid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        /// <summary>
        /// GetAllSubjectsBySubCourseId
        /// </summary>
        /// <param name="SubCourseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getallsubjectsmasterbysubcoursesid")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubjectList>))]
        public async Task<IActionResult> GetAllSubjectsMasterBySubCourseId(Req.GetSubjects subject)
        {
            ErrorResponse? errorResponse;
            #region Validate Request Model
            #endregion
            try
            {
                var result = await _subjectRepository.GetAllSubjectsMasterbySubCourseId(subject);

                if (result.Subjects.Count > 0)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubjectList>.Success("Subjects get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.SubjectList>.Failure("Subjects not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallsubjectsmasterbysubcoursesid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallsubjectsmasterbysubcoursesid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getallsubjects")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubjectList>))]
        public async Task<IActionResult> GetAllSubjects(Req.GetAllSubject allSubject)
        {
            ErrorResponse? errorResponse;
            #region Validate Request Model
            #endregion
            try
            {
                var result = await _subjectRepository.GetAllSubjects(allSubject);

                if (result.Subjects.Any())
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubjectList>.Success("Subjects get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Subjects not found!");
                    return Ok(ResponseResult<Res.SubjectList>.Failure("Subjects not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallsubjects", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallsubjects", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// EditSubjectData
        /// </summary>
        /// <param name="editSubject"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Authorize]
        public async Task<IActionResult> Edit(Req.EditSubject editSubject)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditSubjectValidator.ValidateAsync(editSubject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Duplicacy
                Req.SubjectName subjectName = new()
                {
                    Name = editSubject.SubjectName
                };

                var isDublicate = await _subjectRepository.IsDuplicate(subjectName);
                if (isDublicate)
                {
                    _logger.LogInformation("Coursename is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Coursename is already exist!"));
                }
                #endregion
                editSubject.UserId = this.UserId;
                var result = await _subjectRepository.Edit(editSubject);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Record not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject not edited!"));
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
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong"));
            }

        }


        /// <summary>
        /// GetSubjectsData by ID
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbyid")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Subject>))]
        public async Task<IActionResult> GetById(Req.SubjectById subject)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetSubjectByIdValidator.ValidateAsync(subject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _subjectRepository.GetById(subject);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.Subject>.Success("Subject get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.Subject>.Failure("Subject not found!", new()));
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
        /// NormalDelete Method for Deleteing particular SubjectData
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [Route("deleteold")]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(Req.SubjectById subject)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteSubjectValidator.ValidateAsync(subject);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                subject.UserId = this.UserId;
                var result = await _subjectRepository.Delete(subject);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Subject deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("SubCourse not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Subject not deleted!"));
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
