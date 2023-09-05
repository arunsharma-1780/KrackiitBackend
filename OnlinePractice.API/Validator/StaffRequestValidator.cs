using FluentValidation;
using OnlinePractice.API.Repository.Services;
using Req = OnlinePractice.API.Models.Request;


namespace OnlinePractice.API.Validator
{
    public class CreateStaffValidator : AbstractValidator<Req.CreateStaff>
    {
        public CreateStaffValidator()
        {
            RuleFor(prop => prop.Email.Trim()).NotEmpty().WithMessage("Email must not be empty").NotNull().WithMessage("Email is Required").Matches((@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")).WithMessage("Invalid Email format");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
             .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Staff Name is invalid");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId id required!");
            RuleFor(prop => prop.Permission).NotEmpty().NotNull().WithMessage("Permission is required!");
            RuleFor(prop => prop.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\@\.]+").WithMessage("Your password must contain at least one (!? *@.).");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber with country code can not be less than 10 character without special symbol and ")
            .MaximumLength(10).WithMessage("MobileNumber with country code can not be greater than 10 character").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
        }
    }
    public class EditStaffValidator : AbstractValidator<Req.EditStaff>
    {
        public EditStaffValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop.FullName.Trim()).NotEmpty().NotNull().WithMessage("FullName is required")
            .Matches(@"^[a-z|A-Z]+(?: [a-z|A-Z]+)*$").WithMessage("Staff Name is invalid");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId id required!");
            RuleFor(prop => prop.Permission).NotEmpty().NotNull().WithMessage("Permission is required!");
            RuleFor(prop => prop.MobileNumber).NotEmpty().NotNull().WithMessage("MobileNumber is required!").MinimumLength(10).WithMessage("MobileNumber with country code can not be less than 10 character without special symbol and ")
            .MaximumLength(10).WithMessage("MobileNumber with country code can not be greater than 10 character").Matches(@"^\d+$")
            .WithMessage("MobileNumber is invalid!");
        }
    }

    public class GetAllStaffValidator : AbstractValidator<Req.GetAllStaff>
    {
        public GetAllStaffValidator()
        {
            RuleFor(prop => prop.PageSize).NotEmpty().NotNull().WithMessage("PageSize is required!");
            RuleFor(prop => prop.PageNumber).NotEmpty().NotNull().WithMessage("PageNumber is required!");
        }
    }
    public class GetByIdStaffValidator : AbstractValidator<Req.GetByIdStaff>
    {
        public GetByIdStaffValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
}

