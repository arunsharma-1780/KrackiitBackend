using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IVideoRepository
    {
        public Task<string> UploadImage(Req.VideoThumbnailimage video);
        public Task<string> UploadVideoUrl(Req.VideoUrl video);
        public Task<Res.AuthorAndLanguage?> Create(Req.CreateVideo createVideo);
        public Task<Res.AuthorAndLanguage?> Edit(Req.EditVideo editVideo);
        public Task<Res.Video?> GetVideoById(Req.VideoById video);
        public Task<bool> Delete(Req.VideoById video);
        public Task<Res.VideoList?> GetAllVideos(Req.GetAllVideos videos);
        public Task<Res.VideoList?> GetAll50();
        //public Task<Res.VideoList?> GetAllStaffVideos(Req.GetAllVideos videos);
        public Task<Res.VideoAuthorList?> GetAllAuthors(Req.GetAllVideoAuthors video);
        public  Task<Res.VideoListV2?> Showallids();
    }
}
