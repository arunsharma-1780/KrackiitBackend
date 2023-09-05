using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;


namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IAccountRepository
    {
        public Task<bool> ForgotPassWord(Req.ForgotPassword forgotPassword);
        public Task<string> UploadImage(Req.ProfileImage profileImage);
        public Task<bool> AddAdmin(Req.Register model);
        public string GenerateRandomPassword(PasswordOptions? opts = null);
        public Task<bool> UpdateAdmin(Req.UpdateAdmin admin);
        public Task<Res.UserById?> GetUserById(Req.GetUserById admin);
        public Task<Tokens?> Login(Req.Login login);
        public Task<bool> ResendPassword(Req.ForgotPassword password);
        public Task<bool> CheckPassword(Req.CurrentPassword password);
        public Task<bool> ChangePassWord(Req.ChangePassword password);
        public Task<bool> RemoveImage(Req.RemoveProfile profile);
    }
}
