using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IAdminResultRepository
    {
        public Task<Res.AdminStudentResultList> GetAllResults(Req.GeAdminResult adminResult);
        public Task<Res.StudentResultDetail?> GetResultByMockTestId(Req.GeResultByMockTestId resultByMockTestId);
        public Task<Res.ResultAnalysisList> GetResultStudentAnalysis(Req.GeAdminResult adminResult);
        public Task<Res.ResultAnalysisDetail?> GetResultAnalysisDetails(Req.GeResultAnalysisDetail resultAnalysisDetail);
        public  Task<Res.MockTestList> GetAllMockTest(Req.GetMockTestList mockTestList);
    }
}
