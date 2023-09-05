using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Org.BouncyCastle.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class VideoController : BaseController
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ILogger<VideoController> _logger;
        private readonly ICourseRepository _courseRepository;
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly IVideoValidation _validation;
        private readonly ICommonRepository _commonRepository;

        public VideoController(IVideoRepository videoRepository,
            ILogger<VideoController> logger,
                  ICourseRepository courseRepository,
            IExamTypeRepository examTypeRepository,
            ISubCourseRepository subCourseRepository,
            ISubjectRepository subjectRepository,
            ITopicRepository topicRepository,
            IInstituteRepository instituteRepository,
            IVideoValidation validation,
            ICommonRepository commonRepository)
        {
            _videoRepository = videoRepository;
            _logger = logger;
            _courseRepository = courseRepository;
            _examTypeRepository = examTypeRepository;
            _subCourseRepository = subCourseRepository;
            _subjectRepository = subjectRepository;
            _topicRepository = topicRepository;
            _instituteRepository = instituteRepository;
            _validation = validation;
            _commonRepository = commonRepository;
        }

        /// <summary>
        /// Api for upload thumbnail image
        /// </summary>
        /// <param name="videoThumbnailimage"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadevideothumbnail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.VideoThumbnailimage videoThumbnailimage)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.VideoThumbnailimageValidator.ValidateAsync(videoThumbnailimage);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _videoRepository.UploadImage(videoThumbnailimage);



                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Video Thumbnail upload successfully!");
                    return Ok(ResponseResult<string>.Success("Video Thumbnail upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Video Thumbnail not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Video Thumbnail not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadevideothumbnail", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadevideothumbnail", ex);
                return Ok(ResponseResult<DBNull>.Failure("Video Thumbnail not created!"));
            }
        }



        [HttpPost]
        [RequestSizeLimit(800000000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 800000000)]
        [Route("uploadvideourl")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadVideoUrl([FromForm]Req.VideoUrl video)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.VideoUrlValidator.ValidateAsync(video);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                string result = await _videoRepository.UploadVideoUrl(video);

                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Video Url upload successfully!");
                    return Ok(ResponseResult<string>.Success("Video Url upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Video Url not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Video Url not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadvideourl", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadvideourl", ex);
                return Ok(ResponseResult<DBNull>.Failure("Something went wrong!"));
            }
        }


        /// <summary>
        /// Api for create video details
        /// </summary>
        /// <param name="createVideo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("create")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(Req.CreateVideo createVideo)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Videos,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CreateVideoValidator.ValidateAsync(createVideo);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = createVideo.ExamTypeId
                };

                var isExamExist = await _examTypeRepository.IsExist(exam);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Exam Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Exam Id!"));
                }
                #endregion
                #region Course Id Check
                Req.CourseById course = new()
                {
                    Id = createVideo.CourseId
                };

                var isCourseExist = await _courseRepository.IsExist(course);
                if (!isCourseExist)
                {
                    _logger.LogInformation("Invalid Course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Course Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = createVideo.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region SubjectCategory Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = createVideo.SubjectCategory
                };

                var isSubjectCategoryExist = await _subjectRepository.IsExistCategory(subjectCategory);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid Subject category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject category Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = createVideo.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion
                #region Check Duplicacy
                //Req.CourseName course = new()
                //{
                //    Name = createCourse.CourseName,
                //    ExamId = createCourse.ExamTypeId
                //};

                //var isDuplicate = await _courseRepository.IsDuplicate(course);
                //if (isDuplicate)
                //{
                //    _logger.LogInformation("Coursename is already exist!");
                //    return Ok(ResponseResult<DBNull>.Failure("Coursename is already exist!"));
                //}
                #endregion

                createVideo.UserId = this.UserId;
                var result = await _videoRepository.Create(createVideo);

                if (result!=null)
                {
                    _logger.LogInformation("Video created successfully!");
                    return Ok(ResponseResult<Res.AuthorAndLanguage>.Success("Video created successfully!",result));
                }
                else
                {
                    _logger.LogCritical("Video not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Video not created!"));
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
        /// Api for edit video details
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditVideo video)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Videos,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.EditVideoValidator.ValidateAsync(video);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = video.ExamTypeId
                };

                var isExamExist = await _examTypeRepository.IsExist(exam);
                if (!isExamExist)
                {
                    _logger.LogInformation("Invalid Exam Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Exam Id!"));
                }
                #endregion
                #region Course Id Check
                Req.CourseById course = new()
                {
                    Id = video.CourseId
                };

                var isCourseExist = await _courseRepository.IsExist(course);
                if (!isCourseExist)
                {
                    _logger.LogInformation("Invalid Course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Course Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = video.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region SubjectCategory Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = video.SubjectCategory
                };

                var isSubjectCategoryExist = await _subjectRepository.IsExistCategory(subjectCategory);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid Subject category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject category Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = video.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion
                video.UserId = this.UserId;
                var result = await _videoRepository.Edit(video);
                if (result != null)
                {
                    _logger.LogInformation("Video edited successfully!");
                    return Ok(ResponseResult<Res.AuthorAndLanguage>.Success("Video edited successfully!",result));
                }
                else
                {
                    _logger.LogCritical("Video not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Video not edited!"));
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
        /// Api for video getby id 
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Video>))]
        public async Task<IActionResult> GetById(Req.VideoById video)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Videos,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.VideoByIdValidator.ValidateAsync(video);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _videoRepository.GetVideoById(video);

                if (result != null)
                {
                    _logger.LogInformation("Video get successfully");
                    return Ok(ResponseResult<Res.Video>.Success("Video get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Video not found!");
                    return Ok(ResponseResult<Res.Video>.Failure("Video not found!",new()));
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
        /// Api for delete video
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin,Staff")]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.VideoById video)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Videos,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.VideoByIdValidator.ValidateAsync(video);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                video.UserId = this.UserId;
                var result = await _videoRepository.Delete(video);
                if (result)
                {
                    _logger.LogInformation("Video deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Video deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Video not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Video not deleted!"));
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
        /// Api for get all
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.VideoList>))]
        public async Task<IActionResult> GetAll(Req.GetAllVideos videos)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Videos,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAllVideosValidator.ValidateAsync(videos);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region SubjectCategory Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = videos.SubjectCategoryId
                };

                var isSubjectCategoryExist = await _subjectRepository.IsExistCategory(subjectCategory);
                if (!isSubjectCategoryExist)
                {
                    _logger.LogInformation("Invalid Subject category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject category Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = videos.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion
                var result = await _videoRepository.GetAllVideos(videos);
                if (result != null && result.Videos.Any())
                {
                    _logger.LogInformation("Get all video successfully!");
                    return Ok(ResponseResult<Res.VideoList>.Success("Get all video successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Video not found!");
                    return Ok(ResponseResult<Res.VideoList>.Failure("Video not found!",new()));
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.VideoList>))]
        public async Task<IActionResult> GetAll50()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _videoRepository.GetAll50();
                if (result != null)
                {
                    _logger.LogInformation("Get all Videos successfully!");
                    return Ok(ResponseResult<Res.VideoList>.Success("Get all Videos successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Videos not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Videos not found!"));
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

        [HttpPost]
        [Route("getallauthors")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.VideoAuthorList>))]
        public async Task<IActionResult> GetAllAuthers(Req.GetAllVideoAuthors authors)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllVideosAuthorsValidator.ValidateAsync(authors);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region SubjectCategory Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = authors.SubjectCategoryId
                };

                var isSubjectCategoryExist = await _subjectRepository.IsExistCategory(subjectCategory);
                if (!isSubjectCategoryExist)
                {
                    _logger.LogInformation("Invalid Subject category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject category Id!"));
                }
                #endregion
                var result = await _videoRepository.GetAllAuthors(authors);
                if (result != null && result.VideoAuthors.Any())
                {
                    _logger.LogInformation("Get all authors successfully!");
                    return Ok(ResponseResult<Res.VideoAuthorList>.Success("Get all authors successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Authors not found!");
                    return Ok(ResponseResult<Res.VideoAuthorList>.Failure("Authors not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallauthors", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallauthors", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("showidandname")]
        public async Task<IActionResult> showidandname()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _videoRepository.Showallids();
                if (result != null && result.Videos.Any())
                {
                    _logger.LogInformation("Get all videos successfully!");
                    return Ok(ResponseResult<Res.VideoListV2>.Success("Get all videos successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Videos not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Videos not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "showidandname", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "showidandname", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
    }
}
