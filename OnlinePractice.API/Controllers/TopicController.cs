using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;


namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class TopicController : BaseController
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISubCourseRepository _subCourseRepository;         
        private readonly ILogger<TopicController> _logger;
        private readonly ITopicValidation _validation;
        public TopicController(ITopicRepository topicRepository, ISubjectRepository subjectRepository, ILogger<TopicController> logger, ITopicValidation validation, ISubCourseRepository subCourseRepository)
        {
            _topicRepository = topicRepository;
            _subjectRepository = subjectRepository;
            _logger = logger;
            _validation = validation;
            _subCourseRepository = subCourseRepository;
        }
        /// <summary>
        /// CreateTopic
        /// </summary>
        /// <param name="createTopic"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateTopic createTopic)
        {
            ErrorResponse? errorResponse;
            try
            {
                createTopic.UserId = this.UserId;
                #region Validate Request Model
                var validation = await _validation.CreateTopicValidator.ValidateAsync(createTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.SubjectCategoryById subject = new()
                {
                    Id = createTopic.SubjectCategoryId
                };
                var isSubjectCategory = await _subjectRepository.IsExistCategory(subject);
                if (!isSubjectCategory)
                {
                    _logger.LogInformation("Invalid SubjectCategory Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubjectCategory Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.TopicName topic = new()
                {
                    Name = createTopic.TopicName,
                    SubjectId = createTopic.SubjectCategoryId
                };

                var isDublicate = await _topicRepository.IsDuplicate(topic);
                if (isDublicate)
                {
                    _logger.LogInformation("TopicName is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("TopicName is already exist!"));
                }
                #endregion
                var result = await _topicRepository.Create(createTopic);

                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Topic created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Topic not created!"));
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
        /// getalltopicbysubjectid
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getalltopicbysubjectcategoryid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.TopicList>))]
        public async Task<IActionResult> GetAllTopicsbySubjectCategoryId(Req.TopicById topic)
        {
            ErrorResponse? errorResponse;
            #region Validate Request Model
            var validation = await _validation.GetTopicByIdValidator.ValidateAsync(topic);
            errorResponse = CustomResponseValidator.CheckModelValidation(validation);
            if (errorResponse != null)
            {
                return BadRequest(errorResponse);
            }

            #endregion
            try
            {
                var result = await _topicRepository.GetAllTopicsbySubjectCategoryId(topic);

                if (result.Topic.Count > 0)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.TopicList>.Success("Topics get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.TopicList>.Failure("Topics not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltopicbysubjectid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltopicbysubjectid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        /// <summary>
        /// Get all topics list
        /// by subject and subcourseid
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("gettopiclistbysubjectidandsubcourseid")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.TopicList>))]
        public async Task<IActionResult> GetAllTopicsbySubjectIdandCourseId(Req.GetAllTopics topic)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllTopicsValidator.ValidateAsync(topic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }

                #endregion
                #region Validate SubCourse
                Req.SubCourseById subCourse = new()
                {
                    Id = topic.SubCourseId
                };
                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Validate SubjectIds
                Req.SubjectById subject = new()
                {
                    Id = topic.SubjectId
                };
                var isSubjectExist = await _subjectRepository.IsExist(subject);
                if (!isSubjectExist)
                {
                    _logger.LogInformation("Invalid Subject Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                }

                #endregion

                var result = await _topicRepository.GetAllTopicsbySubjectIdandSubCourseId(topic);

                if (result.Topic.Any())
                {
                    _logger.LogInformation("Topics get successfully");
                    return Ok(ResponseResult<Res.TopicList>.Success("Topics get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Topics not found!");
                    return Ok(ResponseResult<Res.TopicList>.Failure("Topics not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltopicsbysubjectidandsubcourseid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getalltopicsbysubjectidandsubcourseid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }


        /// <summary>
        /// EditTopicData 
        /// </summary>
        /// <param name="editTopic"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Authorize]
        public async Task<IActionResult> Edit(Req.EditTopic editTopic)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditTopicValidator.ValidateAsync(editTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.SubjectCategoryById subject = new()
                {
                    Id = editTopic.SubjectCategoryId
                };
                var isExamExist = await _subjectRepository.IsExistCategory(subject);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Subject Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.TopicName topic = new()
                {
                    Name = editTopic.TopicName,
                    SubjectId = editTopic.SubjectCategoryId
                };

                var isDublicate = await _topicRepository.IsDuplicate(topic);
                if (isDublicate)
                {
                    _logger.LogInformation("TopicName is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("TopicName is already exist!"));
                }
                #endregion
                editTopic.UserId = this.UserId;
                var result = await _topicRepository.Edit(editTopic);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Topic edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Record not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Topic not edited!"));
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
        /// DeleteAllData
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> DeleteAll(Req.TopicById topic)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetTopicByIdValidator.ValidateAsync(topic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                topic.UserId = this.UserId;
                var result = await _topicRepository.DeleteAll(topic);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Topic deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Topic not deleted!"));
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
        /// GetSubjectsData by ID
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Topic>))]
        public async Task<IActionResult> GetById(Req.TopicById topic)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetTopicByIdValidator.ValidateAsync(topic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _topicRepository.GetById(topic);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.Topic>.Success("Topic get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.Topic>.Failure("Topic not found!", new Res.Topic()));
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
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("Something went wrong!"));
            }
        }
    }
}
