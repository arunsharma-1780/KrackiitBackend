using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Validator
{
    public class GetAllStudentValidator : AbstractValidator<Req.GetAllStudent>
    {
        public GetAllStudentValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.InstituteId).NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("PageSize is required!");
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("PageNumber is required!");

        }
    }
    public class AddStudentValidator : AbstractValidator<Req.AddStudent>
    {
        public AddStudentValidator()
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
    public class AddBulkUploadStudentValidator : AbstractValidator<Req.AddBulkUploadStudent>
    {
        public AddBulkUploadStudentValidator()
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
            RuleFor(prop => prop.IsBulkUpload).Must(x => x == false || x == true).WithMessage("IsBulkUpload is required!");
            RuleFor(prop => Convert.ToDouble(prop.Amount)).GreaterThan(0).WithMessage("Amount must be greater than zero!");
        }
    }
    public class UpdateStudentValidator : AbstractValidator<Req.UpdateStudent>
    {
        public UpdateStudentValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id must not be empty");
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Invalid Email format");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
             .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Student Name is invalid");
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
    public class GetStudentByIdValidator : AbstractValidator<Req.GetStudentById>
    {
        public GetStudentByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

}
