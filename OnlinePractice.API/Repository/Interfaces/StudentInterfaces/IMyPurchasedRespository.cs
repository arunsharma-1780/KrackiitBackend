using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Enum;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IMyPurchasedRespository
    {
        public Task<bool> CreateMyPurchased(Req.CreateMyPurchased createMyPurchased);
        public Res.StudentModulesList? GetStudentModules(CurrentUser currentUser);
        public Task<Res.MyPurchasedMockTestsLists?> GetPurchasedMocktest(Req.MyPurchasedMocktest purchasedMocktest);
        public Task<Res.MyPurchasedEbooksLists?> GetPurchasedEbooks(Req.MyPurchasedEbook purchasedEbook);
        public Task<Res.MyPurchasedVideosLists?> GetPurchasedVideos(Req.MyPurchasedVideo purchasedVideo);
        public Task<Res.MyPurchasedPYPLists?> GetPurchasedPreviousYearPaper(Req.MyPurchasedPreviousYearPAper purchasedPreviousYearPAper);
        public Task<bool> IsPurchasedCheck(Guid Id, ProductCategory productCategory);
        public bool PurchasedCheck(Guid Id, ProductCategory productCategory, Guid studentId);
    }
}
