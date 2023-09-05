using Org.BouncyCastle.Ocsp;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IExamTypeRepository
    {
        public Task<bool> Create(Req.CreateExamType examType);
        public Task<bool> Edit(Req.EditExamType editExam);
        public Task<Models.Response.ExamType?> GetById(Req.ExamTypeById examType);
        public Task<bool> Delete(Req.ExamTypeById examType);
        public Task<bool> CreateExamFlow(Req.CreateExamFlow examFlow);
        public Task<Res.ExamTypeList> GetAllExamType();
        public Task<bool> DeleteAll(Req.ExamTypeById examType);
        public Task<bool> IsExist(Req.ExamTypeById examType);
        public Task<bool> IsDuplicate(Req.ExamName exam);
        public Task<Res.ExamList> GetExamListWithCourse();
    }
}
