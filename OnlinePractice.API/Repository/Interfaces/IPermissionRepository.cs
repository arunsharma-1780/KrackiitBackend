using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IPermissionRepository
    {
        public Task<bool> Create(Req.AssignPermission permission, Guid UserId);
        public Task<string[]> GetPermission(string staffUserId);
        public Task<bool> Update(Req.EditPermission permission);
    }
}
