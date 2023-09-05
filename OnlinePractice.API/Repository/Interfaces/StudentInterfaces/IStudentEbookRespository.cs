using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res= OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentEbookRespository
    {
        public Task<Res.StudentEbooksList?> GetStudentsEbook(Req.GetStudentEbook studentEbook);
        public Task<Res.EbookSubjectsList?> GetEbookSubjects(Req.EbooksSubjects ebooksSubjects);
        public Task<Res.GetEbook?> GetEbook(Req.GetEbookById getEbook);
    }
}
