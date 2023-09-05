using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentPYPRepository
    {
        public Task<Res.StudentPYPList> GetPreviousYearPaper(Req.PYPInstitutes pypInstitutes);
        public Task<Res.StudentPYPList?> GetPapersDataByFilter(Req.StudentPreviousYearPaperFilter  previousYearPaperFilter);
        public List<Com.EnumModel> GetPriceWiseSort();
    }
}
