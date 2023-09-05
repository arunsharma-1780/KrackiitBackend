using OnlinePractice.API.Models;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Com = OnlinePractice.API.Models.Common;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Repository.Services
{
    public class CommonRepository : ICommonRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommonRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) { 
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<bool> CheckUserPermission(Com.CheckPermission checkPermission)
        {
            var userInfo = await _userManager.FindByIdAsync(checkPermission.UserId.ToString());
            var moduleInfo = await GetModuleInfo(checkPermission.Module);
            if (userInfo != null && moduleInfo != null)
            {
                var checkRole = await _userManager.GetRolesAsync(userInfo);
                if (checkRole.Contains(UserRoles.Staff))
                {
                    var userPermissionDetails = await _unitOfWork.Repository<DM.Permission>().GetSingle(x => x.StaffUserId == checkPermission.UserId && x.ModuleId == moduleInfo.Id && !x.IsDeleted);
                    if (userPermissionDetails == null)
                    {
                        return false;
                    }
                    return true;
                }
                return true;

            }
            return false;

        }

        public async Task<DM.Module?> GetModuleInfo(ModuleType modules)
        {
            var moduleDetails = await _unitOfWork.Repository<DM.Module>().GetSingle(x => x.ModuleType == modules && !x.IsDeleted);
            return moduleDetails;
        }
    }
}
