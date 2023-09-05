using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IAdminDashboardRepository
    {
        public Task<Res.TotalSaleDetails> GetTotalSale(Req.FilterModel model);
        public Task<Res.TotalEnrollmentDetails> GetTotalEnrollment(Req.FilterModel model);
        public Task<Res.InstituteStudent> GetInstituteStudentCourse(Req.FilterModel model);
        public List<Com.EnumModel> GetDurationFilter();
        public  Task<Res.RecentTransactionandTimeLineList?> GetRecentTransactionsandTimeLineList();
        public Task<Res.TimeLineList?> GetTimeLine();
        public Task<List<KeyValuePair<int, int>>?> GetStudentEnrollmentGraph(Req.FilterModel model);
        public Task<List<KeyValuePair<int, double>>?> GetTotalSalesGraph(Req.FilterModel model);
        public  Task<List<KeyValuePair<DateTime, int>>?> GetStudentEnrollmentGraphV1(Req.FilterModel model);
        public Task<List<KeyValuePair<DateTime, double>>?> GetTotalSalesGraphV2(Req.FilterModel model);
    }
}
