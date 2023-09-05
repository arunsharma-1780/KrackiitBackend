using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateSubTopicValidator : AbstractValidator<Req.CreateSubTopic>
    {
        public CreateSubTopicValidator()
        {
            RuleFor(prop => prop.SubTopicName.Trim()).NotEmpty().WithMessage("SubTopicName must not be empty").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("SubTopic Name is invalid");
            RuleFor(prop => prop.SubTopicDescription.Trim()).NotEmpty().WithMessage("SubTopicName must not be empty");
            RuleFor(prop => prop.TopicId).NotEmpty().WithMessage("Topic Id is required");
        }
    }
    public class EditSubTopicValidator : AbstractValidator<Req.EditSubTopic>
    {
        public EditSubTopicValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubTopicId is required!!!");
            RuleFor(prop => prop).SetValidator( new CreateSubTopicValidator());
        }
    }

    public class GetSubTopicByIdValidator : AbstractValidator<Req.SubTopicById>
    {
        public GetSubTopicByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubTopicId is required!!!");
        }
    }

    public class DeleteSubTopicValidator : AbstractValidator<Req.SubTopicById>
    {
        public DeleteSubTopicValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubTopicId is required!!!");
        }

    }
    public class GetAllSubTopicBySubCourseIdValidator : AbstractValidator<Req.TopicById>
    {
        public GetAllSubTopicBySubCourseIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubjectId is Required");
        }
    }
}
