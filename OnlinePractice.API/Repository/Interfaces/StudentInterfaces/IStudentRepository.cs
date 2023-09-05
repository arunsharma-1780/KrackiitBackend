using OnlinePractice.API.Models.AuthDB;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IStudentRepository
    {
        public Task<Com.ResultMessage> AddStudent(Req.CreateStudent student);
        public Task<Res.StudentById?> GetStudentById(Req.GetUserById student);
        public Task<Com.ResultMessage> EditStudent(Req.EditStudent student);
        public Task<Tokens?> GetTokenByNumber(Req.Tokenlogin login);
        public Task<Res.SendOtp> SendOtp(Req.SendOTP sendOTP);
        public Task<Res.SMSBalance?> CheckSMSBalance();
        public Task<Tokens?> StudentLogin(Req.StudentLogin login);
      //  public Task<bool> LogoutStudent(Req.StudentLogout logout);
        public Task<bool> ForgotPassword(Req.ForgotStudentPassword forgotStudent);
        public Task<bool> DeleteUser(Req.MobNumber number);
        public Task<string> GetToken(CurrentUser userId);
        public Task<Com.ResultMessage> EditStudentProfile(Req.UpdateStudentProfile studentProfile);

        public Task<bool> AddFeedback(Req.AddFeedback addFeedback);
        public  Task<string> UploadImage(Req.ProfileImage profileImage);
        public bool CheckStudentLanguage(string language);
        public Task<Res.StudentInstitueList?> GetStudentInstitute(CurrentUser user);

    }
}

