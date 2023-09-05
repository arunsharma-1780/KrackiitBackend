using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Services
{
    public class SubTopicRepository : ISubTopicRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubTopicRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create SubTopic
        /// </summary>
        /// <param name="createSubTopic"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateSubTopic createSubTopic)
        {
            DM.SubTopic subTopic = new()
            {
                SubTopicName = createSubTopic.SubTopicName.Trim(),
                SubTopicDescription = createSubTopic.SubTopicDescription,
                TopicId = createSubTopic.TopicId,
                IsActive = true,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createSubTopic.UserId,
            };
            int result = await _unitOfWork.Repository<DM.SubTopic>().Insert(subTopic);
            return result > 0;
        }

        /// <summary>
        /// Edit SubTopicData
        /// </summary>
        /// <param name="editSubTopic"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditSubTopic editSubTopic)
        {
                var subTopicData = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == editSubTopic.Id && !x.IsDeleted);
                if (subTopicData != null)
                {
                    subTopicData.SubTopicName = editSubTopic.SubTopicName.Trim();
                    subTopicData.SubTopicDescription = editSubTopic.SubTopicDescription;
                    subTopicData.TopicId = editSubTopic.TopicId;
                    subTopicData.LastModifierUserId = editSubTopic.UserId;
                    subTopicData.LastModifyDate = DateTime.UtcNow;
                    int result = await _unitOfWork.Repository<DM.SubTopic>().Update(subTopicData);
                    return result > 0;
                }
                return false;
        }

        /// <summary>
        /// Get PArticular Subtopic ById
        /// </summary>
        /// <param name="subTopic"></param>
        /// <returns></returns>
        public async Task<Res.SubTopic?> GetById( Req.SubTopicById subTopic)
        {
            var subTopicData = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == subTopic.Id && !x.IsDeleted);
            if (subTopicData != null)
            {
                Models.Response.SubTopic result = new()
                {
                    Id = subTopicData.Id,
                    SubTopicName = subTopicData.SubTopicName,
                    SubTopicDescription = subTopicData.SubTopicDescription,
                    TopicId = subTopicData.TopicId,
                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// Delete TopicData
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.SubTopicById subTopic)
        {
            var subTopicData = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == subTopic.Id && !x.IsDeleted);
            if (subTopicData != null)
            {
                subTopicData.IsDeleted = true;
                subTopicData.IsActive = false;
                subTopicData.DeleterUserId = subTopic.UserId;
                subTopicData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.SubTopic>().Update(subTopicData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetAll topics By SubjectId
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<Res.SubTopicList> GetAllSubTopicsbyTopicsId(Req.TopicById topic)
        {
            Res.SubTopicList subTopic= new()
            {
                SubTopics = new()
            };
            var subTopicList = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == topic.Id && !x.IsDeleted);
            subTopic.SubTopics=subTopicList
                      .Select(o => new Res.SubTopic
                      {
                          Id = o.Id,
                          SubTopicName = o.SubTopicName,
                          SubTopicDescription = o.SubTopicDescription,
                          TopicId = o.TopicId
                      }).ToList();
            return subTopic;
        }

        /// <summary>
        /// IsDuplicate MethodMO
        /// </summary>
        /// <param name="subtopicName"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.SubtopicName subtopicName)
        {
            var result = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.SubTopicName.Trim().ToLower() == subtopicName.Name.Trim().ToLower() && x.TopicId == subtopicName.TopicId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;

        }
        
    }
}
