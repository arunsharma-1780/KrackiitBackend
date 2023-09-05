using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
namespace OnlinePractice.API.Validator
{

    public class InstituteLogoValidator : AbstractValidator<Req.LogoImage>
    {
        public InstituteLogoValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");

            RuleFor(prop => prop.Image).NotNull().Must(x => x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif"))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");

        }
    }
    public class CreateInstituteValidator : AbstractValidator<Req.CreateInstitute>
    {
        public CreateInstituteValidator()
        {
            RuleFor(prop => prop.InstituteName.Trim()).MaximumLength(150).WithMessage("InstituteName must be less than 150 character").NotEmpty().WithMessage("InstituteName must no be Empty!").NotNull().WithMessage("Name is required!").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Invalid institute name!");
            RuleFor(prop => prop.InstituteCode).MaximumLength(15).WithMessage("InstituteCode must be less than 15 character").NotEmpty().WithMessage("InstituteCode must no be Empty!").NotNull().WithMessage("InstituteCode is required!");
            RuleFor(prop => prop.InstituteEmail.Trim()).NotEmpty().WithMessage("InstituteEmail must not be empty").NotNull().WithMessage("InstituteEmail is Required").EmailAddress().WithMessage("Wrong email address!");
            RuleFor(prop => prop.InstituteAddress.Trim()).NotEmpty().WithMessage("Address must not be empty").NotNull().WithMessage("Address is required!").MaximumLength(250).WithMessage("InstituteAddress must be less than 250 character");
            RuleFor(prop => prop.InstituteContactPerson.Trim()).NotEmpty().WithMessage("ContactPersonName must not be empty").NotNull().WithMessage("ContactPersonName is required!");
            RuleFor(prop => prop.InstituteContactNo).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber with country code can not be less than 10 character without special symbol and ")
.MaximumLength(10).WithMessage("MobileNumber with country code can not be greater than 10 character").Matches(@"^\d+$")
.WithMessage("MobileNumber is invalid!");
            RuleFor(prop => prop.InstituteCity.Trim()).NotEmpty().WithMessage("InstituteCity must not be empty").NotNull().WithMessage("InstituteCity is required!");
            RuleFor(prop => prop.InstituteLogo.Trim()).NotEmpty().NotNull().WithMessage("InstituteLogo Url is required!").
                 Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.jpg|.JPG|.jpeg|.JPEG|.bmp|.BMP|.gif|.GIF|.psd|.PSD|.png|.PNG)$").WithMessage("Image path not valid!");
        }
    }


    public class EditInstituteValidator : AbstractValidator<Req.EditInstitute>
    {
        public EditInstituteValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().WithMessage("Id must no be Empty!").NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateInstituteValidator());
        }
    }

    public class GetInstituteByIdValidator : AbstractValidator<Req.InstituteById>
    {
        public GetInstituteByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().WithMessage("Id must no be Empty!").NotNull().WithMessage("Id is required!");
        }
    }

    public class DeleteInstituteValidator : AbstractValidator<Req.InstituteById>
    {
        public DeleteInstituteValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().WithMessage("Id must no be Empty!").NotNull().WithMessage("Id is required!");
        }
    }
}
