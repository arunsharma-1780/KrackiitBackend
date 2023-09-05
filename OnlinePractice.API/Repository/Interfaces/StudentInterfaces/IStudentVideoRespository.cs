using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentVideoRespository
    {
        public Task<Res.VideosSubjectsList?> GetVideoSubjects(Req.StudentSubjects studentSubjects);
        public Task<Res.StudentVideoList?> GetStudentsVideos(Req.GetStudentVideo studentEbook);
        public Task<Res.GetVideo?> GetVideos(Req.GetVideoById getVideo);
    }
}
