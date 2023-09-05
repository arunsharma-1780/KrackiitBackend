
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IModuleRepository
    {
        public  Task<bool> CheckModule(List<Req.ModuleList> moduleList);
        public Task<bool> CheckEditModule(List<Req.EditModuleList> moduleList);
        public Task<bool> Create(Req.CreateModule module);
        public Task<bool> Edit(Req.EditModule module);
        public Task<bool> Delete(Req.GetModule module);
        public Task<Res.ModuleDetailsList?> GetAll();
        public Task<bool> CheckModuleType(Req.CheckModuleType model);
    }
}
