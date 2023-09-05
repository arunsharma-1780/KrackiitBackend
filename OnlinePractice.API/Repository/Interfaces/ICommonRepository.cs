using Com = OnlinePractice.API.Models.Common;
namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ICommonRepository
    {
        public Task<bool> CheckUserPermission(Com.CheckPermission checkPermission);
    }
}
