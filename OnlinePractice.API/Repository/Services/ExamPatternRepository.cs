using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
namespace OnlinePractice.API.Repository.Services
{
    public class ExamPatternRepository : IExamPatternRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubjectRepository _subjectRepository;

        public ExamPatternRepository(IUnitOfWork unitOfWork, ISubjectRepository subjectRepository)
        {
            _unitOfWork = unitOfWork;
            _subjectRepository = subjectRepository;
        }

        /// <summary>
        /// Create ExamPattern Method
        /// </summary>
        /// <param name="createExamPattern"></param>
        /// <returns></returns>
        public async Task<Guid> Create(Req.CreateExamPattern createExamPattern)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(createExamPattern.ExamPatternName))
            {
                DM.ExamPattern examPattern = new()
                {
                    ExamPatternName = createExamPattern.ExamPatternName,
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createExamPattern.UserId,
                };
                result = await _unitOfWork.Repository<DM.ExamPattern>().Insert(examPattern);
                if (result > 0 && createExamPattern.Section != null)
                {
                    foreach (var sec in createExamPattern.Section)
                    {
                        var count = 1;
                        foreach (var subsection in sec.SubSection)
                        {
                            DM.ExamPatternSection examPatternSection = new()
                            {
                                SubjectId = sec.SubjectId,
                                SectionName = "Section-" + count,
                                TotalQuestions = subsection.TotalQuestions,
                                TotalAttempt = subsection.TotalAttempt,
                                ExamPatternId = examPattern.Id,
                                IsDeleted = false,
                                IsActive = true,
                                CreationDate = DateTime.UtcNow,
                                CreatorUserId = createExamPattern.UserId,
                            };
                            count++;
                            await _unitOfWork.Repository<DM.ExamPatternSection>().Insert(examPatternSection);
                        }
                    }
                }
                return examPattern.Id;
            }
            return Guid.Empty;

        }

        /// <summary>
        /// API for ExamPattern  Get By Id  
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        public async Task<Res.ExamPattern?> GetById(Req.GetByExamPatternId examPatternId)
        {
            if (examPatternId == null)
                return null;
            var result = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == examPatternId.Id && !x.IsDeleted);
            if (result != null)
            {
                Res.ExamPattern examPattern = new();
                examPattern.TotalQuestion = 0;
                examPattern.Id = result.Id;
                examPattern.ExamPatternName = result.ExamPatternName;
                examPattern.GeneralInstruction = result.GeneralInstruction;
                var examPatternSection = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == result.Id && !x.IsDeleted);
                if (examPatternSection.Any())
                {
                    foreach (var exam in examPatternSection.DistinctBy(x => x.SubjectId))
                    {
                        Res.Section section = new();
                        section.SubjectId = exam.SubjectId;
                        section.TotalSectionQuestions = 0;
                        var subjectName = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == exam.SubjectId && !x.IsDeleted);
                        section.SubjectName = subjectName != null ? subjectName.SubjectName : "Not Found";
                        var subsection = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.SubjectId == exam.SubjectId && x.ExamPatternId == exam.ExamPatternId && !x.IsDeleted);
                        foreach (var sub in subsection)
                        {
                            Res.Subsection subsectionData = new()
                            {
                                Id = sub.Id,
                                SectionName = sub.SectionName,
                                TotalQuestions = sub.TotalQuestions,
                                TotalAttempt = sub.TotalAttempt
                            };
                            examPattern.TotalQuestion = examPattern.TotalQuestion + sub.TotalAttempt;
                            section.TotalSectionQuestions = section.TotalSectionQuestions + sub.TotalQuestions;
                            section.SubSection.Add(subsectionData);
                        }

                        examPattern.Section.Add(section);
                    }

                }
                return examPattern;
            }
            return null;

        }

        /// <summary>
        /// IsDuplicate Method
        /// </summary>
        /// <param name="examPatternName"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.CheckExamPatternName examPatternName)
        {
            var result = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.ExamPatternName.Trim().ToLower() == examPatternName.ExamPatternName.Trim().ToLower() && !x.IsDeleted);
            if (result != null)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Edit ExamPattern
        /// </summary>
        /// <param name="editExamPattern"></param>
        /// <returns></returns>
        //public async Task<bool> Edit(EditExamPattern editExamPattern)
        //{
        //    int result = 0;
        //    var examPattern = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == editExamPattern.ExamPatternId && !x.IsDeleted);
        //    if (examPattern != null)
        //    {
        //        examPattern.ExamPatternName = editExamPattern.ExamPatternName;
        //        examPattern.LastModifyDate = DateTime.UtcNow;
        //        examPattern.LastModifierUserId = editExamPattern.UserId;
        //        if (editExamPattern.Section != null && editExamPattern.Section.Any())
        //        {
        //            foreach (var sec in editExamPattern.Section)
        //            {
        //                var subjectResult = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.SubjectId == sec.SubjectId && !x.IsDeleted);
        //                if (subjectResult != null)
        //                {
        //                    foreach (var subsection in sec.SubSection)
        //                    {
        //                        var count = 0;
        //                        var examSectionPattern = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == subsection.ExamPatternSectionId && !x.IsDeleted);
        //                        if (examSectionPattern != null)
        //                        {
        //                            examSectionPattern.SubjectId = sec.SubjectId;
        //                            examSectionPattern.TotalQuestions = subsection.TotalQuestions;
        //                            examSectionPattern.TotalAttempt = subsection.TotalAttempt;
        //                            examSectionPattern.LastModifyDate = DateTime.UtcNow;
        //                            examSectionPattern.LastModifierUserId = editExamPattern.UserId;
        //                            result = await _unitOfWork.Repository<DM.ExamPatternSection>().Update(subjectResult);
        //                        }
        //                        else
        //                        {
        //                            DM.ExamPatternSection examPatternSection = new()
        //                            {
        //                                SubjectId = sec.SubjectId,
        //                                SectionName = "Section-",
        //                                TotalQuestions = subsection.TotalQuestions,
        //                                TotalAttempt = subsection.TotalAttempt,
        //                                ExamPatternId = examPattern.Id,
        //                                IsDeleted = false,
        //                                IsActive = true,
        //                                CreationDate = DateTime.UtcNow,
        //                                CreatorUserId = editExamPattern.UserId,
        //                            };
        //                            result = await _unitOfWork.Repository<DM.ExamPatternSection>().Insert(examPatternSection);
        //                        }
        //                    }
        //                }
        //                //else
        //                //{
        //                //    DM.ExamPattern examPattern1 = new()
        //                //    {
        //                //        ExamPatternName = editExamPattern.ExamPatternName,
        //                //        IsDeleted = false,
        //                //        IsActive = true,
        //                //        CreationDate = DateTime.UtcNow,
        //                //        CreatorUserId = editExamPattern.UserId,
        //                //    };
        //                //    result = await _unitOfWork.Repository<DM.ExamPattern>().Insert(examPattern1);
        //                //    if (result > 0 && editExamPattern.Section != null)
        //                //    {
        //                //        foreach (var sec1 in editExamPattern.Section)
        //                //        {
        //                //            var count = 1;
        //                //            foreach (var subsection in sec.SubSection)
        //                //            {
        //                //                DM.ExamPatternSection examPatternSection = new()
        //                //                {
        //                //                    SubjectId = sec.SubjectId,
        //                //                    SectionName = "Section-" + count,
        //                //                    TotalQuestions = subsection.TotalQuestions,
        //                //                    TotalAttempt = subsection.TotalAttempt,
        //                //                    ExamPatternId = examPattern.Id,
        //                //                    IsDeleted = false,
        //                //                    IsActive = true,
        //                //                    CreationDate = DateTime.UtcNow,
        //                //                    CreatorUserId = editExamPattern.UserId,
        //                //                };
        //                //                count++;
        //                //                result = await _unitOfWork.Repository<DM.ExamPatternSection>().Insert(examPatternSection);
        //                //            }
        //                //        }
        //                //    }
        //                return result > 0;

        //            }
        //        }
        //        result = await _unitOfWork.Repository<DM.ExamPattern>().Update(examPattern);
        //        return result > 0;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        /// <summary>
        /// Edit ExamPattern
        /// </summary>
        /// <param name="editExamPattern"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditExamPattern editExamPattern)
        {
            var examPatternSectionList = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == editExamPattern.Id && !x.IsDeleted);
            if (examPatternSectionList.Any())
            {
                await _unitOfWork.Repository<DM.ExamPatternSection>().Delete(examPatternSectionList);
            }
            int result = 0;
            var examPattern = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == editExamPattern.Id && !x.IsDeleted);
            if (examPattern != null)
            {
                examPattern.ExamPatternName = editExamPattern.ExamPatternName;
                examPattern.LastModifyDate = DateTime.UtcNow;
                examPattern.LastModifierUserId = editExamPattern.UserId;

                if (!string.IsNullOrEmpty(editExamPattern.ExamPatternName))
                {
                    if (editExamPattern.Section.Any())
                    {
                        foreach (var sec in editExamPattern.Section)
                        {
                            var count = 1;
                            foreach (var subsection in sec.SubSection)
                            {
                                DM.ExamPatternSection examPatternSection = new()
                                {
                                    SubjectId = sec.SubjectId,
                                    SectionName = "Section-" + count,
                                    TotalQuestions = subsection.TotalQuestions,
                                    TotalAttempt = subsection.TotalAttempt,
                                    ExamPatternId = examPattern.Id,
                                    IsDeleted = false,
                                    IsActive = true,
                                    CreationDate = DateTime.UtcNow,
                                    CreatorUserId = editExamPattern.UserId,
                                };
                                count++;
                                await _unitOfWork.Repository<DM.ExamPatternSection>().Insert(examPatternSection);
                            }
                        }
                        result = await _unitOfWork.Repository<DM.ExamPattern>().Update(examPattern);
                        return result > 0;
                    }
                    return result > 0;
                }

            }
            return false;

        }

        /// <summary>
        /// EditGeneralInstruction  Method
        /// </summary>
        /// <param name="editExamPattern"></param>
        /// <returns></returns>
        public async Task<bool> EditGeneralInstruction(Req.EditGeneralInstruction editGeneralInstruction)
        {
            int result = 0;
            var examPattern = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == editGeneralInstruction.Id && !x.IsDeleted);
            if (examPattern != null)
            {
                examPattern.GeneralInstruction = editGeneralInstruction.GeneralInstruction;
                examPattern.LastModifierUserId = editGeneralInstruction.UserId;
                examPattern.LastModifyDate = DateTime.UtcNow;
                result = await _unitOfWork.Repository<DM.ExamPattern>().Update(examPattern);
                return result > 0;
            }
            return false;

        }

        /// <summary>
        /// Delete ExamPatternData Method
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.GetExamPatternId examPatternId)
        {
            int result = 0;
            var examPattern = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == examPatternId.Id && !x.IsDeleted);
            if (examPattern != null)
            {
                var examPatternSection = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == examPatternId.Id && !x.IsDeleted);
                if (examPatternSection.Any())
                {
                    examPatternSection.ForEach(s => s.IsActive = false);
                    examPatternSection.ForEach(s => s.IsDeleted = true);
                    examPatternSection.ForEach(s => s.DeleterUserId = examPatternId.UserId);
                    examPatternSection.ForEach(s => s.DeletionDate = DateTime.UtcNow);
                    await _unitOfWork.Repository<DM.ExamPatternSection>().Update(examPatternSection);

                }
                examPattern.IsDeleted = true;
                examPattern.IsActive = false;
                examPattern.DeleterUserId = examPatternId.UserId;
                examPattern.DeletionDate = DateTime.UtcNow;
                result = await _unitOfWork.Repository<DM.ExamPattern>().Update(examPattern);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetAll ExamPattern Method
        /// </summary>
        /// <param name="examPattern"></param>
        /// <returns></returns>
        public async Task<Res.ExamPatternList> GetAllExamPattern(Req.GetAllExamPattern allExamPattern)
        {
            Res.ExamPatternList examPatternList = new()
            {
                ExamPatterns = new()
            };
            if (allExamPattern.PageNumber == 0 && allExamPattern.PageSize == 0)
            {
                var examPatternData = await _unitOfWork.Repository<DM.ExamPattern>().Get(x => !x.IsDeleted ,orderBy: x=>x.OrderByDescending(x=>x.CreationDate));
                foreach (var item in examPatternData)
                {
                    Res.ExamPattern1 examPattern1 = new()
                    {
                        Id = item.Id,
                        ExamPatternName = item.ExamPatternName,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    examPatternList.TotalRecords = examPatternData.Count;
                    examPatternList.ExamPatterns.Add(examPattern1);
                }
                return examPatternList;
            }
            else
            {
                var examPatternData = await _unitOfWork.Repository<DM.ExamPattern>().Get(x => !x.IsDeleted , orderBy: x => x.OrderByDescending(x => x.CreationDate));
                foreach (var item in examPatternData)
                {
                    Res.ExamPattern1 exam = new()
                    {
                        Id = item.Id,
                        ExamPatternName = item.ExamPatternName,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    examPatternList.ExamPatterns.Add(exam);
                }
                var result = examPatternList.ExamPatterns.Page(allExamPattern.PageNumber, allExamPattern.PageSize);
                examPatternList.ExamPatterns = result.ToList();
                examPatternList.TotalRecords = examPatternData.Count;
                return examPatternList;
            }
        }

        /// <summary>
        /// Get all subject list 
        /// by exam pattern Id
        /// </summary>
        /// <param name="examPattern"></param>
        /// <returns></returns>
        public async Task<Res.ExamPatternSubjectList> GetAllSubjectByExamPatternId(Req.GetByExamPatternId examPattern)
        {
            Res.ExamPatternSubjectList examPatternSubjectList = new()
            {
                ExamPatternSubjects = new()
            };

            var examPatternData = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == examPattern.Id && !x.IsDeleted);
            foreach (var item in examPatternData.DistinctBy(x=>x.SubjectId))
            {
                Res.ExamPatternSubjects exam = new()
                {
                    SubjectId = item.SubjectId,
                    SubjectName = await _subjectRepository.GetSubjectName(item.SubjectId)
                };
                examPatternSubjectList.ExamPatternSubjects.Add(exam);
            }
            return examPatternSubjectList;
        }

        /// <summary>
        /// Get section list by exam patternid
        /// and subject id
        /// </summary>
        /// <param name="sectionList"></param>
        /// <returns></returns>
        public async Task<Res.ExamPatternSectionList> GetSectionListByPatternIdandSubjectId(Req.GetSectionList sectionList)
        {
            Res.ExamPatternSectionList examPatternSectionList = new()
            {
                ExamPatternSections = new()
            };

            var examPatternData = await _unitOfWork.Repository<DM.ExamPatternSection>().Get(x => x.ExamPatternId == sectionList.ExamPatternId && x.SubjectId == sectionList.SubjectId && !x.IsDeleted);
            foreach (var item in examPatternData)
            {
                Res.ExamPatternSection exam = new()
                {
                    Id = item.Id,
                    SectionName = item.SectionName,
                    TotalQuestions = item.TotalQuestions,
                    TotalAttempt = item.TotalAttempt
                };
                examPatternSectionList.ExamPatternSections.Add(exam);
            }
            return examPatternSectionList;
        }

        /// <summary>
        /// Check subject exist or not
        /// </summary>
        /// <param name="checkSubject"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubjectExists(Req.CheckSubject checkSubject)
        {
            var subjectExist = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == checkSubject.SubjectId && !x.IsDeleted);
            if (subjectExist != null)
                return true;
            return false;
        }

        /// <summary>
        /// Check Exam pattern name
        /// validation
        /// </summary>
        /// <param name="patterNameAndId"></param>
        /// <returns></returns>
        public async Task<bool> CheckPatternandIdExists(Req.CheckExamPatterNameAndId patterNameAndId)
        {
            var subjectExist = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.ExamPatternName == patterNameAndId.ExamPatternName && x.Id != patterNameAndId.Id && !x.IsDeleted);
            if (subjectExist != null)
                return true;
            return false;
        }

        /// <summary>
        /// Check exam pattern id
        /// validation
        /// </summary>
        /// <param name="examPatternId"></param>
        /// <returns></returns>
        public async Task<bool> IsExamPatternIdExist(Req.GetExamPatternId examPatternId)
        {
            var patternIdExist = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == examPatternId.Id && !x.IsDeleted);
            if (patternIdExist != null)
                return true;
            return false;
        }
        public async Task<string> GetSectionName(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.ExamPatternSection>().GetSingle(x => x.Id == Id && !x.IsDeleted);

            return result != null ? result.SectionName : "N/A";
        }
        public async Task<string> GetExamPatternName(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.ExamPattern>().GetSingle(x => x.Id == Id && !x.IsDeleted);

            return result != null ? result.ExamPatternName : "N/A";
        }



    }
}
