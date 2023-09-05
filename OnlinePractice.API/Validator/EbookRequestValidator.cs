using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Org.BouncyCastle.Ocsp;

namespace OnlinePractice.API.Validator
{
    public class EbookThumbnailValidator : AbstractValidator<Req.EbookThumbnailimage>
    {
        public EbookThumbnailValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");
            RuleFor(prop => prop.Image).NotNull().Must(x => x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif"))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");
        }
    }
    public class EbookFileValidator : AbstractValidator<Req.EbookPdfUrl>
    {
        public EbookFileValidator()
        {
            RuleFor(prop => prop.PdfUrl).NotNull().Must(x => x != null && (x.ContentType.Equals("application/pdf"))).WithMessage("File allowed type are [Pdf]!");
        }
    }
    public class CreateEbookValidator : AbstractValidator<Req.CreateEbook>
    {
        public CreateEbookValidator()
        {
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.SubjectCategory).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.EbookTitle).NotEmpty().NotNull().WithMessage("EbookTitle is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.AuthorName.Trim()).NotEmpty().NotNull().WithMessage("Author name is required!").Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Author name is Invalid");
            RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("Language is required!");
            //RuleFor(prop => prop.EbookThumbnail).NotEmpty().NotNull().WithMessage("EbookThumbnail Url is required!").
            //     Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.jpg|.JPG|.jpeg|.JPEG|.bmp|.BMP|.gif|.GIF|.psd|.PSD|.png|.PNG|.jfif)$").WithMessage("Image path not valid!");
            RuleFor(prop => prop.EbookPdfUrl).NotEmpty().NotNull().WithMessage("EbookPdfUrl Url is required!").
                 Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.pdf|.ps|.eps)$").WithMessage("Pdf path not valid!");
            RuleFor(prop => prop.Price).NotNull().WithMessage("Price is required!").GreaterThanOrEqualTo(0).WithMessage("Price must be greater than  or equal to zero!");
        }
    }
    public class EditEbookValidator : AbstractValidator<Req.EditEbook>
    {
        public EditEbookValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateEbookValidator());
        }
    }
    public class DeleteEbookValidator : AbstractValidator<Req.DeleteEbook>
    {
        public DeleteEbookValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
    public class GetByIdEbookValidator : AbstractValidator<Req.EbookById>
    {
        public GetByIdEbookValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

    public class GetAllEbookValidator : AbstractValidator<Req.GetAllEbookV1>
    {
        public GetAllEbookValidator()
        {
            RuleFor(prop => prop.SubjectCategoryId).NotEmpty().NotNull().WithMessage("SubjectCategoryId is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.WriterName).NotEmpty().NotNull().WithMessage("WriterName is required!");
            RuleFor(prop => prop.PageNumber).NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).NotNull().WithMessage("PageSize is required!");
        }
    }
    public class GetAllAuthersValidator : AbstractValidator<Req.GetAllAuthors>
    {
        public GetAllAuthersValidator()
        {
            RuleFor(prop => prop.SubjectCategoryId).NotEmpty().NotNull().WithMessage("SubjectCategoryId is required!");
        }
    }
}
