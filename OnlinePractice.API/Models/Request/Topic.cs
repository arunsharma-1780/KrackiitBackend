namespace OnlinePractice.API.Models.Request
{
    public class CreateTopic : CurrentUser
    {
        public string TopicName { get; set; } = string.Empty;
        public Guid SubjectCategoryId { get; set; }
    }

    public class EditTopic : CreateTopic
    {
        public Guid Id { get; set; }
    }

    public class TopicById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllTopics
    {
        public Guid SubCourseId { get; set; }
        public Guid SubjectId { get; set; }
    }
    public class GetTopic
    {
        public Guid SubjectId { get; set; }
    }

    public class TopicName
    {
        public Guid SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;

    }

}
