using FluentValidation.Results;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using System.Collections.Generic;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services
{
    public class ExamTypeRepository : IExamTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamTypeRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Create ExamType
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateExamType examType)
        {
            DM.ExamType exam = new()
            {
                ExamName = examType.ExamName.Trim(),
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = examType.UserId,
            };
            int result = await _unitOfWork.Repository<DM.ExamType>().Insert(exam);
            return result > 0;
        }

        /// <summary>
        /// CreateExamFlow
        /// </summary>
        /// <param name="examFlow"></param>
        /// <returns></returns>
        public async Task<bool> CreateExamFlow(Req.CreateExamFlow examFlow)
        {
            if (!string.IsNullOrEmpty(examFlow.ExamTypeName))
            {
                DM.ExamType exam = new()
                {
                    ExamName = examFlow.ExamTypeName.Trim(),
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = examFlow.UserId
                };
                int examResult = await _unitOfWork.Repository<DM.ExamType>().Insert(exam);
                if (examResult > 0 && !string.IsNullOrEmpty(examFlow.CourseName))
                {
                    DM.Course course = new()
                    {
                        CourseName = examFlow.CourseName.Trim(),
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = examFlow.UserId,
                        ExamTypeID = exam.Id,

                    };
                    int courseResult = await _unitOfWork.Repository<DM.Course>().Insert(course);
                    if (courseResult > 0 && !string.IsNullOrEmpty(examFlow.SubCourseName))
                    {
                        DM.SubCourse subCourse = new()
                        {
                            SubCourseName = examFlow.SubCourseName.Trim(),
                            IsDeleted = false,
                            IsActive = true,
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = examFlow.UserId,
                            CourseID = course.Id,

                        };
                        int subCourseResult = await _unitOfWork.Repository<DM.SubCourse>().Insert(subCourse);
                        if (subCourseResult > 0 && examFlow.SubjectIds.Any())
                        {
                            foreach (var item in examFlow.SubjectIds)
                            {
                                var subjectCat = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubjectId == item.SubjectId && x.SubCourseId == subCourse.Id);
                                if (subjectCat != null)
                                {
                                    subjectCat.IsActive = true;
                                    subjectCat.IsDeleted = false;
                                    subjectCat.LastModifierUserId = examFlow.UserId;
                                    subjectCat.LastModifyDate = DateTime.UtcNow;
                                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjectCat);
                                }
                                else
                                {
                                    DM.SubjectCategory subjectCategory = new()
                                    {
                                        SubCourseId = subCourse.Id,
                                        SubjectId = item.SubjectId,
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreationDate = DateTime.UtcNow,
                                        CreatorUserId = examFlow.UserId
                                    };
                                    await _unitOfWork.Repository<DM.SubjectCategory>().Insert(subjectCategory);
                                }

                            }
                            return true;
                        }
                        return subCourseResult > 0;
                    }
                    return courseResult > 0;
                }

                return examResult > 0;
            }
            return false;

        }


        /// <summary>
        /// GetAll ExamType
        /// </summary>
        /// <returns></returns>
        public async Task<Res.ExamTypeList> GetAllExamType()
        {
            Res.ExamTypeList examList = new()
            {
                ExamType = new()
            };
            var exams = await _unitOfWork.Repository<DM.ExamType>().Get(x => !x.IsDeleted);

            foreach (var item in exams)
            {
                Res.ExamType examType = new();
                examType.Id = item.Id;
                examType.ExamName = item.ExamName;
                examList.ExamType.Add(examType);
            }

            return examList;
        }

        /// <summary>
        /// GetExamListWithCourse
        /// </summary>
        /// <returns></returns>
        public async Task<Res.ExamList> GetExamListWithCourse()
        {
            Res.ExamList examList = new()
            {
                ExamType = new()
            };
            var exams = await _unitOfWork.Repository<DM.ExamType>().Get(x => !x.IsDeleted, orderBy: x=>x.OrderByDescending(x=>x.CreationDate));

            foreach (var item in exams)
            {
                Res.ExamTypeData examType = new();
                examType.Id = item.Id;
                examType.ExamName = item.ExamName;
                var coursesList = await _unitOfWork.Repository<DM.Course>().Get(x => x.ExamTypeID == item.Id && !x.IsDeleted && x.IsActive);
                examType.CoursesList = coursesList
               .Select(o => new Res.CoursesData
               {
                   Id = o.Id,
                   CourseName = o.CourseName,
               }).ToList();

                examList.ExamType.Add(examType);
            }
            var result = examList.ExamType.OrderBy(x=>x.ExamName).ToList();
            examList.ExamType = result;
            return examList;
        }

        /// <summary>
        /// EditExamType
        /// </summary>
        /// <param name="editExam"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditExamType editExam)
        {
            var examType = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == editExam.Id && !x.IsDeleted);
            if (examType != null)
            {
                examType.ExamName = editExam.ExamName.Trim();
                examType.LastModifierUserId = editExam.UserId;
                examType.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.ExamType>().Update(examType);
                return result > 0;
            }
            return false;

        }


        /// <summary>
        /// GetExamType byId
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        public async Task<Res.ExamType?> GetById(Req.ExamTypeById examType)
        {
            var examTypeData = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == examType.Id && !x.IsDeleted);
            if (examTypeData != null)
            {
                Res.ExamType result = new()
                {
                    Id = examTypeData.Id,
                    ExamName = examTypeData.ExamName,
                };
                return result;
            }
            return null;
        }


        /// <summary>
        /// Delete Exam type byID
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.ExamTypeById examType)
        {
            var examTypedata = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == examType.Id && !x.IsDeleted);
            if (examTypedata != null)
            {
                examTypedata.IsDeleted = true;
                examTypedata.DeleterUserId = examType.UserId;
                examTypedata.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.ExamType>().Update(examTypedata);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// DeleteAll Method
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(Req.ExamTypeById examType)
        {
            var examTypedata = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == examType.Id && !x.IsDeleted);
            if (examTypedata != null)
            {
                List<DM.Course> coursesList = await _unitOfWork.Repository<DM.Course>().Get(x => !x.IsDeleted && x.ExamTypeID == examTypedata.Id);
                if (coursesList.Count > 0)
                {
                    coursesList.ForEach(s => s.IsDeleted = true);
                    coursesList.ForEach(s => s.IsActive = false);
                    coursesList.ForEach(s => s.DeleterUserId = examType.UserId);
                    coursesList.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.Course>().Update(coursesList);

                    foreach (var item in coursesList)
                    {
                        List<DM.SubCourse> subCourseList = await _unitOfWork.Repository<DM.SubCourse>().Get(x => x.CourseID == item.Id && !x.IsDeleted);
                        if (subCourseList.Any())
                        {
                            subCourseList.ForEach(s => s.IsActive = false);
                            subCourseList.ForEach(s => s.IsDeleted = true);
                            subCourseList.ForEach(s => s.DeleterUserId = examType.UserId);
                            subCourseList.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                            await _unitOfWork.Repository<DM.SubCourse>().Update(subCourseList);

                            foreach (var subCourse in subCourseList)
                            {
                                List<DM.SubjectCategory> subjects = await _unitOfWork.Repository<DM.SubjectCategory>().Get(x => x.SubCourseId == subCourse.Id && !x.IsDeleted);
                                if (subjects.Any())
                                {
                                    subjects.ForEach(s => s.IsActive = false);
                                    subjects.ForEach(s => s.IsDeleted = true);
                                    subjects.ForEach(s => s.DeleterUserId = examType.UserId);
                                    subjects.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjects);
                                    foreach (var topic in subjects)
                                    {

                                        List<DM.Topic> topics = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == topic.Id && !x.IsDeleted);

                                        if (topics.Any())
                                        {
                                            topics.ForEach(s => s.IsActive = false);
                                            topics.ForEach(s => s.IsDeleted = true);
                                            topics.ForEach(s => s.DeleterUserId = examType.UserId);
                                            topics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                            await _unitOfWork.Repository<DM.Topic>().Update(topics);

                                            foreach (var subtopic in topics)
                                            {
                                                List<DM.SubTopic> subTopics = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == subtopic.Id && !x.IsDeleted);
                                                if (subTopics.Any())
                                                {
                                                    subTopics.ForEach(s => s.IsActive = false);
                                                    subTopics.ForEach(s => s.IsDeleted = true);
                                                    subTopics.ForEach(s => s.DeleterUserId = examType.UserId);
                                                    subTopics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                                    await _unitOfWork.Repository<DM.SubTopic>().Update(subTopics);
                                                }
                                            }
                                        }
                                    }
                                }


                            }

                        }

                    }
                }
                examTypedata.IsDeleted = true;
                examTypedata.IsActive = false;
                examTypedata.DeleterUserId = examType.UserId;
                examTypedata.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.ExamType>().Update(examTypedata);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// IsExist ExamType Method
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        public async Task<bool> IsExist(Req.ExamTypeById examType)
        {
            var exam = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == examType.Id && !x.IsDeleted);
            if (exam != null)
                return true;
            return false;
        }

        /// <summary>
        /// IsDuplicate Method
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.ExamName exam)
        {
            var result = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.ExamName.Trim().ToLower() == exam.Name.Trim().ToLower() && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }

    }
}
