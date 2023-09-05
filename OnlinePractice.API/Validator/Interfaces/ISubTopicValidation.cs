namespace OnlinePractice.API.Validator.Interfaces
{
    public interface ISubTopicValidation
    {
         public CreateSubTopicValidator CreateSubTopicValidator { get; set; }
        public EditSubTopicValidator EditSubTopicValidator { get; set; }

        public GetSubTopicByIdValidator GetSubTopicByIdValidator { get; set; }
        public DeleteSubTopicValidator DeleteSubTopicValidator { get; set; }
        public GetAllSubTopicBySubCourseIdValidator GetAllSubTopicBySubCourseIdValidator { get; set; }
    }
}
