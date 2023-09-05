using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ISubCourseRepository
    {
        public Task<bool> Create(Req.CreateSubCourse subCourse);
        public Task<bool> Edit(Req.EditSubCourse editSubCourse);
        public Task<Res.SubCourse?> GetById(Req.SubCourseById subCourse);
        public Task<bool> Delete(Req.SubCourseById subCourse);
        public Task<Res.SubCourseDataList?> GetSubCoursesByCourseId(Req.CourseById courseById);
        public Task<bool> IsExist(Req.SubCourseById subCourse);
        public Task<bool> EditMultiple(Req.EditMultipleSubCourse editSubCourse);
        public Task<bool> DeleteAll(Req.SubCourseById subCourseById);
        public Task<bool> IsDuplicate(Req.SubCourseName subCourse);
    }
}
