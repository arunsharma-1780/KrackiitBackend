using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Services
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ModuleRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Create(Req.CreateModule module)
        {
            if (module != null)
            {
                DM.Module mod = new()
                {
                    ModuleName = module.ModuleName,
                    ModuleType = module.ModuleType,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = module.UserId,
                    IsActive = true,
                    IsDeleted = false
                    
                };
                var result = await _unitOfWork.Repository<DM.Module>().Insert(mod);
                return result > 0;
            }
            return false;
        }
        public async Task<bool> Edit(Req.EditModule module)
        {
            var moduleDetails = await _unitOfWork.Repository<DM.Module>().GetSingle(x=>x.Id == module.Id && !x.IsDeleted);
            if (moduleDetails != null)
            {
                moduleDetails.ModuleName= module.ModuleName;
                moduleDetails.LastModifierUserId = module.UserId;
                moduleDetails.LastModifyDate = DateTime.UtcNow;
                var result = await _unitOfWork.Repository<DM.Module>().Update(moduleDetails);
                return result > 0;
            }
            return false;
        }
        public async Task<bool> Delete(Req.GetModule module)
        {
            var moduleDetails = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.Id == module.Id && !x.IsDeleted);
            if (moduleDetails != null)
            {
                moduleDetails.IsDeleted = true;
                moduleDetails.IsActive = false;
                moduleDetails.DeleterUserId = module.UserId;
                moduleDetails.DeletionDate = DateTime.UtcNow;
                var result = await _unitOfWork.Repository<DM.Module>().Update(moduleDetails);
                return result > 0;
            }
            return false;
        }
        public async Task<Res.ModuleDetailsList?> GetAll()
        {
            Res.ModuleDetailsList moduleDetailsList = new();

            var moduleDetails = await _unitOfWork.Repository<DM.Module>().Get(x => !x.IsDeleted);
            if (moduleDetails.Any())
            {
                foreach (var item in moduleDetails)
                {
                    Res.ModuleDetails module = new();
                    module.Id = item.Id;
                    module.ModuleName = item.ModuleName;
                    module.ModuleType = item.ModuleType;
                    moduleDetailsList.ModuleDetails.Add(module);

                }
            }
            moduleDetailsList.TotalRecords = moduleDetailsList.ModuleDetails.Count;
            if(moduleDetailsList.TotalRecords > 0)
            {
                return moduleDetailsList;
            }
            return null;
        }

        public async Task<bool> CheckModuleType(Req.CheckModuleType model)
        {
            var moduleDetails = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.ModuleType == model.ModuleType && !x.IsDeleted);
            if (moduleDetails != null)
            {
                return false;
            }
            return true;
            
        }

        public async Task<bool> CheckModule(List<Req.ModuleList> moduleList)
        {
            bool checkModule = false;
            foreach(var module in moduleList)
            {
                var result = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.Id == module.ModuleId && !x.IsDeleted);
                if (result != null)
                {
                    checkModule = true;
                }
                else
                {
                    return false;
                }
                    
            }
            return checkModule;

        }

        public async Task<bool> CheckEditModule(List<Req.EditModuleList> moduleList)
        {
            foreach (var module in moduleList)
            {
                var result = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.Id == module.ModuleId && !x.IsDeleted);
                if (result == null)
                    return false;
            }
            return true;

        }

    }
}
