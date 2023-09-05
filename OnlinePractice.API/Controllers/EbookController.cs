using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.AuthDB;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class EbookController : BaseController
    {
        private readonly IEbookRepository  _ebookRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly ILogger<EbookController> _logger;
        private readonly IEbookValidation _validation;
        private readonly ICommonRepository _commonRepository;

        public EbookController(IEbookRepository ebookRepository, ILogger<EbookController> logger,IEbookValidation courseValidation,ICourseRepository courseRepository
            ,IExamTypeRepository examTypeRepository,ISubCourseRepository subCourseRepository,ISubjectRepository subjectRepository,
            IInstituteRepository instituteRepository, ICommonRepository commonRepository)
        { 
            _ebookRepository = ebookRepository;
            _logger = logger;
            _validation = courseValidation;
            _courseRepository = courseRepository;
            _examTypeRepository = examTypeRepository;
            _subCourseRepository = subCourseRepository;
            _subjectRepository = subjectRepository;
            _instituteRepository= instituteRepository;
            _commonRepository = commonRepository;
        }


        /// <summary>
        /// Upload Ebook Image
        /// </summary>
        /// <param name="ebookThumbnail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadebookthumbnail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.EbookThumbnailimage ebookThumbnail)
        {
            ErrorResponse? errorResponse;
            try
            {

                #region Validate Request Model
                var validation = await _validation.EbookThumbnailValidator.ValidateAsync(ebookThumbnail);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _ebookRepository.UploadImage(ebookThumbnail);



                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Ebook Thumbnail upload successfully!");
                    return Ok(ResponseResult<string>.Success("Ebook Thumbnail upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebook Thumbnail not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebook Thumbnail not uploaded!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadimage", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "uploadimage", ex);
                return Ok(ResponseResult<DBNull>.Failure("Ebook Thumbnail not created!"));
            }
        }


        /// <summary>
        /// Upload Ebook PDF
        /// </summary>
        /// <param name="ebookPdfUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadebookpdf")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadEbookPdfUrl([FromForm] Req.EbookPdfUrl ebookPdfUrl)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.EbookFileValidator.ValidateAsync(ebookPdfUrl);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _ebookRepository.UploadEbookPdfUrl(ebookPdfUrl);



                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Ebook Pdf upload successfully!");
                    return Ok(ResponseResult<string>.Success("Ebook Pdf upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebook Pdf not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebook Pdf not uploaded!"));
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
        /// Create Ebook Data
        /// </summary>
        /// <param name="createEbook"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateEbook createEbook)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Ebook,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CreateEbookValidator.ValidateAsync(createEbook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = createEbook.ExamTypeId
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
                    Id = createEbook.CourseId
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
                    Id = createEbook.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = createEbook.SubjectCategory
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
                    Id = createEbook.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                createEbook.UserId = this.UserId;
                var result = await _ebookRepository.Create(createEbook);

                if (result!=null)
                {
                    _logger.LogInformation("Ebook created successfully!");
                    return Ok(ResponseResult<Res.AuthorAndLanguage2>.Success("Ebook created successfully!",result));
                }
                else
                {
                    _logger.LogCritical("Ebook not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebook not created!"));
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
        /// Edit Ebook Data
        /// </summary>
        /// <param name="ebook"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditEbook ebook)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Ebook,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.EditEbookValidator.ValidateAsync(ebook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region ExamType Id Check
                Req.ExamTypeById exam = new()
                {
                    Id = ebook.ExamTypeId
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
                    Id = ebook.CourseId
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
                    Id = ebook.SubCourseId
                };

                var isSubCourseExist = await _subCourseRepository.IsExist(subCourse);
                if (!isSubCourseExist)
                {
                    _logger.LogInformation("Invalid SubCourse Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubCourse Id!"));
                }
                #endregion
                #region SubCourse Id Check
                Req.SubjectCategoryById subjectCategory = new()
                {
                    Id = ebook.SubjectCategory
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
                    Id = ebook.InstituteId
                };

                var isInstituteExist = await _instituteRepository.IsInstituteExists(institute);
                if (!isInstituteExist)
                {
                    _logger.LogInformation("Invalid Institute category Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute category Id!"));
                }
                #endregion
                ebook.UserId = this.UserId;
                var result = await _ebookRepository.Edit(ebook);
                if (result !=null)
                {
                    _logger.LogInformation("Ebook edited successfully!");
                    return Ok(ResponseResult<Res.AuthorAndLanguage2>.Success("Ebook edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Ebook not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebook not edited!"));
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
        /// Delete Ebook Data
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin,Staff")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.DeleteEbook ebook)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.DeleteEbookValidator.ValidateAsync(ebook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Ebook,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion

                ebook.UserId = this.UserId;
                var result = await _ebookRepository.Delete(ebook);
                if (result)
                {
                    _logger.LogInformation("Records deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Ebook deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Records not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebook not deleted!"));
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
        /// Get Ebook Data byId
        /// </summary>
        /// <param name="ebook"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getbyid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.Ebook>))]
        public async Task<IActionResult> GetById(Req.EbookById ebook)
        {

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetByIdEbookValidator.ValidateAsync(ebook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Ebook,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                var result = await _ebookRepository.GetEbookById(ebook);

                if (result != null)
                {
                    _logger.LogInformation("Ebook get successfully");
                    return Ok(ResponseResult<Res.Ebook>.Success("Ebook get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Ebook not found!");
                    return Ok(ResponseResult<Res.Ebook>.Failure("Ebook not found!", new()));
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
        /// GetAll Ebookdata by Filter
        /// </summary>
        /// <param name="ebookV1"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles ="Admin,Staff")]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.EbookListV1>))]
        public async Task<IActionResult> GetAll(Req.GetAllEbookV1 ebook)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.Ebook,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAllEbookValidator.ValidateAsync(ebook);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _ebookRepository.GetAll(ebook);
                if (result != null && result.Ebooks.Any())
                {
                    _logger.LogInformation("Get all Ebooks successfully!");
                    return Ok(ResponseResult<Res.EbookListV1>.Success("Get all Ebooks successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebooks not found!");
                    return Ok(ResponseResult<Res.EbookListV1>.Failure("Ebooks not found!", new()));
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.EbookListV1>))]
        public async Task<IActionResult>  GetAll50()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result =  await  _ebookRepository.GetAll50();
                if (result != null )
                {
                    _logger.LogInformation("Get all Ebooks successfully!");
                    return Ok(ResponseResult<Res.EbookListV1>.Success("Get all Ebooks successfully!",result));
                }
                else
                {
                    _logger.LogCritical("Ebooks not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebooks not found!"));
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
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("getallauthors")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.EbookListV1>))]
        public async Task<IActionResult> GetAllAuthors(Req.GetAllAuthors authors)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.GetAllAuthersValidator.ValidateAsync(authors);
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
                var result = await _ebookRepository.GetAllAuthors(authors);
                if (result != null && result.EbookAuthers.Any())
                {
                    _logger.LogInformation("Get all authers successfully!");
                    return Ok(ResponseResult<Res.AutherList>.Success("Get all authers successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Authers not found!");
                    return Ok(ResponseResult<Res.AutherList>.Failure("Authers not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallauthers", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallauthers", ex);
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
                var result = await _ebookRepository.Showallids();
                if (result != null && result.Ebooks.Any())
                {
                    _logger.LogInformation("Get all Ebooks successfully!");
                    return Ok(ResponseResult<Res.EbookListV2>.Success("Get all Ebooks successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Ebooks not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Ebooks not found!"));
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
