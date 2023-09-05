using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using static OnlinePractice.API.Controllers.StripeController;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class MyPurchasedRespository : IMyPurchasedRespository
    {
        private readonly IUnitOfWork _unitOfWork;
        public MyPurchasedRespository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateMyPurchased(Req.CreateMyPurchased createMyPurchased)
        {
            if (createMyPurchased != null)
            {
                DM.MyPurchased purchased = new()
                {
                    ProductCategory = createMyPurchased.ProductCategory,
                    ProductId = createMyPurchased.ProductId,
                    Price = createMyPurchased.Price,
                    StudentId = createMyPurchased.StudentId,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createMyPurchased.UserId,
                };
                await _unitOfWork.Repository<DM.MyPurchased>().Insert(purchased);
                return true;
            }
            return false;
        }
        public Res.StudentModulesList? GetStudentModules(CurrentUser currentUser)
        {
            Res.StudentModulesList modulesList = new();
            var studentData = new List<StudentModules>();
            studentData.Add(new StudentModules()
            {
                ModuleCategoryName = "Mock Test"
            });
            studentData.Add(new StudentModules()
            {
                ModuleCategoryName = "Videos"
            });
            studentData.Add(new StudentModules()
            {
                ModuleCategoryName = "Previous Year Paper"
            });
            studentData.Add(new StudentModules()
            {
                ModuleCategoryName = "eBooks"
            });
            modulesList.studentModules = studentData.ToList();
            return modulesList;
        }
        public async Task<Res.MyPurchasedMockTestsLists?> GetPurchasedMocktest(Req.MyPurchasedMocktest purchasedMocktest)
        {
            Res.MyPurchasedMockTestsLists myPurchasedMockTestsLists = new();
            List<DM.MockTestSettings> mockTestNewList = new();
            var mocktestData = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductCategory == ProductCategory.MockTest &&
            x.StudentId == purchasedMocktest.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate));
            var mocktestIds = mocktestData.Select(x => x.ProductId).ToList();
            var mocktestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => mocktestIds.Contains(x.Id));
            mockTestNewList = mocktestDetails.ToList();
            if (mockTestNewList.Any())
            {
                foreach (var item in mockTestNewList)
                {
                    var mockTestInprogess = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == purchasedMocktest.UserId && !x.IsDeleted && x.IsStarted && !x.IsCompleted);
                    var mockTestComplete = await _unitOfWork.Repository<DM.StudentMockTestStatus>().GetSingle(x => x.MockTestId == item.Id && x.StudentId == purchasedMocktest.UserId && !x.IsDeleted && x.IsStarted && x.IsCompleted);
                    var adminMocktest = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == item.Id && !x.IsDeleted);
                    var totalAttempsRemaining = adminMocktest != null ? adminMocktest.TotalAttempts : 0;
                    var remainingtimeDuration = adminMocktest != null ? adminMocktest.TimeSettingDuration : TimeSpan.Zero;
                    var purchaseDetail = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == item.Id && x.ProductCategory == ProductCategory.MockTest &&
