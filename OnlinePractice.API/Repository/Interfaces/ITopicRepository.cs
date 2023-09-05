using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ITopicRepository
    {
        public Task<bool> Create(Req.CreateTopic createTopic);
        public Task<bool> Edit(Req.EditTopic editTopic);
        public Task<Res.Topic?> GetById(Req.TopicById topic);
        public Task<bool> Delete(Req.TopicById topic);
        public Task<Res.TopicList> GetAllTopicsbySubjectCategoryId(Req.TopicById topicById);
        public Task<bool> IsExist(Req.TopicById topic);
        public Task<bool> DeleteAll(Req.TopicById topic);
        public Task<bool> IsDuplicate(Req.TopicName topicName);
        public Task<Res.TopicList> GetAllTopicsbySubjectIdandSubCourseId(Req.GetAllTopics topics);
    }
}
