namespace OnlinePractice.API.Models.Response
{
    public class SubTopicList
    {
        public List<SubTopic> SubTopics { get; set; } = new();
    }
    public class SubTopic
    {
        public Guid Id { get; set; }
        public string SubTopicName { get; set; } = string.Empty;
        public string SubTopicDescription { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
    }
}
