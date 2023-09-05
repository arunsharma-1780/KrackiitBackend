using FluentValidation;
using OnlinePractice.API.Models.AuthDB;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Validator
{

    public class ProfileImageValidator : AbstractValidator<Req.ProfileImage>
    {
        public ProfileImageValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");

            RuleFor(prop => prop.Image).NotNull().Must(x => x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif"))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");

        }
    }
    public class RegisterModelValidator : AbstractValidator<Req.Register>
    {
        public RegisterModelValidator()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
           .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
           .WithMessage("MobileNumber is invalid!");
            RuleFor(prop => prop.Password).NotEmpty().NotNull().WithMessage("Password is required!");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required!").Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Full Name is Invalid");
        }
    }
    public class LoginValidator : AbstractValidator<Req.Login>
    {
        public LoginValidator()
        {
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches((@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")).WithMessage("Invalid Email format");
            RuleFor(prop => prop.Password).NotEmpty().NotNull().WithMessage("Password is required!");

        }
    }
    public class UpdateAdminValidator : AbstractValidator<Req.UpdateAdmin>
    {
        public UpdateAdminValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id  is required!");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required!").Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Full Name is Invalid");
            RuleFor(prop => prop.Location).NotEmpty().NotNull().WithMessage("Location is required!").Matches(@"^[a-z|A-Z0-9'.,\/\(\)_-]+(?: [a-z|A-Z0-9'.,\/\(\)_-]+)*$").WithMessage("Location is invalid");
            RuleFor(prop => prop.ProfileImage).NotEmpty().WithMessage("ProfileImage is required!");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(12).WithMessage("MobileNumber with country code can not be less than 12 character without special symbol and ").Matches(@"^\d+$");
        }
    }
    public class RemoveProfileValidator : AbstractValidator<Req.RemoveProfile>
    {
        public RemoveProfileValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id  is required!");
        }
    }

    public class ChangePasswordValidator : AbstractValidator<Req.ChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is  required!");
            RuleFor(prop => prop.CurrentPassword).NotNull().NotEmpty().WithMessage("Your CurrentPassword cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.)");
            RuleFor(prop => prop.NewPassword).NotNull().NotEmpty().WithMessage("Your NewPassword cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.)");
            RuleFor(prop => prop.ConfirmPassword).NotNull().NotEmpty().WithMessage("Your ConfirmPassword cannot be empty")
               .MinimumLength(8).WithMessage("Your password length must be at least 8.")
               .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
               .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
               .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
               .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
               .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");

        }
    }
    public class ForgetPasswordValidator : AbstractValidator<Req.ForgotPassword>
    {
        public ForgetPasswordValidator()
        {
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches((@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")).WithMessage("Invalid Email format");
        }
    }

    public class GetUserByIdValidator : AbstractValidator<Req.GetUserById>
    {
        public GetUserByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
}
