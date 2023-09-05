using Microsoft.EntityFrameworkCore;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Identity;

using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using System.Xml.Linq;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using Org.BouncyCastle.Ocsp;
using Microsoft.VisualBasic;
using Stripe;
using System.Linq;
using System.Collections;
using System.Globalization;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using static OnlinePractice.API.Controllers.StripeController;

namespace OnlinePractice.API.Repository.Services
{
    public class MockTestRepository : IMockTestRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamPatternRepository _examPatternRepository;
        public MockTestRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IFileRepository fileRepository,
            IHttpContextAccessor baseurl, ISubjectRepository subjectRepository, IExamPatternRepository examPatternRepository)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _baseUrl = baseurl;
            _subjectRepository = subjectRepository;
            _examPatternRepository = examPatternRepository;
        }

        #region MockTestSettings

        /// <summary>
        /// Create MockTest Data Method
        /// </summary>
        /// <param name="mockTestSetting"></param>
        /// <returns></returns>
        public async Task<Res.MockTestInfo?> Create(Req.CreateMockTestSetting mockTestSetting)
        {
            // string language = (string.Join(",", mockTestSetting.Languages.Select(x => x.Language.ToString())));
            //  var results = String.Join(",", mockTestSetting.Languages);
            TimeSpan TimeSettingDuration = new TimeSpan(mockTestSetting.TimeDurationHours, mockTestSetting.TimeDurationMinutes, 0);
            //   TimeSpan ReattemptsDuration = new TimeSpan(mockTestSetting.ReattemptsHours, mockTestSetting.ReattemptsMinutes, 0);

            DM.MockTestSettings mockTest = new()
            {
                MockTestName = mockTestSetting.MockTestName,
                Description = mockTestSetting.Description,
                InstituteId = mockTestSetting.InstituteId,
                IsFree = mockTestSetting.IsFree,
                Price = mockTestSetting.Price,
                TimeSettingDuration = TimeSettingDuration,
                TestAvailability = mockTestSetting.TestAvailability,
                TestStartTime = mockTestSetting.TestStartTime,
                TestSpecificFromDate = mockTestSetting.TestSpecificFromDate,
                TestSpecificToDate = mockTestSetting.TestSpecificToDate,
                IsAllowReattempts = mockTestSetting.IsAllowReattempts,
                //   IsUnlimitedAttempts = mockTestSetting.IsUnlimitedAttempts,
                TotalAttempts = mockTestSetting.TotalAttempts,
                //ReattemptsDays = mockTestSetting.ReattemptsDays,
                // ReattemptsDuration = ReattemptsDuration,
                IsTestResume = mockTestSetting.IsTestResume,
                //IsUnlimitedResume = mockTestSetting.IsUnlimitedResume,
                //TotalResume = mockTestSetting.TotalResume,
                //BackButton = BackButton.Allowed,
                IsMarksResultFormat = mockTestSetting.IsMarksResultFormat,
                IsPassFailResultFormat = mockTestSetting.IsPassFailResultFormat,
                //IsRankResultFormat = mockTestSetting.IsRankResultFormat,
                ResultDeclaration = mockTestSetting.ResultDeclaration,
                IsShowCorrectAnswer = mockTestSetting.IsShowCorrectAnswer,
                MockTestType = mockTestSetting.MockTestType,
                Language = mockTestSetting.Language,
                SecondaryLanguage = mockTestSetting.SecondaryLanguage,
                IsDraft = true,
                IsDeleted = false,
                IsActive = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = mockTestSetting.UserId,
            };

            int result = await _unitOfWork.Repository<DM.MockTestSettings>().Insert(mockTest);
            if (result > 0)
            {
                Res.MockTestInfo mock = new()
                {
                    Id = mockTest.Id,
                    Langauge = mockTest.Language
                };
                return mock;
            }
            return null;
        }

        public async Task<string> UploadImage(Req.LogoImage image)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(image.Image, "Mocktest");
            return domain + imageUrl;
        }

        /// <summary>
        /// Edit MockTest Data Method
        /// </summary>
        /// <param name="mockTestSetting"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditMockTestSetting mockTestSetting)
        {
            var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTestSetting.Id && !x.IsDeleted);
            TimeSpan TimeSettingDuration = new TimeSpan(mockTestSetting.TimeDurationHours, mockTestSetting.TimeDurationMinutes, 0);
            //   TimeSpan ReattemptsDuration = new TimeSpan(mockTestSetting.ReattemptsHours, mockTestSetting.ReattemptsMinutes, 0);
            if (mockTestData != null)
            {
                mockTestData.MockTestName = mockTestSetting.MockTestName;
                mockTestData.Description = mockTestSetting.Description;
                mockTestData.InstituteId = mockTestSetting.InstituteId;
                mockTestData.IsFree = mockTestSetting.IsFree;
                mockTestData.Price = mockTestSetting.Price;
                mockTestData.TimeSettingDuration = TimeSettingDuration;
                mockTestData.TestAvailability = mockTestSetting.TestAvailability;
                mockTestData.TestStartTime = mockTestSetting.TestStartTime;
                mockTestData.TestSpecificFromDate = mockTestSetting.TestSpecificFromDate;
                mockTestData.TestSpecificToDate = mockTestSetting.TestSpecificToDate;
                mockTestData.IsAllowReattempts = mockTestSetting.IsAllowReattempts;
                //    mockTestData.IsUnlimitedAttempts = mockTestSetting.IsUnlimitedAttempts;
                mockTestData.TotalAttempts = mockTestSetting.TotalAttempts;
                //  mockTestData.ReattemptsDays = mockTestSetting.ReattemptsDays;
                //   mockTestData.ReattemptsDuration = ReattemptsDuration;
                mockTestData.IsTestResume = mockTestSetting.IsTestResume;
                // mockTestData.IsUnlimitedResume = mockTestSetting.IsUnlimitedResume;
                // mockTestData.TotalResume = mockTestSetting.TotalResume;
                //  mockTestData.BackButton = mockTestSetting.BackButton;
                mockTestData.IsMarksResultFormat = mockTestSetting.IsMarksResultFormat;
                mockTestData.IsPassFailResultFormat = mockTestSetting.IsPassFailResultFormat;
                //  mockTestData.IsRankResultFormat = mockTestSetting.IsRankResultFormat;
                mockTestData.ResultDeclaration = mockTestSetting.ResultDeclaration;
                mockTestData.IsShowCorrectAnswer = mockTestSetting.IsShowCorrectAnswer;
                mockTestData.Language = mockTestSetting.Language;
                mockTestData.SecondaryLanguage = mockTestSetting.SecondaryLanguage;
                mockTestData.LastModifierUserId = mockTestSetting.UserId;
                mockTestData.LastModifyDate = DateTime.UtcNow;
                var result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(mockTestData);
                return result > 0;
            }
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mocktestSetting"></param>
        /// <returns></returns>
        public async Task<Res.MockTestSetting?> GetMockTestSettingById(Req.MocktestSettingById mocktestSetting)
        {
            var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mocktestSetting.Id && !x.IsDeleted);
            if (mockTestData != null)
            {
                var instituteInfo = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == mockTestData.InstituteId);
                string instituteName = instituteInfo != null ? instituteInfo.InstituteName : "N/A";
                Res.MockTestSetting result = new()
                {
                    MockTestName = mockTestData.MockTestName,
                    Description = mockTestData.Description,
                    InstituteId = mockTestData.InstituteId,
                    InstituteName = instituteName,
                    Price = mockTestData.Price,
                    IsFree = mockTestData.IsFree,
                    TimeDurationHours = mockTestData.TimeSettingDuration.Hours,
                    TimeDurationMinutes = mockTestData.TimeSettingDuration.Minutes,
                    TimeSettingDuration = mockTestData.TimeSettingDuration,
                    TestAvailability = mockTestData.TestAvailability,
                    TestStartTime = mockTestData.TestStartTime,
                    TestSpecificFromDate = mockTestData.TestSpecificFromDate,
                    TestSpecificToDate = mockTestData.TestSpecificToDate,
                    IsAllowReattempts = mockTestData.IsAllowReattempts,
                    // IsUnlimitedAttempts = mockTestData.IsUnlimitedAttempts,
                    TotalAttempts = mockTestData.TotalAttempts,
                    ReattemptsDays = mockTestData.ReattemptsDays,
                    //ReattemptsHours = mockTestData.ReattemptsDuration.Hours,
                    //ReattemptsMinutes = mockTestData.ReattemptsDuration.Minutes,
                    //ReattemptsDuration = mockTestData.ReattemptsDuration,
                    IsTestResume = mockTestData.IsTestResume,
                    //IsUnlimitedResume = mockTestData.IsUnlimitedResume,
                    //TotalResume = mockTestData.TotalResume,
                    // BackButton = mockTestData.BackButton,
                    IsMarksResultFormat = mockTestData.IsMarksResultFormat,
                    IsPassFailResultFormat = mockTestData.IsPassFailResultFormat,
                    //  IsRankResultFormat = mockTestData.IsRankResultFormat,
                    ResultDeclaration = mockTestData.ResultDeclaration,
                    IsShowCorrectAnswer = mockTestData.IsShowCorrectAnswer,
                    IsDraft = mockTestData.IsDraft,
                    MockTestType = mockTestData.MockTestType,
                    Language = mockTestData.Language,
                    SecondaryLanguage = mockTestData.SecondaryLanguage,

                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allMockTest"></param>
        /// <returns></returns>
        public async Task<Res.MockTestSettingList> GetAllMockTestSettings(Req.GetAllMockTest allMockTest)
        {

            Res.MockTestSettingList mockTestSetting = new()
            {
                GetAllMocktests = new()
            };
            if (allMockTest.PageNumber == 0 && allMockTest.PageSize == 0)
            {
                var MockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => !x.IsDeleted && !x.IsDraft && x.ExamPatternId != Guid.Empty);
                foreach (var item in MockTestData)
                {
                    List<string> lang = item.Language.Split(',').ToList();
                    List<Res.Languages> languageList = new();
                    foreach (var i in lang)
                    {
                        Res.Languages lg = new()
                        {
                            Language = i,
                        };
                        languageList.Add(lg);
                    }

                    Res.GetAllMocktest getAllMocktest = new()
                    {
                        MockTestName = item.MockTestName,
                        //AddedBy = _userManager.FindByIdAsync(allMockTest.UserId.ToString()).Result.DisplayName,
                        //Language = item.Language,
                        InstituteId = item.InstituteId,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    // getAllMocktest.languages.AddRange(languageList);
                    mockTestSetting.TotalRecords = MockTestData.Count;
                    mockTestSetting.GetAllMocktests.Add(getAllMocktest);
                }
                return mockTestSetting;
            }
            else
            {
                var MockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => !x.IsDeleted && !x.IsDraft && x.ExamPatternId != Guid.Empty);
                foreach (var item in MockTestData)
                {
                    List<string> lang = item.Language.Split(',').ToList();
                    List<Res.Languages> languageList = new();
                    foreach (var i in lang)
                    {
                        Res.Languages lg = new()
                        {
                            Language = i,
                        };
                        languageList.Add(lg);
                    }
                    Res.GetAllMocktest getAllMocktest = new()
                    {
                        MockTestName = item.MockTestName,
                        // AddedBy = _userManager.FindByIdAsync(allMockTest.UserId.ToString()).Result.DisplayName,
                        // Language = item.Language,
                        InstituteId = item.InstituteId,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    //getAllMocktest.languages.AddRange(languageList);
                    mockTestSetting.GetAllMocktests.Add(getAllMocktest);
                }
                var result = mockTestSetting.GetAllMocktests.Page(allMockTest.PageNumber, allMockTest.PageSize);
                mockTestSetting.GetAllMocktests = result.ToList();
                mockTestSetting.TotalRecords = MockTestData.Count;
                return mockTestSetting;
            }

        }

        /// <summary>
        /// Get All MockTests
        /// </summary>
        /// <param name="allMockTest"></param>
        /// <returns></returns>
        public async Task<Res.MockTestSettingListV1?> GetAllMockTestSettingsV1(Req.GetAllMockTestV1 allMockTest)
        {
            Res.MockTestSettingListV1 mockTestSetting = new()
            {
                GetAllMocktests = new()
            };
            List<DM.MockTestSettings>? mockTests = new();

            var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.ExamTypeId == allMockTest.ExamTypeId &&
            x.CourseId == allMockTest.CourseId && x.SubCourseId == allMockTest.SubCourseId && x.ExamPatternId == allMockTest.ExamPatternId && !x.IsDeleted && x.IsActive, orderBy: x => x.OrderByDescending(x => x.CreationDate));
            if (mockTestData != null && allMockTest.InstituteId != Guid.Empty && allMockTest.Language != string.Empty)
            {
                mockTests = mockTestData.Where(x => x.InstituteId == allMockTest.InstituteId && x.Language == allMockTest.Language).ToList();
            }
            else if (mockTestData != null && allMockTest.Language != string.Empty && allMockTest.InstituteId == Guid.Empty)
            {
                mockTests = mockTestData.Where(x => x.Language == allMockTest.Language).ToList();
            }
            else if (mockTestData != null && allMockTest.InstituteId != Guid.Empty && allMockTest.Language == string.Empty)
            {
                mockTests = mockTestData.Where((x) => x.InstituteId == allMockTest.InstituteId).ToList();
            }
            else
            {
                mockTests = mockTestData?.ToList();
            }
            if (mockTests != null && mockTests.Any())
            {

                foreach (var item in mockTests)
                {
                    var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                    string instituteName = instituteDetails != null ? instituteDetails.InstituteName : "N/A";
                    var userName = await _userManager.FindByIdAsync(item.CreatorUserId.ToString());
                    string Name = userName != null ? userName.DisplayName : "N/A";
                    Res.GetAllMocktestV1 getAllMocktest = new()
                    {
                        Id = item.Id,
                        MockTestName = item.MockTestName,
                        AddedBy = Name,
                        InstituteName = instituteName,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate,
                        CreationUserId = item.CreatorUserId,
                        Status = item.IsDraft ? "Pending" : "Completed"
                    };
                    getAllMocktest.Languages.Add(item.Language);
                    getAllMocktest.MockTestType.Add(item.MockTestType.ToString());
                    //getAllMocktest.languages.AddRange(languageList);
                    mockTestSetting.GetAllMocktests.Add(getAllMocktest);
                }
                var result = mockTestSetting.GetAllMocktests.Page(allMockTest.PageNumber, allMockTest.PageSize);
                mockTestSetting.GetAllMocktests = result.ToList();
                mockTestSetting.TotalRecords = mockTests.Count;
                return mockTestSetting;

            }
            return null;

        }
        public async Task<bool> CheckCurrentMocktest(Req.MocktestSettingById mocktest)
        {
            var mokctestsData = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.MockTestId == mocktest.Id && !x.IsDeleted && x.IsStarted && x.IsActive);
            if (mokctestsData.Any())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mocktestSettingById"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.MocktestSettingById mocktestSettingById)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(mocktestSettingById.UserId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);
            string role = userRoles.First();
            if (role == "Admin")
            {
                var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mocktestSettingById.Id && !x.IsDeleted);
                if (mockTestData != null)
                {
                    List<DM.MockTestQuestions> mockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mocktestSettingById.Id && !x.IsDeleted);
                    if (mockTestQuestions.Any())
                    {
                        mockTestQuestions.ForEach(s => s.IsActive = false);
                        mockTestQuestions.ForEach(s => s.IsDeleted = true);
                        mockTestQuestions.ForEach(s => s.DeleterUserId = mocktestSettingById.UserId);
                        mockTestQuestions.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                        await _unitOfWork.Repository<DM.MockTestQuestions>().Update(mockTestQuestions);
                    }
                    mockTestData.IsDeleted = true;
                    mockTestData.IsActive = false;
                    mockTestData.DeleterUserId = mocktestSettingById.UserId;
                    mockTestData.DeletionDate = DateTime.UtcNow;
                    int result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(mockTestData);
                    return true;
                }
            }
            else if (role == "Staff")
            {
                var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mocktestSettingById.Id && !x.IsDeleted && x.CreatorUserId == mocktestSettingById.UserId);
                if (mockTestData != null)
                {
                    List<DM.MockTestQuestions> mockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mocktestSettingById.Id && !x.IsDeleted);
                    if (mockTestQuestions.Any())
                    {
                        mockTestQuestions.ForEach(s => s.IsActive = false);
                        mockTestQuestions.ForEach(s => s.IsDeleted = true);
                        mockTestQuestions.ForEach(s => s.DeleterUserId = mocktestSettingById.UserId);
                        mockTestQuestions.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                        await _unitOfWork.Repository<DM.MockTestQuestions>().Update(mockTestQuestions);
                    }
                    mockTestData.IsDeleted = true;
                    mockTestData.IsActive = false;
                    mockTestData.DeleterUserId = mocktestSettingById.UserId;
                    mockTestData.DeletionDate = DateTime.UtcNow;
                    int result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(mockTestData);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        public async Task<bool> CheckInstitute(Req.CheckInstitute institute)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institute.InstituteId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkMockTest"></param>
        /// <returns></returns>
        public async Task<bool> CheckMockTestById(Req.CheckMockTestById checkMockTest)
        {
            var result = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == checkMockTest.MockTestSettingId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }
        /// <summary>
        /// Check dublicate record
        /// </summary>
        /// <param name="mockTestName"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.MockTestNameCheck mockTestName)
        {
            var result = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.MockTestName.Trim().ToLower() == mockTestName.MockTestName.Trim().ToLower() && !x.IsDeleted && x.IsActive);
            if (result != null)
                return true;
            else
                return false;

        }
        public async Task<bool> IsEditDuplicate(Req.EditMockTestNameCheck mockTestName)
        {
            var result = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id != mockTestName.Id && x.MockTestName.Trim().ToLower() == mockTestName.MockTestName.Trim().ToLower() && !x.IsDeleted && x.IsActive);
            if (result != null)
                return true;
            else
                return false;

        }
        public async Task<Res.UserDetailsById?> GetMocktestUserDetails(Req.GetUserEmail model)
        {
            Res.UserDetailsById user = new();
            var adminEmail = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
            var UserData = await _userManager.FindByEmailAsync(model.Email);
            if (UserData != null)
            {
                user.Id = new Guid(UserData.Id);
                user.Name = UserData.DisplayName;
                user.Location = UserData.Location;
                user.MobileNumber = UserData.PhoneNumber;
                user.Email = UserData.Email;
                user.ProfileImage = UserData.ProfileImage;
                user.Role = await GetRole(UserData.Email);
                user.SecondaryEmail = adminEmail.Select(x => x.Email).ToList();
                return user;
            }
            else if (adminEmail != null)
            {
                user.SecondaryEmail = adminEmail.Select(x => x.Email).ToList();
                return user;
            }
            return null;

        }
        private async Task<string> GetRole(string email)
        {
            int number = GenerateRandomNo();
            string alpha = GenerateRandomKeys();
            var unique = number + alpha;
            var UserData = await _userManager.FindByEmailAsync(email);
            var role = UserRoles.Staff + UserData.Password + unique;
            return role;
        }
        public int GenerateRandomNo()
        {
            int _min = 10000;
            int _max = 99999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        /// <summary>
        ///GenerateRandom Password 
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public string GenerateRandomKeys(PasswordOptions? opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        #endregion

        #region MockTestQuestions

        /// <summary>
        /// Get questions by filter
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<Res.MockTestQuestionsList?> GetQuestionsByFilter(Req.GetAllQuestions question)
        {
            Res.MockTestQuestionsList mockTestQuestionsList = new()
            {
                MockTestQuestions = new()
            };

            List<Res.MockTestQuestions> questinBanksList = new();

            mockTestQuestionsList.MockTestQuestions = new();

            var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == question.SubCourseId && x.SubjectId == question.SubjectId);
            Guid subjectCategoryId = subjectCategory != null ? subjectCategory.Id : Guid.Empty;
            List<string> questionList = _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.SubjectCategoryId == subjectCategoryId && x.TopicId == question.TopicId && x.SubTopicId == question.SubTopicId
                                && x.QuestionType == question.QuestionType && !x.IsDeleted).Result.DistinctBy(x => x.QuestionRefId).Select(x => x.QuestionRefId).ToList();
            if (questionList.Any())
            {
                foreach (string item in questionList)
                {
                    Res.MockTestQuestions questinBanks = new();
                    var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == item && !x.IsDeleted);

                    questinBanks.QuestionType = questions.First().QuestionType;
                    questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                    questinBanks.Mark = questions.First().Mark;
                    questinBanks.NegativeMark = questions.First().NegativeMark;
                    questinBanks.QuestionRefId = questions.First().QuestionRefId;
                    foreach (var items in questions)
                    {
                        switch (items.QuestionLanguage)
                        {
                            case "English":
                                Res.MockEnglish english = new();
                                english.Id = items.Id;
                                english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                english.OptionA = items.OptionA;
                                english.OptionB = items.OptionB;
                                english.OptionC = items.OptionC;
                                english.OptionD = items.OptionD;
                                english.Explanation = items.Explanation;
                                english.IsCorrectA = items.IsCorrectA;
                                english.IsCorrectB = items.IsCorrectB;
                                english.IsCorrectC = items.IsCorrectC;
                                english.IsCorrectD = items.IsCorrectD;
                                english.IsAvailable = items.QuestionText != "" ? true : false;
                                questinBanks.QuestionTableData.English = english;
                                break;
                            case "Hindi":
                                Res.MockHindi hindi = new();
                                hindi.Id = items.Id;
                                hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                hindi.OptionA = items.OptionA;
                                hindi.OptionB = items.OptionB;
                                hindi.OptionC = items.OptionC;
                                hindi.OptionD = items.OptionD;
                                hindi.IsCorrectA = items.IsCorrectA;
                                hindi.IsCorrectB = items.IsCorrectB;
                                hindi.IsCorrectC = items.IsCorrectC;
                                hindi.IsCorrectD = items.IsCorrectD;
                                hindi.Explanation = items.Explanation;
                                hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                questinBanks.QuestionTableData.Hindi = hindi;
                                break;
                            case "Gujarati":
                                Res.MockGujarati Gujarati = new();
                                Gujarati.Id = items.Id;
                                Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                Gujarati.OptionA = items.OptionA;
                                Gujarati.OptionB = items.OptionB;
                                Gujarati.OptionC = items.OptionC;
                                Gujarati.OptionD = items.OptionD;
                                Gujarati.IsCorrectA = items.IsCorrectA;
                                Gujarati.IsCorrectB = items.IsCorrectB;
                                Gujarati.IsCorrectC = items.IsCorrectC;
                                Gujarati.IsCorrectD = items.IsCorrectD;
                                Gujarati.Explanation = items.Explanation;
                                Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                questinBanks.QuestionTableData.Gujarati = Gujarati;
                                break;
                            case "Marathi":
                                Res.MockMarathi marathi = new();
                                marathi.Id = items.Id;
                                marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                marathi.OptionA = items.OptionA;
                                marathi.OptionB = items.OptionB;
                                marathi.OptionC = items.OptionC;
                                marathi.OptionD = items.OptionD;
                                marathi.IsCorrectA = items.IsCorrectA;
                                marathi.IsCorrectB = items.IsCorrectB;
                                marathi.IsCorrectC = items.IsCorrectC;
                                marathi.IsCorrectD = items.IsCorrectD;
                                marathi.Explanation = items.Explanation;
                                marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                questinBanks.QuestionTableData.Marathi = marathi;
                                break;
                            default:
                                break;
                        }

                    }

                    mockTestQuestionsList.MockTestQuestions.Add(questinBanks);
                }

                var result = mockTestQuestionsList.MockTestQuestions.Page(question.PageNumber, question.PageSize);
                mockTestQuestionsList.MockTestQuestions = result.ToList();
                mockTestQuestionsList.TotalRecords = questionList.Count();
                return mockTestQuestionsList;
            }
            else
            {
                return null;
            }

        }
        public async Task<bool> CreateMockTestQuestions(Req.CreateMockTestQuestionList mockTestQuestions)
        {
            int result = 0;
            var MockTestSettingData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTestQuestions.MocktestSettingId && !x.IsDeleted);
            if (MockTestSettingData != null)
            {
                MockTestSettingData.ExamTypeId = mockTestQuestions.ExamTypeId;
                MockTestSettingData.CourseId = mockTestQuestions.CourseId;
                MockTestSettingData.SubCourseId = mockTestQuestions.SubCourseId;
                MockTestSettingData.ExamPatternId = mockTestQuestions.ExamPatternId;
                MockTestSettingData.IsDraft = mockTestQuestions.IsDraft;
                result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(MockTestSettingData);
            }
            if (result > 0)
            {
                foreach (var item in mockTestQuestions.MockTestQuestions.DistinctBy(x => x.SubjectId))
                {

                    foreach (var it in item.SectionDetails.DistinctBy(x => x.SectionId))
                    {
                        foreach (var i in it.MockTestQuestions)
                        {
                            DM.MockTestQuestions mockTestQuestionsList = new();
                            mockTestQuestionsList.SubjectId = item.SubjectId;
                            mockTestQuestionsList.SectionId = it.SectionId;
                            mockTestQuestionsList.QuestionRefId = i.QuestionRefId;
                            mockTestQuestionsList.NegativeMark = i.NegativeMark;
                            mockTestQuestionsList.Marks = i.Mark;
                            mockTestQuestionsList.IsActive = true;
                            mockTestQuestionsList.IsDeleted = false;
                            mockTestQuestionsList.CreatorUserId = mockTestQuestions.UserId;
                            mockTestQuestionsList.CreationDate = DateTime.UtcNow;
                            mockTestQuestionsList.MockTestSettingId = mockTestQuestions.MocktestSettingId;
                            result = await _unitOfWork.Repository<DM.MockTestQuestions>().Insert(mockTestQuestionsList);

                        }

                    }
                }
            }
            return result > 0;
        }
        public async Task<bool> UpdateMockTestQuestions(Req.UpdateMockTestQuestionList mockTestQuestions)
        {
            int result = 0;

            var mockTestQuestionList = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestQuestions.MocktestSettingId && !x.IsDeleted);
            if (mockTestQuestionList.Any())
            {
                await _unitOfWork.Repository<DM.MockTestQuestions>().Delete(mockTestQuestionList);
            }
            var MockTestSettingData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTestQuestions.MocktestSettingId && !x.IsDeleted);

            if (MockTestSettingData != null)
            {
                MockTestSettingData.ExamTypeId = mockTestQuestions.ExamTypeId;
                MockTestSettingData.CourseId = mockTestQuestions.CourseId;
                MockTestSettingData.SubCourseId = mockTestQuestions.SubCourseId;
                MockTestSettingData.ExamPatternId = mockTestQuestions.ExamPatternId;
                MockTestSettingData.SubjectId = mockTestQuestions.SubjectId;
                MockTestSettingData.TopicId = mockTestQuestions.TopicId;
                MockTestSettingData.SubTopicId = mockTestQuestions.SubTopicId;
                MockTestSettingData.SectionId = mockTestQuestions.SectionId;
                MockTestSettingData.QuestionType = mockTestQuestions.QuestionType;
                MockTestSettingData.IsDraft = mockTestQuestions.IsDraft;
                MockTestSettingData.IsActive = true;

                result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(MockTestSettingData);
            }
            if (result > 0)
            {
                foreach (var item in mockTestQuestions.MockTestQuestions.DistinctBy(x => x.SubjectId))
                {

                    foreach (var it in item.SectionDetails.DistinctBy(x => x.SectionId))
                    {
                        foreach (var i in it.MockTestQuestions)
                        {
                            DM.MockTestQuestions mockTestQuestionsList = new();
                            mockTestQuestionsList.SubjectId = item.SubjectId;
                            mockTestQuestionsList.SectionId = it.SectionId;
                            mockTestQuestionsList.QuestionRefId = i.QuestionRefId;
                            mockTestQuestionsList.NegativeMark = i.NegativeMark;
                            mockTestQuestionsList.Marks = i.Mark;
                            mockTestQuestionsList.IsActive = true;
                            mockTestQuestionsList.IsDeleted = false;
                            mockTestQuestionsList.CreatorUserId = mockTestQuestions.UserId;
                            mockTestQuestionsList.CreationDate = DateTime.UtcNow;
                            mockTestQuestionsList.MockTestSettingId = mockTestQuestions.MocktestSettingId;
                            result = await _unitOfWork.Repository<DM.MockTestQuestions>().Insert(mockTestQuestionsList);

                        }

                    }
                }
            }
            return result > 0;
        }
        public async Task<Res.MockTestQuestionList?> GetMocktestQuestionById(Req.MockTestById mockTest)
        {
            Res.MockTestQuestionList mockTestQuestionsList = new()
            {
                MockTestQuestions = new()
            };


            var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestSettingId && !x.IsDeleted);
            if (mockTestSetting != null)
            {
                mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;
                mockTestQuestionsList.ExamTypeId = mockTestSetting.ExamTypeId;
                mockTestQuestionsList.ExamPatternId = mockTestSetting.ExamPatternId;
                var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == mockTestSetting.ExamPatternId && !x.IsDeleted);
                mockTestQuestionsList.ExamPatternName = examPatternDetails != null ? examPatternDetails.ExamPatternName : "N/A";
                mockTestQuestionsList.GeneralInstructions = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "N/A";
                mockTestQuestionsList.CourseId = mockTestSetting.CourseId;
                mockTestQuestionsList.SubCourseId = mockTestSetting.SubCourseId;
                mockTestQuestionsList.MockTestSettingId = mockTestSetting.Id;
                mockTestQuestionsList.SubjectId = mockTestSetting.SubjectId;
                mockTestQuestionsList.TopicId = mockTestSetting.TopicId;
                mockTestQuestionsList.SubTopicId = mockTestSetting.SubTopicId;
                mockTestQuestionsList.QuestionType = mockTestSetting.QuestionType;
                mockTestQuestionsList.SectionId = mockTestSetting.SectionId;
                mockTestQuestionsList.IsDraft = mockTestSetting.IsDraft;

                var MockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && !x.IsDeleted);
                List<Res.MockTestQuestionss> mockTest1 = new();
                if (MockTestQuestions.Any())
                {
                    foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                    {

                        Res.MockTestQuestionss mockTestQuestionss = new();
                        mockTestQuestionss.SubjectId = item.SubjectId;
                        mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                        var MockTestQuesSection = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                        mockTestQuestionss.TotalQuestions = 0;
                        mockTestQuestionss.TotalAttempt = 0;
                        var sectionCount = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == mockTestSetting.ExamPatternId && x.SubjectId == item.SubjectId && !x.IsDeleted);

                        mockTestQuestionss.SectionCount = sectionCount != null ? sectionCount.Count : 0;

                        foreach (var it in MockTestQuesSection.DistinctBy(x => x.SectionId))
                        {

                            Res.SectionDetails sectionDetails = new();
                            sectionDetails.SectionId = it.SectionId;
                            sectionDetails.SectionName = await _examPatternRepository.GetSectionName(it.SectionId);
                            var sectionDetailsss = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == it.SectionId && x.ExamPatternId == mockTestSetting.ExamPatternId);
                            sectionDetails.TotalAttempt = sectionDetailsss != null ? sectionDetailsss.TotalAttempt : 0;
                            sectionDetails.TotalQuestions = sectionDetailsss != null ? sectionDetailsss.TotalQuestions : 0;
                            var SectionQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && x.SectionId == it.SectionId && !x.IsDeleted);
                            List<Res.MockTestQuestions> mockTestQuestionsLists = new();
                            foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                            {
                                Res.MockTestQuestions questinBanks = new();
                                var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == i.QuestionRefId && !x.IsDeleted);
                                if (questions.Any())
                                {
                                    questinBanks.TopicId = questions.First().TopicId;
                                    questinBanks.SubTopicId = questions.First().SubTopicId;
                                    questinBanks.QuestionType = questions.First().QuestionType;
                                    questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                                    questinBanks.Mark = questions.First().Mark;
                                    questinBanks.NegativeMark = questions.First().NegativeMark;
                                    questinBanks.QuestionRefId = questions.First().QuestionRefId;
                                    foreach (var items in questions)
                                    {
                                        switch (items.QuestionLanguage)
                                        {
                                            case "English":
                                                Res.MockEnglish english = new();
                                                english.Id = items.Id;
                                                english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                english.OptionA = items.OptionA;
                                                english.OptionB = items.OptionB;
                                                english.OptionC = items.OptionC;
                                                english.OptionD = items.OptionD;
                                                english.Explanation = items.Explanation;
                                                english.IsCorrectA = items.IsCorrectA;
                                                english.IsCorrectB = items.IsCorrectB;
                                                english.IsCorrectC = items.IsCorrectC;
                                                english.IsCorrectD = items.IsCorrectD;
                                                english.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.English = english;
                                                break;
                                            case "Hindi":
                                                Res.MockHindi hindi = new();
                                                hindi.Id = items.Id;
                                                hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                hindi.OptionA = items.OptionA;
                                                hindi.OptionB = items.OptionB;
                                                hindi.OptionC = items.OptionC;
                                                hindi.OptionD = items.OptionD;
                                                hindi.IsCorrectA = items.IsCorrectA;
                                                hindi.IsCorrectB = items.IsCorrectB;
                                                hindi.IsCorrectC = items.IsCorrectC;
                                                hindi.IsCorrectD = items.IsCorrectD;
                                                hindi.Explanation = items.Explanation;
                                                hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Hindi = hindi;
                                                break;
                                            case "Gujarati":
                                                Res.MockGujarati Gujarati = new();
                                                Gujarati.Id = items.Id;
                                                Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                Gujarati.OptionA = items.OptionA;
                                                Gujarati.OptionB = items.OptionB;
                                                Gujarati.OptionC = items.OptionC;
                                                Gujarati.OptionD = items.OptionD;
                                                Gujarati.IsCorrectA = items.IsCorrectA;
                                                Gujarati.IsCorrectB = items.IsCorrectB;
                                                Gujarati.IsCorrectC = items.IsCorrectC;
                                                Gujarati.IsCorrectD = items.IsCorrectD;
                                                Gujarati.Explanation = items.Explanation;
                                                Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                break;
                                            case "Marathi":
                                                Res.MockMarathi marathi = new();
                                                marathi.Id = items.Id;
                                                marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                marathi.OptionA = items.OptionA;
                                                marathi.OptionB = items.OptionB;
                                                marathi.OptionC = items.OptionC;
                                                marathi.OptionD = items.OptionD;
                                                marathi.IsCorrectA = items.IsCorrectA;
                                                marathi.IsCorrectB = items.IsCorrectB;
                                                marathi.IsCorrectC = items.IsCorrectC;
                                                marathi.IsCorrectD = items.IsCorrectD;
                                                marathi.Explanation = items.Explanation;
                                                marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Marathi = marathi;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    mockTestQuestionsLists.Add(questinBanks);
                                }
                                sectionDetails.MockTestQuestions = mockTestQuestionsLists;
                                mockTestQuestionss.NoOfQue++;
                                //mockTestQuestionss.TotalAttempt = sectionDetails.TotalAttempt+;
                                //  sectionDetails.TotalQuestions++;

                            }
                            mockTestQuestionss.TotalAttempt = mockTestQuestionss.TotalAttempt + sectionDetails.TotalAttempt;
                            mockTestQuestionss.TotalQuestions = mockTestQuestionss.TotalQuestions + sectionDetails.TotalQuestions;
                            mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                            mockTestQuestionss.SectionDetails.Add(sectionDetails);
                        }
                        mockTest1.Add(mockTestQuestionss);
                    }
                    mockTestQuestionsList.MockTestQuestions = mockTest1;
                }
                else
                {
                    return null;
                }

                return mockTestQuestionsList;
            }
            return null;

        }
        public async Task<Res.MockTestQuestionListPdf?> GetMocktestQuestionPdf(Req.MockTestById mockTest)
        {
            Res.MockTestQuestionListPdf mockTestQuestionsList = new()
            {
                MockTestQuestions = new()
            };


            var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestSettingId && !x.IsDeleted);
            if (mockTestSetting != null)
            {
                mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;
                mockTestQuestionsList.ExamTypeId = mockTestSetting.ExamTypeId;
                mockTestQuestionsList.ExamPatternId = mockTestSetting.ExamPatternId;
                var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == mockTestSetting.ExamPatternId && !x.IsDeleted);

                mockTestQuestionsList.ExamPatternName = examPatternDetails != null ? examPatternDetails.ExamPatternName : "N/A";
                mockTestQuestionsList.GeneralInstructions = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "N/A";
                mockTestQuestionsList.CourseId = mockTestSetting.CourseId;
                mockTestQuestionsList.SubCourseId = mockTestSetting.SubCourseId;
                mockTestQuestionsList.MockTestSettingId = mockTestSetting.Id;
                mockTestQuestionsList.SubjectId = mockTestSetting.SubjectId;
                mockTestQuestionsList.TopicId = mockTestSetting.TopicId;
                mockTestQuestionsList.SubTopicId = mockTestSetting.SubTopicId;
                mockTestQuestionsList.QuestionType = mockTestSetting.QuestionType;
                mockTestQuestionsList.SectionId = mockTestSetting.SectionId;
                mockTestQuestionsList.IsDraft = mockTestSetting.IsDraft;
                mockTestQuestionsList.InstituteId = mockTestSetting.InstituteId;
                mockTestQuestionsList.TestDate = DateTime.UtcNow;
                mockTestQuestionsList.TotalMarks = 0;
                mockTestQuestionsList.TimeDurationHours = mockTestSetting.TimeSettingDuration.Hours;
                mockTestQuestionsList.TimeDurationMinutes = mockTestSetting.TimeSettingDuration.Minutes;
                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == mockTestSetting.InstituteId && !x.IsDeleted);
                mockTestQuestionsList.InstituteLogo = instituteDetails != null ? instituteDetails.InstituteLogo : "N/A";
                mockTestQuestionsList.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "N/A";

                var MockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && !x.IsDeleted);
                List<Res.MockTestQuestionssPdf> mockTest1 = new();
                if (MockTestQuestions.Any())
                {
                    foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                    {
                        Res.MockTestQuestionssPdf mockTestQuestionss = new();
                        mockTestQuestionss.SubjectId = item.SubjectId;
                        mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                        var MockTestQuesSection = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                        mockTestQuestionss.TotalQuestions = 0;
                        mockTestQuestionss.TotalAttempt = 0;
                        var sectionCount = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == mockTestSetting.ExamPatternId && x.SubjectId == item.SubjectId && !x.IsDeleted);

                        mockTestQuestionss.SectionCount = sectionCount != null ? sectionCount.Count : 0;

                        foreach (var it in MockTestQuesSection.DistinctBy(x => x.SectionId))
                        {

                            Res.SectionDetailsPdf sectionDetails = new();
                            sectionDetails.SectionId = it.SectionId;
                            sectionDetails.SectionName = await _examPatternRepository.GetSectionName(it.SectionId);
                            var sectionDetailsss = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == it.SectionId && x.ExamPatternId == mockTestSetting.ExamPatternId);
                            sectionDetails.TotalAttempt = sectionDetailsss != null ? sectionDetailsss.TotalAttempt : 0;
                            sectionDetails.TotalQuestions = sectionDetailsss != null ? sectionDetailsss.TotalQuestions : 0;
                            sectionDetails.SectionTotalMark = 0;
                            var SectionQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && x.SectionId == it.SectionId && !x.IsDeleted);
                            List<Res.MockTestQuestions> mockTestQuestionsLists = new();
                            foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                            {
                                Res.MockTestQuestions questinBanks = new();
                                var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == i.QuestionRefId && !x.IsDeleted);
                                if (questions.Any())
                                {
                                    questinBanks.TopicId = questions.First().TopicId;
                                    questinBanks.SubTopicId = questions.First().SubTopicId;
                                    questinBanks.QuestionType = questions.First().QuestionType;
                                    questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                                    questinBanks.Mark = questions.First().Mark;
                                    questinBanks.NegativeMark = questions.First().NegativeMark;
                                    questinBanks.QuestionRefId = questions.First().QuestionRefId;
                                    foreach (var items in questions)
                                    {
                                        switch (items.QuestionLanguage)
                                        {
                                            case "English":

                                                Res.MockEnglish english = new();
                                                english.Id = items.Id;
                                                english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                english.OptionA = items.OptionA;
                                                english.OptionB = items.OptionB;
                                                english.OptionC = items.OptionC;
                                                english.OptionD = items.OptionD;
                                                english.Explanation = items.Explanation;
                                                english.IsCorrectA = items.IsCorrectA;
                                                english.IsCorrectB = items.IsCorrectB;
                                                english.IsCorrectC = items.IsCorrectC;
                                                english.IsCorrectD = items.IsCorrectD;
                                                english.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.English = english;
                                                break;
                                            case "Hindi":
                                                Res.MockHindi hindi = new();
                                                hindi.Id = items.Id;
                                                hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                hindi.OptionA = items.OptionA;
                                                hindi.OptionB = items.OptionB;
                                                hindi.OptionC = items.OptionC;
                                                hindi.OptionD = items.OptionD;
                                                hindi.IsCorrectA = items.IsCorrectA;
                                                hindi.IsCorrectB = items.IsCorrectB;
                                                hindi.IsCorrectC = items.IsCorrectC;
                                                hindi.IsCorrectD = items.IsCorrectD;
                                                hindi.Explanation = items.Explanation;
                                                hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Hindi = hindi;
                                                break;
                                            case "Gujarati":
                                                Res.MockGujarati Gujarati = new();
                                                Gujarati.Id = items.Id;
                                                Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                Gujarati.OptionA = items.OptionA;
                                                Gujarati.OptionB = items.OptionB;
                                                Gujarati.OptionC = items.OptionC;
                                                Gujarati.OptionD = items.OptionD;
                                                Gujarati.IsCorrectA = items.IsCorrectA;
                                                Gujarati.IsCorrectB = items.IsCorrectB;
                                                Gujarati.IsCorrectC = items.IsCorrectC;
                                                Gujarati.IsCorrectD = items.IsCorrectD;
                                                Gujarati.Explanation = items.Explanation;
                                                Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                break;
                                            case "Marathi":
                                                Res.MockMarathi marathi = new();
                                                marathi.Id = items.Id;
                                                marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                marathi.OptionA = items.OptionA;
                                                marathi.OptionB = items.OptionB;
                                                marathi.OptionC = items.OptionC;
                                                marathi.OptionD = items.OptionD;
                                                marathi.IsCorrectA = items.IsCorrectA;
                                                marathi.IsCorrectB = items.IsCorrectB;
                                                marathi.IsCorrectC = items.IsCorrectC;
                                                marathi.IsCorrectD = items.IsCorrectD;
                                                marathi.Explanation = items.Explanation;
                                                marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Marathi = marathi;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    mockTestQuestionsLists.Add(questinBanks);

                                    sectionDetails.SectionTotalMark = sectionDetails.SectionTotalMark + questions.First().Mark;
                                }
                                sectionDetails.MockTestQuestions = mockTestQuestionsLists;
                                mockTestQuestionss.NoOfQue++;

                            }
                            mockTestQuestionss.TotalAttempt = mockTestQuestionss.TotalAttempt + sectionDetails.TotalAttempt;
                            mockTestQuestionss.TotalQuestions = mockTestQuestionss.TotalQuestions + sectionDetails.TotalQuestions;
                            mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                            mockTestQuestionss.SubjectTotalMarks = mockTestQuestionss.SubjectTotalMarks + sectionDetails.SectionTotalMark;
                            mockTestQuestionss.SectionDetails.Add(sectionDetails);
                        }
                        mockTestQuestionsList.TotalMarks = mockTestQuestionsList.TotalMarks + mockTestQuestionss.SubjectTotalMarks;
                        mockTestQuestionsList.TotalQuestions = mockTestQuestionsList.TotalQuestions + mockTestQuestionss.TotalQuestions;
                        mockTest1.Add(mockTestQuestionss);
                    }

                    mockTestQuestionsList.MockTestQuestions = mockTest1;
                }
                else
                {
                    return null;
                }

                return mockTestQuestionsList;
            }
            return null;

        }
        #endregion
        #region AutomaticMockTestQuestion
        public async Task<Res.AutomaticMockTest?> GetAllAutomaticMockTestSettings(Req.AutomaticMockTestQuestion mockTestQuestion)
        {
            Res.AutomaticMockTest automaticMockTest = new();
            var mockTestCheck = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.ExamPatternId == mockTestQuestion.ExamPatternId && x.CourseId == mockTestQuestion.CourseId
                                                                                       && x.SubCourseId == mockTestQuestion.SubCourseId && x.ExamTypeId == mockTestQuestion.ExamTypeId && !x.IsDeleted);
            if (mockTestCheck != null)
            {
                //automaticMockTest.ExamPatternId = mockTestCheck.ExamTypeId,
            }
            return null;
        }
        public bool CheckLanguage(string language)
        {
            if (QuestionLanguage.English.ToString() == language)
            {
                return true;
            }
            else if (QuestionLanguage.Hindi.ToString() == language)
            {
                return true;
            }
            else if (QuestionLanguage.Gujarati.ToString() == language)
            {
                return true;
            }
            else if (QuestionLanguage.Marathi.ToString() == language)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckPublishMockTest(Req.MockTestById mockTest)
        {
            var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestSettingId && !x.IsDeleted);
            if (mockTestData != null)
            {
                Guid examPatternId = mockTestData.ExamPatternId;
                Guid mockSettingId = mockTestData.Id;
                var mockTestQuestionData = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockSettingId && !x.IsDeleted);
                var examPatternSectionData = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == examPatternId && !x.IsDeleted);

                if (mockTestQuestionData != null && examPatternSectionData != null)
                {
                    int mocktTestSectionCount = mockTestQuestionData.DistinctBy(x => x.SectionId).Count();
                    int examPatternSectionCount = examPatternSectionData.Count;
                    if (mocktTestSectionCount == examPatternSectionCount)
                    {
                        foreach (var item in examPatternSectionData)
                        {
                            var sectionDetails = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == item.Id && x.ExamPatternId == examPatternId);
                            int SectionTotalQuestion = sectionDetails != null ? sectionDetails.TotalQuestions : 0;
                            int MockTestTotalQuestion = mockTestQuestionData.Count(x => x.SectionId == item.Id);
                            if (SectionTotalQuestion != MockTestTotalQuestion)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                return false;

            }
            return false;
        }

        public async Task<bool> CheckPatternId(Req.GetExamPatternId examPatternId)
        {
            var mokctestsData = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.ExamPatternId == examPatternId.Id && !x.IsDeleted && !x.IsDraft && x.IsActive);
            if (mokctestsData.Any())
            {
                return true;
            }
            return false;
        }
        public async Task<bool> PublishMockTest(Req.MockTestById mockTest)
        {
            var mockTestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestSettingId && !x.IsDeleted);

            if (mockTestData != null)
            {
                mockTestData.IsDraft = false;
                mockTestData.LastModifierUserId = mockTest.UserId;
                mockTestData.LastModifyDate = DateTime.UtcNow;
                var result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(mockTestData);
                return result > 0;
            }
            return false;

        }

        /// <summary>
        /// Generate automatic mocktest
        /// </summary>
        /// <param name="automaticMockTestQuestion"></param>
        /// <returns></returns>
        public async Task<Res.AutoMockTestQuestionList?> GetAutomaticMockTestQuestions(Req.AutomaticMockTestQuestion automaticMockTestQuestion)
        {
            if (automaticMockTestQuestion != null)
            {
                Res.AutoMockTestQuestionList mockTestQuestionsList = new()
                {
                    MockTestQuestions = new()
                };
                var mockTestSettingData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == automaticMockTestQuestion.MockTestSettingId && !x.IsDeleted);
                if (mockTestSettingData != null)
                {
                    mockTestQuestionsList.MockTestSettingId = mockTestSettingData.Id;
                    mockTestQuestionsList.ExamTypeId = automaticMockTestQuestion.ExamTypeId;
                    mockTestQuestionsList.CourseId = automaticMockTestQuestion.CourseId;
                    mockTestQuestionsList.SubCourseId = automaticMockTestQuestion.SubCourseId;
                    mockTestQuestionsList.ExamPatternId = automaticMockTestQuestion.ExamPatternId;
                    mockTestQuestionsList.QuestionLevel = automaticMockTestQuestion.QuestionLevel;
                    mockTestQuestionsList.Language = automaticMockTestQuestion.Language;
                    mockTestQuestionsList.IsDraft = mockTestSettingData.IsDraft;
                }
                List<Res.MockTestQuestionss> mockTest1 = new();
                foreach (var item in automaticMockTestQuestion.AutomaticMockTestQuestionsList.DistinctBy(x => x.SubjectId))
                {
                    Res.MockTestQuestionss mockTestQuestionss = new();
                    mockTestQuestionss.SubjectId = item.SubjectId;
                    mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                    mockTestQuestionss.TotalQuestions = 0;
                    mockTestQuestionss.TotalAttempt = 0;
                    foreach (var it in item.SectionDetailss.DistinctBy(x => x.SectionId))
                    {
                        var sectionDetails = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.ExamPatternId == automaticMockTestQuestion.ExamPatternId && x.Id == it.SectionId);
                        if (sectionDetails != null)
                        {

                            var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == automaticMockTestQuestion.SubCourseId && x.SubjectId == item.SubjectId);
                            Guid subjectCategoryId = subjectCategory != null ? subjectCategory.Id : Guid.Empty;
                            var questionBank = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionType == it.QuestionType && x.QuestionLevel == automaticMockTestQuestion.QuestionLevel
                                                 && x.SubjectCategoryId == subjectCategoryId && x.QuestionLanguage == automaticMockTestQuestion.Language && !x.IsDeleted);

                            if (questionBank != null)
                            {
                                int take = sectionDetails != null ? sectionDetails.TotalQuestions : 0;
                                int totalQuestionAvailble = questionBank.Count;
                                if (totalQuestionAvailble < take)
                                {
                                    return null;
                                }

                                Random rng = new Random();
                                int n = questionBank.Count;
                                while (n > 1)
                                {
                                    n--;
                                    int k = rng.Next(n + 1);
                                    DM.QuestionBank value = questionBank[k];
                                    questionBank[k] = questionBank[n];
                                    questionBank[n] = value;
                                }


                                Res.SectionDetails sectionDetailsr = new();
                                sectionDetailsr.SectionId = it.SectionId;
                                sectionDetailsr.SectionName = await _examPatternRepository.GetSectionName(it.SectionId);
                                sectionDetailsr.TotalQuestions = sectionDetails != null ? sectionDetails.TotalQuestions : 0;
                                sectionDetailsr.TotalAttempt = sectionDetails != null ? sectionDetails.TotalAttempt : 0;
                                var questList = new List<DM.QuestionBank>();
                                if (mockTestQuestionss.SectionDetails.Any())
                                {
                                    var questionRefIds = mockTestQuestionss.SectionDetails.SelectMany(x => x.MockTestQuestions.Select(x => x.QuestionRefId));
                                    //   var questionRefIds = sectionDetailsr.MockTestQuestions.ToList().Select(x=>x.QuestionRefId);

                                    questList = questionBank.Where(x => !questionRefIds.Contains(x.QuestionRefId)).Take(take).ToList();
                                }
                                else
                                {
                                    questList = questionBank.Take(take).ToList();
                                }

                                List<Res.MockTestQuestions> mockTestQuestionsLists = new();
                                foreach (var i in questList)
                                {

                                    Res.MockTestQuestions questinBanks = new();
                                    var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == i.QuestionRefId && !x.IsDeleted);
                                    if (questions != null && questions.Count > 0)
                                    {
                                        questinBanks.TopicId = questions.First().TopicId;
                                        questinBanks.SubTopicId = questions.First().SubTopicId;
                                        questinBanks.QuestionType = questions.First().QuestionType;
                                        questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                                        questinBanks.Mark = questions.First().Mark;
                                        questinBanks.NegativeMark = questions.First().NegativeMark;
                                        questinBanks.QuestionRefId = questions.First().QuestionRefId;
                                        foreach (var items in questions)
                                        {
                                            switch (items.QuestionLanguage)
                                            {
                                                case "English":
                                                    Res.MockEnglish english = new();
                                                    english.Id = items.Id;
                                                    english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                    english.OptionA = items.OptionA;
                                                    english.OptionB = items.OptionB;
                                                    english.OptionC = items.OptionC;
                                                    english.OptionD = items.OptionD;
                                                    english.Explanation = items.Explanation;
                                                    english.IsCorrectA = items.IsCorrectA;
                                                    english.IsCorrectB = items.IsCorrectB;
                                                    english.IsCorrectC = items.IsCorrectC;
                                                    english.IsCorrectD = items.IsCorrectD;
                                                    english.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.English = english;
                                                    break;
                                                case "Hindi":
                                                    Res.MockHindi hindi = new();
                                                    hindi.Id = items.Id;
                                                    hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                    hindi.OptionA = items.OptionA;
                                                    hindi.OptionB = items.OptionB;
                                                    hindi.OptionC = items.OptionC;
                                                    hindi.OptionD = items.OptionD;
                                                    hindi.IsCorrectA = items.IsCorrectA;
                                                    hindi.IsCorrectB = items.IsCorrectB;
                                                    hindi.IsCorrectC = items.IsCorrectC;
                                                    hindi.IsCorrectD = items.IsCorrectD;
                                                    hindi.Explanation = items.Explanation;
                                                    hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Hindi = hindi;
                                                    break;
                                                case "Gujarati":
                                                    Res.MockGujarati Gujarati = new();
                                                    Gujarati.Id = items.Id;
                                                    Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                    Gujarati.OptionA = items.OptionA;
                                                    Gujarati.OptionB = items.OptionB;
                                                    Gujarati.OptionC = items.OptionC;
                                                    Gujarati.OptionD = items.OptionD;
                                                    Gujarati.IsCorrectA = items.IsCorrectA;
                                                    Gujarati.IsCorrectB = items.IsCorrectB;
                                                    Gujarati.IsCorrectC = items.IsCorrectC;
                                                    Gujarati.IsCorrectD = items.IsCorrectD;
                                                    Gujarati.Explanation = items.Explanation;
                                                    Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                    break;
                                                case "Marathi":
                                                    Res.MockMarathi marathi = new();
                                                    marathi.Id = items.Id;
                                                    marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                    marathi.OptionA = items.OptionA;
                                                    marathi.OptionB = items.OptionB;
                                                    marathi.OptionC = items.OptionC;
                                                    marathi.OptionD = items.OptionD;
                                                    marathi.IsCorrectA = items.IsCorrectA;
                                                    marathi.IsCorrectB = items.IsCorrectB;
                                                    marathi.IsCorrectC = items.IsCorrectC;
                                                    marathi.IsCorrectD = items.IsCorrectD;
                                                    marathi.Explanation = items.Explanation;
                                                    marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Marathi = marathi;
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        mockTestQuestionsLists.Add(questinBanks);

                                    }
                                    sectionDetailsr.MockTestQuestions = mockTestQuestionsLists;
                                    mockTestQuestionss.NoOfQue++;
                                }

                                mockTestQuestionss.SectionDetails.Add(sectionDetailsr);
                                mockTestQuestionss.TotalAttempt = mockTestQuestionss.TotalAttempt + sectionDetailsr.TotalAttempt;
                                mockTestQuestionss.TotalQuestions = mockTestQuestionss.TotalQuestions + sectionDetailsr.TotalQuestions;
                                mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                            }
                        }
                    }
                    mockTest1.Add(mockTestQuestionss);

                }

                mockTestQuestionsList.MockTestQuestions = mockTest1;

                if (mockTestQuestionsList.MockTestQuestions.Any())
                {
                    await SaveAutoMaticMocktest(mockTestQuestionsList);
                }
                return mockTestQuestionsList;
            }
            return null;
        }

        /// <summary>
        /// Publish automatic mocktest
        /// </summary>
        /// <param name="autoMockTestQuestion"></param>
        /// <returns></returns>
        public async Task<bool> SaveAutoMaticMocktest(Res.AutoMockTestQuestionList autoMockTestQuestion)
        {
            int result = 0;
            var MockTestSettingData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == autoMockTestQuestion.MockTestSettingId && !x.IsDeleted);
            if (MockTestSettingData != null)
            {
                var mockTestQuestionList = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == MockTestSettingData.Id && !x.IsDeleted);
                if (mockTestQuestionList.Any())
                {
                    await _unitOfWork.Repository<DM.MockTestQuestions>().Delete(mockTestQuestionList);
                }
                MockTestSettingData.ExamTypeId = autoMockTestQuestion.ExamTypeId;
                MockTestSettingData.CourseId = autoMockTestQuestion.CourseId;
                MockTestSettingData.SubCourseId = autoMockTestQuestion.SubCourseId;
                MockTestSettingData.ExamPatternId = autoMockTestQuestion.ExamPatternId;
                MockTestSettingData.IsDraft = true;
                MockTestSettingData.IsActive = true;
                result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(MockTestSettingData);
            }
            if (result > 0)
            {
                foreach (var item in autoMockTestQuestion.MockTestQuestions.DistinctBy(x => x.SubjectId))
                {

                    foreach (var it in item.SectionDetails.DistinctBy(x => x.SectionId))
                    {
                        foreach (var i in it.MockTestQuestions)
                        {
                            DM.MockTestQuestions mockTestQuestionsList = new();
                            mockTestQuestionsList.SubjectId = item.SubjectId;
                            mockTestQuestionsList.SectionId = it.SectionId;
                            mockTestQuestionsList.QuestionRefId = i.QuestionRefId;
                            mockTestQuestionsList.NegativeMark = i.NegativeMark;
                            mockTestQuestionsList.Marks = i.Mark;
                            mockTestQuestionsList.IsActive = true;
                            mockTestQuestionsList.IsDeleted = false;
                            mockTestQuestionsList.CreatorUserId = autoMockTestQuestion.UserId;
                            mockTestQuestionsList.CreationDate = DateTime.UtcNow;
                            mockTestQuestionsList.MockTestSettingId = autoMockTestQuestion.MockTestSettingId;
                            result = await _unitOfWork.Repository<DM.MockTestQuestions>().Insert(mockTestQuestionsList);

                        }

                    }
                }
            }
            return result > 0;
        }
        public async Task<bool> RemoveMockTestQuestions(Req.MocktestSettingById mockTest)
        {
            int result = 0;
            var MockTestSettingData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.Id && !x.IsDeleted);
            if (MockTestSettingData != null)
            {

                var mockTestQuestionList = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == MockTestSettingData.Id && !x.IsDeleted);
                if (mockTestQuestionList.Any())
                {
                    await _unitOfWork.Repository<DM.MockTestQuestions>().Delete(mockTestQuestionList);

                }
                MockTestSettingData.IsDraft = true;
                MockTestSettingData.IsActive = false;
                result = await _unitOfWork.Repository<DM.MockTestSettings>().Update(MockTestSettingData);

            }
            return result > 0;
            #endregion
        }


    }
}


