using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using OnlinePractice.API.Validator.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Models.Enum;
using Com =OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PreviousYearPaperController : BaseController
    {
        private readonly IPreviousYearPaperRepository _previousYearPaperRepository;
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly ILogger<PreviousYearPaperController> _logger;
        private readonly IPreviousYearPaperValidation _validation;
        private readonly ICommonRepository _commonRepository;

        public PreviousYearPaperController(IPreviousYearPaperRepository previousYearPaperRepository, ILogger<PreviousYearPaperController> logger, ICourseRepository courseRepository
            , IExamTypeRepository examTypeRepository, ISubCourseRepository subCourseRepository, IInstituteRepository instituteRepository, IPreviousYearPaperValidation previousYearPaperValidation, ICommonRepository commonRepository)
        {
            _logger = logger;
            _previousYearPaperRepository = previousYearPaperRepository;
            _courseRepository = courseRepository;
            _examTypeRepository = examTypeRepository;
            _subCourseRepository = subCourseRepository;
            _instituteRepository = instituteRepository;
            _validation = previousYearPaperValidation;
            _commonRepository = commonRepository;
        }


        /// <summary>
        /// Upload Ebook PDF
        /// </summary>
        /// <param name="ebookPdfUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadPaperPdf")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadPaperPdfUrl([FromForm] Req.PaperPdfUrl paperPdf)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.PaperFileValidator.ValidateAsync(paperPdf);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _previousYearPaperRepository.UploadPaperPdfUrl(paperPdf);



                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Paper Pdf upload successfully!");
                    return Ok(ResponseResult<string>.Success("Paper Pdf upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Paper Pdf not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Paper Pdf not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadpdf", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadpdf", ex);
                return Ok(ResponseResult<DBNull>.Failure("Ebook Pdf not created!"));
            }
        }


        /// <summary>
        /// Create PreviousYearPaper
        /// </summary>
        /// <param name="previousYearPaper"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreatePreviousYearPaper previousYearPaper)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.PreviousYearPaper,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CreatePaperValidator.ValidateAsync(previousYearPaper);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = previousYearPaper.ExamTypeId
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
                    Id = previousYearPaper.CourseId
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
                    Id = previousYearPaper.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = previousYearPaper.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
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

                previousYearPaper.UserId = this.UserId;
                var result = await _previousYearPaperRepository.Create(previousYearPaper);

                if (result)
                {
                    _logger.LogInformation("paper created successfully!");
                    return Ok(ResponseResult<Guid>.Success("paper created successfully!",this.UserId));
                }
                else
                {
                    _logger.LogCritical("paper not created!");
                    return Ok(ResponseResult<DBNull>.Failure("paper not created!"));
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
        /// Get Paper Data by ID
        /// </summary>
        /// <param name="paperPdf"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.PreviousYearPaper>))]
        public async Task<IActionResult> GetById(Req.GetPaperPdf paperPdf)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.PreviousYearPaper,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetByIdPaperValidator.ValidateAsync(paperPdf);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _previousYearPaperRepository.GetPaperById(paperPdf);

                if (result != null)
                {
                    _logger.LogInformation("Paper get successfully");
                    return Ok(ResponseResult<Res.PreviousYearPaper>.Success("Paper get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Paper not found!");
                    return Ok(ResponseResult<Res.PreviousYearPaper>.Failure("Paper not found!", new()));
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
        /// Edit PreviousYearPaper
        /// </summary>
        /// <param name="previousYearPaper"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditPreviousYearPaper previousYearPaper)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.PreviousYearPaper,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.EditPaperValidator.ValidateAsync(previousYearPaper);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = previousYearPaper.ExamTypeId
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
                    Id = previousYearPaper.CourseId
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
                    Id = previousYearPaper.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = previousYearPaper.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                previousYearPaper.UserId = this.UserId;
                var result = await _previousYearPaperRepository.Edit(previousYearPaper);
                if (result)
                {
                    _logger.LogInformation("Paper edited successfully!");
                    return Ok(ResponseResult<Guid>.Success("Paper edited successfully!",this.UserId));
                }
                else
                {
                    _logger.LogCritical("Paper not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Paper not edited!"));
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
        /// Delete PaperPdf
        /// </summary>
        /// <param name="paperPdf"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin,Staff")]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.GetPaperPdf paperPdf)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.PreviousYearPaper,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.DeletePaperValidator.ValidateAsync(paperPdf);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                paperPdf.UserId = this.UserId;
                var result = await _previousYearPaperRepository.Delete(paperPdf);
                if (result)
                {
                    _logger.LogInformation("Paper deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Paper deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Paper not deleted!"));
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
        /// GetAll Papers by Filters
        /// </summary>
        /// <param name="getAllPapers"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getallpapers")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.PreviousYearPaperList>))]
        public async Task<IActionResult> GetAll(Req.GetAllPapers getAllPapers)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.PreviousYearPaper,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAllPapersValidator.ValidateAsync(getAllPapers);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region SubCourse Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = getAllPapers.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region Institute Id Check
                Req.InstituteById institute = new()
                {
                    Id = getAllPapers.InstituteId
                };
                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion
                var result = await _previousYearPaperRepository.GetAllPapers(getAllPapers);
                if (result != null && result.PreviousYearPapers.Count > 0)
                {
                    _logger.LogInformation("Get all Papers successfully!");
                    return Ok(ResponseResult<Res.PreviousYearPaperList>.Success("Get all Papers successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Papers not found!");
                    return Ok(ResponseResult<Res.PreviousYearPaperList>.Failure("Papers not found!", new()));
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
        /// GetAll 50 recent papers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getall50")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.PreviousYearPaperList>))]
        public async Task<IActionResult> GetAll50()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _previousYearPaperRepository.GetAll50();
                if (result != null)
                {
                    _logger.LogInformation("Get all Papers successfully!");
                    return Ok(ResponseResult<Res.PreviousYearPaperList>.Success("Get all Papers successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Papers not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Papers not found!"));
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getallfaculty")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.VideoAuthorList>))]
        public async Task<IActionResult> GetAllFaculties(Req.GetAllFacultyList facultyList)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                //var validation = await _validation.GetAllVideosAuthorsValidator.ValidateAsync(authors);
                //errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                //if (errorResponse != null)
                //{
                //    return BadRequest(errorResponse);
                //}
                #endregion
                #region SubjectCategory Id Check
                Req.SubCourseById subCourse = new()
                {
                    Id = facultyList.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse category Id!"));
                }
                #endregion
                var result = await _previousYearPaperRepository.GetAllFaculties(facultyList);
                if (result != null && result.Faculties.Any())
                {
                    _logger.LogInformation("Get all Faculties successfully!");
                    return Ok(ResponseResult<Res.FacultyList>.Success("Get all Faculties successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Faculties not found!");
                    return Ok(ResponseResult<Res.FacultyList>.Failure("Faculties not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallfaculties", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallfaculties", ex);
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
                var result = await _previousYearPaperRepository.Showallids();
                if (result != null && result.Videos.Any())
                {
                    _logger.LogInformation("Get all previous year papers successfully!");
                    return Ok(ResponseResult<Res.VideoListV2>.Success("Get all previous year papers successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Previous year papers not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Previous year papers not found!"));
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


        [HttpGet]
        [Route("getyearlist")]
        public IActionResult GetYearList()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result =  _previousYearPaperRepository.GetYearList();
                if (result != null && result.Years.Any())
                {
                    _logger.LogInformation("Get all years successfully!");
                    return Ok(ResponseResult<Res.YearList>.Success("Get all year successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Year not found!");
                    return Ok(ResponseResult<Res.YearList>.Failure("Year not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getyearlist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getyearlist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }

    }
}
