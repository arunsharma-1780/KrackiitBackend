namespace OnlinePractice.API.Models.Response
{
    public class TopicList
    {
        public List<Topic> Topic { get; set; } = new();
    }
    public class Topic
    {
        public Guid Id { get; set; }
        public string TopicName { get; set; } = string.Empty;

        public Guid SubjectCategoryId { get; set; }
    }
}
