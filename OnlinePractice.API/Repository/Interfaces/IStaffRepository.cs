using OnlinePractice.API.Models.AuthDB;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IStaffRepository
    {
        public Task<Com.ResultMessageAdmin> AddStaff(Req.CreateStaff staff);
        public Task<Res.Module> GetModuleList();
        public Task<Res.StaffData> GetAllStaff(Req.GetAllStaff staff);
        public Task<Res.StaffById?> GetStaffById(Req.GetByIdStaff staff);
        public Task<bool> UpdateStaff(Req.EditStaff staff);
        public Task<bool> RemoveStaff(Req.GetByIdStaff staff);
        public Task<bool> IsDuplicate(Req.InstituteCheck instituteCheck);
        public Task<Res.StaffAdminData> GetAllStaffAndAdmin();
    }
}
