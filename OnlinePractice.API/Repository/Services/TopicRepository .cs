using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Services
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public TopicRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// CreateTopicData method
        /// </summary>
        /// <param name="createTopic"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateTopic createTopic)
        {
            DM.Topic topic = new()
            {
                TopicName = createTopic.TopicName.Trim(),
                SubjectCategoryId = createTopic.SubjectCategoryId,
                IsActive = true,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createTopic.UserId,
            };
            int result = await _unitOfWork.Repository<DM.Topic>().Insert(topic);
            return result > 0;
        }

        /// <summary>
        /// EditTopic Data Method
        /// </summary>
        /// <param name="editTopic"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditTopic editTopic)
        {
                var topicData = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == editTopic.Id && !x.IsDeleted);
                if (topicData != null)
                {
                    topicData.TopicName = editTopic.TopicName.Trim();
                    topicData.SubjectCategoryId = editTopic.SubjectCategoryId;
                    topicData.LastModifierUserId = editTopic.UserId;
                    topicData.LastModifyDate = DateTime.UtcNow;
                    int result = await _unitOfWork.Repository<DM.Topic>().Update(topicData);
                    return result > 0;
                }
                return false;
        }

        /// <summary>
        /// GetTopic ByID
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task<Models.Response.Topic?> GetById(Req.TopicById topic)
        {
            var topicData = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topic.Id && !x.IsDeleted);
            if (topicData != null)
            {
                Models.Response.Topic result = new()
                {
                    Id = topicData.Id,
                    TopicName = topicData.TopicName,
                    SubjectCategoryId = topicData.SubjectCategoryId
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
        public async Task<bool> Delete(Req.TopicById topic)
        {
            var topicData = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topic.Id && !x.IsDeleted);
            if (topicData != null)
            {
                topicData.IsDeleted = true;
                topicData.DeleterUserId = topic.UserId;
                topicData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Topic>().Update(topicData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetAll topics By SubjectId
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<Res.TopicList> GetAllTopicsbySubjectCategoryId(Req.TopicById topicById)
        {
            Res.TopicList topicList = new()
            {
                Topic = new()
            };
            var topicdata = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == topicById.Id && !x.IsDeleted);
            topicList.Topic = topicdata
                      .Select(o => new Res.Topic
                      {
                          Id = o.Id,
                          TopicName = o.TopicName,
                          SubjectCategoryId = o.SubjectCategoryId
                      }).ToList();
            return topicList;
        }
        public async Task<Res.TopicList> GetAllTopicsbySubjectIdandSubCourseId(Req.GetAllTopics topics)
        {
            Res.TopicList topicList = new()
            {
                Topic = new()
            };
            var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubjectId == topics.SubjectId && x.SubCourseId == topics.SubCourseId && !x.IsDeleted);
            Guid subjectCategoryId = subjectCategory != null? subjectCategory.Id : Guid.Empty;
            var topicdata = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == subjectCategoryId && !x.IsDeleted);
            topicList.Topic = topicdata
                      .Select(o => new Res.Topic
                      {
                          Id = o.Id,
                          TopicName = o.TopicName,
                          SubjectCategoryId = o.SubjectCategoryId
                      }).ToList();
            return topicList;
        }
        /// <summary>
        /// Delete Topic and thier Reference
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(Req.TopicById topic)
        {
            var topicData = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topic.Id && !x.IsDeleted);
            if (topicData != null)
            {
                List<DM.SubTopic> subTopics = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == topicData.Id && !x.IsDeleted);
                if (subTopics.Any())
                {
                    subTopics.ForEach(s => s.IsActive = false);
                    subTopics.ForEach(s => s.IsDeleted = true);
                    subTopics.ForEach(s => s.DeleterUserId=topic.UserId);
                    subTopics.ForEach(s => s.DeletionDate=DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.SubTopic>().Update(subTopics);
                }
                topicData.IsDeleted = true;
                topicData.IsActive = false;
                topicData.DeleterUserId = topic.UserId;
                topicData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Topic>().Update(topicData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// checkifexistMethod
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<bool> IsExist(Req.TopicById topic)
        {
            var exam = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == topic.Id && !x.IsDeleted);
            if (exam != null)
                return true;
            return false;
        }

        /// <summary>
        /// Check Duplicacy Method
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.TopicName topicName)
        {
            var result = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.TopicName.Trim().ToLower() == topicName.Name.Trim().ToLower() && x.SubjectCategoryId == topicName.SubjectId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }
    }
}
