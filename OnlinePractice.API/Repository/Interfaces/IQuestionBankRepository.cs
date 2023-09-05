using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Common;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IQuestionBankRepository
    {
        public Task<bool> CreateQuestionBank(Req.CreateQuestionBank questionBank);
        public Task<Res.QuestionBank?> GetByRefId(Req.GetQuestionBank questionBank);
        public Task<Res.QuestionBankList?> GetAll(Req.GetAllQuestion questionBank);
        public Task<bool> DeleteQuestionByRefId(Req.QuestionBankRefId bankRefId);
        public List<EnumModel> GetQuestionType();
        public List<EnumModel> GetQuestionLevel();
        public List<EnumModel> GetQuestionLanguage();
        public Task<bool> CheckSubjectCategoryExist(CheckSubjectCategoryId checkSubject);
        public Task<bool> CheckTopicExist(CheckTopicId checkTopic);
        public Task<bool> CheckSubTopicExists(CheckSubtopicId checkSubtopic);
        public Task<bool> CheckReferenceIdExists(CheckReference checkReference);
        public Task<bool> CheckRefIdExists(CheckReference checkReference);
        public Task<bool> EditQuestionBank(Req.EditQuestionBank questionBank);
        public Task<bool> CheckUserIdExists(CheckUserExist userExist);
        public Res.QuestionBankList? GetAll50(Req.GetAll50Question questionBank);
    }

}
