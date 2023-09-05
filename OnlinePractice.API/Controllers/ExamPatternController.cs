using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ExamPatternController : BaseController
    {
        private readonly IExamPatternRepository _examPatternRepository;
        private readonly ILogger<ExamPatternController> _logger;
        private readonly IExamPatternValidation _validation;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMockTestRepository _mockTestRepository;
        public ExamPatternController(IExamPatternRepository examPatternRepository, ILogger<ExamPatternController> logger, IExamPatternValidation validation, ISubjectRepository subjectRepository,
            IMockTestRepository mockTestRepository
            )
        {
            _examPatternRepository = examPatternRepository;
            _logger = logger;
            _validation = validation;
            _subjectRepository = subjectRepository;
            _mockTestRepository = mockTestRepository;
        }


        /// <summary>
        /// API for Create Exam Pattern
        /// </summary>
        /// <param name="createExamPattern"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        //  [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(Req.CreateExamPattern createExamPattern)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateExamPatternValidator.ValidateAsync(createExamPattern);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check Duplicacy
                Req.CheckExamPatternName examPatternName = new()
                {
                    ExamPatternName = createExamPattern.ExamPatternName
                };

                var isDuplicate = await _examPatternRepository.IsDuplicate(examPatternName);
                if (isDuplicate)
                {
                    _logger.LogInformation("Exam pattern name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern name is already exist!"));
                }
                #endregion
                #region SubjectIdCheck
                foreach (var item in createExamPattern.Section)
                {
                    Req.CheckSubject checkSubject = new()
                    {
                        SubjectId = item.SubjectId
                    };

                    var IsSubjecExists = await _examPatternRepository.CheckSubjectExists(checkSubject);
                    if (!IsSubjecExists)
                    {
                        _logger.LogInformation("Invalid Subject Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                    }

                }
                #endregion
                createExamPattern.UserId = this.UserId;
                var result = await _examPatternRepository.Create(createExamPattern);

                if (result != Guid.Empty)
                {
                    _logger.LogInformation("Exam pattern created successfully!");
                    return Ok(ResponseResult<Guid>.Success("Exam pattern created successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Exam pattern not created!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern not created!"));
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
        /// Edit ExamPattern
        /// </summary>
        /// <param name="editExamPattern"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        //   [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(Req.EditExamPattern editExamPattern)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditExamPatternValidator.ValidateAsync(editExamPattern);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check If Exam Pattern Used in MockTest
                Req.GetExamPatternId examPatternId = new()
                {
                    Id = editExamPattern.Id,
                };
                var checkExamPattern = await _mockTestRepository.CheckPatternId(examPatternId);
                if (checkExamPattern)
                {
                    _logger.LogInformation("Exam pattern cannot be edited because it is used in Active MockTest!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern cannot be edited because it is used in Active MockTest!"));
                }
                #endregion
                #region CheckExamPatternNameAndId
                Req.CheckExamPatterNameAndId checkExamPatterNameAndId = new()
                {
                    Id = editExamPattern.Id,
                    ExamPatternName = editExamPattern.ExamPatternName
                };
                var IsExists = await _examPatternRepository.CheckPatternandIdExists(checkExamPatterNameAndId);
                if (IsExists)
                {
                    _logger.LogInformation("Exam pattern name with Same Id already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern Name with same Id already exist!"));
                }
                #endregion
                #region CheckSubjectId Exists
                foreach (var item in editExamPattern.Section)
                {
                    Req.CheckSubject checkSubject = new()
                    {
                        SubjectId = item.SubjectId
                    };

                    var IsSubjecExists = await _examPatternRepository.CheckSubjectExists(checkSubject);
                    if (!IsSubjecExists)
                    {
                        _logger.LogInformation("Invalid Subject Id!");
                        return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                    }

                }
                #endregion

                editExamPattern.UserId = this.UserId;
                var result = await _examPatternRepository.Edit(editExamPattern);
                if (result)
                {
                    _logger.LogInformation("Exam pattern edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Exam pattern not edited!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern not edited!"));
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
        /// EditGeneralInstruction 
        /// </summary>
        /// <param name="editExamPattern"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editgeneralinstructions")]
      //  [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> EditGeneralInstruction(Req.EditGeneralInstruction editGeneralInstruction)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditGeneralInstructionExamPatternValidator.ValidateAsync(editGeneralInstruction);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check ExamPatternId
                Req.GetExamPatternId getExamPatternId = new()
                {
                    Id = editGeneralInstruction.Id
                };

                var isDuplicate = await _examPatternRepository.IsExamPatternIdExist(getExamPatternId);
                if (!isDuplicate)
                {
                    _logger.LogInformation("Exam pattern id is not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern id is not exist!"));
                }
                #endregion
                editGeneralInstruction.UserId = this.UserId;
                var result = await _examPatternRepository.EditGeneralInstruction(editGeneralInstruction);
                if (result)
                {
                    _logger.LogInformation("Exam pattern general instructions updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern general instructions updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Exam pattern not edited!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern not edited!"));
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
        /// Delete ExamPattern 
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(Req.GetExamPatternId examPatternId)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteExamPatternValidator.ValidateAsync(examPatternId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check If Exam Pattern Used in MockTest
                var checkExamPattern = await _mockTestRepository.CheckPatternId(examPatternId);
                if (checkExamPattern)
                {
                    _logger.LogInformation("Exam pattern cannot be deleted because it is used in Active MockTest!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern cannot be deleted because it is used in Active MockTest!"));
                }
                #endregion
                examPatternId.UserId = this.UserId;
                var result = await _examPatternRepository.Delete(examPatternId);
                if (result)
                {
                    _logger.LogInformation("Exam pattern deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("ExamPattern Id is not exist!");
                    return Ok(ResponseResult<DBNull>.Success("Exam pattern id is not exist!"));
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
        /// Get ExamPattern
        /// </summary>
        /// <param name="getAllExam"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.ExamPatternList>))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetExamPattern(Req.GetAllExamPattern getAllExam)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllExamPatternValidator.ValidateAsync(getAllExam);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _examPatternRepository.GetAllExamPattern(getAllExam);

                if (result.ExamPatterns.Any())
                {
                    _logger.LogInformation("Exam pattern get successfully");
                    return Ok(ResponseResult<Res.ExamPatternList>.Success("Exam pattern  get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Exam pattern  not found!");
                    return Ok(ResponseResult<Res.ExamPatternList>.Failure("Exam pattern not found!", new()));
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
        /// Get ExamPattern by ID
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbyid")]
        //  [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetById(Req.GetByExamPatternId examPatternId)
        {
            ErrorResponse? errorResponse;

            try
            {
                #region Validate Request Model
                var validation = await _validation.GetByExamPatternIdValidator.ValidateAsync(examPatternId);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _examPatternRepository.GetById(examPatternId);
                if (result != null)
                {
                    _logger.LogInformation("Get exam pattern successfully!");
                    return Ok(ResponseResult<Res.ExamPattern>.Success("Get exam pattern successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Exam pattern not found!");
                    return Ok(ResponseResult<Res.ExamPattern>.Success("Exam pattern not found!", new()));
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
        /// Get Subject List by pattern id
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getsubjectlistbyexampatternid")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetSubjectListByExamPatternId(Req.GetByExamPatternId examPattern)
        {
            ErrorResponse? errorResponse;

            try
            {
                #region Validate Request Model
                var validation = await _validation.GetByExamPatternIdValidator.ValidateAsync(examPattern);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check ExamPatternId
                Req.GetExamPatternId getExamPatternId = new()
                {
                    Id = examPattern.Id
                };

                var isDuplicate = await _examPatternRepository.IsExamPatternIdExist(getExamPatternId);
                if (!isDuplicate)
                {
                    _logger.LogInformation("Exam pattern id is not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern id is not exist!"));
                }
                #endregion
                var result = await _examPatternRepository.GetAllSubjectByExamPatternId(examPattern);
                if (result != null)
                {
                    _logger.LogInformation("Get subjects successfully!");
                    return Ok(ResponseResult<Res.ExamPatternSubjectList>.Success("Get subjects successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Subjects not found!");
                    return Ok(ResponseResult<Res.ExamPatternSubjectList>.Success("Subjects not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlistbyexampatternid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlistbyexampatternid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Get section list by 
        /// passing exam pattern id and subject id
        /// </summary>
        /// <param name="sectionList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getsectionlistbypatternidandsubjectid")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetSectionListByExamPatternIdandSubjectId(Req.GetSectionList sectionList)
        {
            ErrorResponse? errorResponse;

            try
            {
                #region Validate Request Model
                var validation = await _validation.GetSectionListValidator.ValidateAsync(sectionList);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check ExamPatternId
                Req.GetExamPatternId getExamPatternId = new()
                {
                    Id = sectionList.ExamPatternId
                };

                var isDuplicate = await _examPatternRepository.IsExamPatternIdExist(getExamPatternId);
                if (!isDuplicate)
                {
                    _logger.LogInformation("Exam pattern id is not exist!");
                    return Ok(ResponseResult<DBNull>.Failure("Exam pattern id is not exist!"));
                }
                #endregion
                #region Validate SubjectIds
                Req.SubjectById subject = new()
                {
                    Id = sectionList.SubjectId
                };
                var isSubjectExist = await _subjectRepository.IsExist(subject);
                if (!isSubjectExist)
                {
                    _logger.LogInformation("Invalid Subject Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                }

                #endregion
                var result = await _examPatternRepository.GetSectionListByPatternIdandSubjectId(sectionList);
                if (result != null)
                {
                    _logger.LogInformation("Get section list successfully!");
                    return Ok(ResponseResult<Res.ExamPatternSectionList>.Success("Get section list successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Sections not found!");
                    return Ok(ResponseResult<Res.ExamPatternSectionList>.Success("Sections not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsectionlistbyexampatternid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsectionlistbyexampatternid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
    }

}
