using FluentValidation;
using OnlinePractice.API.Validator.Interfaces;
using Org.BouncyCastle.Ocsp;

namespace OnlinePractice.API.Validator.Services
{
    public class VideoValidation : IVideoValidation
    {
        public CreateVideoValidator CreateVideoValidator { get; set; } = new();
        public VideoThumbnailimageValidator VideoThumbnailimageValidator { get; set; }  =new();
        public EditVideoValidator EditVideoValidator { get; set; } =new();
        public VideoByIdValidator VideoByIdValidator { get; set; } =new();
        public GetAllVideosValidator GetAllVideosValidator { get; set; } =new();
        public GetAllVideosAuthorsValidator GetAllVideosAuthorsValidator { get; set; } =new();
        public VideoUrlValidator VideoUrlValidator { get; set; } =new();
    }
}
