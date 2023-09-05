using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Models;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Validator.Services.Student_Services;

namespace OnlinePractice.API.Controllers.StudentController
{
    public class StudentVideoController : BaseController
    {
        private readonly ILogger<StudentEbookController> _logger;
        private readonly IStudentVideoRespository _studentVideoRespository;
        private readonly IStudentRepository _studentRespository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IStudentVideoValidation _studentVideoValidation;
        private readonly ISubjectRepository _subjectRepository;

        public StudentVideoController(IStudentVideoRespository studentVideoRespository, IStudentRepository studentRespository,
            ILogger<StudentEbookController> logger, IInstituteRepository instituteRepository, ISubCourseRepository subCourseRepository,
            IStudentVideoValidation studentVideoValidation, ISubjectRepository subjectRepository
            )
        {
            _studentVideoRespository = studentVideoRespository;
            _studentRespository = studentRespository;
            _logger = logger;
            _instituteRepository = instituteRepository;
            _subCourseRepository = subCourseRepository;
            _studentVideoValidation = studentVideoValidation;
            _subjectRepository = subjectRepository;
        }

        
        /// <summary>
        /// Get All Subject List by Institute
        /// </summary>
        /// <param name="studentSubjects"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getsubjectlist")]
        public async Task<IActionResult> GetSubjectList(Req.StudentSubjects studentSubjects)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token.
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentVideoValidation.SubjectListValidator.ValidateAsync(studentSubjects);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = studentSubjects.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = studentSubjects.SubcourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion

                var result = await _studentVideoRespository.GetVideoSubjects(studentSubjects);
                if (result != null && result.videoSubjects.Any())
                {
                    _logger.LogInformation("Subjects list gets Successfully!");
                    return Ok(ResponseResult<Res.VideosSubjectsList?>.Success("Subjects list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Subjects not Found!");
                    return Ok(ResponseResult<Res.VideosSubjectsList?>.Failure("Subjects not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getsubjectlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get Videos List by Subject
        /// </summary>
        /// <param name="studentVideo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getvideoslistbysubject")]
        public async Task<IActionResult> GetVideosListBySubject(Req.GetStudentVideo studentVideo)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentVideoValidation.VideoListValidator.ValidateAsync(studentVideo);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = studentVideo.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion

                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = studentVideo.SubcourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion

                #region Subject Id Check
                Req.SubjectById subject = new()
                {
                    Id = studentVideo.SubcourseId
                };

                var isSubjectExist = await _subjectRepository.IsExist(subject);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid subject Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                }
                #endregion
                studentVideo.UserId = this.UserId;
                var result = await _studentVideoRespository.GetStudentsVideos(studentVideo);
                if (result != null && result.studentVideoDatas.Any())
                {
                    _logger.LogInformation("Video list gets Successfully!");
                    return Ok(ResponseResult<Res.StudentVideoList>.Success("Video list gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Video not Found!");
                    return Ok(ResponseResult<Res.StudentVideoList?>.Failure("Video not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getvideoslistbysubject", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getvideoslistbysubject", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// Get Video data by Id
        /// </summary>
        /// <param name="getVideo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("getvideobyid")]
        public async Task<IActionResult> GetVideoById(Req.GetVideoById getVideo)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check Login Token.
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                CurrentUser user = new()
                {
                    UserId = this.UserId,
                };
                var oldToken = await _studentRespository.GetToken(user);
                if (token != oldToken)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _studentVideoValidation.GetVideoValidator.ValidateAsync(getVideo);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                getVideo.UserId = this.UserId;
                var result = await _studentVideoRespository.GetVideos(getVideo);
                if (result != null)
                {
                    _logger.LogInformation("Videos gets Successfully!");
                    return Ok(ResponseResult<Res.GetVideo>.Success("Videos gets Successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Videos not Found!");
                    //return Ok(ResponseResult<DBNull>.Failure("Videos not Found!"));
                    return Ok(ResponseResult<Res.GetVideo?>.Failure("Videos not Found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getvideobyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getvideobyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}
