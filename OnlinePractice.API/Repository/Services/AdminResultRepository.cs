using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Response;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using System.Collections.Generic;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Services
{
    public class AdminResultRepository : IAdminResultRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminResultRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Res.AdminStudentResultList> GetAllResults(Req.GeAdminResult adminResult)
        {
            Res.AdminStudentResultList studentResultListing = new()
            {
                StudentResults = new()
            };
            if(adminResult.MockTestId == Guid.Empty)
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.SubCourseId == adminResult.SubCourseId && x.InstituteId == adminResult.InstituteId && !x.IsDeleted);
                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == adminResult.InstituteId);
                var instituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                var mockTestIds = mockTestDetails.Select(x => x.Id).ToList();
                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => mockTestIds.Contains(x.MockTestId) && !x.IsCustom, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                List<Res.AdminStudentResults> studentResultListData = new();
                foreach (var studentResult in studentResultList.DistinctBy(x => x.UniqueMockTetId))
                {
                    var userInfo = await _userManager.FindByIdAsync(studentResult.StudentId.ToString());
                    if(userInfo != null && !userInfo.IsDeleted)
                    {
                        var userName = userInfo != null ? userInfo.DisplayName : string.Empty;
                        var mockTestInfo = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == studentResult.MockTestId && !x.IsDeleted);
                        var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == studentResult.MockTestId && !x.IsDeleted);
                        var studentResultListV1 = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == studentResult.StudentId && x.MockTestId == studentResult.MockTestId && !x.IsDeleted);
                        var obtainMarks = studentResultListV1.Sum(x => x.ObtainMarks);
                        var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                        double percentage = 0;
                        var totalQuestionCount = mockTestQuestionDetails.Count;
                        if (totalMarks > 0)
                        {
                            percentage = (obtainMarks / totalMarks) * 100;
                        }
                        Res.AdminStudentResults studentResults = new();
                        studentResults.MockTestId = studentResult.MockTestId;
                        studentResults.MockTestId = mockTestInfo != null ? mockTestInfo.Id : Guid.Empty;
                        studentResults.MockTestName = mockTestInfo != null ? mockTestInfo.MockTestName : "";
                        studentResults.StudentId = studentResult.StudentId;
                        studentResults.StudentName = userName;
                        studentResults.TotalMarks = totalMarks;
                        studentResults.TotalObtainMark = obtainMarks;
                        studentResults.Percentage = Math.Round(percentage, 2);
                        studentResults.InstituteName = instituteName;
                        studentResults.Result = percentage < 33 ? "Fail" : "Pass";
                        studentResultListData.Add(studentResults);
                    }
                }
                var result = studentResultListData.Page(adminResult.PageNumber, adminResult.PageSize);
                studentResultListing.TotalRecords = studentResultListData.Count;
                studentResultListing.StudentResults = result.ToList();
                return studentResultListing;
            }
            else
            {
                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == adminResult.InstituteId);
                var instituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == adminResult.MockTestId && !x.IsCustom, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                List<Res.AdminStudentResults> studentResultListData = new();
                foreach (var studentResult in studentResultList.DistinctBy(x => x.UniqueMockTetId))
                {
                    var userInfo = await _userManager.FindByIdAsync(studentResult.StudentId.ToString());
                    if (userInfo != null && !userInfo.IsDeleted)
                    {
                        var userName = userInfo != null ? userInfo.DisplayName : string.Empty;
                        var mockTestInfo = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == studentResult.MockTestId && !x.IsDeleted);
                        var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == studentResult.MockTestId && !x.IsDeleted);
                        var studentResultListV1 = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == studentResult.StudentId && x.MockTestId == studentResult.MockTestId && !x.IsDeleted);
                        var obtainMarks = studentResultListV1.Sum(x => x.ObtainMarks);
                        var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                        double percentage = 0;
                        var totalQuestionCount = mockTestQuestionDetails.Count;
                        if (totalMarks > 0)
                        {
                            percentage = (obtainMarks / totalMarks) * 100;
                        }
                        Res.AdminStudentResults studentResults = new();
                        studentResults.MockTestId = studentResult.MockTestId;
                        studentResults.MockTestId = mockTestInfo != null ? mockTestInfo.Id : Guid.Empty;
                        studentResults.MockTestName = mockTestInfo != null ? mockTestInfo.MockTestName : "";
                        studentResults.StudentId = studentResult.StudentId;
                        studentResults.StudentName = userName;
                        studentResults.TotalMarks = totalMarks;
                        studentResults.TotalObtainMark = obtainMarks;
                        studentResults.Percentage = Math.Round(percentage, 2);
                        studentResults.InstituteName = instituteName;
                        studentResults.Result = percentage < 33 ? "Fail" : "Pass";
                        studentResultListData.Add(studentResults);
                    }
                }
                var result = studentResultListData.Page(adminResult.PageNumber, adminResult.PageSize);
                studentResultListing.TotalRecords = studentResultListData.Count;
                studentResultListing.StudentResults = result.ToList();
                return studentResultListing;
            } 
        }
        public async Task<Res.MockTestList> GetAllMockTest(Req.GetMockTestList mockTestList)
        {
            Res.MockTestList mockTestLists = new()
            {
                MockTestInfoModels = new()
            };
            var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.SubCourseId == mockTestList.SubCourseId && x.InstituteId == mockTestList.InstituteId && !x.IsDeleted, orderBy: x=>x.OrderByDescending(x=>x.CreationDate));
            List<Res.MockTestInfoModel> mockTestInfoModels = new();
            foreach (var item in mockTestDetails)
            {
                Res.MockTestInfoModel model = new();
                model.MockTestId = item.Id;
                model.MockTestName = item.MockTestName;
                mockTestInfoModels.Add(model);
            }
            mockTestLists.TotalRecords = mockTestInfoModels.Count;
            mockTestLists.MockTestInfoModels = mockTestInfoModels;
            return mockTestLists;
        }
        public async Task<Res.StudentResultDetail?> GetResultByMockTestId(Req.GeResultByMockTestId resultByMockTestId)
        {
            Res.StudentResultDetail studentResultDetail = new();
            var userInfo = await _userManager.FindByIdAsync(resultByMockTestId.StudentId.ToString());
            if (userInfo != null)
            {
                #region Add student details
                studentResultDetail.StudentId = Guid.Parse(userInfo.Id);
                studentResultDetail.StudentName = userInfo.DisplayName;
                studentResultDetail.ProfileImage = userInfo.ProfileImage;
                var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x=>x.Id == userInfo.InstituteId);
                studentResultDetail.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                studentResultDetail.InstituteCode = instituteDetail !=null ? instituteDetail.InstituteCode: string.Empty;
                var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == resultByMockTestId.StudentId && !x.IsDeleted);
                Guid subCourseId = studentsubCourse!= null ? studentsubCourse.SubCourseId: Guid.Empty;
                var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                studentResultDetail.SubCourseName =  subCourseDetail != null ? subCourseDetail.SubCourseName: string.Empty;
                Guid courseId =  subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id==courseId && !x.IsDeleted);
                studentResultDetail.CourseName = courseDetail!= null ? courseDetail.CourseName: string.Empty;
                #endregion
                #region Add mocktest details
                MocktestResultDetails mocktestResultDetails = new();
                mocktestResultDetails.Id = resultByMockTestId.MockTestId;
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == resultByMockTestId.MockTestId&& !x.IsDeleted);
                mocktestResultDetails.MockTestName = mockTestDetails != null ? mockTestDetails.MockTestName : string.Empty;
                var mockTestQuestionDetails = await _unitOfWork.Repository<DM.MockTestQuestions>().Get(x => x.MockTestSettingId == resultByMockTestId.MockTestId && !x.IsDeleted);
                var totalMarks = mockTestQuestionDetails.Sum(x => x.Marks);
                mocktestResultDetails.TotalQuestions = mockTestQuestionDetails.Count();
                mocktestResultDetails.TotalMarks = mockTestQuestionDetails.Sum(x=>x.Marks);
                mocktestResultDetails.Duration = mockTestDetails != null ? mockTestDetails.TimeSettingDuration : TimeSpan.Zero;
                studentResultDetail.MocktestResultDetails = mocktestResultDetails;
                #endregion
                #region Add result details
                ResultDetail resultDetail = new();
                var resultDetails = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == resultByMockTestId.MockTestId && x.StudentId == resultByMockTestId.StudentId);
                resultDetail.TotalObtainMark = resultDetails != null ? resultDetails.Sum(x=>x.ObtainMarks) : 0;
                double percentage = 0;
                if(mocktestResultDetails.TotalMarks > 0)
                {
                     percentage = (resultDetail.TotalObtainMark / mocktestResultDetails.TotalMarks) * 100;
                }
                #region Calculate rank 

                var instituteId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
                Res.StudentMockTestWiseResultAnalysis resultAnalysis = new();
                var studentResultInfo = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == resultByMockTestId.StudentId && x.MockTestId == resultByMockTestId.MockTestId && !x.IsDeleted && !x.IsCustom);
                resultAnalysis.Rank = 1;
                resultAnalysis.Name = userInfo != null ? userInfo.DisplayName : string.Empty;
                resultAnalysis.TotalMarks = studentResultInfo.Sum(x => x.TotalMarks);
                resultAnalysis.TotalObtainedMarks = studentResultInfo.Sum(x => x.ObtainMarks);

                resultAnalysis.AveragePercentage = Math.Round(percentage, 2);
                var rankScores = await _unitOfWork.Repository<DM.StudentMocktestRank>().Get(x => x.SubCourseId == subCourseId && x.InstituteId == instituteId && x.MockTestId == resultByMockTestId.MockTestId && !x.IsDeleted);
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
                var isStudentExist = await _unitOfWork.Repository<DM.StudentMocktestRank>().GetSingle(x => x.StudentId == resultByMockTestId.StudentId && x.MockTestId == resultByMockTestId.MockTestId && x.SubCourseId == subCourseId && x.InstituteId == instituteId);
                if (isStudentExist == null)
                {
                    DM.StudentMocktestRank studentRank = new()
                    {
                        MockTestId = resultByMockTestId.MockTestId,
                        AveragePercentage = resultAnalysis.AveragePercentage,
                        TotalMark = totalMarks,
                        TotalObtainMark = resultAnalysis.TotalObtainedMarks,
                        StudentId = resultByMockTestId.StudentId,
                        SubCourseId = subCourseId,
                        InstituteId = instituteId,
                        Rank = rank,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = resultByMockTestId.UserId,
                        IsActive = true,
                        IsDeleted = false
                    };
                    await _unitOfWork.Repository<DM.StudentMocktestRank>().Insert(studentRank);
                }
                else
                {
                    isStudentExist.MockTestId = resultByMockTestId.MockTestId;
                    isStudentExist.AveragePercentage = resultAnalysis.AveragePercentage;
                    isStudentExist.TotalMark = totalMarks;
                    isStudentExist.TotalObtainMark = resultAnalysis.TotalObtainedMarks;
                    isStudentExist.StudentId = resultByMockTestId.StudentId;
                    isStudentExist.SubCourseId = subCourseId;
                    isStudentExist.InstituteId = instituteId;
                    isStudentExist.Rank = rank;
                    isStudentExist.LastModifierUserId = resultByMockTestId.UserId;
                    isStudentExist.LastModifyDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<DM.StudentMocktestRank>().Update(isStudentExist);
                }
                resultDetail.Rank = rank;
                #endregion
                studentResultDetail.ResultDetail = resultDetail;

                #endregion
                #region Add subject wise performance
                SubjectDetail subjectDetail = new();
                List<Res.SubjectResultDetail> subjectList = new();
                var studentResultList= await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.MockTestId == resultByMockTestId.MockTestId  && x.StudentId == resultByMockTestId.StudentId && !x.IsDeleted);
                foreach (var item in studentResultList.DistinctBy(x => x.SubjectId))
                {
                    Res.SubjectResultDetail student = new();
                    student.SubjectId = item.SubjectId;
                    var subjectInfo = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == item.SubjectId);
                    student.SubjectName = subjectInfo != null ? subjectInfo.SubjectName : "";
                    student.Correct = item.CorrectAnswer;
                    student.InCorrect = item.InCorrectAnswer;
                    student.Skipped = item.SkippedAnswer;
                    subjectList.Add(student);
                }
                subjectDetail.TotalCorrect = subjectList.Sum(x=>x.Correct);
                subjectDetail.TotalIncorrect = subjectList.Sum(x => x.InCorrect);
                subjectDetail.TotalSkipped = subjectList.Sum(x => x.Skipped);
                subjectDetail.SubjectResultDetails = subjectList;
                studentResultDetail.SubjectDetail = subjectDetail;
                #endregion
                return studentResultDetail;
            }
            return null;
        }
        public async Task<Res.ResultAnalysisList> GetResultStudentAnalysis(Req.GeAdminResult adminResult)
        {
            Res.ResultAnalysisList resultAnalysisList = new();
            var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.SubCourseId == adminResult.SubCourseId && x.InstituteId == adminResult.InstituteId && !x.IsDeleted);
            var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == adminResult.InstituteId);
            var instituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
            var mockTestIds = mockTestDetails.Select(x => x.Id).ToList();
            var studentResultList = await _unitOfWork.Repository<DM.StudentResult>().Get(x => mockTestIds.Contains(x.MockTestId) && !x.IsCustom);
            List<Res.ResultAnalysis> resultAnalyses= new ();
            foreach (var student in studentResultList.DistinctBy(x=>x.StudentId))
            {
                int rank = 0;
                ApplicationUser userInfo = await _userManager.FindByIdAsync(student.StudentId.ToString());
                var instituteId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
                var studentSubcourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == student.StudentId && !x.IsDeleted);
                var subcorseId = studentSubcourse != null ? studentSubcourse.SubCourseId : Guid.Empty;
                Res.ResultAnalysis resultAnalysis = new();
                var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == student.StudentId && !x.IsDeleted && !x.IsCustom);
                resultAnalysis.Rank = 1;
                resultAnalysis.StudentId = userInfo != null ? Guid.Parse(userInfo.Id) : Guid.Empty;
                resultAnalysis.StudentName = userInfo != null ? userInfo.DisplayName : string.Empty;
                resultAnalysis.TotalMockTest = studentResult.Select(x => x.MockTestId).Distinct().Count();
                resultAnalysis.TotalMarks = studentResult.Sum(x => x.TotalMarks);
                resultAnalysis.TotalObtainedMarks = studentResult.Sum(x => x.ObtainMarks);
                resultAnalysis.InstituteName = instituteName;
                double percentage = 0;
                if (resultAnalysis.TotalMarks > 0)
                {
                    percentage = (resultAnalysis.TotalObtainedMarks / resultAnalysis.TotalMarks) * 100;
                }

                resultAnalysis.AveragePercentage = Math.Round(percentage, 2);
                var rankScores = await _unitOfWork.Repository<DM.StudentRank>().Get(x => x.SubCourseId == subcorseId && x.InstituteId == instituteId && !x.IsDeleted);
                var ransScoresArray = rankScores.Select(x => x.TotalObtainMark).ToList();
                double[] scores = ransScoresArray.ToArray();
                 rank = 1;
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
                var isStudentExist = await _unitOfWork.Repository<DM.StudentRank>().GetSingle(x => x.StudentId == student.StudentId && x.SubCourseId == subcorseId && x.InstituteId == instituteId);
                if (isStudentExist == null)
                {
                    DM.StudentRank studentRank = new()
                    {
                        AveragePercentage = resultAnalysis.AveragePercentage,
                        TotalMark = resultAnalysis.TotalMarks,
                        TotalObtainMark = resultAnalysis.TotalObtainedMarks,
                        TotalMockTest = resultAnalysis.TotalMockTest,
                        StudentId = student.StudentId,
                        SubCourseId = subcorseId,
                        InstituteId = instituteId,
                        Rank = rank,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = adminResult.UserId
                    };
                    await _unitOfWork.Repository<DM.StudentRank>().Insert(studentRank);
                }
                else
                {
                    isStudentExist.AveragePercentage = resultAnalysis.AveragePercentage;
                    isStudentExist.TotalMark = resultAnalysis.TotalMarks;
                    isStudentExist.TotalObtainMark = resultAnalysis.TotalObtainedMarks;
                    isStudentExist.TotalMockTest = resultAnalysis.TotalMockTest;
                    isStudentExist.StudentId = student.StudentId;
                    isStudentExist.SubCourseId = subcorseId;
                    isStudentExist.InstituteId = instituteId;
                    isStudentExist.Rank = rank;
                    isStudentExist.LastModifierUserId = adminResult.UserId;
                    isStudentExist.LastModifyDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<DM.StudentRank>().Update(isStudentExist);
                }
                resultAnalysis.Rank = rank;
                resultAnalyses.Add(resultAnalysis);
            }
           var count = resultAnalyses.Count;
           resultAnalysisList.ResultAnalyses = resultAnalyses;
            var result = resultAnalysisList.ResultAnalyses.Page(adminResult.PageNumber, adminResult.PageSize);
            resultAnalysisList.ResultAnalyses = result.ToList();
            resultAnalysisList.TotalRecord = count;
            return resultAnalysisList;
        }
        public async Task<Res.ResultAnalysisDetail?> GetResultAnalysisDetails(Req.GeResultAnalysisDetail resultAnalysisDetail)
        {
            Res.ResultAnalysisDetail studentResultDetail = new();
            #region Add student details
            ApplicationUser userInfo = await _userManager.FindByIdAsync(resultAnalysisDetail.StudentId.ToString());
            if (userInfo != null)
            {
                var instituteId =  userInfo.InstituteId;
                studentResultDetail.StudentId = Guid.Parse(userInfo.Id);
                studentResultDetail.StudentName =  userInfo.DisplayName;
                var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == userInfo.InstituteId);
                studentResultDetail.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                studentResultDetail.InstituteCode = instituteDetail != null ? instituteDetail.InstituteCode : string.Empty;
                var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == resultAnalysisDetail.StudentId && !x.IsDeleted);
                Guid subCourseId = studentsubCourse != null ? studentsubCourse.SubCourseId : Guid.Empty;
                var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                studentResultDetail.SubCourseName = subCourseDetail != null ? subCourseDetail.SubCourseName : string.Empty;
                Guid courseId = subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                studentResultDetail.CourseName = courseDetail != null ? courseDetail.CourseName : string.Empty;
                var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == resultAnalysisDetail.StudentId && !x.IsDeleted && !x.IsCustom);
                studentResultDetail.Rank = 1;
                studentResultDetail.TotalMockTest = studentResult.Select(x => x.MockTestId).Distinct().Count();
                studentResultDetail.TotalMarks = studentResult.Sum(x => x.TotalMarks);
                studentResultDetail.TotalObtainedMarks = studentResult.Sum(x => x.ObtainMarks);
                double percentage = 0;

                if (studentResultDetail.TotalMarks > 0)
                {
                    percentage = (studentResultDetail.TotalObtainedMarks / studentResultDetail.TotalMarks) * 100;
                }

                studentResultDetail.AveragePercentage = Math.Round(percentage, 2);
                var rankScores = await _unitOfWork.Repository<DM.StudentRank>().Get(x => x.SubCourseId == subCourseId && x.InstituteId == instituteId && !x.IsDeleted);
                var ransScoresArray = rankScores.Select(x => x.TotalObtainMark).ToList();
                double[] scores = ransScoresArray.ToArray();
                int rank = 1;

                Array.Sort(scores);
                Array.Reverse(scores);

                double currentScore = studentResultDetail.TotalObtainedMarks;

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
                var isStudentExist = await _unitOfWork.Repository<DM.StudentRank>().GetSingle(x => x.StudentId == resultAnalysisDetail.StudentId && x.SubCourseId == subCourseId && x.InstituteId == instituteId);
                if (isStudentExist == null)
                {
                    DM.StudentRank studentRank = new()
                    {
                        AveragePercentage = studentResultDetail.AveragePercentage,
                        TotalMark = studentResultDetail.TotalMarks,
                        TotalObtainMark = studentResultDetail.TotalObtainedMarks,
                        TotalMockTest = studentResultDetail.TotalMockTest,
                        StudentId = studentResultDetail.StudentId,
                        SubCourseId = subCourseId,
                        InstituteId = instituteId,
                        Rank = rank,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = resultAnalysisDetail.UserId
                    };
                    await _unitOfWork.Repository<DM.StudentRank>().Insert(studentRank);
                }
                else
                {
                    isStudentExist.AveragePercentage = studentResultDetail.AveragePercentage;
                    isStudentExist.TotalMark = studentResultDetail.TotalMarks;
                    isStudentExist.TotalObtainMark = studentResultDetail.TotalObtainedMarks;
                    isStudentExist.TotalMockTest = studentResultDetail.TotalMockTest;
                    isStudentExist.StudentId = studentResultDetail.StudentId;
                    isStudentExist.SubCourseId = subCourseId;
                    isStudentExist.InstituteId = instituteId;
                    isStudentExist.Rank = rank;
                    isStudentExist.LastModifierUserId = resultAnalysisDetail.UserId;
                    isStudentExist.LastModifyDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<DM.StudentRank>().Update(isStudentExist);
                }

                studentResultDetail.Rank = rank;
                return studentResultDetail;
            }
            return null;
            #endregion
        }
    }
}
