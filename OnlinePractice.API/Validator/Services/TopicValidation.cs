using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class TopicValidation:ITopicValidation
    {
        public CreateTopicValidator CreateTopicValidator { get; set; } = new();
        public EditTopicValidator EditTopicValidator { get; set; } = new();

        public GetTopicByIdValidator GetTopicByIdValidator { get; set; } = new();
        public DeleteTopicValidator DeleteTopicValidator { get; set; } = new();
        public GetAllTopicBySubCourseIdValidator GetAllTopicBySubCourseIdValidator { get; set; } = new();
        public GetAllTopicsValidator GetAllTopicsValidator { get; set; } =new();
    }
}
