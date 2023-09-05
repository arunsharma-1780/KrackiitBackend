namespace OnlinePractice.API.Models.Request
{
    public class CreateSubTopic : CurrentUser
    {
        public string SubTopicName { get; set; } = string.Empty;
        public string SubTopicDescription { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
    }

    public class EditSubTopic : CreateSubTopic
    {
        public Guid Id { get; set; }
    }

    public class SubTopicById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetSubTopic
    {
        public Guid TopicId { get; set; }
    }
    public class SubtopicName
    {
        public string Name { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
    }
}
