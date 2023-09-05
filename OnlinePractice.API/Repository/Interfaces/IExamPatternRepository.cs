using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IExamPatternRepository
    {
        public Task<Guid> Create(Req.CreateExamPattern createExamPattern);
        public Task<bool> CheckSubjectExists(Req.CheckSubject checkSubject);
        public Task<bool> IsDuplicate(Req.CheckExamPatternName examPatternName);
        public Task<Res.ExamPattern?> GetById(Req.GetByExamPatternId examPatternId);
        public Task<bool> Edit(Req.EditExamPattern editExamPattern);
        public Task<bool> EditGeneralInstruction(Req.EditGeneralInstruction editGeneralInstruction);
        public Task<bool> Delete(Req.GetExamPatternId examPatternId);
        public Task<Res.ExamPatternList> GetAllExamPattern(Req.GetAllExamPattern allExamPattern);
        public Task<bool> CheckPatternandIdExists(Req.CheckExamPatterNameAndId patterNameAndId);
        public Task<bool> IsExamPatternIdExist(Req.GetExamPatternId examPatternId);
        public Task<Res.ExamPatternSubjectList> GetAllSubjectByExamPatternId(Req.GetByExamPatternId examPattern);
        public Task<Res.ExamPatternSectionList> GetSectionListByPatternIdandSubjectId(Req.GetSectionList sectionList);
        public Task<string> GetSectionName(Guid Id);
        public Task<string> GetExamPatternName(Guid Id);

    }
}
