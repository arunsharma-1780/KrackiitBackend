using OnlinePractice.API.Models;
using Req =OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentResultRepository
    {
        public Task<Res.StudentResultAnalysis> GetResultAnalysis(CurrentUser user);
        public Task<Res.ExistingMockTestList?> GetExistingMockTestList(CurrentUser user);
        public Task<Res.StudentMockTestWiseResultAnalysis> GetMockTestWisePerformance(Req.StudentResultMockTestId studentResultMockTest);
        public  Task<Res.MocktestDataResultList?> GetCompletedMockTestList(CurrentUser user);

    }
}
