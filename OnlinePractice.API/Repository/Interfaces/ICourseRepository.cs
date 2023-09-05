using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ICourseRepository
    {
        public Task<bool> Create(Req.CreateCourse createCourse);
        public Task<bool> Edit(Req.EditCourse editCourse);
        public Task<Res.Courses?> GetById(Req.CourseById course);
        public Task<bool> Delete(Req.CourseById course);
        public Task<Res.CourseList> GetCoursesByExamId(Req.GetExamId examId);
        public Task<Res.CourseList> GetAllCourses();
        public Task<bool> IsExist(Req.CourseById course);
        public Task<bool> DeleteAll(Req.CourseById courseById);
        public  Task<bool> IsDuplicate(Req.CourseName course);
        public Task<Res.InstituteData?> GetInstituteCourseData(Req.InstituteDataID instituteData);
    }
}
