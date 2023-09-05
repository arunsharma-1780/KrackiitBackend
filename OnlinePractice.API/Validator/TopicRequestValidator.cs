using FluentValidation;
using OnlinePractice.API.Models.Request;
using System.Data;
using Req=OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateTopicValidator : AbstractValidator<Req.CreateTopic>
    {
        public CreateTopicValidator()
        {
            RuleFor(prop => prop.TopicName.Trim()).NotEmpty().WithMessage("TopicName must not be empty").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Topic Name is invalid"); 
            RuleFor(prop => prop.SubjectCategoryId).NotEmpty().NotNull().WithMessage("SubjectCategory Id is required!");
        }
    }
    public class EditTopicValidator : AbstractValidator<Req.EditTopic>
    {
        public EditTopicValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("TopicId is required!!!");
            RuleFor(prop => prop).SetValidator(new CreateTopicValidator());
        }
    }

    public class GetTopicByIdValidator : AbstractValidator<Req.TopicById>
    {
        public GetTopicByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("TopicId is required!!!");
        }
    }
    public class GetAllTopicsValidator : AbstractValidator<Req.GetAllTopics>
    {
        public GetAllTopicsValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!!!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!!!");
        }
    }
    public class DeleteTopicValidator : AbstractValidator<Req.TopicById>
    {
        public DeleteTopicValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("TopicId is required!!!");
        }

    }
    public class GetAllTopicBySubCourseIdValidator : AbstractValidator<Req.GetTopic>
    {
        public GetAllTopicBySubCourseIdValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is Required");
        }
    }

}
