using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using FluentValidation;

namespace OnlinePractice.API.Validator.Services
{
    public class StudentRegistrationValidation : IStudentRegistrationValidation
    {
        public GetAllStudentValidator GetAllStudentValidator { get; set; } = new();
        public AddStudentValidator AddStudentValidator { get; set; } = new();
        public UpdateStudentValidator UpdateStudentValidator { get; set; } = new();
        public GetStudentByIdValidator GetStudentByIdValidator { get; set; } = new();
        public AddBulkUploadStudentValidator AddBulkUploadStudentValidator { get; set; } = new();
    }
}
