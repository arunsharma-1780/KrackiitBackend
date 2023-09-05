using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using SendGrid;
using static OnlinePractice.API.Controllers.StripeController;
using NgrokAspNetCore;
using static System.Collections.Specialized.BitVector32;
using System.Text.RegularExpressions;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentMockTestRepository : IStudentMockTestRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamPatternRepository _examPatternRepository;
        public StudentMockTestRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IFileRepository fileRepository,
            IHttpContextAccessor baseurl, ISubjectRepository subjectRepository, IExamPatternRepository examPatternRepository)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _baseUrl = baseurl;
            _subjectRepository = subjectRepository;
            _examPatternRepository = examPatternRepository;
        }

        #region Student MockTest Dashboard
        public async Task<Res.InstituteMockTestList?> GetMockTestListByInstitute(Req.MockTestInstitute institute)
        {
            Res.InstituteMockTestList instituteMockTestList = new();
            var studentCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == institute.UserId && !x.IsDeleted);
            if (studentCourse != null)
            {
                var subCourseId = studentCourse.SubCourseId != Guid.Empty ? studentCourse.SubCourseId : Guid.Empty;

                var mocktestList = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.InstituteId == institute.InstituteId && x.SubCourseId == subCourseId && x.TestAvailability == TestAvailability.Specific && x.Price == 0 && !x.IsDraft && !x.IsDeleted, orderBy: x => x.OrderBy(z => z.TestSpecificFromDate));
                var userMocktest = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductCategory == ProductCategory.MockTest && x.StudentId == institute.UserId && !x.IsDeleted);
                if (userMocktest.Any())
                {
                    var userPurchaseMocktest = userMocktest.Select(x => x.ProductId).ToList();
                    var purchasedMockt = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => userPurchaseMocktest.Contains(x.Id) && x.InstituteId == institute.InstituteId && x.TestAvailability == TestAvailability.Specific && x.TestSpecificToDate.Value.Date >= DateTime.UtcNow.Date && x.TestStartTime > DateTime.UtcNow);
                    if (purchasedMockt.Any())
                    {
                        mocktestList.AddRange(purchasedMockt);
                    }
                }

                var mockTestComplete = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                var completedMocktestIds = mockTestComplete.Select(x => x.MockTestId).ToList();
                var mockTestInprogess = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                var inprogressMockTestIds = mockTestInprogess.Select(x => x.MockTestId).ToList();

                var finalmockTests = mocktestList.Where(x => !completedMocktestIds.Contains(x.Id) && !inprogressMockTestIds.Contains(x.Id) && x.TestSpecificToDate.GetValueOrDefault().Date >= DateTime.UtcNow.Date && x.TestStartTime > DateTime.UtcNow).ToList();
                if (finalmockTests != null)
                {

                    foreach (var item in finalmockTests.DistinctBy(x => x.Id))
                    {
                        Res.InstituteMockTest instituteMockTest = new();
                        instituteMockTest.MockTestId = item.Id;
                        instituteMockTest.MockTestName = item.MockTestName;
                        instituteMockTest.TestAvailaiblityType = item.TestAvailability.ToString();
                        instituteMockTest.Description = item.Description;
                        var date = item.TestSpecificFromDate != null ? item.TestSpecificFromDate : DateTime.UtcNow;

                        if (item.TestAvailability == TestAvailability.Specific && (DateTime.UtcNow > item.TestSpecificFromDate))
                        {
                            instituteMockTest.IsDay = false;

                        }
                        else if (item.TestAvailability == TestAvailability.Specific && (DateTime.UtcNow < item.TestSpecificFromDate.Value.AddDays(1)))
                        {
                            instituteMockTest.IsDay = true;
                        }
                        instituteMockTest.Status = "Not Visited";
                        instituteMockTest.StartsInDays = item.TestSpecificFromDate;
                        instituteMockTest.StartsInTime = item.TestStartTime;
                        instituteMockTest.MockTestDuration = item.TimeSettingDuration;
                        instituteMockTestList.InstituteMockTest.Add(instituteMockTest);
                    }
                    var count = instituteMockTestList.InstituteMockTest.Count();
                    var result = instituteMockTestList.InstituteMockTest.Page(institute.PageNumber, institute.PageSize);
                    instituteMockTestList.InstituteMockTest = result.ToList();
                    instituteMockTestList.TotalRecords = count;
                    return instituteMockTestList;
                }
            }
            return null;
        }
        #endregion

        #region Student Mocktest
        public async Task<bool> IsCustomeDuplicate(Req.CustomeMockTestNameCheck mockTestName)
        {
            var result = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.MockTestName.Trim().ToLower() == mockTestName.MockTestName.Trim().ToLower() && x.CreatorUserId == mockTestName.UserId && !x.IsDeleted && x.IsActive);
            if (result != null)
                return true;
            else
                return false;

        }
        public async Task<Res.StudentMockTestList?> GetMockTestListByFilters(Req.StudentMockTest institute)
        {
            Res.StudentMockTestList studentMockTestList = new();
            Req.StudentMockTest student = new();
            var statusFilter = institute.StatusFilter;
            var pricingFilter = institute.PricingFilter;
            List<DM.MockTestSettings> mockTestNewList = new();
            var studentCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == institute.UserId && !x.IsDeleted);
            if (studentCourse != null)
            {
                var subCourseId = studentCourse.SubCourseId != Guid.Empty ? studentCourse.SubCourseId : Guid.Empty;

                var mocktestList = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.InstituteId == institute.InstituteId && x.SubCourseId == subCourseId && !x.IsDraft && !x.IsDeleted, orderBy: x => x.OrderByDescending(z => z.CreationDate));
                mockTestNewList = mocktestList.ToList();
                if (institute.LanguageFilter.ToString() != "All")
                {
                    mockTestNewList = mocktestList.Where(x => x.Language == institute.LanguageFilter.ToString()).ToList();
                }
                switch (statusFilter)
                {
                    case StatusFilter.All:
                        break;
                    case StatusFilter.NotVisted:
                        var mocktestDataIds = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted);
                        var mockIds = mocktestDataIds.Select(x => x.MockTestId).ToList();
                        var expireMocktest = mockTestNewList.Where(x => x.TestSpecificToDate.GetValueOrDefault().Date <= DateTime.UtcNow.Date && x.TestStartTime < DateTime.UtcNow && x.TestAvailability == TestAvailability.Specific).ToList();
                        var expireIds = expireMocktest.Select(x => x.Id).ToList();

                        mockTestNewList = mockTestNewList.Where(x => !mockIds.Contains(x.Id) && !expireIds.Contains(x.Id)).ToList();
                        mockTestNewList = mockTestNewList.Where(x => x.TestAvailability == TestAvailability.Always || (x.TestSpecificFromDate.GetValueOrDefault().Date >= DateTime.UtcNow.Date && x.TestStartTime > DateTime.UtcNow)).ToList();
                        break;
                    case StatusFilter.Expired:
                        var statusIds = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted);
                        var ids = statusIds.Select(x => x.MockTestId).ToList();
                        mockTestNewList = mockTestNewList.Where(x => !ids.Contains(x.Id) && x.TestSpecificToDate.GetValueOrDefault().Date <= DateTime.UtcNow.Date && x.TestStartTime < DateTime.UtcNow && x.TestAvailability == TestAvailability.Specific).ToList();
                        break;
                    case StatusFilter.InProgress:
                        var studentMocktestStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                        var mockTestIds = studentMocktestStatus.Select(x => x.MockTestId).ToList();
                        var mocktestCompletedStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                        var mockTestComplteListIds = mocktestCompletedStatus.Select(x => x.MockTestId).ToList();
                        var expriedIds = mockTestNewList.Where(x => x.TestSpecificToDate.GetValueOrDefault().Date <= DateTime.UtcNow.Date && x.TestStartTime < DateTime.UtcNow && x.TestAvailability == TestAvailability.Specific).ToList();
                        var exp = expriedIds.Select(x => x.Id).ToList();
                        mockTestNewList = mockTestNewList.Where(x => mockTestIds.Contains(x.Id) && !exp.Contains(x.Id) && !mockTestComplteListIds.Contains(x.Id)).ToList();
                        break;
                    case StatusFilter.Completed:
                        var mocktestStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                        var mockTestListIds = mocktestStatus.Select(x => x.MockTestId).ToList();
                        mockTestNewList = mockTestNewList.Where(x => mockTestListIds.Contains(x.Id)).ToList();
                        break;
                }
                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        mockTestNewList = mockTestNewList.Where(x => x.IsFree).ToList();
                        break;
                    case PricingFilter.Premium:
                        mockTestNewList = mockTestNewList.Where(x => !x.IsFree && x.Price > 0).ToList();
                        break;
                }

                if (mockTestNewList.Any())
                {
                    foreach (var item in mockTestNewList)
                    {
                        var mockTestInprogess = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                        var mockTestComplete = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                        var adminMocktest = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == item.Id && !x.IsDeleted);
                        var isPurchasedCheck = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == item.Id && x.ProductCategory == ProductCategory.MockTest && x.StudentId == institute.UserId && !x.IsDeleted);

                        var totalAttempsRemaining = adminMocktest != null ? adminMocktest.TotalAttempts : 0;
                        var remainingtimeDuration = adminMocktest != null ? adminMocktest.TimeSettingDuration : TimeSpan.Zero;

                        Res.StudentMockTest studentMockTest = new();
                        studentMockTest.MockTestId = item.Id;
                        studentMockTest.MocktestName = item.MockTestName;
                        studentMockTest.Description = item.Description;
                        studentMockTest.IsRetake = item.IsAllowReattempts;
                        studentMockTest.TestAvailability = item.TestAvailability.ToString();
                        studentMockTest.TotalAttempts = item.TotalAttempts;
                        var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == item.Id && !x.IsDeleted);
                        var attemptsCount = studentResult.DistinctBy(x => x.UniqueMockTetId).Count();
                        studentMockTest.AlreadyResultGenerated = attemptsCount;
                        studentMockTest.IsPurchased = isPurchasedCheck != null ? true : false;

                        if (mockTestInprogess == null && mockTestComplete == null)
                        {
                            studentMockTest.RemainingAttempts = item.TotalAttempts;
                            studentMockTest.RemainingDuration = item.TimeSettingDuration;

                        }
                        else
                        {
                            studentMockTest.RemainingAttempts = mockTestInprogess != null && mockTestInprogess.RemainingAttempts > 0 ? mockTestInprogess.RemainingAttempts : 0;
                            studentMockTest.RemainingDuration = mockTestInprogess != null && mockTestInprogess.RemainingDuration > TimeSpan.Zero ? mockTestInprogess.RemainingDuration : TimeSpan.Zero;
                        }
                        var totalGapBetweenReattempts = new TimeSpan(item.ReattemptsDays, item.ReattemptsDuration.Hours, item.ReattemptsDuration.Minutes, item.ReattemptsDuration.Seconds);
                        studentMockTest.TotalGapBetweenReattempts = totalGapBetweenReattempts;
                        studentMockTest.SubjectId = item.SubjectId;
                        var subjectNameCheck = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == studentMockTest.SubjectId && !x.IsDeleted);
                        studentMockTest.SubjectName = subjectNameCheck != null ? subjectNameCheck.SubjectName : "";

                        studentMockTest.TopicId = item.TopicId;
                        var topicNameCheck = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == studentMockTest.TopicId && !x.IsDeleted);
                        studentMockTest.TopicName = topicNameCheck != null ? topicNameCheck.TopicName : "";

                        studentMockTest.SubTopicId = item.SubTopicId;
                        var subTopicNameCheck = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == studentMockTest.SubTopicId && !x.IsDeleted);
                        studentMockTest.SubTopicName = subTopicNameCheck != null ? subTopicNameCheck.SubTopicName : "";
                        studentMockTest.MockTestDuration = item.TimeSettingDuration;
                        studentMockTest.TestStartTime = item.TestStartTime;
                        studentMockTest.Language = item.Language;
                        var priceCheck = item.IsFree && item.Price == 0 ? 0 : item.Price;

                        studentMockTest.Price = priceCheck;
                        if (mockTestComplete != null)
                        {
                            studentMockTest.Status = StatusFilter.Completed.ToString();
                        }
                        else if (mockTestInprogess != null && item.TestAvailability == TestAvailability.Specific && institute.StatusFilter != StatusFilter.Expired)
                        {
                            studentMockTest.Status = StatusFilter.InProgress.ToString();
                        }
                        else if (mockTestInprogess != null && item.TestAvailability == TestAvailability.Always && institute.StatusFilter != StatusFilter.Expired)
                        {
                            studentMockTest.Status = StatusFilter.InProgress.ToString();
                        }
                        else
                        {
                            studentMockTest.Status = item.TestSpecificToDate.GetValueOrDefault().Date <= DateTime.UtcNow.Date && item.TestStartTime < DateTime.UtcNow && item.TestAvailability == TestAvailability.Specific ? "Expired" : "Not Visited";
                        }

                        if (item.TestAvailability == TestAvailability.Specific && (DateTime.UtcNow > item.TestSpecificFromDate))
                        {
                            studentMockTest.IsDay = false;

                        }
                        else if (item.TestAvailability == TestAvailability.Specific && (DateTime.UtcNow < item.TestSpecificFromDate.Value.AddDays(1)))
                        {
                            studentMockTest.IsDay = true;
                        }

                        if (item.TestAvailability == TestAvailability.Always)
                        {
                            studentMockTest.IsDay = false;
                            studentMockTest.StartsInDays = null;
                            studentMockTest.StartsInTime = null;
                        }
                        else
                        {
                            //instituteMockTest.IsDay = false;
                            studentMockTest.StartsInDays = item.TestSpecificFromDate;
                            studentMockTest.StartsInTime = item.TestStartTime;
                        }
                        studentMockTestList.StudentMockTests.Add(studentMockTest);
                    }
                    var count = studentMockTestList.StudentMockTests.Count();
                    var result = studentMockTestList.StudentMockTests.Page(institute.PageNumber, institute.PageSize);
                    studentMockTestList.StudentMockTests = result.ToList();
                    studentMockTestList.TotalRecords = count;
                    return studentMockTestList;
                }
            }
            return null;
        }
        public List<Com.EnumModel> GetMockTestLanguage()
        {
            List<EnumModel> enums = ((LanguageFilter[])Enum.GetValues(typeof(LanguageFilter))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public List<Com.EnumModel> GetMockTestStatus()
        {
            List<EnumModel> enums = ((StatusFilter[])Enum.GetValues(typeof(StatusFilter))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public List<Com.EnumModel> GetMockTestPricing()
        {
            List<EnumModel> enums = ((PricingFilter[])Enum.GetValues(typeof(PricingFilter))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public List<Com.EnumModel> GetCustomeMockTestStatus()
        {
            List<EnumModel> enums = ((CustomeStatusFilter[])Enum.GetValues(typeof(CustomeStatusFilter))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public async Task<Com.ResultDto> GenerateAutomaticMockTestForStudent2(Req.StudentAutomaticMockTestQuestion automaticMockTestQuestion)
        {
            ResultDto resultDto = new();
            if (automaticMockTestQuestion != null)
            {
                Res.StudentAutoMockTestQuestionList mockTestQuestionsList = new()
                {
                    MockTestQuestions = new()
                };
                var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == automaticMockTestQuestion.SubCourseId && x.SubjectId == automaticMockTestQuestion.SubjectId);
                Guid subjectCategoryId = subjectCategory != null ? subjectCategory.Id : Guid.Empty;
                var questionBank = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionType == QuestionType.SingleChoice && x.QuestionLevel == automaticMockTestQuestion.QuestionLevel
                                     && x.SubjectCategoryId == subjectCategoryId && x.QuestionLanguage == automaticMockTestQuestion.Language.ToString() && !string.IsNullOrEmpty(x.QuestionText) && !x.IsDeleted);
                if (questionBank == null)
                {
                    resultDto.Result = false;
                    resultDto.Message = "Questions are not available!";
                    return resultDto;
                }
                int totalQuestionAvailble = questionBank.Count;
                if (totalQuestionAvailble < 25)
                {
                    resultDto.Result = false;
                    resultDto.Message = "Available questions are less than 25!";
                    return resultDto;
                }


                DM.StudentMockTest studentMockTest = new()
                {
                    MockTestName = automaticMockTestQuestion.MockTestName,
                    SubCourseId = automaticMockTestQuestion.SubCourseId,
                    SubjectId = automaticMockTestQuestion.SubjectId,
                    InstituteId = automaticMockTestQuestion.InstituteId,
                    TotalQuetsion = 25,
                    TimeDuration = TimeSpan.FromHours(1),
                    QuestionType = QuestionType.SingleChoice,
                    QuestionLevel = automaticMockTestQuestion.QuestionLevel,
                    Language = automaticMockTestQuestion.Language.ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = automaticMockTestQuestion.UserId
                };
                int result = await _unitOfWork.Repository<DM.StudentMockTest>().Insert(studentMockTest);
                if (result > 0)
                {
                    mockTestQuestionsList.StudentMockTestId = studentMockTest.Id;
                    mockTestQuestionsList.SubjectId = studentMockTest.SubjectId;

                    List<Res.MockTestQuestionss> mockTest1 = new();

                    Res.MockTestQuestionss mockTestQuestionss = new();
                    mockTestQuestionss.SubjectId = studentMockTest.SubjectId;
                    int take = 25;
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
                    var questList = new List<DM.QuestionBank>();
                    if (mockTestQuestionss.SectionDetails.Any())
                    {
                        var questionRefIds = mockTestQuestionss.SectionDetails.SelectMany(x => x.MockTestQuestions.Select(x => x.QuestionRefId));
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
                                if (items.QuestionLanguage == QuestionLanguage.English.ToString())
                                {
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
                                }
                                if (items.QuestionLanguage == QuestionLanguage.Hindi.ToString())
                                {
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
                                }
                                if (items.QuestionLanguage == QuestionLanguage.Gujarati.ToString())
                                {
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
                                }
                                if (items.QuestionLanguage == QuestionLanguage.Marathi.ToString())
                                {
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

                    mockTest1.Add(mockTestQuestionss);

                    mockTestQuestionsList.MockTestQuestions = mockTest1;

                    if (mockTestQuestionsList.MockTestQuestions.Any())
                    {
                        await SaveAutoMaticMocktestStudent(mockTestQuestionsList);
                    }
                    resultDto.Result = true;
                    resultDto.Message = "Mocktest created successfully!";
                    return resultDto;
                }
            }
            resultDto.Result = false;
            resultDto.Message = "Mocktest not created!";
            return resultDto;
        }

        public async Task<Com.ResultDto> GenerateAutomaticMockTestForStudent(Req.StudentAutomaticMockTestQuestion automaticMockTestQuestion)
        {
            ResultDto resultDto = new();
            if (automaticMockTestQuestion != null)
            {
                Res.StudentAutoMockTestQuestionList mockTestQuestionsList = new()
                {
                    MockTestQuestions = new()
                };
                var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == automaticMockTestQuestion.SubCourseId && x.SubjectId == automaticMockTestQuestion.SubjectId);
                Guid subjectCategoryId = subjectCategory != null ? subjectCategory.Id : Guid.Empty;
                var questionBank = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionType == QuestionType.SingleChoice && x.QuestionLevel == automaticMockTestQuestion.QuestionLevel
                                     && x.SubjectCategoryId == subjectCategoryId && x.QuestionLanguage == automaticMockTestQuestion.Language.ToString() && !string.IsNullOrEmpty(x.QuestionText) && !x.IsDeleted);
                if (questionBank == null)
                {
                    resultDto.Result = false;
                    resultDto.Message = "Questions are not available!";
                    return resultDto;
                }
                int totalQuestionAvailble = questionBank.Count;
                if (totalQuestionAvailble < 25)
                {
                    resultDto.Result = false;
                    resultDto.Message = "Available questions are less than 25!";
                    return resultDto;
                }


                DM.StudentMockTest studentMockTest = new()
                {
                    MockTestName = automaticMockTestQuestion.MockTestName,
                    SubCourseId = automaticMockTestQuestion.SubCourseId,
                    SubjectId = automaticMockTestQuestion.SubjectId,
                    InstituteId = automaticMockTestQuestion.InstituteId,
                    TotalQuetsion = 25,
                    TimeDuration = TimeSpan.FromHours(1),
                    QuestionType = QuestionType.SingleChoice,
                    QuestionLevel = automaticMockTestQuestion.QuestionLevel,
                    Language = automaticMockTestQuestion.Language.ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = automaticMockTestQuestion.UserId
                };
                int result = await _unitOfWork.Repository<DM.StudentMockTest>().Insert(studentMockTest);
                if (result > 0)
                {
                    mockTestQuestionsList.StudentMockTestId = studentMockTest.Id;
                    mockTestQuestionsList.SubjectId = studentMockTest.SubjectId;

                    List<Res.MockTestQuestionss> mockTest1 = new();

                    Res.MockTestQuestionss mockTestQuestionss = new();
                    mockTestQuestionss.SubjectId = studentMockTest.SubjectId;
                    int take = 25;
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
                    var questList = new List<DM.QuestionBank>();
                    if (mockTestQuestionss.SectionDetails.Any())
                    {
                        var questionRefIds = mockTestQuestionss.SectionDetails.SelectMany(x => x.MockTestQuestions.Select(x => x.QuestionRefId));
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

                    mockTest1.Add(mockTestQuestionss);

                    mockTestQuestionsList.MockTestQuestions = mockTest1;

                    if (mockTestQuestionsList.MockTestQuestions.Any())
                    {
                        await SaveAutoMaticMocktestStudent(mockTestQuestionsList);
                    }
                    resultDto.Result = true;
                    resultDto.Message = "Mocktest created successfully!";
                    return resultDto;
                }
            }
            resultDto.Result = false;
            resultDto.Message = "Mocktest not created!";
            return resultDto;
        }
        public async Task<bool> SaveAutoMaticMocktestStudent(Res.StudentAutoMockTestQuestionList autoMockTestQuestion)
        {
            int result = 0;
            var studentMockTestData = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == autoMockTestQuestion.StudentMockTestId && !x.IsDeleted);
            if (studentMockTestData != null)
            {
                var mockTestQuestionList = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == studentMockTestData.Id && !x.IsDeleted);
                if (mockTestQuestionList.Any())
                {
                    await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Delete(mockTestQuestionList);
                }
                studentMockTestData.IsActive = true;
                studentMockTestData.IsDeleted = false;
                result = await _unitOfWork.Repository<DM.StudentMockTest>().Update(studentMockTestData);
            }
            if (result > 0)
            {
                foreach (var item in autoMockTestQuestion.MockTestQuestions.DistinctBy(x => x.SubjectId))
                {

                    foreach (var it in item.SectionDetails.DistinctBy(x => x.SectionId))
                    {
                        foreach (var i in it.MockTestQuestions)
                        {
                            DM.StudentMockTestQuestions mockTestQuestionsList = new();
                            mockTestQuestionsList.SubjectId = item.SubjectId;
                            mockTestQuestionsList.QuestionRefId = i.QuestionRefId;
                            mockTestQuestionsList.NegativeMark = i.NegativeMark;
                            mockTestQuestionsList.Marks = i.Mark;
                            mockTestQuestionsList.IsActive = true;
                            mockTestQuestionsList.IsDeleted = false;
                            mockTestQuestionsList.CreatorUserId = autoMockTestQuestion.UserId;
                            mockTestQuestionsList.CreationDate = DateTime.UtcNow;
                            mockTestQuestionsList.StudentMockTestId = autoMockTestQuestion.StudentMockTestId;
                            result = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Insert(mockTestQuestionsList);

                        }

                    }
                }
            }
            return result > 0;
        }
        public async Task<Res.CustomeStudentMockTestList?> GetCustomeMockTestListByFilter(Req.CustomeStudentMockTest institute)
        {
            Res.CustomeStudentMockTestList studentMockTestList = new();
            Req.StudentMockTest student = new();
            List<DM.StudentMockTest> mockTestNewList = new();
            var statusFilter = institute.StatusFilter;

            var mocktestList = await _unitOfWork.Repository<DM.StudentMockTest>().Get(x => x.InstituteId == institute.InstituteId && x.CreatorUserId == institute.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(z => z.CreationDate));
            mockTestNewList = mocktestList.ToList();
            if (institute.LanguageFilter.ToString() != "All")
            {
                mockTestNewList = mocktestList.Where(x => x.Language == institute.LanguageFilter.ToString()).ToList();
            }
            switch (statusFilter)
            {
                case CustomeStatusFilter.All:
                    break;
                case CustomeStatusFilter.NotVisted:
                    var mocktestDataIds = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted);
                    var mockIds = mocktestDataIds.Select(x => x.MockTestId).ToList();
                    mockTestNewList = mockTestNewList.Where(x => !mockIds.Contains(x.Id)).ToList();
                    mockTestNewList = mockTestNewList.ToList();
                    break;
                case CustomeStatusFilter.InProgress:
                    var studentMocktestStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                    var mockTestIds = studentMocktestStatus.Select(x => x.MockTestId).ToList();
                    var mocktestCompletedStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                    var mockTestComplteListIds = mocktestCompletedStatus.Select(x => x.MockTestId).ToList();
                    mockTestNewList = mockTestNewList.Where(x => mockTestIds.Contains(x.Id) && !mockTestComplteListIds.Contains(x.Id)).ToList();
                    break;
                case CustomeStatusFilter.Completed:
                    var mocktestStatus = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                    var mockTestListIds = mocktestStatus.Select(x => x.MockTestId).ToList();
                    mockTestNewList = mockTestNewList.Where(x => mockTestListIds.Contains(x.Id)).ToList();
                    break;
            }

            if (mockTestNewList.Any())
            {
                foreach (var item in mockTestNewList)
                {
                    Res.CustomeStudentMockTest studentMockTest = new();
                    studentMockTest.MockTestId = item.Id;
                    studentMockTest.MocktestName = item.MockTestName;
                    studentMockTest.SubjectId = item.SubjectId;
                    var subjectNameCheck = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == studentMockTest.SubjectId && !x.IsDeleted);
                    studentMockTest.SubjectName = subjectNameCheck != null ? subjectNameCheck.SubjectName : "";
                    studentMockTest.MockTestDuration = item.TimeDuration;
                    var mockTestInprogess = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                    var mockTestComplete = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == institute.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                    studentMockTest.IsRetake = true;
                    if (mockTestInprogess == null && mockTestComplete == null)
                    {
                        studentMockTest.RemainingAttempts = 3;
                        studentMockTest.RemainingDuration = TimeSpan.FromHours(1);
                    }
                    else
                    {
                        studentMockTest.RemainingAttempts = mockTestInprogess != null && mockTestInprogess.RemainingAttempts > 0 ? mockTestInprogess.RemainingAttempts : 0;
                        studentMockTest.RemainingDuration = mockTestInprogess != null && mockTestInprogess.RemainingDuration > TimeSpan.Zero ? mockTestInprogess.RemainingDuration : TimeSpan.Zero;
                    }
                    if (mockTestComplete != null)
                    {
                        studentMockTest.Status = CustomeStatusFilter.Completed.ToString();
                    }
                    else if (mockTestInprogess != null)
                    {
                        studentMockTest.Status = CustomeStatusFilter.InProgress.ToString();
                    }
                    else
                    {
                        studentMockTest.Status = "Not Visited";
                    }
                    studentMockTest.TotalQuestion = item.TotalQuetsion;
                    studentMockTest.Score = 0;
                    var languageList = new List<string>();
                    languageList.Add(item.Language);
                    studentMockTest.Language = languageList;
                    studentMockTest.IsCustome = true;
                    studentMockTestList.StudentMockTests.Add(studentMockTest);
                }
                var count = studentMockTestList.StudentMockTests.Count();
                var result = studentMockTestList.StudentMockTests.Page(institute.PageNumber, institute.PageSize);
                studentMockTestList.StudentMockTests = result.ToList();
                studentMockTestList.TotalRecords = count;
                return studentMockTestList;
            }
            return null;
        }

        #endregion

        #region Student Question Panel
        public async Task<Res.StudentMocktestPanel?> GetStudentQuestionPanel(Req.GetStudentQuestionPanel mockTest)
        {
            if (!mockTest.IsCustome)
            {
                Res.StudentMocktestPanel mockTestQuestionsList = new()
                {
                    MocktestPanelList = new()
                };
                var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestId && !x.IsDeleted && !x.IsDraft);
                if (mockTestSetting != null)
                {
                    mockTestQuestionsList.MockTestId = mockTestSetting.Id;
                    mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;
                    mockTestQuestionsList.TimeDuration = mockTestSetting.TimeSettingDuration;
                    mockTestQuestionsList.TestStartTime = mockTestSetting.TestStartTime;
                    mockTestQuestionsList.IsRetake = mockTestSetting.IsAllowReattempts;
                    var examPatternId = mockTestSetting != null ? mockTestSetting.ExamPatternId : Guid.Empty;
                    var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == examPatternId && !x.IsDeleted);
                    mockTestQuestionsList.GeneralInstructions = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "";
                    var mockTestStatusInfo = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == mockTest.MockTestId && x.StudentId == mockTest.UserId && x.IsStarted);
                    if (mockTestStatusInfo == null)
                    {
                        mockTestQuestionsList.RemainingDuration = mockTestSetting != null ? mockTestSetting.TimeSettingDuration : TimeSpan.Zero;
                        mockTestQuestionsList.RemainingAttempts = mockTestSetting != null ? mockTestSetting.TotalAttempts : 0;
                    }
                    else
                    {
                        mockTestQuestionsList.RemainingDuration = mockTestStatusInfo != null && mockTestStatusInfo.RemainingDuration > TimeSpan.Zero ? mockTestStatusInfo.RemainingDuration : TimeSpan.Zero;
                        mockTestQuestionsList.RemainingAttempts = mockTestStatusInfo != null && mockTestStatusInfo.RemainingAttempts > 0 ? mockTestStatusInfo.RemainingAttempts : 0;
                    }
                    mockTestQuestionsList.TotalAttempts = mockTestSetting != null ? mockTestSetting.TotalAttempts : 0;
                    mockTestQuestionsList.IsShowCorrectAnswer = mockTestSetting != null ? mockTestSetting.IsShowCorrectAnswer : false;
                    mockTestQuestionsList.ResultDeclaration = mockTestSetting != null ? mockTestSetting.ResultDeclaration : 0;
                    mockTestQuestionsList.ExamPatternName = examPatternDetails != null ? examPatternDetails.ExamPatternName : "N/A";
                    mockTestQuestionsList.GeneralInstructions = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "N/A";
                    var languageList = new List<string>();
                    languageList.Add(mockTestSetting.Language);
                    if (!string.IsNullOrEmpty(mockTestSetting.SecondaryLanguage))
                    {
                        languageList.Add(mockTestSetting.SecondaryLanguage);
                    }
                    mockTestQuestionsList.Languages = languageList;
                    var MockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && !x.IsDeleted);
                    List<Res.StudentMocktestPanelList> mockTest1 = new();
                    if (MockTestQuestions.Any())
                    {
                        foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                        {
                            var isSubjectExist = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId && !x.IsDeleted);
                            if (isSubjectExist != null)
                            {
                                Res.StudentMocktestPanelList mockTestQuestionss = new();
                                mockTestQuestionss.SubjectId = item.SubjectId;
                                mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                                var MockTestQuesSection = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                mockTestQuestionss.TotalQuestions = 0;
                                mockTestQuestionss.TotalAttempt = 0;
                                var sectionCount = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == mockTestSetting.ExamPatternId && x.SubjectId == item.SubjectId && !x.IsDeleted);

                                mockTestQuestionss.SectionCount = sectionCount != null ? sectionCount.Count : 0;

                                foreach (var it in MockTestQuesSection.DistinctBy(x => x.SectionId))
                                {

                                    Res.SubjectwiseSection sectionDetails = new();
                                    sectionDetails.SectionId = it.SectionId;
                                    sectionDetails.SectionName = await _examPatternRepository.GetSectionName(it.SectionId);
                                    var sectionDetailsss = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == it.SectionId && x.ExamPatternId == mockTestSetting.ExamPatternId);
                                    sectionDetails.TotalAttempt = sectionDetailsss != null ? sectionDetailsss.TotalAttempt : 0;
                                    sectionDetails.TotalQuestions = sectionDetailsss != null ? sectionDetailsss.TotalQuestions : 0;
                                    var SectionQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && x.SectionId == it.SectionId && !x.IsDeleted);
                                    List<Res.StudentMockTestQuestions> mockTestQuestionsLists = new();
                                    foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                                    {
                                        Res.StudentMockTestQuestions questinBanks = new();
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
                                                        Res.StudentMockEnglish english = new();
                                                        english.Id = items.Id;
                                                        english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                        english.OptionA = items.OptionA;
                                                        english.OptionB = items.OptionB;
                                                        english.OptionC = items.OptionC;
                                                        english.OptionD = items.OptionD;
                                                        // english.Explanation = items.Explanation;
                                                        english.IsCorrectA = false;
                                                        english.IsCorrectB = false;
                                                        english.IsCorrectC = false;
                                                        english.IsCorrectD = false;
                                                        english.IsAvailable = items.QuestionText != "" ? true : false;
                                                        questinBanks.QuestionTableData.English = english;
                                                        break;
                                                    case "Hindi":

                                                        Res.StudentMockHindi hindi = new();
                                                        hindi.Id = items.Id;
                                                        hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                        hindi.OptionA = items.OptionA;
                                                        hindi.OptionB = items.OptionB;
                                                        hindi.OptionC = items.OptionC;
                                                        hindi.OptionD = items.OptionD;
                                                        hindi.IsCorrectA = false;
                                                        hindi.IsCorrectB = false;
                                                        hindi.IsCorrectC = false;
                                                        hindi.IsCorrectD = false;
                                                        // hindi.Explanation = items.Explanation;
                                                        hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                        questinBanks.QuestionTableData.Hindi = hindi;
                                                        break;
                                                    case "Gujarati":

                                                        Res.StudentMockGujarati Gujarati = new();
                                                        Gujarati.Id = items.Id;
                                                        Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                        Gujarati.OptionA = items.OptionA;
                                                        Gujarati.OptionB = items.OptionB;
                                                        Gujarati.OptionC = items.OptionC;
                                                        Gujarati.OptionD = items.OptionD;
                                                        Gujarati.IsCorrectA = false;
                                                        Gujarati.IsCorrectB = false;
                                                        Gujarati.IsCorrectC = false;
                                                        Gujarati.IsCorrectD = false;
                                                        // Gujarati.Explanation = items.Explanation;
                                                        Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                        questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                        break;
                                                    case "Marathi":
                                                        Res.StudentMockMarathi marathi = new();
                                                        marathi.Id = items.Id;
                                                        marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                        marathi.OptionA = items.OptionA;
                                                        marathi.OptionB = items.OptionB;
                                                        marathi.OptionC = items.OptionC;
                                                        marathi.OptionD = items.OptionD;
                                                        marathi.IsCorrectA = false;
                                                        marathi.IsCorrectB = false;
                                                        marathi.IsCorrectC = false;
                                                        marathi.IsCorrectD = false;
                                                        // marathi.Explanation = items.Explanation;
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

                                    }
                                    mockTestQuestionss.TotalAttempt = mockTestQuestionss.TotalAttempt + sectionDetails.TotalAttempt;
                                    mockTestQuestionss.TotalQuestions = mockTestQuestionss.TotalQuestions + sectionDetails.TotalQuestions;
                                    mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                                    mockTestQuestionss.SubjectwiseSection.Add(sectionDetails);
                                }
                                mockTest1.Add(mockTestQuestionss);

                                mockTestQuestionsList.MocktestPanelList = mockTest1;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return mockTestQuestionsList;
                }
                return null;
            }
            else
            {
                Res.StudentMocktestPanel mockTestQuestionsList = new()
                {
                    MocktestPanelList = new()
                };
                var mockTestSetting = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == mockTest.MockTestId && !x.IsDeleted);
                if (mockTestSetting != null)
                {
                    var mockTestStatusInfo = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == mockTest.MockTestId && x.StudentId == mockTest.UserId && x.IsStarted && !x.IsCompleted);
                    mockTestQuestionsList.MockTestId = mockTestSetting.Id;
                    mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;
                    mockTestQuestionsList.TimeDuration = mockTestSetting.TimeDuration;
                    mockTestQuestionsList.IsRetake = true;
                    mockTestQuestionsList.IsShowCorrectAnswer = true;
                    mockTestQuestionsList.TotalAttempts = 3;
                    Guid examPatternId = Guid.Parse("85f4662f-6a61-4307-7281-08db81efd53e");
                    Res.GeneralInstructions generalInstructions = new();
                    var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.ExamPatternName == "Custom" && !x.IsDeleted);
                    mockTestQuestionsList.GeneralInstructions = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "";

                    //  mockTestQuestionsList.RemainingDuration = mockTestStatusInfo != null ? mockTestStatusInfo.RemainingDuration : TimeSpan.Zero;
                    if (mockTestStatusInfo == null)
                    {
                        mockTestQuestionsList.RemainingDuration = TimeSpan.FromHours(1);
                        mockTestQuestionsList.RemainingAttempts = 3;
                    }
                    else
                    {
                        mockTestQuestionsList.RemainingDuration = mockTestStatusInfo != null && mockTestStatusInfo.RemainingDuration > TimeSpan.Zero ? mockTestStatusInfo.RemainingDuration : TimeSpan.Zero;
                        mockTestQuestionsList.RemainingAttempts = mockTestStatusInfo != null && mockTestStatusInfo.RemainingAttempts > 0 ? mockTestStatusInfo.RemainingAttempts : 0;
                    }

                    var languageList = new List<string>();
                    languageList.Add(mockTestSetting.Language);
                    var MockTestQuestions = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && !x.IsDeleted);
                    mockTestQuestionsList.Languages = languageList;
                    List<Res.StudentMocktestPanelList> mockTest1 = new();
                    if (MockTestQuestions.Any())
                    {
                        foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                        {
                            var isSubjectExist = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId && !x.IsDeleted);
                            if (isSubjectExist != null)
                            {
                                Res.StudentMocktestPanelList mockTestQuestionss = new();
                                mockTestQuestionss.SubjectId = item.SubjectId;
                                mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                                var MockTestQuesSection = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                mockTestQuestionss.TotalQuestions = 25;
                                mockTestQuestionss.TotalAttempt = 25;
                                mockTestQuestionss.SectionCount = 1;

                                Res.SubjectwiseSection sectionDetails = new();
                                sectionDetails.SectionId = Guid.NewGuid();
                                sectionDetails.SectionName = "Section-1";
                                sectionDetails.TotalQuestions = 25;
                                sectionDetails.TotalAttempt = 25;
                                var SectionQuestions = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                List<Res.StudentMockTestQuestions> mockTestQuestionsLists = new();
                                foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                                {
                                    Res.StudentMockTestQuestions questinBanks = new();
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
                                                    Res.StudentMockEnglish english = new();
                                                    english.Id = items.Id;
                                                    english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                    english.OptionA = items.OptionA;
                                                    english.OptionB = items.OptionB;
                                                    english.OptionC = items.OptionC;
                                                    english.OptionD = items.OptionD;
                                                    // english.Explanation = items.Explanation;
                                                    english.IsCorrectA = false;
                                                    english.IsCorrectB = false;
                                                    english.IsCorrectC = false;
                                                    english.IsCorrectD = false;
                                                    english.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.English = english;
                                                    break;
                                                case "Hindi":

                                                    Res.StudentMockHindi hindi = new();
                                                    hindi.Id = items.Id;
                                                    hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                    hindi.OptionA = items.OptionA;
                                                    hindi.OptionB = items.OptionB;
                                                    hindi.OptionC = items.OptionC;
                                                    hindi.OptionD = items.OptionD;
                                                    hindi.IsCorrectA = false;
                                                    hindi.IsCorrectB = false;
                                                    hindi.IsCorrectC = false;
                                                    hindi.IsCorrectD = false;
                                                    // hindi.Explanation = items.Explanation;
                                                    hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Hindi = hindi;
                                                    break;
                                                case "Gujarati":

                                                    Res.StudentMockGujarati Gujarati = new();
                                                    Gujarati.Id = items.Id;
                                                    Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                    Gujarati.OptionA = items.OptionA;
                                                    Gujarati.OptionB = items.OptionB;
                                                    Gujarati.OptionC = items.OptionC;
                                                    Gujarati.OptionD = items.OptionD;
                                                    Gujarati.IsCorrectA = false;
                                                    Gujarati.IsCorrectB = false;
                                                    Gujarati.IsCorrectC = false;
                                                    Gujarati.IsCorrectD = false;
                                                    // Gujarati.Explanation = items.Explanation;
                                                    Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                    break;
                                                case "Marathi":
                                                    Res.StudentMockMarathi marathi = new();
                                                    marathi.Id = items.Id;
                                                    marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                    marathi.OptionA = items.OptionA;
                                                    marathi.OptionB = items.OptionB;
                                                    marathi.OptionC = items.OptionC;
                                                    marathi.OptionD = items.OptionD;
                                                    marathi.IsCorrectA = false;
                                                    marathi.IsCorrectB = false;
                                                    marathi.IsCorrectC = false;
                                                    marathi.IsCorrectD = false;
                                                    // marathi.Explanation = items.Explanation;
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

                                }
                                mockTestQuestionss.TotalAttempt = 25;
                                mockTestQuestionss.TotalQuestions = 25;
                                mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                                mockTestQuestionss.SubjectwiseSection.Add(sectionDetails);
                                mockTest1.Add(mockTestQuestionss);
                                mockTestQuestionsList.MocktestPanelList = mockTest1;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return mockTestQuestionsList;
                }
                return null;


            }
        }
        public async Task<Res.StudentQuestionResponseV2?> SaveStudentResponses(Req.StudentQuestionResponse response)
        {
            Res.StudentQuestionResponseV2 studentQuestionResponse = new();
            //For Non Admin side mocktest
            var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && x.IsStarted && !x.IsDeleted);
            if (isExists != null)
            {
                isExists.RemainingDuration = response.RemainingDuration;
                await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
            }
            if (!response.IsCustome)
            {

                var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == response.MockTestId && !x.IsDeleted);
                if (mockTestSetting == null)
                {
                    return null;
                }
                var checkResponse = await _unitOfWork.Repository<DM.StudentAnswers>().GetSingle(x => x.MockTestId == response.MockTestId && x.QuestionRefId == response.QuestionRefId && x.StudentId == response.UserId);
                var questionBankDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == response.QuestionRefId && !x.IsDeleted);
                if (checkResponse != null)
                {
                    checkResponse.SubjectId = response.SubjectId;
                    checkResponse.SectionId = response.SectionId;
                    checkResponse.QuestionType = response.QuestionType;
                    checkResponse.StudentId = response.UserId;
                    checkResponse.IsCorrectA = response.IsCorrectA;
                    checkResponse.IsCorrectB = response.IsCorrectB;
                    checkResponse.IsVisited = true;
                    checkResponse.IsCorrectC = response.IsCorrectC;
                    checkResponse.IsCorrectD = response.IsCorrectD;
                    checkResponse.StudentAnswer = response.StudentAnswer;
                    checkResponse.IsActive = true;
                    checkResponse.IsDeleted = false;
                    checkResponse.IsMarkReview = response.IsMarkReview;
                    checkResponse.LastModifyDate = DateTime.UtcNow;
                    checkResponse.LastModifierUserId = response.UserId;
                    checkResponse.SubjectId = response.SubjectId;
                    checkResponse.IsAnswered = response.IsAnswered;
                    var result = await _unitOfWork.Repository<DM.StudentAnswers>().Update(checkResponse);
                    if (result > 0)
                    {
                        if (mockTestSetting.IsShowCorrectAnswer && mockTestSetting.ResultDeclaration == ResultDeclaration.ShowResultAfterEachQuestion)
                        {

                            studentQuestionResponse = new()
                            {
                                Id = checkResponse.Id,
                                MockTestId = response.MockTestId,
                                QuestionRefId = response.QuestionRefId,
                                QuestionType = response.QuestionType,
                                SubjectId = response.SubjectId,
                                SectionId = response.SectionId,
                                IsCorrectA = response.IsCorrectA,
                                IsCorrectB = response.IsCorrectB,
                                IsCorrectC = response.IsCorrectC,
                                IsCorrectD = response.IsCorrectD,
                                IsMarkReview = response.IsMarkReview,
                                StudentAnswer = response.StudentAnswer,
                                IsAnswered = response.IsAnswered,
                                IsVisited = checkResponse.IsVisited,
                                IsShowResult = true,
                                IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false,
                                IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false,
                                IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false,
                                IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false,
                                CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "",
                                CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "",
                                CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "",
                                CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "",
                                Explaination = questionBankDetail != null ? questionBankDetail.Explanation : ""

                            };


                        }
                        else
                        {
                            studentQuestionResponse = new()
                            {
                                Id = checkResponse.Id,
                                MockTestId = response.MockTestId,
                                QuestionRefId = response.QuestionRefId,
                                QuestionType = response.QuestionType,
                                SubjectId = response.SubjectId,
                                SectionId = response.SectionId,
                                IsCorrectA = response.IsCorrectA,
                                IsCorrectB = response.IsCorrectB,
                                IsCorrectC = response.IsCorrectC,
                                IsCorrectD = response.IsCorrectD,
                                IsMarkReview = response.IsMarkReview,
                                StudentAnswer = response.StudentAnswer,
                                IsAnswered = response.IsAnswered,
                                IsVisited = checkResponse.IsVisited,
                                IsShowResult = false,
                                IsActualCorrectA = false,
                                IsActualCorrectB = false,
                                IsActualCorrectC = false,
                                IsActualCorrectD = false,
                                CorrectAnswerA = "",
                                CorrectAnswerB = "",
                                CorrectAnswerC = "",
                                CorrectAnswerD = "",
                                Explaination = ""
                            };
                        }
                        return studentQuestionResponse;
                    }
                    return null;
                }
                else //For student created mocktest
                {
                    DM.StudentAnswers studentAnswers = new()
                    {
                        QuestionRefId = response.QuestionRefId,
                        QuestionType = response.QuestionType,
                        SubjectId = response.SubjectId,
                        SectionId = response.SectionId,
                        StudentId = response.UserId,
                        MockTestId = response.MockTestId,
                        IsCorrectA = response.IsCorrectA,
                        IsCorrectB = response.IsCorrectB,
                        IsCorrectC = response.IsCorrectC,
                        IsCorrectD = response.IsCorrectD,
                        IsMarkReview = response.IsMarkReview,
                        IsVisited = true,
                        StudentAnswer = response.StudentAnswer,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = response.UserId,
                        IsAnswered = response.IsAnswered
                    };
                    var result = await _unitOfWork.Repository<DM.StudentAnswers>().Insert(studentAnswers);
                    if (result > 0)
                    {
                        if (mockTestSetting.IsShowCorrectAnswer && mockTestSetting.ResultDeclaration == ResultDeclaration.ShowResultAfterEachQuestion)
                        {
                            studentQuestionResponse = new()
                            {
                                Id = studentAnswers.Id,
                                MockTestId = response.MockTestId,
                                QuestionRefId = response.QuestionRefId,
                                QuestionType = response.QuestionType,
                                SubjectId = response.SubjectId,
                                SectionId = response.SectionId,
                                IsCorrectA = response.IsCorrectA,
                                IsCorrectB = response.IsCorrectB,
                                IsCorrectC = response.IsCorrectC,
                                IsCorrectD = response.IsCorrectD,
                                IsMarkReview = response.IsMarkReview,
                                StudentAnswer = response.StudentAnswer,
                                IsAnswered = response.IsAnswered,
                                IsVisited = studentAnswers.IsVisited,
                                IsShowResult = true,
                                IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false,
                                IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false,
                                IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false,
                                IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false,
                                CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "",
                                CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "",
                                CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "",
                                CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "",
                                Explaination = questionBankDetail != null ? questionBankDetail.Explanation : ""

                            };
                        }
                        else
                        {
                            studentQuestionResponse = new()
                            {
                                Id = studentAnswers.Id,
                                MockTestId = response.MockTestId,
                                QuestionRefId = response.QuestionRefId,
                                QuestionType = response.QuestionType,
                                SubjectId = response.SubjectId,
                                SectionId = response.SectionId,
                                IsCorrectA = response.IsCorrectA,
                                IsCorrectB = response.IsCorrectB,
                                IsCorrectC = response.IsCorrectC,
                                IsCorrectD = response.IsCorrectD,
                                IsMarkReview = response.IsMarkReview,
                                StudentAnswer = response.StudentAnswer,
                                IsAnswered = response.IsAnswered,
                                IsVisited = studentAnswers.IsVisited,
                                IsShowResult = false,
                                IsActualCorrectA = false,
                                IsActualCorrectB = false,
                                IsActualCorrectC = false,
                                IsActualCorrectD = false,
                                CorrectAnswerA = "",
                                CorrectAnswerB = "",
                                CorrectAnswerC = "",
                                CorrectAnswerD = "",
                                Explaination = ""
                            };
                        }
                        return studentQuestionResponse;
                    }
                    return null;
                }
            }
            else
            {
                var checkResponse = await _unitOfWork.Repository<DM.StudentAnswers>().GetSingle(x => x.MockTestId == response.MockTestId && x.QuestionRefId == response.QuestionRefId && x.StudentId == response.UserId);
                var questionBankDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == response.QuestionRefId && !x.IsDeleted);
                if (checkResponse != null)
                {
                    checkResponse.SubjectId = response.SubjectId;
                    checkResponse.SectionId = response.SectionId;
                    checkResponse.QuestionType = response.QuestionType;
                    checkResponse.StudentId = response.UserId;
                    checkResponse.IsCorrectA = response.IsCorrectA;
                    checkResponse.IsCorrectB = response.IsCorrectB;
                    checkResponse.IsCorrectC = response.IsCorrectC;
                    checkResponse.IsCorrectD = response.IsCorrectD;
                    checkResponse.StudentAnswer = response.StudentAnswer;
                    checkResponse.IsActive = true;
                    checkResponse.IsDeleted = false;
                    checkResponse.IsMarkReview = response.IsMarkReview;
                    checkResponse.IsVisited = true;
                    checkResponse.LastModifyDate = DateTime.UtcNow;
                    checkResponse.LastModifierUserId = response.UserId;
                    checkResponse.SubjectId = response.SubjectId;
                    checkResponse.IsAnswered = response.IsAnswered;

                    var result = await _unitOfWork.Repository<DM.StudentAnswers>().Update(checkResponse);
                    if (result > 0)
                    {
                        studentQuestionResponse = new()
                        {
                            Id = checkResponse.Id,
                            MockTestId = response.MockTestId,
                            QuestionRefId = response.QuestionRefId,
                            QuestionType = response.QuestionType,
                            SubjectId = response.SubjectId,
                            SectionId = response.SectionId,
                            IsCorrectA = response.IsCorrectA,
                            IsCorrectB = response.IsCorrectB,
                            IsCorrectC = response.IsCorrectC,
                            IsCorrectD = response.IsCorrectD,
                            IsMarkReview = response.IsMarkReview,
                            StudentAnswer = response.StudentAnswer,
                            IsAnswered = response.IsAnswered,
                            IsVisited = checkResponse.IsVisited,
                            IsShowResult = true,
                            IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false,
                            IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false,
                            IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false,
                            IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false,
                            CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "",
                            CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "",
                            CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "",
                            CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "",
                            Explaination = questionBankDetail != null ? questionBankDetail.Explanation : ""

                        };
                        return studentQuestionResponse;
                    }
                    return null;
                }
                else
                {
                    DM.StudentAnswers studentAnswers = new()
                    {
                        QuestionRefId = response.QuestionRefId,
                        QuestionType = response.QuestionType,
                        SubjectId = response.SubjectId,
                        SectionId = response.SectionId,
                        StudentId = response.UserId,
                        MockTestId = response.MockTestId,
                        IsCorrectA = response.IsCorrectA,
                        IsCorrectB = response.IsCorrectB,
                        IsCorrectC = response.IsCorrectC,
                        IsCorrectD = response.IsCorrectD,
                        IsMarkReview = response.IsMarkReview,
                        IsVisited = true,
                        StudentAnswer = response.StudentAnswer,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = response.UserId,
                        IsAnswered = response.IsAnswered


                    };
                    var result = await _unitOfWork.Repository<DM.StudentAnswers>().Insert(studentAnswers);
                    if (result > 0)
                    {
                        studentQuestionResponse = new()
                        {
                            Id = studentAnswers.Id,
                            MockTestId = response.MockTestId,
                            QuestionRefId = response.QuestionRefId,
                            QuestionType = response.QuestionType,
                            SubjectId = response.SubjectId,
                            SectionId = response.SectionId,
                            IsCorrectA = response.IsCorrectA,
                            IsCorrectB = response.IsCorrectB,
                            IsCorrectC = response.IsCorrectC,
                            IsCorrectD = response.IsCorrectD,
                            IsMarkReview = response.IsMarkReview,
                            StudentAnswer = response.StudentAnswer,
                            IsAnswered = response.IsAnswered,
                            IsVisited = studentAnswers.IsVisited,
                            IsShowResult = true,
                            IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false,
                            IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false,
                            IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false,
                            IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false,
                            CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "",
                            CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "",
                            CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "",
                            CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "",
                            Explaination = questionBankDetail != null ? questionBankDetail.Explanation : ""

                        };
                        return studentQuestionResponse;
                    }
                    return null;
                }

            }
        }
        public async Task<bool> MarkAsSeen(Req.MarkAsSeen response)
        {
            var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && x.IsStarted && !x.IsDeleted);
            if (isExists != null)
            {

                isExists.RemainingDuration = response.RemainingDuration;
                await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
            }
            var checkResponse = await _unitOfWork.Repository<DM.StudentAnswers>().GetSingle(x => x.MockTestId == response.MockTestId && x.QuestionRefId == response.QuestionRefId && x.StudentId == response.UserId);
            if (checkResponse != null)
            {

                checkResponse.StudentId = response.UserId;
                checkResponse.SubjectId = response.SubjectId;
                checkResponse.SectionId = response.SectionId;
                checkResponse.QuestionType = response.QuestionType;
                checkResponse.IsActive = true;
                checkResponse.IsDeleted = false;
                checkResponse.LastModifyDate = DateTime.UtcNow;
                checkResponse.LastModifierUserId = response.UserId;
                checkResponse.IsVisited = response.IsVisited;
                var result = await _unitOfWork.Repository<DM.StudentAnswers>().Update(checkResponse);
                return result > 0;
            }
            else
            {
                DM.StudentAnswers studentAnswers = new()
                {
                    QuestionRefId = response.QuestionRefId,
                    SubjectId = response.SubjectId,
                    SectionId = response.SectionId,
                    QuestionType = response.QuestionType,
                    StudentId = response.UserId,
                    MockTestId = response.MockTestId,
                    IsVisited = response.IsVisited,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = response.UserId
                };
                var result = await _unitOfWork.Repository<DM.StudentAnswers>().Insert(studentAnswers);
                return result > 0;
            }

        }
        public async Task<Res.StudentAnswersPanelList?> GetStudentAnswerPanel(Req.StudentAnwersPanel panel)
        {
            if (!panel.IsCustome)
            {
                var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == panel.MockTestId && !x.IsDeleted);

                Res.StudentAnswersPanelList studentAnswersPanelList = new()
                {
                    StudentQuestionResponse = new()
                };
                List<Res.StudentQuestionResponseV1> studentQuestionResponses = new();
                var studentAnswers = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == panel.MockTestId && !x.IsDeleted && x.StudentId == panel.UserId);
                foreach (var studentAnswer in studentAnswers)
                {
                    var questionBankDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == studentAnswer.QuestionRefId && !x.IsDeleted);
                    Res.StudentQuestionResponseV1 studentQuestion = new();
                    studentQuestion.Id = studentAnswer.Id;
                    studentQuestion.MockTestId = studentAnswer.MockTestId;
                    studentQuestion.QuestionRefId = studentAnswer.QuestionRefId;
                    studentQuestion.QuestionType = studentAnswer.QuestionType;
                    studentQuestion.SubjectId = studentAnswer.SubjectId;
                    studentQuestion.SectionId = studentAnswer.SectionId;
                    studentQuestion.IsCorrectA = studentAnswer.IsCorrectA;
                    studentQuestion.IsCorrectB = studentAnswer.IsCorrectB;
                    studentQuestion.IsCorrectC = studentAnswer.IsCorrectC;
                    studentQuestion.IsCorrectD = studentAnswer.IsCorrectD;
                    studentQuestion.IsMarkReview = studentAnswer.IsMarkReview;
                    studentQuestion.StudentAnswer = studentAnswer.StudentAnswer;
                    studentQuestion.QuestionType = studentAnswer.QuestionType;
                    studentQuestion.IsAnswered = studentAnswer.IsAnswered;
                    studentQuestion.IsVisited = studentAnswer.IsVisited;
                    if (mockTestSetting != null && mockTestSetting.IsShowCorrectAnswer && mockTestSetting.ResultDeclaration == ResultDeclaration.ShowResultAfterEachQuestion)
                    {
                        studentQuestion.IsShowResult = true;
                        studentQuestion.IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false;
                        studentQuestion.IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false;
                        studentQuestion.IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false;
                        studentQuestion.IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false;
                        studentQuestion.CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "";
                        studentQuestion.CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "";
                        studentQuestion.CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "";
                        studentQuestion.CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "";
                        studentQuestion.Explaination = questionBankDetail != null ? questionBankDetail.Explanation : "";
                    }
                    studentQuestionResponses.Add(studentQuestion);
                }
                studentAnswersPanelList.StudentQuestionResponse = studentQuestionResponses;
                return studentAnswersPanelList;
            }
            else
            {

                Res.StudentAnswersPanelList studentAnswersPanelList = new()
                {
                    StudentQuestionResponse = new()
                };
                List<Res.StudentQuestionResponseV1> studentQuestionResponses = new();
                var studentAnswers = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == panel.MockTestId && !x.IsDeleted && x.StudentId == panel.UserId);
                foreach (var studentAnswer in studentAnswers)
                {
                    var questionBankDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == studentAnswer.QuestionRefId && !x.IsDeleted);
                    Res.StudentQuestionResponseV1 studentQuestion = new();
                    studentQuestion.Id = studentAnswer.Id;
                    studentQuestion.MockTestId = studentAnswer.MockTestId;
                    studentQuestion.QuestionRefId = studentAnswer.QuestionRefId;
                    studentQuestion.QuestionType = studentAnswer.QuestionType;
                    studentQuestion.SubjectId = studentAnswer.SubjectId;
                    studentQuestion.SectionId = studentAnswer.SectionId;
                    studentQuestion.IsCorrectA = studentAnswer.IsCorrectA;
                    studentQuestion.IsCorrectB = studentAnswer.IsCorrectB;
                    studentQuestion.IsCorrectC = studentAnswer.IsCorrectC;
                    studentQuestion.IsCorrectD = studentAnswer.IsCorrectD;
                    studentQuestion.IsMarkReview = studentAnswer.IsMarkReview;
                    studentQuestion.StudentAnswer = studentAnswer.StudentAnswer;
                    studentQuestion.QuestionType = studentAnswer.QuestionType;
                    studentQuestion.IsAnswered = studentAnswer.IsAnswered;
                    studentQuestion.IsVisited = studentAnswer.IsVisited;
                    studentQuestion.IsShowResult = true;
                    studentQuestion.IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false;
                    studentQuestion.IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false;
                    studentQuestion.IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false;
                    studentQuestion.IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false;
                    studentQuestion.CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "";
                    studentQuestion.CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "";
                    studentQuestion.CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "";
                    studentQuestion.CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "";
                    studentQuestion.Explaination = questionBankDetail != null ? questionBankDetail.Explanation : "";
                    studentQuestionResponses.Add(studentQuestion);
                }
                studentAnswersPanelList.StudentQuestionResponse = studentQuestionResponses;
                return studentAnswersPanelList;
            }
        }
        public async Task<Res.StudentQuestionResponseV2?> ReviewStudentAnswer(Req.ReviewAnswer panel)
        {
            var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == panel.MockTestId && x.StudentId == panel.UserId && x.IsStarted && !x.IsDeleted);
            if (isExists != null)
            {

                isExists.RemainingDuration = panel.RemainingDuration;
                await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
            }
            var studentAnswer = await _unitOfWork.Repository<DM.StudentAnswers>().GetSingle(x => x.MockTestId == panel.MockTestId && !x.IsDeleted && x.StudentId == panel.UserId && x.QuestionRefId == panel.QuestionRefId);
            if (studentAnswer != null)
            {
                var questionBankDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == studentAnswer.QuestionRefId && !x.IsDeleted);
                Res.StudentQuestionResponseV2 studentQuestion = new();
                studentQuestion.Id = studentAnswer.Id;
                studentQuestion.MockTestId = studentAnswer.MockTestId;
                studentQuestion.QuestionRefId = studentAnswer.QuestionRefId;
                studentQuestion.QuestionType = studentAnswer.QuestionType;
                studentQuestion.SubjectId = studentAnswer.SubjectId;
                studentQuestion.SectionId = studentAnswer.SectionId;
                studentQuestion.IsCorrectA = studentAnswer.IsCorrectA;
                studentQuestion.IsCorrectB = studentAnswer.IsCorrectB;
                studentQuestion.IsCorrectC = studentAnswer.IsCorrectC;
                studentQuestion.IsCorrectD = studentAnswer.IsCorrectD;
                studentQuestion.IsMarkReview = studentAnswer.IsMarkReview;
                studentQuestion.StudentAnswer = studentAnswer.StudentAnswer;
                studentQuestion.QuestionType = studentAnswer.QuestionType;
                studentQuestion.IsAnswered = studentAnswer.IsAnswered;
                studentQuestion.IsVisited = studentAnswer.IsVisited;
                studentQuestion.IsShowResult = true;
                studentQuestion.IsActualCorrectA = questionBankDetail != null ? questionBankDetail.IsCorrectA : false;
                studentQuestion.IsActualCorrectB = questionBankDetail != null ? questionBankDetail.IsCorrectB : false;
                studentQuestion.IsActualCorrectC = questionBankDetail != null ? questionBankDetail.IsCorrectC : false;
                studentQuestion.IsActualCorrectD = questionBankDetail != null ? questionBankDetail.IsCorrectD : false;
                studentQuestion.CorrectAnswerA = questionBankDetail != null && questionBankDetail.IsCorrectA == true ? questionBankDetail.OptionA : "";
                studentQuestion.CorrectAnswerB = questionBankDetail != null && questionBankDetail.IsCorrectB == true ? questionBankDetail.OptionB : "";
                studentQuestion.CorrectAnswerC = questionBankDetail != null && questionBankDetail.IsCorrectC == true ? questionBankDetail.OptionC : "";
                studentQuestion.CorrectAnswerD = questionBankDetail != null && questionBankDetail.IsCorrectD == true ? questionBankDetail.OptionD : "";
                studentQuestion.Explaination = questionBankDetail != null ? questionBankDetail.Explanation : "";
                return studentQuestion;
            }
            return null;

        }
        public async Task<Res.GeneralInstructions?> GetGeneralInstructions(Req.StudentMockTestId mockTestId)
        {
            if (!mockTestId.IsCustom)
            {
                Res.GeneralInstructions generalInstructions = new();
                var mocktestData = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTestId.MockTestId && !x.IsDeleted && !x.IsDraft);
                var examPatternId = mocktestData != null ? mocktestData.ExamPatternId : Guid.Empty;
                var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == examPatternId && !x.IsDeleted);
                generalInstructions.GeneralInstruction = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "";
                return generalInstructions;
            }
            else
            {
                Guid examPatternId = Guid.Parse("85f4662f-6a61-4307-7281-08db81efd53e");
                Res.GeneralInstructions generalInstructions = new();
                var examPatternDetails = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.ExamPatternName == "Custom" && !x.IsDeleted);
                generalInstructions.GeneralInstruction = examPatternDetails != null ? examPatternDetails.GeneralInstruction : "";
                return generalInstructions;
            }

        }
        public async Task<Res.RestultStatus> SaveMockTestStatus(Req.StudentMockTestStatus response)
        {
            Res.RestultStatus status = new();
            if (response != null)
            {
                int result = 0;
                if (!response.IsCustome)
                {
                    var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && !x.IsDeleted);
                    if (!response.IsCompleted)
                    {
                        var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == response.MockTestId && !x.IsDeleted && x.IsAllowReattempts);
                        int totalReattempts = mockTestDetails != null ? mockTestDetails.TotalAttempts : 0;


                        if (isExists == null)
                        {

                            DM.StudentMockTestStatus studentMockTestStatus = new()
                            {
                                MockTestId = response.MockTestId,
                                StudentId = response.UserId,
                                IsActive = true,
                                IsDeleted = false,
                                IsStarted = response.IsStarted,
                                IsCompleted = false,
                                StartTime = response.IsStarted == true ? DateTime.UtcNow : null,
                                EndTime = response.IsCompleted == true ? DateTime.UtcNow : null,
                                RemainingAttempts = totalReattempts > 0 ? totalReattempts - 1 : 0,
                                CreationDate = DateTime.UtcNow,
                                CreatorUserId = response.UserId,
                                IsCustom = false
                            };
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Insert(studentMockTestStatus);
                            status.Id = studentMockTestStatus.Id;
                            status.IsCompleted = false;
                            return status;
                        }
                        else
                        {
                            isExists.IsStarted = response.IsStarted;
                            isExists.IsCompleted = false;
                            isExists.IsCustom = false;
                            isExists.StartTime = response.IsStarted == true ? DateTime.UtcNow : null;
                            isExists.RemainingAttempts = totalReattempts > 0 && isExists.RemainingAttempts > 0 ? isExists.RemainingAttempts - 1 : 0;
                            isExists.LastModifierUserId = response.UserId;
                            isExists.LastModifyDate = DateTime.UtcNow;
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
                            status.Id = isExists.Id;
                            status.IsCompleted = false;
                            return status;
                        }

                    }
                    else if (isExists != null && response.IsCompleted)
                    {
                        var isAnswer = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && x.IsVisited && x.IsAnswered && !x.IsDeleted);
                        if (isAnswer.Any())
                        {
                            isExists.IsCompleted = true;
                            isExists.RemainingDuration = TimeSpan.Zero;
                            isExists.IsCustom = false;
                            isExists.EndTime = response.IsCompleted == true ? DateTime.UtcNow : null;
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
                            if (result > 0)
                            {
                                Req.GetResult rs = new()
                                {
                                    UniqueMockTestId = Guid.NewGuid(),
                                    MockTestId = response.MockTestId,
                                    IsCustome = response.IsCustome,
                                    UserId = response.UserId

                                };
                                var final = await CalculateResult(rs);
                                status.Id = final;
                                status.IsCompleted = true;
                                return status;
                            }
                        }
                        status.Id = Guid.Empty;
                        status.IsCompleted = false;
                        return status;
                    }
                }
                else
                {
                    var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && !x.IsDeleted);
                    if (!response.IsCompleted)
                    {

                        if (isExists == null)
                        {

                            DM.StudentMockTestStatus studentMockTestStatus = new()
                            {
                                MockTestId = response.MockTestId,
                                StudentId = response.UserId,
                                IsActive = true,
                                IsDeleted = false,
                                IsStarted = true,
                                IsCompleted = false,
                                StartTime = response.IsStarted == true ? DateTime.UtcNow : null,
                                EndTime = response.IsCompleted == true ? DateTime.UtcNow : null,
                                RemainingAttempts = 2,
                                IsCustom = true

                            };
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Insert(studentMockTestStatus);
                            status.Id = studentMockTestStatus.Id;
                            status.IsCompleted = false;
                            return status;
                        }
                        else
                        {
                            isExists.IsStarted = response.IsStarted;
                            isExists.IsCompleted = false;
                            isExists.IsCustom = true;
                            isExists.RemainingAttempts = isExists.RemainingAttempts > 0 ? isExists.RemainingAttempts - 1 : 0;
                            //isExists.IsPaused = false;
                            isExists.StartTime = response.IsStarted == true ? DateTime.UtcNow : null;
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
                            status.Id = isExists.Id;
                            status.IsCompleted = false;
                            return status;
                        }

                    }
                    else if (isExists != null && response.IsCompleted)
                    {
                        var isAnswer = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && x.IsVisited && x.IsAnswered && !x.IsDeleted);
                        if (isAnswer.Any())
                        {
                            isExists.IsCompleted = true;
                            isExists.RemainingDuration = TimeSpan.Zero;
                            isExists.IsCustom = true;
                            isExists.EndTime = response.IsCompleted == true ? DateTime.UtcNow : null;
                            result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
                            if (result > 0)
                            {
                                Req.GetResult rs = new()
                                {
                                    UniqueMockTestId = Guid.NewGuid(),
                                    MockTestId = response.MockTestId,
                                    IsCustome = response.IsCustome,
                                    UserId = response.UserId

                                };
                                var final = await CalculateResult(rs);
                                status.Id = final;
                                status.IsCompleted = true;
                                return status;
                            }
                        }
                        status.Id = Guid.Empty;
                        status.IsCompleted = false;
                        return status;
                    }
                }

            }
            status.Id = Guid.Empty;
            status.IsCompleted = false;
            return status;
        }
        public async Task<bool> ResumeMocktest(Req.ResumeMockTest response)
        {
            if (response != null)
            {
                int result = 0;
                var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == response.MockTestId && x.StudentId == response.UserId && x.IsStarted && !x.IsCompleted && !x.IsDeleted);
                if (isExists != null)
                {


                    isExists.RemainingDuration = response.RemainingDuration;
                    result = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);

                    return result > 0;
                }

            }
            return false;
        }
        public async Task<bool> RemoveStudentAnser(Req.RemoveAnswer answer)
        {
            var isExists = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == answer.MockTestId && x.StudentId == answer.UserId && x.IsStarted && !x.IsDeleted);
            if (isExists != null)
            {

                isExists.RemainingDuration = answer.RemainingDuration;
                await _unitOfWork.Repository<DM.StudentMockTestStatus>().Update(isExists);
            }

            var studentAnswerData = await _unitOfWork.Repository<DM.StudentAnswers>().GetSingle(x => x.MockTestId == answer.MockTestId && x.QuestionRefId == answer.QuestionRefId && x.StudentId == answer.UserId && !x.IsDeleted);
            if (studentAnswerData != null)
            {
                studentAnswerData.IsVisited = true;
                studentAnswerData.IsMarkReview = false;
                studentAnswerData.IsAnswered = false;
                studentAnswerData.IsCorrectA = false;
                studentAnswerData.IsCorrectB = false;
                studentAnswerData.IsCorrectC = false;
                studentAnswerData.IsCorrectD = false;
                studentAnswerData.StudentAnswer = string.Empty;
                studentAnswerData.LastModifyDate = DateTime.UtcNow;
                studentAnswerData.LastModifierUserId = answer.UserId;
                int result = await _unitOfWork.Repository<DM.StudentAnswers>().Update(studentAnswerData);
                return result > 0;
            }
            return false;
        }
        #endregion

        public async Task<Guid> CalculateResult(Req.GetResult result)
        {
            #region parameters
            Res.StudentResults studentFinalResult = new();

            bool isPartialFourCorrect = false;
            bool isPartialThreeCorrect = false;
            bool isPartialTwoCorrect = false;
            bool isPartialOneCorrect = false;
            #endregion
            if (!result.IsCustome)
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == result.MockTestId && !x.IsDeleted);

                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;

                var subjectids = mockTestQuestionDetails.DistinctBy(x => x.SubjectId).Select(x => x.SubjectId).ToList();
                studentFinalResult.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                studentFinalResult.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                studentFinalResult.Duration = mockTestDetails != null ? mockTestDetails.TimeSettingDuration : TimeSpan.Zero;
                studentFinalResult.TotalMarks = totalMarks;
                studentFinalResult.TotalQuestion = totalQuestionCount;


                var studentResults = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == result.MockTestId && x.StudentId == result.UserId && !x.IsMarkReview && !x.IsDeleted && x.IsAnswered && x.IsVisited);

                List<Res.SubjectWisePermormance> SubjectWisePermormance = new();

                foreach (var studentAnswer in studentResults.DistinctBy(x => x.SubjectId))
                {
                    int totalMark = 0;
                    int totalNegativeMark = 0;
                    int correctCount = 0;
                    int inCorrectCount = 0;
                    int partialMark = 0;
                    Res.SubjectWisePermormance subjectWise = new();
                    subjectWise.SubjectId = studentAnswer.SubjectId;
                    var subDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectWise.SubjectId && !x.IsDeleted);
                    subjectWise.SubjectName = subDetails != null ? subDetails.SubjectName : "";
                    var mockTestSubjectQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == result.MockTestId && x.SubjectId == subjectWise.SubjectId && !x.IsDeleted);
                    var subjectTotalMarks = mockTestSubjectQuestionDetails.Sum(x => x.Marks);
                    var answerList = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.SubjectId == studentAnswer.SubjectId && x.MockTestId == result.MockTestId && x.StudentId == result.UserId && x.IsAnswered && x.IsVisited && !x.IsMarkReview && !x.IsDeleted);
                    foreach (var answers in answerList)
                    {
                        bool isCorrect = false;
                        var questionDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == answers.QuestionRefId && !x.IsDeleted);
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.SingleChoice)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.MatchTheColumn)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.MCQ)
                        {
                            if (questionDetail.IsPartiallyCorrect)
                            {
                                #region New Code
                                //If 4 options are correct but only 4 options are chosen
                                if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD))
                                {
                                    #region 4 Option region
                                    if (answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD)
                                    {
                                        isPartialFourCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.Mark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    #endregion
                                    #region 3 Option region
                                    //If 3 options are correct but only 3 options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 options are chosen
                                    else if (!answers.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 correct options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 correct options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    #endregion
                                    #region 2 option region
                                    else if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                 || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {

                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }

                                    }
                                    #endregion 2 option 
                                    #region 1 Option 
                                    else if (
                                    (answers.IsCorrectA == questionDetail.IsCorrectA || answers.IsCorrectB == questionDetail.IsCorrectB || answers.IsCorrectC == questionDetail.IsCorrectC || answers.IsCorrectD == questionDetail.IsCorrectD))
                                    {
                                        isPartialFourCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialOneCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }
                                    #endregion

                                }

                                //(ABC, ABD, ACD, and BCD).
                                //If 3 options are correct but only 3 options are chosen
                                else if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (!questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && !questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD))
                                {
                                    //If 3 options are correct but only 3 options are chosen
                                    #region 3 option
                                    if ((answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectD) || (!answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD) || (answers.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                    {
                                        //If 3 options are correct but only 3 options are chosen
                                        if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectD) && (answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 options are chosen
                                        else if ((!questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (!answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 correct options are chosen
                                        else if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 correct options are chosen
                                        else if ((questionDetail.IsCorrectA && !questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (answers.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }
                                        #endregion 3 option
                                    }

                                    //If 3 or more options are correct but only 2 options are chosen
                                    #region 2 option region
                                    else if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                  || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {
                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    #endregion 2 option 
                                    //If 3 or more options are correct but only 1 options are chosen
                                    #region 1 Option 
                                    //If 3 or more options are correct but only 1 options are chosen
                                    else if ((answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectB && !answers.IsCorrectA && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectC && !answers.IsCorrectA && !questionDetail.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC))
                                    {
                                        #region 1 Option 
                                        if ((questionDetail.IsCorrectA && answers.IsCorrectA))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectB && answers.IsCorrectB))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectC && answers.IsCorrectC))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectD && answers.IsCorrectD))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }

                                    #endregion

                                }
                                //AB, AC, AD, BC, BD, and CD.
                                //If 2 options are correct but only 2 correct options are chosen
                                else if (((questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD)
                                    || (questionDetail.IsCorrectA && questionDetail.IsCorrectC && !questionDetail.IsCorrectB && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && questionDetail.IsCorrectD && !questionDetail.IsCorrectC && !questionDetail.IsCorrectB) || (questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectA && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectB && questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectC) || (questionDetail.IsCorrectC && questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB)))
                                {
                                    #region 2 option region
                                    if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {

                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else if ((answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectB && !answers.IsCorrectA && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectC && !answers.IsCorrectA && !questionDetail.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC))
                                    {
                                        #region 1 Option 
                                        if ((questionDetail.IsCorrectA && answers.IsCorrectA))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectB && answers.IsCorrectB))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectC && answers.IsCorrectC))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectD && answers.IsCorrectD))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }
                                }
                                //A,B,C,D
                                //If one correct option and one option are choosen 
                                else if ((questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectB && !questionDetail.IsCorrectA && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectC && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectC))
                                {
                                    if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                                    {
                                        isCorrect = true;
                                        var assignedMark = questionDetail.Mark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    if (!isCorrect)
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;

                                    }
                                }
                                #endregion

                                #endregion 2 option 

                            }
                            else
                            {
                                if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                                {
                                    isCorrect = true;
                                    var assignedMark = questionDetail.Mark;
                                    totalMark += assignedMark;
                                    correctCount++;
                                }
                                if (!isCorrect)
                                {
                                    var assignedMark = (questionDetail.NegativeMark);
                                    totalNegativeMark += assignedMark;
                                    inCorrectCount++;

                                }
                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.Phrases)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.IntegerType)
                        {
                            string output = Regex.Replace(questionDetail.OptionA, "<.*?>", "");

                            if (output == answers.StudentAnswer)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                    }
                    subjectWise.TotalSubjectQuestion = mockTestSubjectQuestionDetails.Count;
                    subjectWise.Correct = correctCount;
                    subjectWise.InCorrect = inCorrectCount;
                    subjectWise.Skipped = (subjectWise.TotalSubjectQuestion - answerList.Count);
                    int finalMarks = totalMark - totalNegativeMark;
                    DM.StudentResult studentResult = new()
                    {
                        MockTestId = result.MockTestId,
                        UniqueMockTetId = result.UniqueMockTestId,
                        StudentId = result.UserId,
                        SubjectId = studentAnswer.SubjectId,
                        CorrectAnswer = subjectWise.Correct,
                        InCorrectAnswer = subjectWise.InCorrect,
                        SkippedAnswer = subjectWise.Skipped,
                        TotalMarks = subjectTotalMarks,
                        TotalQuestion = subjectWise.TotalSubjectQuestion,
                        ObtainMarks = finalMarks,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = result.UserId,
                        IsCustom = false

                    };
                    await _unitOfWork.Repository<DM.StudentResult>().Insert(studentResult);
                    SubjectWisePermormance.Add(subjectWise);

                }
                studentFinalResult.SubjectWisePermormance = SubjectWisePermormance;
                studentFinalResult.OverAllCorrect = SubjectWisePermormance.Sum(x => x.Correct);
                studentFinalResult.OverAllInCorrect = SubjectWisePermormance.Sum(x => x.InCorrect);
                studentFinalResult.OverAllSkipped = studentFinalResult.TotalQuestion - studentResults.Count;
                return result.UniqueMockTestId;
            }
            else
            {

                var mockTestDetails = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == result.MockTestId && !x.IsDeleted);
                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;
                var subjectids = mockTestQuestionDetails.DistinctBy(x => x.SubjectId).Select(x => x.SubjectId).ToList();
                studentFinalResult.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                studentFinalResult.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                studentFinalResult.Duration = mockTestDetails != null ? mockTestDetails.TimeDuration : TimeSpan.Zero;
                studentFinalResult.TotalMarks = totalMarks;
                studentFinalResult.TotalQuestion = totalQuestionCount;

                var studentResults = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.MockTestId == result.MockTestId && x.StudentId == result.UserId && !x.IsMarkReview && !x.IsDeleted && x.IsAnswered && x.IsVisited);

                List<Res.SubjectWisePermormance> SubjectWisePermormance = new();
                foreach (var studentAnswer in studentResults.DistinctBy(x => x.SubjectId))
                {
                    int totalMark = 0;
                    int totalNegativeMark = 0;
                    int correctCount = 0;
                    int inCorrectCount = 0;
                    int partialMark = 0;
                    Res.SubjectWisePermormance subjectWise = new();
                    subjectWise.SubjectId = studentAnswer.SubjectId;
                    var subDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectWise.SubjectId && !x.IsDeleted);
                    subjectWise.SubjectName = subDetails != null ? subDetails.SubjectName : "";
                    var mockTestSubjectQuestionDetails = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == result.MockTestId && x.SubjectId == subjectWise.SubjectId && !x.IsDeleted);
                    var subjectTotalMarks = mockTestSubjectQuestionDetails.Sum(x => x.Marks);
                    var answerList = await _unitOfWork.Repository<DM.StudentAnswers>().Get(x => x.SubjectId == studentAnswer.SubjectId && x.MockTestId == result.MockTestId && x.StudentId == result.UserId && !x.IsMarkReview && !x.IsDeleted && x.IsVisited && x.IsAnswered);
                    foreach (var answers in answerList)
                    {
                        bool isCorrect = false;
                        var questionDetail = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == answers.QuestionRefId && !x.IsDeleted);
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.SingleChoice)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.MatchTheColumn)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.MCQ)
                        {
                            if (questionDetail.IsPartiallyCorrect)
                            {
                                #region New Code
                                //If 4 options are correct but only 4 options are chosen
                                if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD))
                                {
                                    #region 4 Option region
                                    if (answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD)
                                    {
                                        isPartialFourCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.Mark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    #endregion
                                    #region 3 Option region
                                    //If 3 options are correct but only 3 options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 options are chosen
                                    else if (!answers.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 correct options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    //If 3 options are correct but only 3 correct options are chosen
                                    else if (answers.IsCorrectA == questionDetail.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD)
                                    {
                                        isPartialThreeCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialThreeCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    #endregion
                                    #region 2 option region
                                    else if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                 || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {

                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }

                                    }
                                    #endregion 2 option 
                                    #region 1 Option 
                                    else if (
                                    (answers.IsCorrectA == questionDetail.IsCorrectA || answers.IsCorrectB == questionDetail.IsCorrectB || answers.IsCorrectC == questionDetail.IsCorrectC || answers.IsCorrectD == questionDetail.IsCorrectD))
                                    {
                                        isPartialFourCorrect = true;
                                        isCorrect = true;
                                        var assignedMark = questionDetail.PartialOneCorrectMark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }
                                    #endregion

                                }

                                //(ABC, ABD, ACD, and BCD).
                                //If 3 options are correct but only 3 options are chosen
                                else if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (!questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && !questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD))
                                {
                                    //If 3 options are correct but only 3 options are chosen
                                    #region 3 option
                                    if ((answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectD) || (!answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD) || (answers.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                    {
                                        //If 3 options are correct but only 3 options are chosen
                                        if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectD) && (answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 options are chosen
                                        else if ((!questionDetail.IsCorrectA && questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (!answers.IsCorrectA && answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 correct options are chosen
                                        else if ((questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        //If 3 options are correct but only 3 correct options are chosen
                                        else if ((questionDetail.IsCorrectA && !questionDetail.IsCorrectB && questionDetail.IsCorrectC && questionDetail.IsCorrectD) && (answers.IsCorrectA && !answers.IsCorrectB && answers.IsCorrectC && answers.IsCorrectD))
                                        {
                                            isPartialThreeCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }
                                        #endregion 3 option
                                    }

                                    //If 3 or more options are correct but only 2 options are chosen
                                    #region 2 option region
                                    else if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                  || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {
                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialTwoCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    #endregion 2 option 
                                    //If 3 or more options are correct but only 1 options are chosen
                                    #region 1 Option 
                                    //If 3 or more options are correct but only 1 options are chosen
                                    else if ((answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectB && !answers.IsCorrectA && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectC && !answers.IsCorrectA && !questionDetail.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC))
                                    {
                                        #region 1 Option 
                                        if ((questionDetail.IsCorrectA && answers.IsCorrectA))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectB && answers.IsCorrectB))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectC && answers.IsCorrectC))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectD && answers.IsCorrectD))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }

                                    #endregion

                                }
                                //AB, AC, AD, BC, BD, and CD.
                                //If 2 options are correct but only 2 correct options are chosen
                                else if (((questionDetail.IsCorrectA && questionDetail.IsCorrectB && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD)
                                    || (questionDetail.IsCorrectA && questionDetail.IsCorrectC && !questionDetail.IsCorrectB && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectA && questionDetail.IsCorrectD && !questionDetail.IsCorrectC && !questionDetail.IsCorrectB) || (questionDetail.IsCorrectB && questionDetail.IsCorrectC && !questionDetail.IsCorrectA && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectB && questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectC) || (questionDetail.IsCorrectC && questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB)))
                                {
                                    #region 2 option region
                                    if (((answers.IsCorrectA && answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                || (answers.IsCorrectA && answers.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectA && answers.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB) || (answers.IsCorrectB && answers.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD) || (answers.IsCorrectB && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC) || (answers.IsCorrectC && answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)))
                                    {

                                        //AB, AC, AD, BC, BD, and CD.
                                        if
                                       (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectB == questionDetail.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectB && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectA == questionDetail.IsCorrectA && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectC && !answers.IsCorrectB)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectC == questionDetail.IsCorrectC && !answers.IsCorrectA && !answers.IsCorrectD)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectB == questionDetail.IsCorrectB && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectC)
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if (answers.IsCorrectC == questionDetail.IsCorrectC && answers.IsCorrectD == questionDetail.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB)
                                        {

                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.Mark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else if ((answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectB && !answers.IsCorrectA && !answers.IsCorrectC && !answers.IsCorrectD) || (answers.IsCorrectC && !answers.IsCorrectA && !questionDetail.IsCorrectB && !answers.IsCorrectD) || (answers.IsCorrectD && !answers.IsCorrectA && !answers.IsCorrectB && !answers.IsCorrectC))
                                    {
                                        #region 1 Option 
                                        if ((questionDetail.IsCorrectA && answers.IsCorrectA))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectB && answers.IsCorrectB))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectC && answers.IsCorrectC))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else if ((questionDetail.IsCorrectD && answers.IsCorrectD))
                                        {
                                            isPartialFourCorrect = true;
                                            isCorrect = true;
                                            var assignedMark = questionDetail.PartialOneCorrectMark;
                                            totalMark += assignedMark;
                                            correctCount++;
                                        }
                                        else
                                        {
                                            var assignedMark = (questionDetail.NegativeMark);
                                            totalNegativeMark += assignedMark;
                                            inCorrectCount++;
                                        }

                                    }
                                    else
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;
                                    }
                                }
                                //A,B,C,D
                                //If one correct option and one option are choosen 
                                else if ((questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectB && !questionDetail.IsCorrectA && !questionDetail.IsCorrectC && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectC && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectD) || (questionDetail.IsCorrectD && !questionDetail.IsCorrectA && !questionDetail.IsCorrectB && !questionDetail.IsCorrectC))
                                {
                                    if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                                    {
                                        isCorrect = true;
                                        var assignedMark = questionDetail.Mark;
                                        totalMark += assignedMark;
                                        correctCount++;
                                    }
                                    if (!isCorrect)
                                    {
                                        var assignedMark = (questionDetail.NegativeMark);
                                        totalNegativeMark += assignedMark;
                                        inCorrectCount++;

                                    }
                                }
                                #endregion

                                #endregion 2 option 

                            }
                            else
                            {
                                if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                                {
                                    isCorrect = true;
                                    var assignedMark = questionDetail.Mark;
                                    totalMark += assignedMark;
                                    correctCount++;
                                }
                                if (!isCorrect)
                                {
                                    var assignedMark = (questionDetail.NegativeMark);
                                    totalNegativeMark += assignedMark;
                                    inCorrectCount++;

                                }
                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.Phrases)
                        {
                            if (questionDetail.IsCorrectA == answers.IsCorrectA && questionDetail.IsCorrectB == answers.IsCorrectB && questionDetail.IsCorrectC == answers.IsCorrectC && questionDetail.IsCorrectD == answers.IsCorrectD)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                        if (questionDetail != null && questionDetail.QuestionType == QuestionType.IntegerType)
                        {
                            string output = Regex.Replace(questionDetail.OptionA, "<.*?>", "");

                            if (output == answers.StudentAnswer)
                            {
                                isCorrect = true;
                                var assignedMark = questionDetail.Mark;
                                totalMark += assignedMark;
                                correctCount++;
                            }

                            if (!isCorrect)
                            {
                                var assignedMark = (questionDetail.NegativeMark);
                                totalNegativeMark += assignedMark;
                                inCorrectCount++;

                            }
                        }
                    }
                    subjectWise.TotalSubjectQuestion = mockTestSubjectQuestionDetails.Count;
                    subjectWise.Correct = correctCount;
                    subjectWise.InCorrect = inCorrectCount;
                    subjectWise.Skipped = (subjectWise.TotalSubjectQuestion - answerList.Count);
                    int finalMarks = totalMark - totalNegativeMark;
                    subjectWise.Score = finalMarks;

                    DM.StudentResult studentResult = new()
                    {
                        MockTestId = result.MockTestId,
                        UniqueMockTetId = result.UniqueMockTestId,
                        StudentId = result.UserId,
                        SubjectId = studentAnswer.SubjectId,
                        CorrectAnswer = subjectWise.Correct,
                        InCorrectAnswer = subjectWise.InCorrect,
                        SkippedAnswer = subjectWise.Skipped,
                        TotalMarks = subjectTotalMarks,
                        ObtainMarks = finalMarks,
                        TotalQuestion = subjectWise.TotalSubjectQuestion,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = result.UserId,
                        IsCustom = true
                    };
                    await _unitOfWork.Repository<DM.StudentResult>().Insert(studentResult);
                    SubjectWisePermormance.Add(subjectWise);

                }
                studentFinalResult.SubjectWisePermormance = SubjectWisePermormance;
                studentFinalResult.OverAllCorrect = SubjectWisePermormance.Sum(x => x.Correct);
                studentFinalResult.OverAllInCorrect = SubjectWisePermormance.Sum(x => x.InCorrect);
                studentFinalResult.OverAllSkipped = studentFinalResult.TotalQuestion - studentResults.Count;
                studentFinalResult.Score = SubjectWisePermormance.Sum(x => x.Score);
                return result.UniqueMockTestId;
            }
        }
        public async Task<Res.StudentResults> GetStudentResults(Req.GetResult result)
        {
            Res.StudentResults studentResults = new();

            if (!result.IsCustome)
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == result.MockTestId && !x.IsDeleted);

                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;

                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == result.MockTestId && x.UniqueMockTetId == result.UniqueMockTestId && !x.IsDeleted);

                studentResults.MockTestId = result.MockTestId;
                studentResults.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                studentResults.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                studentResults.Duration = mockTestDetails != null ? mockTestDetails.TimeSettingDuration : TimeSpan.Zero;
                List<Res.SubjectWisePermormance> subjecttList = new();
                foreach (var studentResult in studentResultList.DistinctBy(x => x.SubjectId))
                {


                    Res.SubjectWisePermormance student = new();
                    student.SubjectId = studentResult.SubjectId;
                    var subjectDetail = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == studentResult.SubjectId);
                    student.SubjectName = subjectDetail != null ? subjectDetail.SubjectName : "";
                    student.Correct = studentResult.CorrectAnswer;
                    student.InCorrect = studentResult.InCorrectAnswer;
                    student.Skipped = studentResult.SkippedAnswer;
                    student.Score = studentResult.ObtainMarks;
                    student.TotalMarks = studentResult.TotalMarks;
                    student.TotalSubjectQuestion = studentResult.TotalQuestion;
                    subjecttList.Add(student);

                }
                studentResults.SubjectWisePermormance = subjecttList;
                studentResults.TotalMarks = totalMarks;
                studentResults.TotalQuestion = totalQuestionCount;
                studentResults.Score = subjecttList.Sum(x => x.Score);
                studentResults.OverAllCorrect = subjecttList.Sum(x => x.Correct);
                studentResults.OverAllInCorrect = subjecttList.Sum(x => x.InCorrect);
                studentResults.OverAllSkipped = subjecttList.Sum(x => x.Skipped);
                return studentResults;
            }
            else
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == result.MockTestId && !x.IsDeleted);

                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;

                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == result.MockTestId && x.UniqueMockTetId == result.UniqueMockTestId && !x.IsDeleted);

                studentResults.MockTestId = result.MockTestId;
                studentResults.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                studentResults.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                studentResults.Duration = mockTestDetails != null ? mockTestDetails.TimeDuration : TimeSpan.Zero;
                List<Res.SubjectWisePermormance> subjecttList = new();
                foreach (var studentResult in studentResultList.DistinctBy(x => x.SubjectId))
                {


                    Res.SubjectWisePermormance student = new();
                    student.SubjectId = studentResult.SubjectId;
                    var subjectDetail = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == studentResult.SubjectId);
                    student.SubjectName = subjectDetail != null ? subjectDetail.SubjectName : "";
                    student.Correct = studentResult.CorrectAnswer;
                    student.InCorrect = studentResult.InCorrectAnswer;
                    student.Skipped = studentResult.SkippedAnswer;
                    student.Score = studentResult.ObtainMarks;
                    student.TotalMarks = studentResult.TotalMarks;
                    student.TotalSubjectQuestion = studentResult.TotalQuestion;
                    subjecttList.Add(student);

                }
                studentResults.SubjectWisePermormance = subjecttList;
                studentResults.TotalMarks = totalMarks;
                studentResults.TotalQuestion = totalQuestionCount;
                studentResults.Score = subjecttList.Sum(x => x.Score);
                studentResults.OverAllCorrect = subjecttList.Sum(x => x.Correct);
                studentResults.OverAllInCorrect = subjecttList.Sum(x => x.InCorrect);
                studentResults.OverAllSkipped = subjecttList.Sum(x => x.Skipped);
                return studentResults;
            }
        }
        public async Task<Res.StudentResultList> GetStudentPreviousResults(Req.GetStudentResult result)
        {
            Res.StudentResultList studentResultListing = new()
            {
                StudentResults = new()
            };

            List<Res.StudentResults> studentResultListV2 = new();

            if (!result.IsCustome)
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == result.MockTestId && !x.IsDeleted);
                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;
                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == result.MockTestId && x.StudentId == result.UserId && !x.IsDeleted);
                foreach (var studentResult in studentResultList.DistinctBy(x => x.UniqueMockTetId))
                {
                    Res.StudentResults studentResults = new();
                    studentResults.MockTestId = result.MockTestId;
                    studentResults.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                    studentResults.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                    studentResults.Duration = mockTestDetails != null ? mockTestDetails.TimeSettingDuration : TimeSpan.Zero;


                    List<Res.SubjectWisePermormance> subjecttList = new();
                    var studentResultListV1 = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.UniqueMockTetId == studentResult.UniqueMockTetId && x.MockTestId == result.MockTestId && !x.IsDeleted);
                    foreach (var item in studentResultListV1.DistinctBy(x => x.SubjectId))
                    {
                        Res.SubjectWisePermormance student = new();
                        student.SubjectId = item.SubjectId;
                        var subjectDetail = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId);
                        student.SubjectName = subjectDetail != null ? subjectDetail.SubjectName : "";
                        student.Correct = item.CorrectAnswer;
                        student.InCorrect = item.InCorrectAnswer;
                        student.Skipped = item.SkippedAnswer;
                        student.TotalMarks = item.TotalMarks;
                        student.Score = item.ObtainMarks;
                        student.TotalSubjectQuestion = item.TotalQuestion;
                        subjecttList.Add(student);
                    }
                    studentResults.SubjectWisePermormance = subjecttList;
                    studentResults.TotalMarks = totalMarks;
                    studentResults.TotalQuestion = totalQuestionCount;
                    studentResults.OverAllCorrect = subjecttList.Sum(x => x.Correct);
                    studentResults.OverAllInCorrect = subjecttList.Sum(x => x.InCorrect);
                    studentResults.OverAllSkipped = subjecttList.Sum(x => x.Skipped);
                    studentResults.Score = subjecttList.Sum(x => x.Score);
                    double percentage = 0;
                    if (studentResult.TotalMarks > 0)
                    {
                        percentage = (studentResults.Score / studentResults.TotalMarks) * 100;
                    }
                    studentResults.Result = percentage >= 33 ? "Pass" : "Fail";

                    #region Calculate rank 

                    ApplicationUser userInfo = await _userManager.FindByIdAsync(result.UserId.ToString());
                    var instituteId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
                    var studentSubcourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == result.UserId && !x.IsDeleted);
                    var subcorseId = studentSubcourse != null ? studentSubcourse.SubCourseId : Guid.Empty;
                    Res.StudentMockTestWiseResultAnalysis resultAnalysis = new();
                    var studentResultInfo = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == result.UserId && x.MockTestId == result.MockTestId && !x.IsDeleted && !x.IsCustom);
                    resultAnalysis.Rank = 1;
                    resultAnalysis.Name = userInfo != null ? userInfo.DisplayName : string.Empty;
                    resultAnalysis.TotalMarks = studentResultInfo.Sum(x => x.TotalMarks);
                    resultAnalysis.TotalObtainedMarks = studentResultInfo.Sum(x => x.ObtainMarks);

                    resultAnalysis.AveragePercentage = Math.Round(percentage, 2);
                    var rankScores = await _unitOfWork.Repository<DM.StudentMocktestRank>().Get(x => x.SubCourseId == subcorseId && x.InstituteId == instituteId && x.MockTestId == result.MockTestId && !x.IsDeleted);
                    var ransScoresArray = rankScores.Select(x => x.TotalObtainMark).ToList();
                    double[] scores = ransScoresArray.ToArray();
                    int rank = 1;

                    Array.Sort(scores);
                    Array.Reverse(scores);

                    double currentScore = resultAnalysis.TotalObtainedMarks;

                    foreach (int score in scores)
                    {
                        if (score > currentScore)
                        {
                            rank++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    var isStudentExist = await _unitOfWork.Repository<DM.StudentMocktestRank>().GetSingle(x => x.StudentId == result.UserId && x.MockTestId == result.MockTestId && x.SubCourseId == subcorseId && x.InstituteId == instituteId);
                    if (isStudentExist == null)
                    {
                        DM.StudentMocktestRank studentRank = new()
                        {
                            MockTestId = result.MockTestId,
                            AveragePercentage = resultAnalysis.AveragePercentage,
                            TotalMark = totalMarks,
                            TotalObtainMark = resultAnalysis.TotalObtainedMarks,
                            StudentId = result.UserId,
                            SubCourseId = subcorseId,
                            InstituteId = instituteId,
                            Rank = rank,
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = result.UserId,
                            IsActive = true,
                            IsDeleted = false
                        };
                        await _unitOfWork.Repository<DM.StudentMocktestRank>().Insert(studentRank);
                    }
                    else
                    {
                        isStudentExist.MockTestId = result.MockTestId;
                        isStudentExist.AveragePercentage = resultAnalysis.AveragePercentage;
                        isStudentExist.TotalMark = totalMarks;
                        isStudentExist.TotalObtainMark = resultAnalysis.TotalObtainedMarks;
                        isStudentExist.StudentId = result.UserId;
                        isStudentExist.SubCourseId = subcorseId;
                        isStudentExist.InstituteId = instituteId;
                        isStudentExist.Rank = rank;
                        isStudentExist.LastModifierUserId = result.UserId;
                        isStudentExist.LastModifyDate = DateTime.UtcNow;
                        await _unitOfWork.Repository<DM.StudentMocktestRank>().Update(isStudentExist);
                    }

                    studentResults.Rank = rank;
                    #endregion
                    studentResultListV2.Add(studentResults);
                }
                studentResultListing.StudentResults = studentResultListV2;
                studentResultListing.TotalRecords = studentResultListV2.Count;
                return studentResultListing;
            }
            else
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == result.MockTestId && !x.IsDeleted);

                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == result.MockTestId && !x.IsDeleted);

                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                var totalQuestionCount = mockTestQuestionDetails.Count;

                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == result.MockTestId && !x.IsDeleted);

                foreach (var studentResult in studentResultList.DistinctBy(x => x.UniqueMockTetId))
                {
                    Res.StudentResults studentResults = new();
                    studentResults.MockTestId = mockTestDetails != null ? mockTestDetails.Id : Guid.Empty;
                    studentResults.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : "";
                    studentResults.Duration = mockTestDetails != null ? mockTestDetails.TimeDuration : TimeSpan.Zero;

                    List<Res.SubjectWisePermormance> subjecttList = new();
                    var studentResultListV1 = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.UniqueMockTetId == studentResult.UniqueMockTetId && x.MockTestId == result.MockTestId && !x.IsDeleted);
                    foreach (var item in studentResultListV1.DistinctBy(x => x.SubjectId))
                    {
                        Res.SubjectWisePermormance student = new();
                        student.SubjectId = item.SubjectId;
                        var subjectDetail = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId);
                        student.SubjectName = subjectDetail != null ? subjectDetail.SubjectName : "";
                        student.Correct = item.CorrectAnswer;
                        student.InCorrect = item.InCorrectAnswer;
                        student.Skipped = item.SkippedAnswer;
                        student.TotalSubjectQuestion = item.TotalQuestion;
                        student.TotalMarks = item.TotalMarks;
                        student.Score = item.ObtainMarks;
                        subjecttList.Add(student);
                    }
                    studentResults.SubjectWisePermormance = subjecttList;
                    studentResults.TotalMarks = totalMarks;
                    studentResults.TotalQuestion = totalQuestionCount;
                    studentResults.Score = subjecttList.Sum(x => x.Score);
                    studentResults.OverAllCorrect = subjecttList.Sum(x => x.Correct);
                    studentResults.OverAllInCorrect = subjecttList.Sum(x => x.InCorrect);
                    studentResults.OverAllSkipped = subjecttList.Sum(x => x.Skipped);
                    studentResultListV2.Add(studentResults);
                }
                studentResultListing.StudentResults = studentResultListV2;
                studentResultListing.TotalRecords = studentResultListV2.Count;
                return studentResultListing;
            }
        }
        #endregion
        #endregion
        #endregion
        #endregion
        #region Student MockTest Solution
        public async Task<Res.StudentQuestionSolution?> GetStudentQuestionSolution(Req.GetStudentQuestionSolution mockTest)
        {
            if (!mockTest.IsCustome)
            {
                Res.StudentQuestionSolution mockTestQuestionsList = new()
                {
                    MocktestPanelList = new()
                };
                var mockTestSetting = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == mockTest.MockTestId && !x.IsDeleted);
                if (mockTestSetting != null)
                {
                    mockTestQuestionsList.MockTestId = mockTestSetting.Id;
                    mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;

                    var MockTestQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && !x.IsDeleted);
                    List<Res.StudentMocktestPanelListSolution> mockTest1 = new();
                    if (MockTestQuestions.Any())
                    {
                        foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                        {
                            var isSubjectExist = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId && !x.IsDeleted);
                            if (isSubjectExist != null)
                            {
                                Res.StudentMocktestPanelListSolution mockTestQuestionss = new();
                                mockTestQuestionss.SubjectId = item.SubjectId;
                                mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                                var MockTestQuesSection = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                var sectionCount = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == mockTestSetting.ExamPatternId && x.SubjectId == item.SubjectId && !x.IsDeleted);

                                mockTestQuestionss.SectionCount = sectionCount != null ? sectionCount.Count : 0;

                                foreach (var it in MockTestQuesSection.DistinctBy(x => x.SectionId))
                                {

                                    Res.SubjectwiseSectionSolution sectionDetails = new();
                                    sectionDetails.SectionId = it.SectionId;
                                    sectionDetails.SectionName = await _examPatternRepository.GetSectionName(it.SectionId);
                                    var sectionDetailsss = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == it.SectionId && x.ExamPatternId == mockTestSetting.ExamPatternId);
                                    var SectionQuestions = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == mockTestSetting.Id && x.SubjectId == item.SubjectId && x.SectionId == it.SectionId && !x.IsDeleted);
                                    List<Res.StudentMockTestQuestionsSolution> mockTestQuestionsLists = new();
                                    foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                                    {
                                        Res.StudentMockTestQuestionsSolution questinBanks = new();
                                        var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == i.QuestionRefId && !x.IsDeleted);
                                        if (questions.Any())
                                        {

                                            questinBanks.QuestionType = questions.First().QuestionType;
                                            questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                                            questinBanks.QuestionRefId = questions.First().QuestionRefId;
                                            foreach (var items in questions)
                                            {
                                                if (items.QuestionLanguage == QuestionLanguage.English.ToString())
                                                {
                                                    Res.StudentMockEnglishSolution english = new();
                                                    english.Id = items.Id;
                                                    english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                    english.Explanation = items.Explanation;
                                                    english.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.English = english;
                                                }
                                                if (items.QuestionLanguage == QuestionLanguage.Hindi.ToString())
                                                {
                                                    Res.StudentMockHindiSolution hindi = new();
                                                    hindi.Id = items.Id;
                                                    hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                    hindi.Explanation = items.Explanation;
                                                    hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Hindi = hindi;
                                                }
                                                if (items.QuestionLanguage == QuestionLanguage.Gujarati.ToString())
                                                {
                                                    Res.StudentMockGujaratiSolution Gujarati = new();
                                                    Gujarati.Id = items.Id;
                                                    Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                    Gujarati.Explanation = items.Explanation;
                                                    Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Gujarati = Gujarati;
                                                }
                                                if (items.QuestionLanguage == QuestionLanguage.Marathi.ToString())
                                                {
                                                    Res.StudentMockMarathiSolution marathi = new();
                                                    marathi.Id = items.Id;
                                                    marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                    marathi.Explanation = items.Explanation;
                                                    marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                                    questinBanks.QuestionTableData.Marathi = marathi;

                                                }
                                            }
                                            mockTestQuestionsLists.Add(questinBanks);
                                        }
                                        sectionDetails.MockTestQuestions = mockTestQuestionsLists;
                                        mockTestQuestionss.NoOfQue++;

                                    }
                                    mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                                    mockTestQuestionss.SubjectwiseSection.Add(sectionDetails);
                                }
                                mockTest1.Add(mockTestQuestionss);

                                mockTestQuestionsList.MocktestPanelList = mockTest1;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return mockTestQuestionsList;
                }
                return null;
            }
            else
            {
                Res.StudentQuestionSolution mockTestQuestionsList = new()
                {
                    MocktestPanelList = new()
                };
                var mockTestSetting = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == mockTest.MockTestId && !x.IsDeleted);
                if (mockTestSetting != null)
                {
                    var mockTestStatusInfo = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == mockTest.MockTestId && x.StudentId == mockTest.UserId && x.IsStarted && !x.IsCompleted);
                    mockTestQuestionsList.MockTestId = mockTestSetting.Id;
                    mockTestQuestionsList.MockTestName = mockTestSetting.MockTestName;
                    var MockTestQuestions = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && !x.IsDeleted);
                    List<Res.StudentMocktestPanelListSolution> mockTest1 = new();
                    if (MockTestQuestions.Any())
                    {
                        foreach (var item in MockTestQuestions.DistinctBy(x => x.SubjectId))
                        {
                            var isSubjectExist = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId && !x.IsDeleted);
                            if (isSubjectExist != null)
                            {
                                Res.StudentMocktestPanelListSolution mockTestQuestionss = new();
                                mockTestQuestionss.SubjectId = item.SubjectId;
                                mockTestQuestionss.SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId);
                                var MockTestQuesSection = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                mockTestQuestionss.SectionCount = 1;

                                Res.SubjectwiseSectionSolution sectionDetails = new();
                                sectionDetails.SectionId = Guid.NewGuid();
                                sectionDetails.SectionName = "Section-1";
                                var SectionQuestions = await _unitOfWork.Repository<DM.StudentMockTestQuestions>().Get(x => x.StudentMockTestId == mockTestSetting.Id && x.SubjectId == item.SubjectId && !x.IsDeleted);
                                List<Res.StudentMockTestQuestionsSolution> mockTestQuestionsLists = new();
                                foreach (var i in SectionQuestions.DistinctBy(x => x.QuestionRefId))
                                {
                                    Res.StudentMockTestQuestionsSolution questinBanks = new();
                                    var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == i.QuestionRefId && !x.IsDeleted);
                                    if (questions.Any())
                                    {
                                        questinBanks.QuestionType = questions.First().QuestionType;
                                        questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                                        questinBanks.QuestionRefId = questions.First().QuestionRefId;
                                        foreach (var items in questions)
                                        {
                                            if (items.QuestionLanguage == QuestionLanguage.English.ToString())
                                            {
                                                Res.StudentMockEnglishSolution english = new();
                                                english.Id = items.Id;
                                                english.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in english.";
                                                english.Explanation = items.Explanation;
                                                english.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.English = english;
                                            }
                                            if (items.QuestionLanguage == QuestionLanguage.Hindi.ToString())
                                            {
                                                Res.StudentMockHindiSolution hindi = new();
                                                hindi.Id = items.Id;
                                                hindi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in hindi.";
                                                hindi.Explanation = items.Explanation;
                                                hindi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Hindi = hindi;
                                            }
                                            if (items.QuestionLanguage == QuestionLanguage.Gujarati.ToString())
                                            {
                                                Res.StudentMockGujaratiSolution Gujarati = new();
                                                Gujarati.Id = items.Id;
                                                Gujarati.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in gujarati.";
                                                Gujarati.Explanation = items.Explanation;
                                                Gujarati.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Gujarati = Gujarati;
                                            }
                                            if (items.QuestionLanguage == QuestionLanguage.Marathi.ToString())
                                            {
                                                Res.StudentMockMarathiSolution marathi = new();
                                                marathi.Id = items.Id;
                                                marathi.QuestionText = items.QuestionText != "" ? items.QuestionText : "Question is not available in marathi.";
                                                marathi.Explanation = items.Explanation;
                                                marathi.IsAvailable = items.QuestionText != "" ? true : false;
                                                questinBanks.QuestionTableData.Marathi = marathi;

                                            }
                                        }
                                        mockTestQuestionsLists.Add(questinBanks);
                                    }
                                    sectionDetails.MockTestQuestions = mockTestQuestionsLists;
                                    mockTestQuestionss.NoOfQue++;

                                }
                                mockTestQuestionss.NoOfQue = mockTestQuestionss.NoOfQue;
                                mockTestQuestionss.SubjectwiseSection.Add(sectionDetails);
                                mockTest1.Add(mockTestQuestionss);
                                mockTestQuestionsList.MocktestPanelList = mockTest1;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return mockTestQuestionsList;
                }
                return null;


            }
        }
        #endregion

    }

}


