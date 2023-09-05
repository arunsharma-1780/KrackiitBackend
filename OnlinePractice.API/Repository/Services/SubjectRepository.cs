using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Services
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// CreateSubject
        /// </summary>
        /// <param name="createSubject"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateSubject createSubject)
        {
            DM.Subject subject = new()
            {
                SubjectName = createSubject.SubjectName,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createSubject.UserId,
            };
            int result = await _unitOfWork.Repository<DM.Subject>().Insert(subject);
            return result > 0;
        }

        /// <summary>
        /// CreateSubjectCategory Method
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
        public async Task<bool> CreateSubjectCategory(Req.CreateSubjectCategory create)
        {
            foreach (var item in create.SubjectIds)
            {
                var subjectCat = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubjectId == item.Id && x.SubCourseId == create.SubCourseId);
                if (subjectCat != null)
                {
                    subjectCat.IsActive = true;
                    subjectCat.IsDeleted = false;
                    subjectCat.LastModifierUserId = create.UserId;
                    subjectCat.LastModifyDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjectCat);
                }
                else
                {
                    DM.SubjectCategory subjectCategory = new()
                    {
                        SubCourseId = create.SubCourseId,
                        SubjectId = item.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = create.UserId
                    };
                    await _unitOfWork.Repository<DM.SubjectCategory>().Insert(subjectCategory);
                }

            }
            return true;
        }

        /// <summary>
        /// UpdatesubjectCategory Method
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        public async Task<bool> UpdatesubjectCategory(Req.EditSubjectCategory edit)
        {
            foreach (var item in edit.SubjectIds)
            {
                var subjectCat = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubjectId == item.Id && x.SubCourseId == edit.SubCourseId);
                if (subjectCat != null)
                {
                    subjectCat.IsActive = item.Value;
                    subjectCat.IsDeleted = !item.Value;
                    subjectCat.LastModifierUserId = edit.UserId;
                    subjectCat.LastModifyDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjectCat);
                }
                else if (item.Value)
                {
                    DM.SubjectCategory subjectCategory = new()
                    {
                        SubCourseId = edit.SubCourseId,
                        SubjectId = item.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = edit.UserId
                    };
                    await _unitOfWork.Repository<DM.SubjectCategory>().Insert(subjectCategory);
                }

            }
            return true;
        }

        /// <summary>
        /// GetAll Subjects
        /// </summary>
        /// <returns></returns>
        public async Task<Res.SubjectList> GetAllSubjects(Req.GetAllSubject subject)
        {
            Res.SubjectList subjectList = new()
            {
                Subjects = new()
            };
            if (subject.PageNumber == 0 && subject.PageSize == 0)
            {
                var subjects = await _unitOfWork.Repository<DM.Subject>().Get(x => !x.IsDeleted,orderBy: x=>x.OrderBy(x=>x.SubjectName));
         var subjectsList = subjects
                          .Select(o => new Res.Subject
                          {
                              Id = o.Id,
                              SubjectName = o.SubjectName
                          }).ToList();
                subjectList.Subjects = subjectsList;
                subjectList.TotalRecords = subjects.Count;
                return subjectList;
            }
            else 
            {
                var subjects = await _unitOfWork.Repository<DM.Subject>().Get(x => !x.IsDeleted, orderBy: x => x.OrderBy(x => x.SubjectName));
                subjectList.Subjects = subjects
                          .Select(o => new Res.Subject
                          {
                              Id = o.Id,
                              SubjectName = o.SubjectName
                          }).ToList();
                var result = subjectList.Subjects.Page(subject.PageNumber, subject.PageSize);
                subjectList.Subjects = result.ToList();
                subjectList.TotalRecords = subjects.Count;
                return subjectList;
            }

        }

        /// <summary>
        /// EditSubjectData
        /// </summary>
        /// <param name="editSubject"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditSubject editSubject)
        {
            var subjectData = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == editSubject.Id && !x.IsDeleted);
            if (subjectData != null)
            {
                subjectData.SubjectName = editSubject.SubjectName.Trim();
                subjectData.LastModifierUserId = editSubject.UserId;
                subjectData.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Subject>().Update(subjectData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetSubject byID
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<Res.Subject?> GetById(Req.SubjectById subject)
        {
            var subjectData = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subject.Id && !x.IsDeleted);
            if (subjectData != null)
            {
                Res.Subject result = new()
                {
                    Id = subjectData.Id,
                    SubjectName = subjectData.SubjectName
                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// Delete SubjectData by Id
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.SubjectById subject)
        {
            var subjectData = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subject.Id && !x.IsDeleted);
            if (subjectData != null)
            {
                subjectData.IsDeleted = true;
                subjectData.IsActive = false;
                subjectData.DeleterUserId = subject.UserId;
                subjectData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Subject>().Update(subjectData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetAllSubjectsbySubCourseId Method
        /// </summary>
        /// <param name="subjects"></param>
        /// <returns></returns>
        public async Task<Res.SubjectCategoryList> GetAllSubjectsbySubCourseId(Req.GetSubjects subjects)
        {
            Res.SubjectCategoryList subjectList = new();

            subjectList.SubjectCategories = await (from subjectCategory in _unitOfWork.GetContext().SubjectCategories
                                                   join subject in _unitOfWork.GetContext().Subjects on subjectCategory.SubjectId equals subject.Id
                                                   join subCourse in _unitOfWork.GetContext().SubCourses on subjectCategory.SubCourseId equals subCourse.Id
                                                   where !subjectCategory.IsDeleted && !subject.IsDeleted && !subCourse.IsDeleted && subjects.SubCourseId == subCourse.Id && subjectCategory.SubjectId == subject.Id
                                                   select new Res.SubjectCategory
                                                   {
                                                       SubjectCategoryId = subjectCategory.Id,
                                                       SubjectName = subject.SubjectName,
                                                   }).ToListAsync();
            return subjectList;
        }

        /// <summary>
        /// GetAllSubjectsMasterbySubCourseId Method
        /// </summary>
        /// <param name="subjects"></param>
        /// <returns></returns>
        public async Task<Res.SubjectList> GetAllSubjectsMasterbySubCourseId(Req.GetSubjects subjects)
        {
            Res.SubjectList subjectList = new();

            subjectList.Subjects = await (from subjectCategory in _unitOfWork.GetContext().SubjectCategories
                                          join subject in _unitOfWork.GetContext().Subjects on subjectCategory.SubjectId equals subject.Id
                                          join subCourse in _unitOfWork.GetContext().SubCourses on subjectCategory.SubCourseId equals subCourse.Id
                                          where !subjectCategory.IsDeleted && !subject.IsDeleted && !subCourse.IsDeleted && subjects.SubCourseId == subCourse.Id && subjectCategory.SubjectId == subject.Id
                                          select new Res.Subject
                                          {
                                              Id = subject.Id,
                                              SubjectName = subject.SubjectName,
                                          }).ToListAsync();
            return subjectList;
        }


        /// <summary>
        /// checkifexistMethod
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<bool> IsExist(Req.SubjectById subject)
        {
            var subjectData = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subject.Id && !x.IsDeleted);
            if (subjectData != null)
                return true;
            return false;
        }

        /// <summary>
        /// IsExistCategory Method
        /// </summary>
        /// <param name="subjectCategory"></param>
        /// <returns></returns>
        public async Task<bool> IsExistCategory(Req.SubjectCategoryById subjectCategory)
        {
            var subjectCategoryData = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == subjectCategory.Id && !x.IsDeleted);
            if (subjectCategoryData != null)
                return true;
            return false;
        }

        /// <summary>
        /// Delete Subject and thier Child Reference
        /// </summary>
        /// <param name="subjectById"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(Req.SubjectById subjectById)
        {
            var subjectData = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectById.Id && !x.IsDeleted);
            if (subjectData != null)
            {
                List<DM.SubjectCategory> subjectCategories = await _unitOfWork.Repository<DM.SubjectCategory>().Get(x => x.SubjectId == subjectData.Id && !x.IsDeleted);
                if (subjectCategories.Any())
                {
                    subjectCategories.ForEach(s => s.IsActive = false);
                    subjectCategories.ForEach(s => s.IsDeleted = true);
                    subjectCategories.ForEach(s => s.DeleterUserId = subjectById.UserId);
                    subjectCategories.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.SubjectCategory>().Update(subjectCategories);
                    foreach (var subjectCategory in subjectCategories)
                    {
                        List<DM.Topic> topics = await _unitOfWork.Repository<DM.Topic>().Get(x => x.SubjectCategoryId == subjectCategory.Id && !x.IsDeleted);
                        if (topics.Any())
                        {
                            topics.ForEach(s => s.IsActive = false);
                            topics.ForEach(s => s.IsDeleted = true);
                            topics.ForEach(s => s.DeleterUserId = subjectById.UserId);
                            topics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                            await _unitOfWork.Repository<DM.Topic>().Update(topics);
                            foreach (var subTopic in topics)
                            {

                                List<DM.SubTopic> subTopics = await _unitOfWork.Repository<DM.SubTopic>().Get(x => x.TopicId == subTopic.Id && !x.IsDeleted);

                                if (subTopics.Any())
                                {
                                    subTopics.ForEach(s => s.IsActive = false);
                                    subTopics.ForEach(s => s.IsDeleted = true);
                                    subTopics.ForEach(s => s.DeleterUserId = subjectById.UserId);
                                    subTopics.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                                    await _unitOfWork.Repository<DM.SubTopic>().Update(subTopics);
                                }
                            }
                        }
                    }
                }
                subjectData.IsDeleted = true;
                subjectData.IsActive = false;
                subjectData.DeleterUserId = subjectById.UserId;
                subjectData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Subject>().Update(subjectData);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// IsDuplicate Method
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.SubjectName subjectName)
        {
            var result = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.SubjectName.Trim().ToLower() == subjectName.Name.Trim().ToLower() && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }

        /// <summary>
        /// Get subject name by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<string> GetSubjectName(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == Id && !x.IsDeleted);

            return result != null ? result.SubjectName : "N/A";
        }

        public async Task<string> GetSubjectNameBySubjectCategoryId(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == Id && !x.IsDeleted);
            if(result != null)
            {
                var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == result.SubjectId && !x.IsDeleted);

                return subjectDetails != null ? subjectDetails.SubjectName : string.Empty;
            }
            return string.Empty;           
        }


    }
}
