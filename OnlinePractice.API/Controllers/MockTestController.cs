using Microsoft.AspNetCore.Http;
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
using FluentValidation;
using OnlinePractice.API.Models.Request;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Repository.Services;

namespace OnlinePractice.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class MockTestController : BaseController
    {
        public readonly IMockTestRepository _mockTestRepository;
        private readonly ILogger<MockTestController> _logger;
        private readonly IMockTestValidation _validation;
        public readonly IQuestionBankRepository _questionBankRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISubCourseRepository _subCourseRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IExamTypeRepository _examTypeRepository;
        private readonly IExamPatternRepository _examPatternRepository;
        private readonly ICommonRepository _commonRepository;

        public MockTestController
            (
            ILogger<MockTestController> logger,
            IMockTestRepository mockTestRepository,
            IMockTestValidation validation,
            IQuestionBankRepository questionBankRepository,
            ISubjectRepository subjectRepository,
            ISubCourseRepository subCourseRepository,
            IExamTypeRepository examTypeRepository,
            IExamPatternRepository examPatternRepository,
            ICourseRepository courseRepository,
            ICommonRepository commonRepository
            )
        {
            _logger = logger;
            _mockTestRepository = mockTestRepository;
            _validation = validation;
            _questionBankRepository = questionBankRepository;
            _subjectRepository = subjectRepository;
            _subCourseRepository = subCourseRepository;
            _examTypeRepository = examTypeRepository;
            _examPatternRepository = examPatternRepository;
            _courseRepository = courseRepository;
            _commonRepository = commonRepository;
        }

        #region MockTest Setting

        /// <summary>
        /// Api for Upload logo
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadmocktestlogoimage")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImage([FromForm] Req.LogoImage image)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.MocktTestLogoValidator.ValidateAsync(image);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                string result = await _mockTestRepository.UploadImage(image);



                if (!string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("MockTest Setting Logo upload successfully!");
                    return Ok(ResponseResult<string>.Success("MockTest Setting Logo upload successfully!", result));
                }
                else
                {
                    _logger.LogCritical("MockTest Setting Logo not uploaded!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Setting Logo not uploaded!"));
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
                return Ok(ResponseResult<DBNull>.Failure("MockTest Setting Logo not created!"));
            }
        }



        /// <summary>
        /// Create MockTestSettings
        /// </summary>
        /// <param name="mockTestSetting"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("create")]
        public async Task<IActionResult> Create(Req.CreateMockTestSetting mockTestSetting)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CreateMockTestSettingValidator.ValidateAsync(mockTestSetting);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region CheckNameDuplicacy
                Req.MockTestNameCheck mockTestName = new()
                {
                    MockTestName = mockTestSetting.MockTestName
                };
                var isDuplicate = await _mockTestRepository.IsDuplicate(mockTestName);
                if (isDuplicate)
                {
                    _logger.LogInformation("MockTest name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest name is already exist!"));
                }
                #endregion
                #region InstituteIdCheckExists
                Req.CheckInstitute institute = new()
                {
                    InstituteId = mockTestSetting.InstituteId
                };

                var IsInstituteExists = await _mockTestRepository.CheckInstitute(institute);
                if (!IsInstituteExists)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion

                #region Check Language

                var language = _mockTestRepository.CheckLanguage(mockTestSetting.Language);

                if (!language)
                {
                    _logger.LogInformation("Invalid Language!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Language!"));
                }
                #endregion
                mockTestSetting.UserId = this.UserId;
                var result = await _mockTestRepository.Create(mockTestSetting);
                if (result != null)
                {
                    _logger.LogInformation("Mocktest Setting created successfully!");
                    return Ok(ResponseResult<Res.MockTestInfo?>.Success("Mocktest Setting created successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest Setting not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Mocktest Setting not created!"));
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
        /// Edit MockTestSetting Method
        /// </summary>
        /// <param name="mockTestSetting"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getbyid")]
        public async Task<IActionResult> GetById(Req.MocktestSettingById mockTestSetting)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetMockTestSettingByIdValidator.ValidateAsync(mockTestSetting);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestIdCheckExists
                Req.CheckMockTestById checkMockTest = new()
                {
                    MockTestSettingId = mockTestSetting.Id
                };

                var IsMockTestSettingIdExists = await _mockTestRepository.CheckMockTestById(checkMockTest);
                if (!IsMockTestSettingIdExists)
                {
                    _logger.LogInformation("Invalid MockTestSetting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid MockTestSetting Id!"));
                }
                #endregion
                mockTestSetting.UserId = this.UserId;
                var result = await _mockTestRepository.GetMockTestSettingById(mockTestSetting);
                if (result != null)
                {
                    _logger.LogInformation("Mocktest Setting get successfully!");
                    return Ok(ResponseResult<Res.MockTestSetting>.Success("Mocktest Setting get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest Setting are not found!");
                    return Ok(ResponseResult<Res.MockTestSetting>.Failure("Mocktest Setting are not found!",new()));
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
        /// Edit MockTestSetting Method
        /// </summary>
        /// <param name="mockTestSetting"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        [Route("edit")]
        public async Task<IActionResult> Edit(Req.EditMockTestSetting mockTestSetting)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.EditMockTestSettingValidator.ValidateAsync(mockTestSetting);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region InstituteIdCheckExists
                Req.CheckInstitute institute = new()
                {
                    InstituteId = mockTestSetting.InstituteId
                };

                var IsInstituteExists = await _mockTestRepository.CheckInstitute(institute);
                if (!IsInstituteExists)
                {
                    _logger.LogInformation("Invalid Institute Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Institute Id!"));
                }
                #endregion
                #region CheckNameDuplicacy
                Req.EditMockTestNameCheck mockTestName = new()
                {
                    Id = mockTestSetting.Id,
                    MockTestName = mockTestSetting.MockTestName
                };
                var isDuplicate = await _mockTestRepository.IsEditDuplicate(mockTestName);
                if (isDuplicate)
                {
                    _logger.LogInformation("MockTest name is already exist!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest name is already exist!"));
                }
                #endregion
                mockTestSetting.UserId = this.UserId;
                var result = await _mockTestRepository.Edit(mockTestSetting);
                if (result)
                {
                    _logger.LogInformation("Mocktest Setting edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Mocktest Setting edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Mocktest Setting not edited!");
                    return Ok(ResponseResult<DBNull>.Failure("Mocktest Setting not edited!"));
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
        /// API for delete mocktest
        /// </summary>
        /// <param name="mocktestSetting"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin,Staff")]
        [Route("delete")]
        public async Task<IActionResult> Delete(Req.MocktestSettingById mocktestSetting)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.DeleteMockTestSettingValidator.ValidateAsync(mocktestSetting);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Check If Mocktest started by user
                var checkmockTest = await _mockTestRepository.CheckCurrentMocktest(mocktestSetting);
                if (checkmockTest)
                {
                    _logger.LogInformation("Mock test cannot be deleted because user has started the mocktest!");
                    return Ok(ResponseResult<DBNull>.Failure("Mock test cannot be deleted because user has started the mocktest!"));
                }
                #endregion
                mocktestSetting.UserId = this.UserId;
                var result = await _mockTestRepository.Delete(mocktestSetting);
                if (result)
                {
                    _logger.LogInformation("MockTest Settings deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("MockTest Settings deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("MockTest Settings not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Settings not deleted!"));
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
        /// 
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getallv1")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.InstituteList>))]
        public async Task<IActionResult> GetAllMockTestSettings(Req.GetAllMockTest allMockTest)
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _mockTestRepository.GetAllMockTestSettings(allMockTest);

                if (result.GetAllMocktests.Any())
                {
                    _logger.LogInformation("record get successfully");
                    return Ok(ResponseResult<Res.MockTestSettingList>.Success("MockTest Setting gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("record not found!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Setting not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestsettings", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallmocktestsettings", ex);
                return Ok(ResponseResult<DBNull>.Failure("Mocktest Settings not Found!"));
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("getmocktestuser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<Res.UserDetailsById>))]
        public async Task<IActionResult> GetUsersMocktest(string Id)
        {
            ErrorResponse? errorResponse;
            try
            {
                Req.GetUserEmail model = new()
                {
                    Email = Id
                };
                var result = await _mockTestRepository.GetMocktestUserDetails(model);
                if (result != null)
                {
                    _logger.LogInformation(" Details get successfully!");
                    return Ok(ResponseResult<Res.UserDetailsById>.Success("Details get successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Admin not found!");
                    return Ok(ResponseResult<Res.UserDetailsById>.Failure("Admin not found!", new Res.UserDetailsById()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getuserdetails", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getuserdetails", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Get all mocktest
        /// </summary>
        /// <param name="allMockTest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getall")]
        public async Task<IActionResult> GetAllMockTestSettingsV1(Req.GetAllMockTestV1 allMockTest)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAllQuestionsValidator.ValidateAsync(allMockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                var result = await _mockTestRepository.GetAllMockTestSettingsV1(allMockTest);

                if (result != null && result.GetAllMocktests.Any())
                {
                    _logger.LogInformation("Get all mocktest successfully");
                    return Ok(ResponseResult<Res.MockTestSettingListV1>.Success("Get all mocktest successfully", result));
                }
                else
                {
                    _logger.LogInformation("record not found!");
                    return Ok(ResponseResult<Res.MockTestSettingListV1>.Failure("MockTest Setting not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallv1", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallv1", ex);
                return Ok(ResponseResult<DBNull>.Failure("Mocktest Settings not Found!"));
            }
        }
        #endregion

        #region MockTestQuestions
        /// <summary>
        /// API for getting mocktest questions
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("getquestionsbyfilter")]
        public async Task<IActionResult> GetAllQuestion(Req.GetAllQuestions questions)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAllQuestionsNewValidator.ValidateAsync(questions);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region Validate SubCourse
                Req.SubCourseById subCourse = new()
                {
                    Id = questions.SubCourseId
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
                    Id = questions.SubjectId
                };
                var isSubjectExist = await _subjectRepository.IsExist(subject);
                if (!isSubjectExist)
                {
                    _logger.LogInformation("Invalid Subject Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Subject Id!"));
                }

                #endregion
                #region TopicExists
                Req.CheckTopicId topicId = new()
                {
                    Id = questions.TopicId
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
                    Id = questions.SubTopicId
                };
                var isSubTopicExists = await _questionBankRepository.CheckSubTopicExists(subtopicId);
                if (!isSubTopicExists)
                {
                    _logger.LogInformation("Invalid SubTopicId Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid SubTopicId Id!"));
                }
                #endregion
                var result = await _mockTestRepository.GetQuestionsByFilter(questions);
                if (result != null && result.MockTestQuestions.Any())
                {
                    _logger.LogInformation("Get all question successfully!");
                    return Ok(ResponseResult<Res.MockTestQuestionsList>.Success("Get all question successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Questions not found!");
                    return Ok(ResponseResult<Res.MockTestQuestionsList>.Failure("Questions not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getquestionsbyfilter", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getquestionsbyfilter", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }



        /// <summary>
        /// createMockTestQuestions
        /// </summary>
        /// <param name="createMockTestQuestions"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("createmocktestquestions")]
        public async Task<IActionResult> CreateMockTestQuestion(Req.CreateMockTestQuestionList MockTestQuestions)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.CreateMockTestQuestionsValidator.ValidateAsync(MockTestQuestions);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = MockTestQuestions.MocktestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion

                #region ExamTypeIdCheckExists
                Req.ExamTypeById examType = new()
                {
                    Id = MockTestQuestions.ExamTypeId
                };

                var isExamTypeExists = await _examTypeRepository.IsExist(examType);
                if (!isExamTypeExists)
                {
                    _logger.LogInformation("Invalid examType Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid ExamType Id!"));
                }
                #endregion

                #region CourseIdCheckExists
                Req.CourseById course = new()
                {
                    Id = MockTestQuestions.CourseId
                };

                var IsCoursExists = await _courseRepository.IsExist(course);
                if (!IsCoursExists)
                {
                    _logger.LogInformation("Invalid course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid course Id!"));
                }
                #endregion
                #region SubCourseIdCheckExists
                Req.SubCourseById subCourse = new()
                {
                    Id = MockTestQuestions.SubCourseId
                };

                var IsSubCourseExists = await _subCourseRepository.IsExist(subCourse);
                if (!IsSubCourseExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                #region ExamPatternIdCheckExists
                Req.GetExamPatternId examPatternId = new()
                {
                    Id = MockTestQuestions.ExamPatternId
                };

                var IsExamPatternIdExists = await _examPatternRepository.IsExamPatternIdExist(examPatternId);
                if (!IsExamPatternIdExists)
                {
                    _logger.LogInformation("Invalid ExamPatternId  Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid ExamPatternId  Id!"));
                }
                #endregion
                MockTestQuestions.UserId = this.UserId;
                var result = await _mockTestRepository.CreateMockTestQuestions(MockTestQuestions);
                if (result)
                {
                    _logger.LogInformation("Mocktest Questions created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Mocktest Questions created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Mocktest Questions not created!");
                    return Ok(ResponseResult<DBNull>.Failure("Mocktest Questions not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Api for update mocktest questions
        /// </summary>
        /// <param name="MockTestQuestions"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("updatemocktestquestions")]
        public async Task<IActionResult> UpdateMockTestQuestion(Req.UpdateMockTestQuestionList MockTestQuestions)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.UpdateMockTestQuestionListValidator.ValidateAsync(MockTestQuestions);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = MockTestQuestions.MocktestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                MockTestQuestions.UserId = this.UserId;
                var result = await _mockTestRepository.UpdateMockTestQuestions(MockTestQuestions);
                if (result)
                {
                    _logger.LogInformation("Mocktest Questions created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Mocktest Questions created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Mocktest Questions not found!");
                    return Ok(ResponseResult<DBNull>.Failure("Mocktest Questions not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// Get mocktest questions by id
        /// </summary>
        /// <param name="mockTestById"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("mocktestquestiongetbyid")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> MockTestQuestionGetById(Req.MockTestById mockTestById)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.MockTestByIdValidator.ValidateAsync(mockTestById);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = mockTestById.MockTestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                var result = await _mockTestRepository.GetMocktestQuestionById(mockTestById);
                if (result != null && result.MockTestQuestions.Any())
                {
                    _logger.LogInformation("Get Mocktest Questions successfully!");
                    return Ok(ResponseResult<Res.MockTestQuestionList>.Success("Get Mocktest Questions successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest Questions not found!");
                    return Ok(ResponseResult<Res.MockTestQuestionList>.Failure("Mocktest Questions not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "mocktestquestiongetbyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "mocktestquestiongetbyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        [HttpPost]
        [Route("mocktestquestionspdf")]
        public async Task<IActionResult> MockTestQuestionsPdf(Req.MockTestById mockTestById)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.MockTestByIdValidator.ValidateAsync(mockTestById);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = mockTestById.MockTestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                var result = await _mockTestRepository.GetMocktestQuestionPdf(mockTestById);
                if (result != null && result.MockTestQuestions.Any())
                {
                    _logger.LogInformation("Get Mocktest Questions successfully!");
                    return Ok(ResponseResult<Res.MockTestQuestionListPdf>.Success("Get Mocktest Questions successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Mocktest Questions not found!");
                    return Ok(ResponseResult<Res.MockTestQuestionListPdf>.Failure("Mocktest Questions not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "mocktestquestiongetbyid", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "mocktestquestiongetbyid", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for publish mocktest
        /// </summary>
        /// <param name="mockTest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("publishmocktest")]
        public async Task<IActionResult> PublishMockTest(Req.MockTestById mockTest)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.MockTestByIdValidator.ValidateAsync(mockTest);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = mockTest.MockTestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                #region
                Req.MockTestById publishMock = new()
                {
                    MockTestSettingId = mockTest.MockTestSettingId

                };
               var CheckPublish = await _mockTestRepository.CheckPublishMockTest(publishMock);
                if (!CheckPublish)
                {
                    _logger.LogInformation("Please fill all subjects ,sections and questions before publish!");
                    return Ok(ResponseResult<DBNull>.Failure("Please fill all subjects ,sections and questions before publish!"));
                }

                #endregion
                mockTest.UserId = this.UserId;
                var result = await _mockTestRepository.PublishMockTest(mockTest);
                if (result)
                {
                    _logger.LogInformation("Mocktest published successfully!");
                    return Ok(ResponseResult<Res.MockTestQuestionList>.Success("Mocktest published successfully!"));
                }
                else
                {
                    _logger.LogCritical("Mocktest  not published!");
                    return Ok(ResponseResult<Res.MockTestQuestionList>.Failure("Mocktest not published!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createMockTestQuestions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// Generate Automatic mocktest
        /// </summary>
        /// <param name="automaticMockTestQuestion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [Route("generateautomatictmocktest")]
        public async Task<IActionResult> GenerateAutomatictMockTest(Req.AutomaticMockTestQuestion automaticMockTestQuestion)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.GetAutomaticMockTestQuestionValidator.ValidateAsync(automaticMockTestQuestion);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                #region MockTestSettingsIdCheckExists
                Req.CheckMockTestById mocktestSettingById = new()
                {
                    MockTestSettingId = automaticMockTestQuestion.MockTestSettingId
                };

                var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
                if (!IsMocktestSettingExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion

                #region ExamTypeIdCheckExists
                Req.ExamTypeById examType = new()
                {
                    Id = automaticMockTestQuestion.ExamTypeId
                };

                var isExamTypeExists = await _examTypeRepository.IsExist(examType);
                if (!isExamTypeExists)
                {
                    _logger.LogInformation("Invalid examType Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid ExamType Id!"));
                }
                #endregion

                #region CourseIdCheckExists
                Req.CourseById course = new()
                {
                    Id = automaticMockTestQuestion.CourseId
                };

                var IsCoursExists = await _courseRepository.IsExist(course);
                if (!IsCoursExists)
                {
                    _logger.LogInformation("Invalid course Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid course Id!"));
                }
                #endregion
                #region SubCourseIdCheckExists
                Req.SubCourseById subCourse = new()
                {
                    Id = automaticMockTestQuestion.SubCourseId
                };

                var IsSubCourseExists = await _subCourseRepository.IsExist(subCourse);
                if (!IsSubCourseExists)
                {
                    _logger.LogInformation("Invalid Mocktest Setting Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
                }
                #endregion
                #region ExamPatternIdCheckExists
                Req.GetExamPatternId examPatternId = new()
                {
                    Id = automaticMockTestQuestion.ExamPatternId
                };

                var IsExamPatternIdExists = await _examPatternRepository.IsExamPatternIdExist(examPatternId);
                if (!IsExamPatternIdExists)
                {
                    _logger.LogInformation("Invalid ExamPatternId  Id!");
                    return Ok(ResponseResult<DBNull>.Failure("Invalid ExamPatternId  Id!"));
                }
                #endregion
                var result = await _mockTestRepository.GetAutomaticMockTestQuestions(automaticMockTestQuestion);
                if (result != null)
                {
                    _logger.LogInformation("Get automatic mocktest questions successfully!");
                    return Ok(ResponseResult<Res.AutoMockTestQuestionList>.Success("Get automatic mocktest questions successfully!", result));
                }
                else
                {
                    _logger.LogCritical("All Questions are not availble!");
                    return Ok(ResponseResult<Res.AutoMockTestQuestionList>.Failure("All Questions are not availble!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getautomatictmocktestquestion", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getautomatictmocktestquestion", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }
        /// <summary>
        /// API for publishing mocktest
        /// </summary>
        /// <param name="automaticMockTestQuestion"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("publishautomaticmocktest")]
        //public async Task<IActionResult> PublishAutoMaticMockTest(Req.AutoMockTestQuestionList automaticMockTestQuestion)
        //{
        //    ErrorResponse? errorResponse;
        //    try
        //    {
        //        #region Validate Request Model
        //        var validation = await _validation.PublishAutoMaticMocktestQuestionValidator.ValidateAsync(automaticMockTestQuestion);
        //        errorResponse = CustomResponseValidator.CheckModelValidation(validation);
        //        if (errorResponse != null)
        //        {
        //            return BadRequest(errorResponse);
        //        }
        //        #endregion
        //        #region MockTestSettingsIdCheckExists
        //        Req.CheckMockTestById mocktestSettingById = new()
        //        {
        //            MockTestSettingId = automaticMockTestQuestion.MockTestSettingId
        //        };

        //        var IsMocktestSettingExists = await _mockTestRepository.CheckMockTestById(mocktestSettingById);
        //        if (!IsMocktestSettingExists)
        //        {
        //            _logger.LogInformation("Invalid Mocktest Setting Id!");
        //            return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
        //        }
        //        #endregion

        //        #region ExamTypeIdCheckExists
        //        Req.ExamTypeById examType = new()
        //        {
        //            Id = automaticMockTestQuestion.ExamTypeId
        //        };

        //        var isExamTypeExists = await _examTypeRepository.IsExist(examType);
        //        if (!isExamTypeExists)
        //        {
        //            _logger.LogInformation("Invalid examType Id!");
        //            return Ok(ResponseResult<DBNull>.Failure("Invalid ExamType Id!"));
        //        }
        //        #endregion

        //        #region CourseIdCheckExists
        //        Req.CourseById course = new()
        //        {
        //            Id = automaticMockTestQuestion.CourseId
        //        };

        //        var IsCoursExists = await _courseRepository.IsExist(course);
        //        if (!IsCoursExists)
        //        {
        //            _logger.LogInformation("Invalid course Id!");
        //            return Ok(ResponseResult<DBNull>.Failure("Invalid course Id!"));
        //        }
        //        #endregion
        //        #region SubCourseIdCheckExists
        //        Req.SubCourseById subCourse = new()
        //        {
        //            Id = automaticMockTestQuestion.SubCourseId
        //        };

        //        var IsSubCourseExists = await _subCourseRepository.IsExist(subCourse);
        //        if (!IsSubCourseExists)
        //        {
        //            _logger.LogInformation("Invalid Mocktest Setting Id!");
        //            return Ok(ResponseResult<DBNull>.Failure("Invalid Mocktest Setting Id!"));
        //        }
        //        #endregion
        //        #region ExamPatternIdCheckExists
        //        Req.GetExamPatternId examPatternId = new()
        //        {
        //            Id = automaticMockTestQuestion.ExamPatternId
        //        };

        //        var IsExamPatternIdExists = await _examPatternRepository.IsExamPatternIdExist(examPatternId);
        //        if (!IsExamPatternIdExists)
        //        {
        //            _logger.LogInformation("Invalid ExamPatternId  Id!");
        //            return Ok(ResponseResult<DBNull>.Failure("Invalid ExamPatternId  Id!"));
        //        }
        //        #endregion
        //        automaticMockTestQuestion.UserId = this.UserId;
        //        var result = await _mockTestRepository.PublishAutoMaticMocktest(automaticMockTestQuestion);
        //        if (result)
        //        { 
        //            _logger.LogInformation("Mocktest published successfully!");
        //            return Ok(ResponseResult<Res.MockTestQuestionList>.Success("Mocktest published successfully!"));
        //        }
        //        else
        //        {
        //            _logger.LogCritical("Mocktest  not published!");
        //            return Ok(ResponseResult<DBNull>.Failure("Mocktest not published!"));
        //        }
        //    }
        //    catch (DbUpdateException exp)
        //    {
        //        var ex = exp.InnerException as SqlException;
        //        errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "publishautomaticmocktest", ex);
        //        return BadRequest(errorResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "publishautomaticmocktest", ex);
        //        return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
        //    }

        //}

        [HttpDelete]
        [Authorize(Roles = "Admin,Staff")]
        [Route("deleteallmocktestquestions")]
        public async Task<IActionResult> RemoveMockTestQuestions(Req.MocktestSettingById mocktestSetting)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Check User Permission
                Com.CheckPermission checkPermission = new()
                {
                    Module = ModuleType.MockTest,
                    UserId = this.UserId

                };
                var checkUserPermission = await _commonRepository.CheckUserPermission(checkPermission);
                if (!checkUserPermission)
                {
                    return Unauthorized();
                }
                #endregion
                #region Validate Request Model
                var validation = await _validation.DeleteMockTestSettingValidator.ValidateAsync(mocktestSetting);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion
                mocktestSetting.UserId = this.UserId;
                var result = await _mockTestRepository.RemoveMockTestQuestions(mocktestSetting);
                if (result)
                {
                    _logger.LogInformation("MockTest Questions deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("MockTest Questions deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("MockTest Questions not deleted!");
                    return Ok(ResponseResult<DBNull>.Failure("MockTest Questions not deleted!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removemocktestquestions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removemocktestquestions", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

        #endregion


    }
}
