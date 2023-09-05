using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class VideoThumbnailimageValidator : AbstractValidator<Req.VideoThumbnailimage>
    {
        public VideoThumbnailimageValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");
            RuleFor(prop => prop.Image).NotNull().Must(x => x != null && (x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif")))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");
        }
    }

    public class VideoUrlValidator : AbstractValidator<Req.VideoUrl>
    {
        public VideoUrlValidator()
        {
            RuleFor(prop => prop.VideoLinkUrl.Length).NotNull().GreaterThanOrEqualTo(1000000).WithMessage("VideoLinkUrl should be greater than 1mb!");
            RuleFor(prop => prop.VideoLinkUrl.Length).NotNull().LessThanOrEqualTo(500000000).WithMessage("VideoLinkUrl should be less than 500mb!");
            RuleFor(prop => prop.VideoLinkUrl).NotNull().Must(x => x != null &&( x.ContentType.Equals("video/mov") || x.ContentType.Equals("video/quicktime") || x.ContentType.Equals("video/avi") || x.ContentType.Equals("video/mp4") || x.ContentType.Equals("video/mpeg")))
      .WithMessage("VideoLinkUrl allowed type are [.mov|.avi|.mp4|.mpeg]");
        }
    }

    public class CreateVideoValidator : AbstractValidator<Req.CreateVideo>
    {
        public CreateVideoValidator()
        {
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.SubjectCategory).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.VideoTitle).NotEmpty().NotNull().WithMessage("VideoTitle is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.FacultyName.Trim()).NotEmpty().NotNull().WithMessage("FacultyName is required!").Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Faculty Name is Invalid");
            //RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("Language is required!");
            //RuleFor(prop => prop.VideoThumbnail).NotEmpty().NotNull().WithMessage("VideoThumbnail Url is required!").
            //     Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.jpg|.JPG|.jpeg|.JPEG|.bmp|.BMP|.gif|.GIF|.psd|.PSD|.png|.PNG|.jfif)$").WithMessage("Image path not valid!");
            RuleFor(prop => prop.VideoUrl).NotEmpty().NotNull().WithMessage("VideoUrl  is required!").
                 Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.mov|.avi|.mp4|.mpeg)$").WithMessage("VideoUrl path not valid!");
            RuleFor(prop => prop.Price).NotNull().WithMessage("Price is required!").GreaterThanOrEqualTo(0).WithMessage("Price must be greater than  or equal to zero!");
        }
    }

    public class EditVideoValidator : AbstractValidator<Req.EditVideo>
    {
        public EditVideoValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateVideoValidator());
        }
    }

    public class VideoByIdValidator : AbstractValidator<Req.VideoById>
    {
        public VideoByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
    public class GetAllVideosValidator : AbstractValidator<Req.GetAllVideos>
    {
        public GetAllVideosValidator()
        {
            RuleFor(prop => prop.SubjectCategoryId).NotEmpty().NotNull().WithMessage("SubjectCategoryId is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.FacultyName).NotEmpty().NotNull().WithMessage("FacultyName is required!");
            RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("Language is required!");
            RuleFor(prop => prop.PageNumber).NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).NotEmpty().NotNull().WithMessage("PageSize is required!");
        }
    }

    public class GetAllVideosAuthorsValidator : AbstractValidator<Req.GetAllVideoAuthors>
    {
        public GetAllVideosAuthorsValidator()
        {
            RuleFor(prop => prop.SubjectCategoryId).NotEmpty().NotNull().WithMessage("SubjectCategoryId is required!");
        }
    }
}
