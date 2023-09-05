using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using OnlinePractice.API.Models.Common;
using Com = OnlinePractice.API.Models.Common;

using Microsoft.AspNetCore.Authorization;


namespace OnlinePractice.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //piExplorerSettings(IgnoreApi = true)]
    public class QuestionBankController : BaseController
    {
        public readonly IQuestionBankRepository _questionBankRepository;
        private readonly ILogger<SubCourseController> _logger;
        private readonly IQuestionBankValidation _validation;

        public QuestionBankController(ILogger<SubCourseController> logger, IQuestionBankValidation validation, IQuestionBankRepository questionBankRepository)
        {
            _logger = logger;
            _validation = validation;
            _questionBankRepository = questionBankRepository;
        }

        /// <summary>
        /// Create QuestionBank 
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(Req.CreateQuestionBank questionBank)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateQuestionBankValidator.ValidateAsync(questionBank);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region SubjectCategoryExist
                Req.CheckSubjectCategoryId subject = new()
                {
                    Id = questionBank.SubjectCategoryId
                };

                var IsSubjecCategoryExists = await _questionBankRepository.CheckSubjectCategoryExist(subject);
                if (!IsSubjecCategoryExists)
                {
                    _logger.LogInformation("Invalid SubjectCategory Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubjectCategory Id!"));
                }
                #endregion
                #region TopicExists
                Req.CheckTopicId topicId = new()
                {
                    Id = questionBank.TopicId
                };
                var isTopicExists = await _questionBankRepository.CheckTopicExist(topicId);
                if (!isTopicExists)
                {
                    _logger.LogInformation("Invalid Topic Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Topic Id!"));
                }
                #endregion
                #region SubTopicExists
                Req.CheckSubtopicId subtopicId = new()
                {
                    Id = questionBank.SubTopicId
                };
                var isSubTopicExists = await _questionBankRepository.CheckSubTopicExists(subtopicId);
                if (!isSubTopicExists)
                {
                    _logger.LogInformation("Invalid SubTopicId Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubTopicId Id!"));
                }
                #endregion
                #region ReferenceId Check
                Req.CheckReference checkReference = new()
                {
                    ReferenceId = questionBank.QuestionTableData.QuestionRefId
                };
                var isReferenceIdExists = await _questionBankRepository.CheckReferenceIdExists(checkReference);
                if (isReferenceIdExists)
                {
                    _logger.LogInformation(" QuestionRefId already Exists!");
                    return Ok(ResponseResult<DBNull>.Failure("QuestionRefId already Exists"));
                }
                #endregion
                questionBank.UserId = this.UserId;
                var result = await _questionBankRepository.CreateQuestionBank(questionBank);

                if (result)
                {
                    _logger.LogInformation("Question created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Question created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Question not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Question not created!"));
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
        /// GetQuestionBank by Id
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbyid")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.QuestionBank>))]
     //   [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetById(Req.GetQuestionBank questionBank)
        {
            ErrorResponse? errorResponse;
            try
            {

                #region Validate Request Model
                var validation = await _validation.GetByReferenceIdQuestionBankValidator.ValidateAsync(questionBank);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _questionBankRepository.GetByRefId(questionBank);
                if (result != null)
                {
                    _logger.LogInformation("Question get successfully!");
                    return Ok(ResponseResult<Res.QuestionBank>.Success("Question get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Question not found!");
                    return Ok(ResponseResult<Res.QuestionBank>.Failure("Question not found!",new()));
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
        /// GetAll QuestionBank Data
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.QuestionBankList>))]
        public async Task<IActionResult> GetAll(Req.GetAllQuestion questionBank)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllQuestionBankValidator.ValidateAsync(questionBank);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region UserExists
                Req.CheckUserExist userExist = new()
                {
                    Id = questionBank.CreatorUserId
                };
                var isUserExists = await _questionBankRepository.CheckUserIdExists(userExist);
                if (!isUserExists)
                {
                    _logger.LogInformation("Invalid UserId!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid UserId!"));
                }
                #endregion
                var result = await _questionBankRepository.GetAll(questionBank);
                if (result != null && result.QuestionBanks.Count > 0)
                {
                    _logger.LogInformation("Get all question successfully!");
                    return Ok(ResponseResult<Res.QuestionBankList>.Success("Get all question successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Questions not found!");
                    return Ok(ResponseResult<Res.QuestionBankList>.Failure("Questions not found!",new()));
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

        [HttpPost]
        [Route("getall50")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.QuestionBankList>))]
        public IActionResult GetAll50(Req.GetAll50Question question)
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _questionBankRepository.GetAll50(question);
                if (result != null && result.QuestionBanks.Count > 0)
                {
                    _logger.LogInformation("Get all question successfully!");
                    return Ok(ResponseResult<Res.QuestionBankList>.Success("Get all question successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Questions not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Questions not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall50", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall50", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Edit QuestionBank 
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(Req.EditQuestionBank questionBank)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditQuestionBankValidator.ValidateAsync(questionBank);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region SubjectCategoryExist
                Req.CheckSubjectCategoryId subject = new()
                {
                    Id = questionBank.SubjectCategoryId
                };

                var IsSubjecCategoryExists = await _questionBankRepository.CheckSubjectCategoryExist(subject);
                if (!IsSubjecCategoryExists)
                {
                    _logger.LogInformation("Invalid SubjectCategory Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubjectCategory Id!"));
                }
                #endregion
                #region TopicExists
                Req.CheckTopicId topicId = new()
                {
                    Id = questionBank.TopicId
                };
                var isTopicExists = await _questionBankRepository.CheckTopicExist(topicId);
                if (!isTopicExists)
                {
                    _logger.LogInformation("Invalid Topic Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Topic Id!"));
                }
                #endregion
                #region SubTopicExists
                Req.CheckSubtopicId subtopicId = new()
                {
                    Id = questionBank.SubTopicId
                };
                var isSubTopicExists = await _questionBankRepository.CheckSubTopicExists(subtopicId);
                if (!isSubTopicExists)
                {
                    _logger.LogInformation("Invalid SubTopicId Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubTopicId Id!"));
                }
                #endregion



                #region ReferenceId Check
                Req.CheckReference checkReference = new()
                {
                    ReferenceId = questionBank.QuestionTableData.QuestionRefId
                };
                var isReferenceIdExists = await _questionBankRepository.CheckRefIdExists(checkReference);
                if (!isReferenceIdExists)
                {
                    _logger.LogInformation(" QuestionRefId not Exist!");
                    return Ok(ResponseResult<DBNull>.Failure("QuestionRefId not Exist"));
                }
                #endregion

                questionBank.UserId = this.UserId;
                var result = await _questionBankRepository.EditQuestionBank(questionBank);

                if (result)
                {
                    _logger.LogInformation("Question updated successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Question updated successfully!"));
                }
                else
                {
                    _logger.LogCritical("Question not updated!");
                    return Ok(ResponseResult<DBNull>.Failure("Question not updated!"));
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
        /// Delete QuestionBank 
        /// </summary>
        /// <param name="questionBankRef"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(Req.QuestionBankRefId questionBankRef)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteQuestionBankValidator.ValidateAsync(questionBankRef);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ReferenceId Check
                Req.CheckReference checkReference = new()
                {
                    ReferenceId = questionBankRef.QuestionRefId
                };
                var isReferenceIdExists = await _questionBankRepository.CheckReferenceIdExists(checkReference);
                if (!isReferenceIdExists)
                {
                    _logger.LogInformation("QuestionRefId Not Exists!");
                    return Ok(ResponseResult<DBNull>.Failure("QuestionRefId Not Exists!"));
                }
                #endregion

                questionBankRef.UserId = this.UserId;
                var result = await _questionBankRepository.DeleteQuestionByRefId(questionBankRef);

                if (result)
                {
                    _logger.LogInformation("Questionbank Deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("QuestionBank Deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("QuestionBank are not created!");
                    return Ok(ResponseResult<DBNull>.Failure("QuestionBank are not created!"));
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
        /// GetAll QuestionType 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestiontype")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllQuestionType()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _questionBankRepository.GetQuestionType();



                if (result != null)
                {
                    _logger.LogInformation("Question type get successfully!");
                    return Ok(ResponseResult<List<EnumModel>>.Success("Question type get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Question type not found!");
                    return Ok(ResponseResult<List<EnumModel>>.Failure("Question type not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestiontype", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestiontype", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get All QuestionLevel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionlevel")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllQuestionLevel()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _questionBankRepository.GetQuestionLevel();



                if (result != null)
                {
                    _logger.LogInformation("Question Level get successfully!");
                    return Ok(ResponseResult<List<EnumModel>>.Success("Question Level get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Question Level not found!");
                    return Ok(ResponseResult<List<EnumModel>>.Failure("Question Level not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestionlevel", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestionlevel", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

        /// <summary>
        /// GetAll QuestionLanguage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getallquestionlanguage")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetAllQuestionLanguage()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _questionBankRepository.GetQuestionLanguage();
                if (result != null)
                {
                    _logger.LogInformation("Question Language get successfully!");
                    return Ok(ResponseResult<List<EnumModel>>.Success("Question Language get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Question Language not found!");
                    return Ok(ResponseResult<List<EnumModel>>.Failure("Question Language not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestionlangauge", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallquestionlanguage", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}
