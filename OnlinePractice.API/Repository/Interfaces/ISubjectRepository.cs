using OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ISubjectRepository
    {
        public Task<bool> Create(Req.CreateSubject createSubject);
        public Task<bool> Edit(Req.EditSubject editSubject);
        public Task<Res.Subject?> GetById(Req.SubjectById subject);
        public Task<bool> Delete(Req.SubjectById subject);
        public Task<bool> IsExist(Req.SubjectById subject);
        public Task<bool> DeleteAll(Req.SubjectById subjectById);
        public Task<bool> IsDuplicate(Req.SubjectName subjectName);
        public Task<Res.SubjectCategoryList> GetAllSubjectsbySubCourseId(Req.GetSubjects subjects);
        public  Task<bool> IsExistCategory(Req.SubjectCategoryById subjectCategory);
        public  Task<bool> CreateSubjectCategory(Req.CreateSubjectCategory create);
        public  Task<Res.SubjectList> GetAllSubjects(Req.GetAllSubject subject);
        public Task<Res.SubjectList> GetAllSubjectsMasterbySubCourseId(Req.GetSubjects subjects);
        public  Task<bool> UpdatesubjectCategory(Req.EditSubjectCategory edit);
        public Task<string> GetSubjectName(Guid Id);
        public Task<string> GetSubjectNameBySubjectCategoryId(Guid Id);
    }
}
