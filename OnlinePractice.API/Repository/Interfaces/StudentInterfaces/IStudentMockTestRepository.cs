using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentMockTestRepository
    {
        #region Student MockTestDasboard
        public Task<Res.InstituteMockTestList?> GetMockTestListByInstitute(Req.MockTestInstitute institute);
        #endregion

        #region StudentMockTest
        public Task<Res.StudentMockTestList?> GetMockTestListByFilters(Req.StudentMockTest institute);
        public List<Com.EnumModel> GetMockTestLanguage();
        public List<Com.EnumModel> GetMockTestStatus();
        public List<Com.EnumModel> GetCustomeMockTestStatus();
        public List<Com.EnumModel> GetMockTestPricing();
        public Task<Com.ResultDto> GenerateAutomaticMockTestForStudent(Req.StudentAutomaticMockTestQuestion automaticMockTestQuestion);
        public Task<Res.CustomeStudentMockTestList?> GetCustomeMockTestListByFilter(Req.CustomeStudentMockTest institute);
        #endregion

        #region Student Question Panel
        public Task<Res.StudentMocktestPanel?> GetStudentQuestionPanel(Req.GetStudentQuestionPanel mockTest);
        public Task<Res.StudentQuestionResponseV2?> SaveStudentResponses(Req.StudentQuestionResponse response);
        public Task<Res.StudentAnswersPanelList?> GetStudentAnswerPanel(Req.StudentAnwersPanel panel);
        public Task<Res.GeneralInstructions?> GetGeneralInstructions(Req.StudentMockTestId mockTestId);
        public Task<Res.RestultStatus> SaveMockTestStatus(Req.StudentMockTestStatus response);
        public Task<bool> IsCustomeDuplicate(Req.CustomeMockTestNameCheck mockTestName);
        public Task<bool> MarkAsSeen(Req.MarkAsSeen response);
        public Task<Res.StudentQuestionResponseV2?> ReviewStudentAnswer(Req.ReviewAnswer panel);
        public Task<bool> RemoveStudentAnser(Req.RemoveAnswer answer);
        public  Task<bool> ResumeMocktest(Req.ResumeMockTest response);

        #endregion

        #region Student Result 
        //public Task<Res.StudentResults> CalculateResult(Req.GetStudentResult result);
        public Task<Res.StudentResultList> GetStudentPreviousResults(Req.GetStudentResult result);
        public Task<Res.StudentResults> GetStudentResults(Req.GetResult result);
        #endregion

        #region Student Solution
        public Task<Res.StudentQuestionSolution?> GetStudentQuestionSolution(Req.GetStudentQuestionSolution mockTest);
        #endregion
    }
}
