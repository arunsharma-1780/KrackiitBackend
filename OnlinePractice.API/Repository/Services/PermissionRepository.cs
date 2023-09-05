using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;



namespace OnlinePractice.API.Repository.Services
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public PermissionRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Create(Req.AssignPermission permission,Guid UserId)
        {
            if (permission != null)
            {
                foreach (var item in permission.Modules.Select(x=>x.ModuleId))
                {
                    
                    var module = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.Id == item && !x.IsDeleted);
                    if (module == null) continue;
                    var permissions = await _unitOfWork.Repository<DM.Permission>().GetSingle(x => x.StaffUserId == permission.UserId &&  x.ModuleId == item);
                    if (permissions == null)
                    {
                        DM.Permission prMission = new();
                        prMission.ModuleName = module.ModuleName;
                        prMission.ModuleId = module.Id;
                        prMission.StaffUserId = permission.UserId;
                        prMission.CreationDate = DateTime.UtcNow;
                        prMission.CreatorUserId = UserId;
                        prMission.IsDeleted = false;
                        prMission.IsActive = true;
                        await _unitOfWork.Repository<DM.Permission>().Insert(prMission);
                    }
                    else
                    {
                        permissions.IsDeleted = false;
                        permissions.IsActive = true;
                        permissions.LastModifyDate = DateTime.UtcNow;
                        permissions.LastModifierUserId = UserId;
                        await _unitOfWork.Repository<DM.Permission>().Update(permissions);

                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Req.EditPermission permission)
        {
            if (permission != null)
            {
                foreach (var item in permission.Modules)
                {
                    var Permission = await _unitOfWork.Repository<DM.Permission>().GetSingle(x => x.ModuleId == item.ModuleId &&  x.StaffUserId == permission.StaffUserId);
                    if (Permission != null)
                    {
                        Permission.IsActive = item.Value;
                        Permission.IsDeleted = !item.Value;
                        Permission.LastModifierUserId = permission.UserId;
                        Permission.LastModifyDate = DateTime.UtcNow;
                        await _unitOfWork.Repository<DM.Permission>().Update(Permission);
                    }
                    else if(item.Value)
                    {
                        var module = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.Id == item.ModuleId  && !x.IsDeleted);
                        if (module == null) continue;
                        DM.Permission prMission = new();
                        prMission.ModuleName = module.ModuleName;
                        prMission.ModuleId = module.Id;
                        prMission.StaffUserId = permission.StaffUserId;
                        prMission.CreatorUserId = permission.UserId;
                        prMission.CreationDate = DateTime.UtcNow;
                        prMission.IsDeleted = false;
                        prMission.IsActive = true;
                        await _unitOfWork.Repository<DM.Permission>().Insert(prMission);
                    }

                }
                return true;
            }
            return false;

        }
        public async Task<string[]> GetPermission(string staffUserId)
        {
            Guid UserId = new(staffUserId);
            List<Res.Permission> prMissions = new();
            var module = await _unitOfWork.Repository<DM.Permission>().Get(x => x.StaffUserId == UserId && !x.IsDeleted && x.IsActive);
            foreach (var item in module)
            {
                Res.Permission prMission = new();
                prMission.ModuleName = item.ModuleName;
                prMissions.Add(prMission);
            }
            string[] Modules = prMissions.Select(i => i.ModuleName.ToString()).ToArray();
            return Modules;
        }

    }
}

