using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using OnlinePractice.API.Validator.Services;
using Microsoft.AspNetCore.Authorization;

namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]

    public class SubTopicController : BaseController
    {
        private readonly ISubTopicRepository _subTopicRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ILogger<SubTopicController> _logger;
        private readonly ISubTopicValidation _validation;
        public SubTopicController( ISubTopicRepository subTopicRepository, ITopicRepository topicRepository, ILogger<SubTopicController> logger,ISubTopicValidation validation)
        {
            _subTopicRepository = subTopicRepository;
            _topicRepository = topicRepository;
            _logger = logger;
            _validation = validation;
        }


      /// <summary>
      /// Create SubTopic Data
      /// </summary>
      /// <param name="createSubTopic"></param>
      /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateSubTopic createSubTopic)
        {
            ErrorResponse? errorResponse;
            try
            {
                createSubTopic.UserId = this.UserId;
                #region Validate Request Model
                var validation = await _validation.CreateSubTopicValidator.ValidateAsync(createSubTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.TopicById topic = new()
                {
                    Id = createSubTopic.TopicId
                };
                var isExamExist = await _topicRepository.IsExist(topic);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Topic Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Topic Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.SubtopicName subtopicName= new()
                {
                    Name = createSubTopic.SubTopicName,
                    TopicId = createSubTopic.TopicId
                };

                var isDublicate = await _subTopicRepository.IsDuplicate(subtopicName);
                if (isDublicate)
                {
                    _logger.LogInformation("SubTopic is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("SubTopic is already exist!"));
                }
                #endregion
                var result = await _subTopicRepository.Create(createSubTopic);

                if (result)
                {
                    _logger.LogInformation("Records created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubTopic created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not created!");
                    return Ok(ResponseResult<DBNull>.Failure("SubTopic not created!"));
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
        /// getallsubtopicbyTOpicID
        /// </summary>
        /// <param name="TopicId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getallsubtopicbyTopicid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubTopicList>))]
        public async Task<IActionResult> GetAllSubTopicsbyTopicId(Req.TopicById topic)
        {
            ErrorResponse? errorResponse;
            #region Validate Request Model
            var validation = await _validation.GetAllSubTopicBySubCourseIdValidator.ValidateAsync(topic);
            errorResponse = CustomResponseValidator.CheckModelValidation(validation);
            if (errorResponse != null)
            {
                return BadRequest(errorResponse);
            }
            #endregion
            try
            {
                var result = await _subTopicRepository.GetAllSubTopicsbyTopicsId(topic);

                if (result.SubTopics.Count>0)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubTopicList>.Success("Subtopics get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.SubTopicList>.Failure("Subtopics not found!", new()));
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
        /// Edit SubTopic Data
        /// </summary>
        /// <param name="editSubTopic"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("edit")]
        public async Task<IActionResult> Edit( Req.EditSubTopic editSubTopic)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EditSubTopicValidator.ValidateAsync(editSubTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckExist
                Req.TopicById topic = new()
                {
                    Id = editSubTopic.TopicId
                };
                var isExamExist = await _topicRepository.IsExist(topic);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Topic Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Topic Id!"));
                }
                #endregion
                #region Check Duplicacy
                Req.SubtopicName subtopicName = new()
                {
                    Name = editSubTopic.SubTopicName,
                    TopicId = editSubTopic.TopicId
                };

                var isDublicate = await _subTopicRepository.IsDuplicate(subtopicName);
                if (isDublicate)
                {
                    _logger.LogInformation("SubTopic is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("SubTopic is already exist!"));
                }
                #endregion
                editSubTopic.UserId = this.UserId;
                var result = await _subTopicRepository.Edit(editSubTopic);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubTopic edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Record not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("SubTopic not edited!"));
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
       /// getSubTopic Data by Id
       /// </summary>
       /// <param name="subTopic"></param>
       /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.SubTopic>))]
        public async Task<IActionResult> GetById(Req.SubTopicById subTopic)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetSubTopicByIdValidator.ValidateAsync(subTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _subTopicRepository.GetById(subTopic);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.SubTopic>.Success("SubTopic get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<Res.SubTopic>.Failure("SubTopic not found!", new()));
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
        /// Delete SubTopic Data by Particular Id
        /// </summary>
        /// <param name="subTopic"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> Delete( Req.SubTopicById subTopic)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetSubTopicByIdValidator.ValidateAsync(subTopic);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                subTopic.UserId = this.UserId;
                var result = await _subTopicRepository.Delete(subTopic);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("SubTopic deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Success("SubTopic not deleted!"));
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
