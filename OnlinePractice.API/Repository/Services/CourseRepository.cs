using Microsoft.Data.SqlClient;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace OnlinePractice.API.Repository.Services
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly DBContext _dbContext;


        public CourseRepository(IUnitOfWork unitOfWork, IConfiguration configuration, DBContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Create CourseData
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateCourse createCourse)
        {
            DM.Course exam = new()
            {
                CourseName = createCourse.CourseName.Trim(),
                ExamTypeID = createCourse.ExamTypeId,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createCourse.UserId,
            };
            int result = await _unitOfWork.Repository<DM.Course>().Insert(exam);
            return result > 0;
        }

        /// <summary>
        /// Edit Courses Data
        /// </summary>
        /// <param name="editCourse"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditCourse editCourse)
        {
            var course = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == editCourse.Id && x.ExamTypeID == editCourse.ExamTypeId && !x.IsDeleted);
            if (course != null)
            {
                course.CourseName = editCourse.CourseName.Trim();
                course.ExamTypeID = editCourse.ExamTypeId;
                course.LastModifierUserId = editCourse.UserId;
                course.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Course>().Update(course);
                return result > 0;
            }
            return false;

        }

        /// <summary>
        /// Get Course By ID
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<Res.Courses?> GetById(Req.CourseById course)
        {
            var examTypeData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == course.Id && !x.IsDeleted);
            if (examTypeData != null)
            {
                Res.Courses result = new()
                {
                    Id = examTypeData.Id,
                    CourseName = examTypeData.CourseName,
                    ExamTypeID = examTypeData.ExamTypeID,
                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// Delete Course
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.CourseById course)
        {
            var courseData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == course.Id && !x.IsDeleted);
            if (courseData != null)
            {
                courseData.IsDeleted = true;
                courseData.IsActive = false;
                courseData.DeleterUserId = course.UserId;
                courseData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Course>().Update(courseData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Delete All Course and Related Data Method
        /// </summary>
        /// <param name="courseById"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(Req.CourseById courseById)
        {
            var couresData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseById.Id && !x.IsDeleted);
            if (couresData != null)
            {
                List<DM.SubCourse> subCourseList = await _unitOfWork.Repository<DM.SubCourse>().Get(x => x.CourseID == couresData.Id && !x.IsDeleted);
                if (subCourseList.Any())
                {
                    subCourseList.ForEach(s => s.IsActive = false);
                    subCourseList.ForEach(s => s.IsDeleted = true);
                    subCourseList.ForEach(s => s.DeleterUserId = courseById.UserId);
                    subCourseList.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.SubCourse>().Update(subCourseList);

                    foreach (var subCourse in subCourseList)
                    {
                        List<DM.SubjectCategory> subjects = await _unitOfWork.Repository<DM.SubjectCategory>().Get(x => x.SubCourseId == subCourse.Id && !x.IsDeleted);
                        if (subjects.Any())
                        {
                            subjects.ForEach(s => s.IsActive = false);
                            subjects.ForEach(s => s.IsDeleted = true);
                            subjects.ForEach(s => s.DeleterUserId = courseById.UserId);
                            subjects.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                            await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjects);
                            foreach (var topic in subjects)
                            {

                                List<DM.Topic> topics = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == topic.Id && !x.IsDeleted);

                                if (topics.Any())
                                {
                                    topics.ForEach(s => s.IsActive = false);
                                    topics.ForEach(s => s.IsDeleted = true);
                                    topics.ForEach(s => s.DeleterUserId = courseById.UserId);
                                    topics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                    await _unitOfWork.Repository<DM.Topic>().Update(topics);

                                    foreach (var subtopic in topics)
                                    {
                                        List<DM.SubTopic> subTopics = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == subtopic.Id && !x.IsDeleted);
                                        if (subTopics.Any())
                                        {
                                            subTopics.ForEach(s => s.IsActive = false);
                                            subTopics.ForEach(s => s.IsDeleted = true);
                                            subTopics.ForEach(s => s.DeleterUserId = courseById.UserId);
                                            subTopics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                            await _unitOfWork.Repository<DM.SubTopic>().Update(subTopics);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                couresData.IsDeleted = true;
                couresData.IsActive = false;
                couresData.DeleterUserId = courseById.UserId;
                couresData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Course>().Update(couresData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Get courses by exam id
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task<Res.CourseList> GetCoursesByExamId(Req.GetExamId examId)
        {
            Res.CourseList courseList = new()
            {
                Courses = new()
            };
            var courses = await _unitOfWork.Repository<DM.Course>().Get(x => x.ExamTypeID == examId.ExamId && !x.IsDeleted);
            courseList.Courses = courses
                      .Select(o => new Res.Courses
                      {
                          Id = o.Id,
                          CourseName = o.CourseName,
                          ExamTypeID = o.ExamTypeID
                      }).ToList();
            return courseList;
        }

        /// <summary>
        /// Get all courses
        /// </summary>
        /// <returns></returns>
        public async Task<Res.CourseList> GetAllCourses()
        {
            Res.CourseList courseList = new()
            {
                Courses = new()
            };
            var courses = await _unitOfWork.Repository<DM.Course>().Get(x => !x.IsDeleted);
            courseList.Courses = courses
                      .Select(o => new Res.Courses
                      {
                          Id = o.Id,
                          CourseName = o.CourseName,
                          ExamTypeID = o.ExamTypeID
                      }).ToList();
            return courseList;
        }

        /// <summary>
        /// Check if tier exist or not
        /// </summary>
        /// <param name="tier"></param>
        /// <returns></returns>
        public async Task<bool> IsExist(Req.CourseById course)
        {
            var exam = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == course.Id && !x.IsDeleted);
            if (exam != null)
                return true;
            return false;
        }
        public async Task<bool> IsDuplicate(Req.CourseName course)
        {
            var result = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.CourseName.Trim().ToLower() == course.Name.Trim().ToLower() && x.ExamTypeID == course.ExamId && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }

        public async Task<Res.InstituteData?> GetInstituteCourseData(Req.InstituteDataID instituteData)
        {
            Res.InstituteData instituteData1 = new();
            List<Res.InstituteDataList> instituteDataList = new();

            var pypDataCheck = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == instituteData.InstituteId && !x.IsDeleted);
            if (pypDataCheck != null)
            {
                var paperGuidList = pypDataCheck.Select(x => x.CourseId).Distinct().ToList();
                foreach (var item in paperGuidList)
                {
                    Res.InstituteDataList moduleData = new()
                    {
                        CourseId = item,
                    };
                    instituteData1.InstituteDataLists.Add(moduleData);
                }
            }

            var ebookDataCheck = await _unitOfWork.Repository<DM.Ebook>().Get(x => x.InstituteId == instituteData.InstituteId && !x.IsDeleted);
            if (ebookDataCheck != null)
            {
                var ebookcourseGuidList = ebookDataCheck.Select(x => x.CourseId).Distinct().ToList();

                foreach (var item in ebookcourseGuidList)
                {
                    Res.InstituteDataList moduleData = new()
                    {
                        CourseId = item
                    };
                    instituteData1.InstituteDataLists.Add(moduleData);
                }
            }

            var videoDataCheck = await _unitOfWork.Repository<DM.Video>().Get(x => x.InstituteId == instituteData.InstituteId && !x.IsDeleted);
            if (videoDataCheck != null)
            {
                var videocourseGuidList = videoDataCheck.Select(x => x.CourseId).Distinct().ToList();

                foreach (var item in videocourseGuidList)
                {
                    Res.InstituteDataList moduleData = new()
                    {
                        CourseId = item
                    };
                    instituteData1.InstituteDataLists.Add(moduleData);
                }
            }

            var mocktestDataCheck = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.InstituteId == instituteData.InstituteId && !x.IsDeleted);//x.CourseId!=Guid.Empty);
            if (mocktestDataCheck != null)
            {
                var mockcourseGuidList = mocktestDataCheck.Select(x => x.CourseId).Distinct().ToList();

                foreach (var item in mockcourseGuidList)
                {
                    Res.InstituteDataList moduleData = new()
                    {
                        CourseId = item
                    };
                    instituteData1.InstituteDataLists.Add(moduleData);
                }

            }
            var courseIdList = instituteData1.InstituteDataLists.Select(x => x.CourseId).Distinct().ToList();
            instituteData1.InstituteDataLists = new();
            foreach (var courseId in courseIdList)
            {
                Res.InstituteDataList instituteData2 = new();
                var courseName = _unitOfWork.GetContext().Courses.FirstOrDefault(x => x.Id == courseId && !x.IsDeleted);
                if (courseName != null)
                {
                    instituteData2.CourseName = courseName != null ? courseName.CourseName : "N/A";
                    instituteData2.CourseId = courseId;
                    instituteData1.InstituteDataLists.Add(instituteData2);
                }
            }
            return instituteData1;
        }

       
    }
}
