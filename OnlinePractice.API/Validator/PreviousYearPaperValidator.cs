using FluentValidation;
using OnlinePractice.API.Validator.Interfaces;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class PaperFileValidator : AbstractValidator<Req.PaperPdfUrl>
    {
        public PaperFileValidator()
        {
            RuleFor(prop => prop.PdfUrl).NotNull().Must(x => x != null && (x.ContentType.Equals("application/pdf"))).WithMessage("File allowed type are [Pdf]!");
        }
    }
    public class CreatePaperValidator : AbstractValidator<Req.CreatePreviousYearPaper>
    {
        public CreatePaperValidator()
        {
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.Year).NotNull().WithMessage("Year is required!").GreaterThanOrEqualTo(1900).WithMessage("Year must be greater than  or equal to 1900!");
            RuleFor(prop => prop.Year).NotNull().WithMessage("Year is required!").LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must be less than  or equal to " + DateTime.UtcNow.Year);
            RuleFor(prop => prop.PaperTitle).NotEmpty().NotNull().WithMessage("PaperTitle is required!");
            RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("language is required!");
            RuleFor(prop => prop.PaperPdfUrl).NotEmpty().NotNull().WithMessage("PaperPdfUrl Url is required!").
                 Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.pdf|.ps|.eps)$").WithMessage("Pdf path not valid,it must be in Pdf format!");
            RuleFor(prop => prop.Price).NotNull().WithMessage("Price is required!").GreaterThanOrEqualTo(0).WithMessage("Price must be greater than  or equal to zero!");
        }
    }
    public class EditPaperValidator : AbstractValidator<Req.EditPreviousYearPaper>
    {
        public EditPaperValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreatePaperValidator());
        }
    }
    public class DeletePaperValidator : AbstractValidator<Req.GetPaperPdf>
    {
        public DeletePaperValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
    public class GetByIdPaperValidator : AbstractValidator<Req.GetPaperPdf>
    {
        public GetByIdPaperValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

    public class GetAllPapersValidator : AbstractValidator<Req.GetAllPapers>
    {
        public GetAllPapersValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourse Id is required!");
            RuleFor(prop => prop.Year).NotNull().WithMessage("Year is required!").GreaterThanOrEqualTo(1900).WithMessage("Year must be greater than  or equal to 1900!");
            RuleFor(prop => prop.Year).NotNull().WithMessage("Year is required!").LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must be less than  or equal to " + DateTime.UtcNow.Year);
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("Institute Id is required!");
            //RuleFor(prop => prop.CreationUserId).NotEmpty().NotNull().WithMessage("CreationUserId Id is required!");
            RuleFor(prop => prop.PageNumber).NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).NotNull().WithMessage("Page Size is required!");
        }
    }

    public class  GetAllFacultiesValidator : AbstractValidator<Req.GetAllFacultyList>
    {
        public GetAllFacultiesValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourse Id is required!");
            RuleFor(prop => prop.AddedBy).NotEmpty().NotNull().WithMessage("Added by is required!");
        }
    }

}
