using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ISubTopicRepository
    {
        public Task<bool> Create(Req.CreateSubTopic createSubTopic);
        public Task<bool> Edit(Req.EditSubTopic editSubTopic);
        public Task<Res.SubTopic?> GetById(Req.SubTopicById subTopic);
        public Task<bool> Delete(Req.SubTopicById subTopic);
        public Task<Res.SubTopicList> GetAllSubTopicsbyTopicsId(Req.TopicById topic);
        public Task<bool> IsDuplicate(Req.SubtopicName subtopicName);
    }
}