x.StudentId == purchasedMocktest.UserId && !x.IsDeleted);
                    Res.MyPurchasedMockTests studentMockTest = new();
                    studentMockTest.MockTestId = item.Id;
                    studentMockTest.MocktestName = item.MockTestName;
                    studentMockTest.IsRetake = item.IsAllowReattempts;
                    studentMockTest.TestAvailability = item.TestAvailability.ToString();
                    studentMockTest.TotalAttempts = item.TotalAttempts;
                    var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == item.Id && !x.IsDeleted);
                    var attemptsCount = studentResult.DistinctBy(x => x.UniqueMockTetId).Count();
                    studentMockTest.AlreadyResultGenerated = attemptsCount;

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
                    studentMockTest.PurchaseDate = purchaseDetail != null ? purchaseDetail.CreationDate : null;
                    var priceCheck = item.IsFree && item.Price == 0 ? 0 : item.Price;

                    studentMockTest.Price = priceCheck;
                    if (mockTestComplete != null)
                    {
                        studentMockTest.Status = StatusFilter.Completed.ToString();
                    }
                    else if (mockTestInprogess != null && item.TestAvailability == TestAvailability.Specific)
                    {
                        studentMockTest.Status = StatusFilter.InProgress.ToString();
                    }
                    else if (mockTestInprogess != null && item.TestAvailability == TestAvailability.Always)
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
                        studentMockTest.StartsInDays = item.TestSpecificFromDate;
                        studentMockTest.StartsInTime = item.TestStartTime;
                    }
                    myPurchasedMockTestsLists.MyPurchasedMockTests.Add(studentMockTest);
                }
                var resultInfo = myPurchasedMockTestsLists.MyPurchasedMockTests.OrderByDescending(x=>x.PurchaseDate).ToList();
                var total = resultInfo.Count();
                var final = resultInfo.Page(purchasedMocktest.PageNumber, purchasedMocktest.PageSize);
                myPurchasedMockTestsLists.MyPurchasedMockTests = final.ToList();
                myPurchasedMockTestsLists.TotalRecords = total;
                return myPurchasedMockTestsLists;
            }
            var count = myPurchasedMockTestsLists.MyPurchasedMockTests.Count();
            var result = myPurchasedMockTestsLists.MyPurchasedMockTests.Page(purchasedMocktest.PageNumber, purchasedMocktest.PageSize);
            myPurchasedMockTestsLists.MyPurchasedMockTests = result.ToList();
            myPurchasedMockTestsLists.TotalRecords = count;
            if (myPurchasedMockTestsLists.MyPurchasedMockTests.Any())
            {
                return myPurchasedMockTestsLists;
            }
            return null;
        }
        public async Task<Res.MyPurchasedEbooksLists?> GetPurchasedEbooks(Req.MyPurchasedEbook purchasedEbook)
        {
            Res.MyPurchasedEbooksLists myPurchasedEbooksLists = new();
            List<DM.Ebook> ebookNewLists = new();
            if (purchasedEbook != null)
            {
                var ebookData = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductCategory == ProductCategory.eBook &&
                     x.StudentId == purchasedEbook.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                var ebookIds = ebookData.Select(x => x.ProductId).ToList();
                var ebookDetails = await _unitOfWork.Repository<DM.Ebook>().Get(x => ebookIds.Contains(x.Id));
                ebookNewLists = ebookDetails.ToList();
                if (ebookNewLists.Any())
                {
                    foreach (var item in ebookNewLists)
                    {
                        var ebookIdCheck = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == item.Id && !x.IsDeleted);
                        var purchaseDetail = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == item.Id && x.ProductCategory == ProductCategory.eBook &&
x.StudentId == purchasedEbook.UserId && !x.IsDeleted);
                        if (ebookIdCheck != null && purchaseDetail != null)
                        {
                            Res.MyPurchasedEbooks myPurchasedEbooks = new();
                            myPurchasedEbooks.EbookId = ebookIdCheck.Id;
                            myPurchasedEbooks.EbookTitle = ebookIdCheck.EbookTitle;
                            myPurchasedEbooks.EbookThumbnail = ebookIdCheck.EbookThumbnail;
                            myPurchasedEbooks.Language = ebookIdCheck.Language;
                            myPurchasedEbooks.EbookPdfURL = ebookIdCheck.EbookPdfUrl;
                            myPurchasedEbooks.Price = ebookIdCheck.Price;
                            var subjectCategoryId = ebookIdCheck.SubjectCategoryId;
                            var subjectCategoryDetails = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == subjectCategoryId && !x.IsDeleted);
                            if (subjectCategoryDetails != null)
                            {
                                var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectCategoryDetails.SubjectId && !x.IsDeleted);
                                myPurchasedEbooks.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : string.Empty;
                            }
                            var topicId = ebookIdCheck.TopicId != Guid.Empty ? ebookIdCheck.TopicId : Guid.Empty;
                            var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topicId && !x.IsDeleted);
                            if (topicDetails != null)
                            {
                                myPurchasedEbooks.TopicName = topicDetails.TopicName;
                            }
                            myPurchasedEbooks.PurchaseDate = purchaseDetail.CreationDate;
                            myPurchasedEbooksLists.MyPurchasedEbooks.Add(myPurchasedEbooks);
                        }
                    }
                    var resultInfo = myPurchasedEbooksLists.MyPurchasedEbooks.OrderByDescending(x => x.PurchaseDate).ToList();

                    var count = resultInfo.Count();
                    var result = resultInfo.Page(purchasedEbook.PageNumber, purchasedEbook.PageSize);
                    myPurchasedEbooksLists.MyPurchasedEbooks = result.ToList();
                    myPurchasedEbooksLists.TotalRecords = count;
                    if (myPurchasedEbooksLists.MyPurchasedEbooks.Any())
                    {
                        return myPurchasedEbooksLists;
                    }
                }
                return null;
            }
            return null;

        }
        public async Task<Res.MyPurchasedVideosLists?> GetPurchasedVideos(Req.MyPurchasedVideo purchasedVideo)
        {
            Res.MyPurchasedVideosLists myPurchasedVideosLists = new();
            List<DM.Video> videoNewLists = new();
            var videoData = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductCategory == ProductCategory.Video &&
                 x.StudentId == purchasedVideo.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate));
            var videosIds = videoData.Select(x => x.ProductId).ToList();
            var videoDetails = await _unitOfWork.Repository<DM.Video>().Get(x => videosIds.Contains(x.Id));
            videoNewLists = videoDetails.ToList();
            if (videoNewLists.Any())
            {
                foreach (var item in videoNewLists)
                {
                    var videoIdCheck = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == item.Id && !x.IsDeleted);
                    var purchaseDetail = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == item.Id && x.ProductCategory == ProductCategory.Video &&
x.StudentId == purchasedVideo.UserId && !x.IsDeleted);
                    if (videoIdCheck != null && purchaseDetail != null)
                    {

                        Res.MyPurchasedVideos myPurchasedVideos1 = new();
                        myPurchasedVideos1.VideoId = videoIdCheck.Id;
                        myPurchasedVideos1.VideoTitle = videoIdCheck.VideoTitle;
                        myPurchasedVideos1.VideoThumbnail = videoIdCheck.VideoThumbnail;
                        myPurchasedVideos1.Language = videoIdCheck.Language;
                        myPurchasedVideos1.VideoURL = videoIdCheck.VideoUrl;
                        myPurchasedVideos1.Price = videoIdCheck.Price;
                        var subjectCategoryId = videoIdCheck.SubjectCategoryId != Guid.Empty ? videoIdCheck.SubjectCategoryId : Guid.Empty;
                        var subjectCategoryDetails = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == subjectCategoryId && !x.IsDeleted);
                        if (subjectCategoryDetails != null)
                        {
                            var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectCategoryDetails.SubjectId && !x.IsDeleted);
                            myPurchasedVideos1.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : string.Empty;
                        }
                        var topicId = videoIdCheck.TopicId != Guid.Empty ? videoIdCheck.TopicId : Guid.Empty;
                        var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topicId && !x.IsDeleted);
                        if (topicDetails != null)
                        {
                            myPurchasedVideos1.TopicName = topicDetails.TopicName;
                        }
                        myPurchasedVideos1.PurchaseDate = purchaseDetail.CreationDate;
                        myPurchasedVideosLists.MyPurchasedVideos.Add(myPurchasedVideos1);
                    }
                }
                var resultInfo = myPurchasedVideosLists.MyPurchasedVideos.OrderByDescending(x => x.PurchaseDate).ToList();
                var count = resultInfo.Count();
                var result = resultInfo.Page(purchasedVideo.PageNumber, purchasedVideo.PageSize);
                myPurchasedVideosLists.MyPurchasedVideos = result.ToList();
                myPurchasedVideosLists.TotalRecords = count;
                if (myPurchasedVideosLists.MyPurchasedVideos.Any())
                {
                    return myPurchasedVideosLists;
                }
            }
            return null;
        }
        public async Task<Res.MyPurchasedPYPLists?> GetPurchasedPreviousYearPaper(Req.MyPurchasedPreviousYearPAper purchasedPreviousYearPAper)
        {
            Res.MyPurchasedPYPLists myPurchasedPYPLists = new();
            List<DM.PreviousYearPaper> previousYearPaperNewLists = new();
            var pypData = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductCategory == ProductCategory.PreviouseYearPaper &&
                x.StudentId == purchasedPreviousYearPAper.UserId && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate));
            var pypIds = pypData.Select(x => x.ProductId).ToList();
            var pypDetails = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => pypIds.Contains(x.Id));
            previousYearPaperNewLists = pypDetails.ToList();
            if (previousYearPaperNewLists.Any())
            {
                foreach (var item in previousYearPaperNewLists)
                {
                    var paperIdCheck = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == item.Id && !x.IsDeleted);
                    var purchaseDetail = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == item.Id && x.ProductCategory == ProductCategory.PreviouseYearPaper &&
        x.StudentId == purchasedPreviousYearPAper.UserId && !x.IsDeleted);
                    if (paperIdCheck != null && purchaseDetail != null)
                    {
                        Res.MyPurchasedPYP myPurchasedPYP1 = new();
                        myPurchasedPYP1.PaperId = paperIdCheck.Id;
                        myPurchasedPYP1.PaperTitle = paperIdCheck.PaperTitle;
                        myPurchasedPYP1.Year = paperIdCheck.Year;
                        myPurchasedPYP1.Language = paperIdCheck.Language;
                        myPurchasedPYP1.PaperPdfUrl = paperIdCheck.PaperPdfUrl;
                        myPurchasedPYP1.Price = paperIdCheck.Price;
                        myPurchasedPYP1.PurchaseDate = purchaseDetail.CreationDate;
                        myPurchasedPYPLists.MyPurchasedPYPs.Add(myPurchasedPYP1);
                    }
                }
                var resultInfo = myPurchasedPYPLists.MyPurchasedPYPs.OrderByDescending(x => x.PurchaseDate).ToList();
                var count = resultInfo.Count();
                var result = resultInfo.Page(purchasedPreviousYearPAper.PageNumber, purchasedPreviousYearPAper.PageSize);
                myPurchasedPYPLists.MyPurchasedPYPs = result.ToList();
                myPurchasedPYPLists.TotalRecords = count;
                if (myPurchasedPYPLists.MyPurchasedPYPs.Any())
                {
                    return myPurchasedPYPLists;
                }
            }
            return null;
        }
        public async Task<bool> IsPurchasedCheck(Guid Id, ProductCategory productCategory)
        {

            var isPurchasedCheck = await _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.Id == Id && x.ProductCategory == productCategory && !x.IsDeleted);
            return isPurchasedCheck != null;
        }
        public bool PurchasedCheck(Guid Id, ProductCategory productCategory, Guid studentId)
        {

            var isPurchasedCheck = _unitOfWork.Repository<DM.MyPurchased>().GetSingle(x => x.ProductId == Id && x.ProductCategory == productCategory && x.StudentId == studentId && !x.IsDeleted);
            if (isPurchasedCheck.Result != null)
            {
                return true;
            }
            return false;
        }
    }
}
