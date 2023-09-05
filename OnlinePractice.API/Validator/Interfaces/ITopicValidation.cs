namespace OnlinePractice.API.Validator.Interfaces
{
    public interface ITopicValidation
    {
        public CreateTopicValidator CreateTopicValidator { get; set; }
        public EditTopicValidator EditTopicValidator { get; set; }

        public GetTopicByIdValidator GetTopicByIdValidator { get; set; }
        public DeleteTopicValidator DeleteTopicValidator { get; set; }
        public GetAllTopicBySubCourseIdValidator GetAllTopicBySubCourseIdValidator { get; set; }
        public GetAllTopicsValidator GetAllTopicsValidator { get; set; }
    }
}
