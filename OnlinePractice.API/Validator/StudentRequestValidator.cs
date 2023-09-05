using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateStudentValidator : AbstractValidator<Req.CreateStudent>
    {
        public CreateStudentValidator()
        {
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Invalid Email format");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
             .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Student Name is invalid");
            //RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId id required!");
            RuleFor(prop => prop.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
            .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
        }
    }



    public class EditStudentValidator : AbstractValidator<Req.EditStudent>
    {
        public EditStudentValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id must not be empty");
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Invalid Email format");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
             .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Student Name is invalid");
            //RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId id required!");
            //RuleFor(prop => prop.Password).NotEmpty().WithMessage("Your password cannot be empty")
            //        .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            //        .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            //        .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            //        .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            //        .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            //        .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
            .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
            //RuleFor(prop => prop.ProfileImage).NotEmpty().WithMessage("ProfileImage is required!");
            //RuleFor(prop => prop.InstituteId).NotNull().WithMessage("InstituteId must not be empty");
            //RuleFor(prop => prop.SubcourseId).NotNull().WithMessage("InstituteId must not be empty");
        }
    }

    public class EditStudentProfileValidator : AbstractValidator<Req.UpdateStudentProfile>
    {
        public EditStudentProfileValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id must not be empty");
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Invalid Email format");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
             .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Student Name is invalid");
            //RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId id required!");
            //RuleFor(prop => prop.Password).NotEmpty().WithMessage("Your password cannot be empty")
            //        .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            //        .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            //        .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            //        .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            //        .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            //        .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
            .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
            //RuleFor(prop => prop.ProfileImage).NotEmpty().WithMessage("ProfileImage is required!");
            //RuleFor(prop => prop.InstituteId).NotNull().WithMessage("InstituteId must not be empty");
            //RuleFor(prop => prop.SubcourseId).NotNull().WithMessage("InstituteId must not be empty");
        }
    }


    public class StudentLoginValidatior : AbstractValidator<Req.StudentLogin>
    {
        public StudentLoginValidatior()
        {

            RuleFor(prop => prop.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");
        }
    }

    public class EmailValidatior : AbstractValidator<Req.StudentLogin>
    {
        public EmailValidatior()
        {
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Invalid Email format");

        }
    }

    public class MobileValidatior : AbstractValidator<Req.StudentLogin>
    {
        public MobileValidatior()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
           .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
           .WithMessage("MobileNumber is invalid!");

        }
    }


    public class GetTokenByNumberValidator : AbstractValidator<Req.Tokenlogin>
    {
        public GetTokenByNumberValidator()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ")
            .MaximumLength(10).WithMessage("MobileNumber can not be greater than 10 number").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
        }
    }

    public class ForgotPasswordValidator : AbstractValidator<Req.ForgotStudentPassword>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ");
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
    public class SendOTPValidator : AbstractValidator<Req.SendOTP>
    {
        public SendOTPValidator()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ");
            RuleFor(prop => prop.CallingUnit).NotEmpty().NotNull().WithMessage("CallingUnit is required!");

        }
    }

    public class MobNumberValidator : AbstractValidator<Req.MobNumber>
    {
        public MobNumberValidator()
        {
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber can not be less than 10 numbers ");
        }
    }

    public class StudentProfileImageValidator : AbstractValidator<Req.ProfileImage>
    {
        public StudentProfileImageValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");

            RuleFor(prop => prop.Image).NotNull().Must(x => x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif"))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");

        }
    }

    public class AddFeedbackValidator : AbstractValidator<Req.AddFeedback>
    {
        public AddFeedbackValidator()
        {
            RuleFor(prop => prop.StudentFeedback).NotNull().WithMessage("StudentFeedback must not be empty");
        }
    }

}
