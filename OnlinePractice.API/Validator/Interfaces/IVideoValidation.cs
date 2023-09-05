namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IVideoValidation
    {
        public CreateVideoValidator CreateVideoValidator { get; set; }
        public VideoThumbnailimageValidator VideoThumbnailimageValidator { get; set; }
        public EditVideoValidator EditVideoValidator { get; set; }
        public VideoByIdValidator VideoByIdValidator { get; set; }
        public GetAllVideosValidator GetAllVideosValidator { get; set; }
        public GetAllVideosAuthorsValidator GetAllVideosAuthorsValidator { get; set; }

        public VideoUrlValidator VideoUrlValidator { get; set; }
    }
}
