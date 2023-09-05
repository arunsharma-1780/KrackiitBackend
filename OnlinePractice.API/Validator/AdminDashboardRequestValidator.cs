using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Validator
{


    public class FilterModelValidator : AbstractValidator<Req.FilterModel>
    {
        public FilterModelValidator()
        {
            RuleFor(prop => prop.DurationFilter).IsInEnum().WithMessage("Duration filter[Today = 1,Week = 2,Month = 3] is required!");
        }
    }
}
