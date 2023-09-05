using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Org.BouncyCastle.Ocsp;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Repository.Services
{
    public class SubCourseRepository : ISubCourseRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubCourseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create SubCourse Method
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateSubCourse subCourse)
        {
            DM.SubCourse course = new()
            {
                SubCourseName = subCourse.SubCourseName.Trim(),
                CourseID = subCourse.CourseId,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = subCourse.UserId,
            };
            int result = await _unitOfWork.Repository<DM.SubCourse>().Insert(course);
            return result > 0;
        }

        /// <summary>
        /// Edit SubCourseData Method
        /// </summary>
        /// <param name="editSubCourse"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditSubCourse editSubCourse)
        {

            var subCourse = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == editSubCourse.Id && !x.IsDeleted);
            if (subCourse != null)
            {
                subCourse.SubCourseName = editSubCourse.SubCourseName.Trim();
                subCourse.Id = editSubCourse.Id;
                subCourse.LastModifierUserId = editSubCourse.UserId;
                subCourse.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.SubCourse>().Update(subCourse);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// EditMultiple SubCourse Data Method
        /// </summary>
        /// <param name="editSubCourse"></param>
        /// <returns></returns>
        public async Task<bool> EditMultiple(Req.EditMultipleSubCourse editSubCourse)
        {
            var course = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == editSubCourse.CourseID && !x.IsDeleted);
            if (course != null)
            {

                if (editSubCourse.SubCourses.Any())
                {
                    foreach (var item in editSubCourse.SubCourses)
                    {
                        var subCourse = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == item.Id && !x.IsDeleted && x.CourseID == editSubCourse.CourseID);
                        if (subCourse != null)
                        {
                            subCourse.SubCourseName = item.SubCourseName.Trim();
                            subCourse.CourseID = editSubCourse.CourseID;
                            subCourse.LastModifierUserId = editSubCourse.UserId;
                            subCourse.LastModifyDate = DateTime.UtcNow;
                            await _unitOfWork.Repository<DM.SubCourse>().Update(subCourse);

                        }
                    }
                    course.CourseName = editSubCourse.CourseName.Trim();
                    course.LastModifierUserId = editSubCourse.UserId;
                    course.LastModifyDate = DateTime.UtcNow;
                    int result = await _unitOfWork.Repository<DM.Course>().Update(course);
                    return result > 0;

                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Get subCoursebyId
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        public async Task<Res.SubCourse?> GetById(Req.SubCourseById subCourse)
        {
            var subCourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourse.Id && !x.IsDeleted);
            if (subCourseData != null)
            {
                Models.Response.SubCourse result = new()
                {
                    Id = subCourseData.Id,
                    SubCourseName = subCourseData.SubCourseName,
                    CourseID = subCourseData.CourseID
                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// Delete SubCourseData Method
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.SubCourseById subCourse)
        {
            var subCourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourse.Id && !x.IsDeleted);
            if (subCourseData != null)
            {
                subCourseData.IsDeleted = true;
                subCourseData.IsActive = false;
                subCourseData.DeleterUserId = subCourse.UserId;
                subCourseData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.SubCourse>().Update(subCourseData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Delete SubCourse and Related Data method
        /// </summary>
        /// <param name="subCourseById"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(Req.SubCourseById subCourseById)
        {
            var subCourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseById.Id && !x.IsDeleted);
            if (subCourseData != null)
            {
                List<DM.SubjectCategory> subjects = await _unitOfWork.Repository<DM.SubjectCategory>().Get(x => x.SubCourseId == subCourseById.Id && !x.IsDeleted);
                if (subjects.Count > 0)
                {
                    subjects.ForEach(s => s.IsActive = false);
                    subjects.ForEach(s => s.IsDeleted = true);
                    subjects.ForEach(s => s.DeleterUserId = subCourseById.UserId);
                    subjects.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjects);
                    foreach (var topic in subjects)
                    {

                        List<DM.Topic> topics = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == topic.Id && !x.IsDeleted);

                        if (topics.Any())
                        {
                            topics.ForEach(s => s.IsActive = false);
                            topics.ForEach(s => s.IsDeleted = true);
                            topics.ForEach(s => s.DeleterUserId = subCourseById.UserId);
                            topics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                            await _unitOfWork.Repository<DM.Topic>().Update(topics);

                            foreach (var subtopic in topics)
                            {
                                List<DM.SubTopic> subTopics = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == subtopic.Id && !x.IsDeleted);
                                if (subTopics.Any())
                                {
                                    subTopics.ForEach(s => s.IsActive = false);
                                    subTopics.ForEach(s => s.IsDeleted = true);
                                    subTopics.ForEach(s => s.DeleterUserId = subCourseById.UserId);
                                    subTopics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                    await _unitOfWork.Repository<DM.SubTopic>().Update(subTopics);
                                }
                            }
                        }
                    }
                }
                subCourseData.IsDeleted = true;
                subCourseData.IsActive = false;
                subCourseData.DeleterUserId = subCourseById.UserId;
                subCourseData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.SubCourse>().Update(subCourseData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetSubcourses List by CourseId
        /// </summary>
        /// <param name="courseById"></param>
        /// <returns></returns>
        public async Task<Res.SubCourseDataList?> GetSubCoursesByCourseId(Req.CourseById courseById)
        {
            Res.SubCourseDataList subCourses = new()
            {

                SubCourses = new()
            };
            var courses = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseById.Id && !x.IsDeleted);

            if (courses != null)
            {
                subCourses.CourseID = courses.Id;
                subCourses.CourseName = courses.CourseName;
                var subCourseList = await _unitOfWork.Repository<DM.SubCourse>().Get(x => x.CourseID == courseById.Id && !x.IsDeleted);
                subCourses.SubCourses = subCourseList
                          .Select(o => new Res.SubCourses
                          {
                              Id = o.Id,
                              SubCourseName = o.SubCourseName
                          }).ToList();
                if(subCourseList.Count == 0)
                {
                    subCourses.SubCourses = null;
                }
                return subCourses;
            }
            return null;
        }

        /// <summary>
        /// IsExist Method
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>

        public async Task<bool> IsExist(Req.SubCourseById subCourse)
        {
            var exam = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourse.Id && !x.IsDeleted);
            if (exam != null)
                return true;
            return false;
        }

        /// <summary>
        /// IsDuplicate Method
        /// </summary>
        /// <param name="subCourse"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.SubCourseName subCourse)
        {
            var result = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.SubCourseName.Trim().ToLower() == subCourse.Name.Trim().ToLower() && x.CourseID == subCourse.CourseId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }
    }
}
