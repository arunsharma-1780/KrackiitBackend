using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class SubTopicValidation:ISubTopicValidation
    {
        public CreateSubTopicValidator CreateSubTopicValidator { get; set; } = new();
        public EditSubTopicValidator EditSubTopicValidator { get; set; } = new();

        public GetSubTopicByIdValidator GetSubTopicByIdValidator { get; set; } = new();
        public DeleteSubTopicValidator DeleteSubTopicValidator { get; set; } = new();
        public GetAllSubTopicBySubCourseIdValidator GetAllSubTopicBySubCourseIdValidator { get; set; } = new();
    }
}
