using OnlinePractice.API.Models;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using System.Runtime.CompilerServices;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentResultRepository : IStudentResultRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentResultRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Res.StudentResultAnalysis> GetResultAnalysis(CurrentUser user)
        {
            ApplicationUser userInfo = await _userManager.FindByIdAsync(user.UserId.ToString());
            var instituteId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
            var studentSubcourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == user.UserId && !x.IsDeleted);
            var subcorseId = studentSubcourse != null ? studentSubcourse.SubCourseId : Guid.Empty;
            var instituteInfo = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteId && !x.IsDeleted);
            Res.StudentResultAnalysis resultAnalysis = new();
            var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == user.UserId && !x.IsDeleted && !x.IsCustom);
            resultAnalysis.Rank = 1;
            resultAnalysis.Name = userInfo != null ? userInfo.DisplayName : string.Empty;
            resultAnalysis.InstituteCode = instituteInfo != null ? instituteInfo.InstituteCode: string.Empty;
            resultAnalysis.TotalMockTest = studentResult.Select(x => x.MockTestId).Distinct().Count();
            resultAnalysis.TotalMarks = studentResult.Sum(x => x.TotalMarks);
            resultAnalysis.TotalObtainedMarks = studentResult.Sum(x => x.ObtainMarks);
            double percentage = 0;
            if (resultAnalysis.TotalMarks > 0)
            {
                percentage = (resultAnalysis.TotalObtainedMarks / resultAnalysis.TotalMarks) * 100;
            }

            resultAnalysis.AveragePercentage = Math.Round(percentage, 2);
            var rankScores = await _unitOfWork.Repository<DM.StudentRank>().Get(x => x.SubCourseId == subcorseId && x.InstituteId == instituteId && !x.IsDeleted);
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
            var isStudentExist = await _unitOfWork.Repository<DM.StudentRank>().GetSingle(x => x.StudentId == user.UserId && x.SubCourseId == subcorseId && x.InstituteId == instituteId);
            if (isStudentExist == null)
            {
                DM.StudentRank studentRank = new()
                {
                    AveragePercentage = resultAnalysis.AveragePercentage,
                    TotalMark = resultAnalysis.TotalMarks,
                    TotalObtainMark = resultAnalysis.TotalObtainedMarks,
                    TotalMockTest = resultAnalysis.TotalMockTest,
                    StudentId = user.UserId,
                    SubCourseId = subcorseId,
                    InstituteId = instituteId,
                    Rank = rank,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = user.UserId
                };
                await _unitOfWork.Repository<DM.StudentRank>().Insert(studentRank);
            }
            else
            {
                isStudentExist.AveragePercentage = resultAnalysis.AveragePercentage;
                isStudentExist.TotalMark = resultAnalysis.TotalMarks;
                isStudentExist.TotalObtainMark = resultAnalysis.TotalObtainedMarks;
                isStudentExist.TotalMockTest = resultAnalysis.TotalMockTest;
                isStudentExist.StudentId = user.UserId;
                isStudentExist.SubCourseId = subcorseId;
                isStudentExist.InstituteId = instituteId;
                isStudentExist.Rank = rank;
                isStudentExist.LastModifierUserId = user.UserId;
                isStudentExist.LastModifyDate = DateTime.UtcNow;
                await _unitOfWork.Repository<DM.StudentRank>().Update(isStudentExist);
            }

            resultAnalysis.Rank = rank;
            return resultAnalysis;
        }
        public async Task<Res.ExistingMockTestList?> GetExistingMockTestList(CurrentUser user)
        {
            Res.ExistingMockTestList existingMockTestList = new();
            var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == user.UserId && !x.IsDeleted && !x.IsCustom);
            if (studentResult.Any())
            {
                foreach (var student in studentResult.DistinctBy(x => x.MockTestId))
                {
                    Res.ExistingMockTestDetails existingMockTest = new();
                    existingMockTest.MockTestId = student.MockTestId;
                    var mockTestInfo = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == student.MockTestId && !x.IsDeleted);
                    existingMockTest.MockTestName = mockTestInfo != null ? mockTestInfo.MockTestName : string.Empty;
                    existingMockTestList.ExistingMockTestDetails.Add(existingMockTest);
                }
                existingMockTestList.TotalRecords = existingMockTestList.ExistingMockTestDetails.Count;
                return existingMockTestList;
            }
            return null;
        }
        public async Task<Res.MocktestDataResultList?> GetCompletedMockTestList(CurrentUser user)
        {
            Res.MocktestDataResultList mockTestList = new();
            var studentResult = await _unitOfWork.Repository<DM.StudentMockTestStatus>().Get(x => x.StudentId == user.UserId && !x.IsDeleted && x.IsCompleted && !x.IsCustom && x.IsStarted , orderBy:x=>x.OrderByDescending(x=>x.CreationDate));
            if (studentResult.Any())
            {
                foreach (var student in studentResult.DistinctBy(x => x.MockTestId))
                {
                    if (student.IsCustom)
                    {
                        var mockTestInfo = await _unitOfWork.Repository<DM.StudentMockTest>().GetSingle(x => x.Id == student.MockTestId && !x.IsDeleted);
                        if (mockTestInfo != null)
                        {
                            Res.MockTestDetails mock = new();
                            mock.MockTestId = student.MockTestId;
                            mock.MockTestName = mockTestInfo.MockTestName;
                            mock.Language = mockTestInfo.Language;
                            mock.Duration = mockTestInfo.TimeDuration;
                            var subjectId = mockTestInfo.SubjectId;
                            var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId);
                            var subjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                            mock.Price = 0;
                            mock.SubjectName = subjectName;
                            mock.IsCustom = true;
                            mockTestList.MockTestDetails.Add(mock);
                        }
                    }
                    else
                    {
                        var mockTestInfo = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == student.MockTestId && !x.IsDeleted);
                        if (mockTestInfo != null)
                        {
                            Res.MockTestDetails mock = new();
                            mock.MockTestId = student.MockTestId;
                            mock.MockTestName = mockTestInfo.MockTestName;
                            mock.Language = mockTestInfo.Language;
                            mock.Duration = mockTestInfo.TimeSettingDuration;
                            mock.Price = mockTestInfo.Price;
                            var subjectId = mockTestInfo.SubjectId;
                            var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId);
                            var subjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                            mock.SubjectName = subjectName;
                            var topicId = mockTestInfo.TopicId;
                            var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topicId);
                            var topicName = topicDetails != null ? topicDetails.TopicName : string.Empty;
                            mock.TopicName = topicName;
                            var subTopicId = mockTestInfo != null ? mockTestInfo.SubTopicId : Guid.Empty;
                            var subtTopicDetails = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == subTopicId);
                            var subTopicName = subtTopicDetails != null ? subtTopicDetails.SubTopicName : string.Empty;
                            mock.SubTopicName = subTopicName;
                            mock.IsCustom = false;
                            mockTestList.MockTestDetails.Add(mock);
                        }
                    }


                }
                mockTestList.TotalRecords = mockTestList.MockTestDetails.Count;
                return mockTestList;
            }
            return null;
        }
        public async Task<Res.StudentMockTestWiseResultAnalysis> GetMockTestWisePerformance(Req.StudentResultMockTestId studentResultMockTest)
        {
            ApplicationUser userInfo = await _userManager.FindByIdAsync(studentResultMockTest.UserId.ToString());
            var instituteId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
            var studentSubcourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == studentResultMockTest.UserId && !x.IsDeleted);
            var subcorseId = studentSubcourse != null ? studentSubcourse.SubCourseId : Guid.Empty;
            Res.StudentMockTestWiseResultAnalysis resultAnalysis = new();
            var studentResult = await _unitOfWork.Repository<DM.StudentResult>().Get(x => x.StudentId == studentResultMockTest.UserId && x.MockTestId == studentResultMockTest.MockTestId && !x.IsDeleted && !x.IsCustom);
            resultAnalysis.Rank = 1;
            resultAnalysis.Name = userInfo != null ? userInfo.DisplayName : string.Empty;
            resultAnalysis.TotalMarks = studentResult.Sum(x => x.TotalMarks);
            resultAnalysis.TotalObtainedMarks = studentResult.Sum(x => x.ObtainMarks);
            double percentage = 0;
            if (resultAnalysis.TotalMarks != 0)
            {
                percentage = (resultAnalysis.TotalObtainedMarks / resultAnalysis.TotalMarks) * 100;
            }
            resultAnalysis.AveragePercentage = Math.Round(percentage, 2);
            var rankScores = await _unitOfWork.Repository<DM.StudentMocktestRank>().Get(x => x.SubCourseId == subcorseId && x.InstituteId == instituteId && !x.IsDeleted);
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
            var isStudentExist = await _unitOfWork.Repository<DM.StudentMocktestRank>().GetSingle(x => x.StudentId == studentResultMockTest.UserId && x.MockTestId == studentResultMockTest.MockTestId && x.SubCourseId == subcorseId && x.InstituteId == instituteId);
            if (isStudentExist == null)
            {
                DM.StudentMocktestRank studentRank = new()
                {
                    MockTestId = studentResultMockTest.MockTestId,
                    AveragePercentage = resultAnalysis.AveragePercentage,
                    TotalMark = resultAnalysis.TotalMarks,
                    TotalObtainMark = resultAnalysis.TotalObtainedMarks,
                    StudentId = studentResultMockTest.UserId,
                    SubCourseId = subcorseId,
                    InstituteId = instituteId,
                    Rank = rank,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = studentResultMockTest.UserId,
                    IsActive = true,
                    IsDeleted = false
                };
                await _unitOfWork.Repository<DM.StudentMocktestRank>().Insert(studentRank);
            }
            else
            {
                isStudentExist.MockTestId = studentResultMockTest.MockTestId;
                isStudentExist.AveragePercentage = resultAnalysis.AveragePercentage;
                isStudentExist.TotalMark = resultAnalysis.TotalMarks;
                isStudentExist.TotalObtainMark = resultAnalysis.TotalObtainedMarks;
                isStudentExist.StudentId = studentResultMockTest.UserId;
                isStudentExist.SubCourseId = subcorseId;
                isStudentExist.InstituteId = instituteId;
                isStudentExist.Rank = rank;
                isStudentExist.LastModifierUserId = studentResultMockTest.UserId;
                isStudentExist.LastModifyDate = DateTime.UtcNow;
                await _unitOfWork.Repository<DM.StudentMocktestRank>().Update(isStudentExist);
            }

            resultAnalysis.Rank = rank;
            return resultAnalysis;
        }
    }
}
