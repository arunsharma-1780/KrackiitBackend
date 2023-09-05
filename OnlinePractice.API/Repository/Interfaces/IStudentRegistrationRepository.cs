using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using Microsoft.AspNetCore.Mvc;
using IronXL;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IStudentRegistrationRepository
    {
        public Task<Res.StudentList> GetAllStudent(Req.GetAllStudent allStudent);
        public Task<Com.ResultMessageAdmin> AddStudent([FromBody] Req.AddStudent student);
        public Task<Com.ResultMessageAdmin> EditStudent(Req.UpdateStudent student);
        public Task<Res.StudentByIdInfo?> GetStudentById(Req.GetStudentById student);
        public Task<bool> RemoveStudent(Req.GetStudentById student);
        public Task<bool>BulkUpload([FromForm] Req.BulkUpload bulkUpload);
        public FileStreamResult? GetSample();
        public  Task<bool> UpdateRecords();
    }
}
