using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Org.BouncyCastle.Asn1.Cmp;
using static OnlinePractice.API.Controllers.StripeController;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services
{
    public class EbookRepository : IEbookRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISubjectRepository _subjectRepository;
        public EbookRepository(IUnitOfWork unitOfWork, IFileRepository fileRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ISubjectRepository subjectRepository)
        {
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _baseUrl = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _subjectRepository = subjectRepository;
        }

        /// <summary>
        /// createEbook Method
        /// </summary>
        /// <param name="createEbook"></param>
        /// <returns></returns>
        public async Task<Res.AuthorAndLanguage2?> Create(Req.CreateEbook createEbook)
        {
            if (createEbook != null)
            {
                DM.Ebook ebook = new()
                {
                    ExamTypeId = createEbook.ExamTypeId,
                    CourseId = createEbook.CourseId,
                    SubCourseId = createEbook.SubCourseId,
                    SubjectCategoryId = createEbook.SubjectCategory,
                    TopicId = createEbook.TopicId,
                    EbookTitle = createEbook.EbookTitle,
                    Description = createEbook.Description,
                    InstituteId = createEbook.InstituteId,
                    AuthorName = createEbook.AuthorName.Trim(),
                    Language = createEbook.Language,
                    EbookPdfUrl = createEbook.EbookPdfUrl,
                    EbookThumbnail = createEbook.EbookThumbnail,
                    Price = createEbook.Price,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createEbook.UserId,
                };
                int result = await _unitOfWork.Repository<DM.Ebook>().Insert(ebook);
                if (result > 0)
                {
                    Res.AuthorAndLanguage2 authorAndLanguage = new()
                    {
                        authorName = createEbook.AuthorName,
                        Language = createEbook.Language,
                    };
                    return authorAndLanguage;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Upload ebookThumbnail Method
        /// </summary>
        /// <param name="ebookThumbnail"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(Req.EbookThumbnailimage ebookThumbnail)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(ebookThumbnail.Image, "MediaContent/EbookThumbnail");
            return domain + imageUrl;
        }

        /// <summary>
        /// Upload ebookPDF Url Method
        /// </summary>
        /// <param name="ebookPdfUrl"></param>
        /// <returns></returns>
        public async Task<string> UploadEbookPdfUrl(Req.EbookPdfUrl ebookPdfUrl)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SavePdfUrl(ebookPdfUrl.PdfUrl, "MediaContent/EbookFile");
            return domain + imageUrl;
        }

        /// <summary>
        /// Edit Ebook Data Method
        /// </summary>
        /// <param name="editEbook"></param>
        /// <returns></returns>
        public async Task<Res.AuthorAndLanguage2?> Edit(Req.EditEbook editEbook)
        {

            var ebook = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == editEbook.Id && !x.IsDeleted);
            if (ebook != null)
            {
                ebook.ExamTypeId = editEbook.ExamTypeId;
                ebook.CourseId = editEbook.CourseId;
                ebook.SubCourseId = editEbook.SubCourseId;
                ebook.SubjectCategoryId = editEbook.SubjectCategory;
                ebook.TopicId = editEbook.TopicId;
                ebook.EbookTitle = editEbook.EbookTitle;
                ebook.Description = editEbook.Description;
                ebook.InstituteId = editEbook.InstituteId;
                ebook.AuthorName = editEbook.AuthorName.Trim();
                ebook.Language = editEbook.Language;
                ebook.EbookPdfUrl = editEbook.EbookPdfUrl;
                ebook.EbookThumbnail = editEbook.EbookThumbnail;
                ebook.Price = editEbook.Price;
                ebook.LastModifyDate = DateTime.UtcNow;
                ebook.LastModifierUserId = editEbook.UserId;
                int result = await _unitOfWork.Repository<DM.Ebook>().Update(ebook);
                if (result > 0)
                {
                    Res.AuthorAndLanguage2 authorAndLanguage = new()
                    {
                        authorName = editEbook.AuthorName,
                        Language = editEbook.Language,
                    };
                    return authorAndLanguage;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Delete Ebook Data Method
        /// </summary>
        /// <param name="deleteEbook"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.DeleteEbook deleteEbook)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(deleteEbook.UserId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);
            string role = userRoles.First();
            if (role == "Admin")
            {
                var ebook = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == deleteEbook.Id && !x.IsDeleted);
                if (ebook != null)
                {
                    ebook.IsActive = false;
                    ebook.IsDeleted = true;
                    ebook.DeletionDate = DateTime.UtcNow;
                    ebook.DeleterUserId = deleteEbook.UserId;
                    await _unitOfWork.Repository<DM.Ebook>().Update(ebook);
                    return true;
                }
            }
            else if (role == "Staff")
            {
                var ebook = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == deleteEbook.Id && x.CreatorUserId == deleteEbook.UserId && !x.IsDeleted);
                if (ebook != null)
                {
                    ebook.IsActive = false;
                    ebook.IsDeleted = true;
                    ebook.DeletionDate = DateTime.UtcNow;
                    ebook.DeleterUserId = deleteEbook.UserId;
                    await _unitOfWork.Repository<DM.Ebook>().Update(ebook);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Ebook By Id
        /// </summary>
        /// <param name="ebook"></param>
        /// <returns></returns>
        public async Task<Res.Ebook?> GetEbookById(Req.EbookById ebook)
        {
            var ebookCheck = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == ebook.Id && !x.IsDeleted);
            if (ebookCheck != null)
            {
                Res.Ebook ebookInfo = new();
                ebookInfo.EbookId = ebookCheck.Id;
                ebookInfo.ExamTypeId = ebookCheck.ExamTypeId;
                var examTypeData = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == ebookCheck.ExamTypeId && !x.IsDeleted);
                ebookInfo.ExamTypeName = examTypeData != null ? examTypeData.ExamName : "N/A";
                ebookInfo.CourseId = ebookCheck.CourseId;
                var courseData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == ebookCheck.CourseId && !x.IsDeleted);
                ebookInfo.CourseName = courseData != null ? courseData.CourseName : "N/A";
                ebookInfo.SubCourseId = ebookCheck.SubCourseId;
                var SubcourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == ebookCheck.SubCourseId && !x.IsDeleted);
                ebookInfo.SubCourseName = SubcourseData != null ? SubcourseData.SubCourseName : "N/A";
                ebookInfo.SubjectCategoryId = ebookCheck.SubjectCategoryId;
                var subjectCategoryData = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == ebookCheck.SubjectCategoryId && !x.IsDeleted);
                Guid subjectId = subjectCategoryData != null ? subjectCategoryData.SubjectId : Guid.Empty;
                var subjects = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                ebookInfo.SubjectName = subjects != null ? subjects.SubjectName : "N/A";
                ebookInfo.TopicId = ebookCheck.TopicId;
                var topicData = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == ebookCheck.TopicId && !x.IsDeleted);
                ebookInfo.TopicName = topicData != null ? topicData.TopicName : "N/A";
                ebookInfo.EbookTitle = ebookCheck.EbookTitle;
                ebookInfo.Description = ebookCheck.Description;
                ebookInfo.InstituteId = ebookCheck.InstituteId;
                var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == ebookCheck.InstituteId && !x.IsDeleted);
                ebookInfo.InstituteName = instituteData != null ? instituteData.InstituteName : "N/A";
                ebookInfo.AuthorName = ebookCheck.AuthorName;
                ebookInfo.Language = ebookCheck.Language;
                ebookInfo.EbookPdfUrl = ebookCheck.EbookPdfUrl;
                ebookInfo.EbookThumbnail = ebookCheck.EbookThumbnail;
                ebookInfo.Price = ebookCheck.Price;
                ebookInfo.CreatorUserId = ebookCheck.CreatorUserId;
                ebookInfo.CreationDateTime = ebookCheck.CreationDate;
                var totalPurchaseCount = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductId == ebookCheck.Id && x.ProductCategory == ProductCategory.eBook && !x.IsDeleted);
                ebookInfo.TotalPurchase = totalPurchaseCount != null ? totalPurchaseCount.Count : 0;
                return ebookInfo;
            }
            return null;
        }

        /// <summary>
        /// GetAll Ebooks Data with pagination
        /// </summary>
        /// <param name="ebook"></param>
        /// <returns></returns>
        /// <summary>
        /// Get All Ebooks by filter
        /// </summary>
        /// <param name="ebook"></param>
        /// <returns></returns>
        public async Task<Res.EbookListV1?> GetAll(Req.GetAllEbookV1 ebook)
        {
            Res.EbookListV1 ebookList = new();
            List<Res.EbookV1> ebbokListV1 = new();
            if (ebook.TopicId != Guid.Empty)
            {
                var ebookAll = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted && x.InstituteId == ebook.InstituteId && x.SubjectCategoryId == ebook.SubjectCategoryId && x.TopicId == ebook.TopicId && x.AuthorName == ebook.WriterName, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (ebookAll.Any())
                {
                    var newList = ebookAll.Page(ebook.PageNumber, ebook.PageSize).ToList();
                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectName= await _subjectRepository.GetSubjectNameBySubjectCategoryId(item.SubjectCategoryId);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.EbookV1 ebbokList = new();
                        ebbokList.Id = item.Id;
                        ebbokList.EbookPdfUrl = item.EbookPdfUrl;
                        ebbokList.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        ebbokList.SubjectName = subjectName;
                        ebbokList.EbookTitle = item.EbookTitle;
                        ebbokList.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        ebbokList.AuthorName = item.AuthorName;
                        ebbokList.EbookThumbnail = item.EbookThumbnail;
                        ebbokList.Price = item.Price;
                        ebbokList.CreationDateTime = item.CreationDate;
                        ebbokList.CreatorUserId = item.CreatorUserId;
                        ebbokListV1.Add(ebbokList);
                    }
                }
                if (ebbokListV1.Any())
                {
                    var result = ebbokListV1;
                   // var resultV1 = result.Page(ebook.PageNumber, ebook.PageSize);
                    ebookList.Ebooks = result.ToList();
                    ebookList.TotalRecords = ebookAll.Count;
                    return ebookList;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                var ebookAll = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted && x.InstituteId == ebook.InstituteId && x.SubjectCategoryId == ebook.SubjectCategoryId  && x.AuthorName == ebook.WriterName, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (ebookAll.Any())
                {
                    var newList = ebookAll.Page(ebook.PageNumber, ebook.PageSize).ToList();

                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectName = await _subjectRepository.GetSubjectNameBySubjectCategoryId(item.SubjectCategoryId);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.EbookV1 ebbokList = new();
                        ebbokList.Id = item.Id;
                        ebbokList.EbookPdfUrl = item.EbookPdfUrl;
                        ebbokList.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        ebbokList.SubjectName = subjectName;
                        ebbokList.EbookTitle = item.EbookTitle;
                        ebbokList.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        ebbokList.AuthorName = item.AuthorName;
                        ebbokList.EbookThumbnail = item.EbookThumbnail;
                        ebbokList.Price = item.Price;
                        ebbokList.CreationDateTime = item.CreationDate;
                        ebbokList.CreatorUserId = item.CreatorUserId;
                        ebbokListV1.Add(ebbokList);
                    }

                }

                if (ebbokListV1.Any())
                {
                    var result = ebbokListV1;
                    ebookList.Ebooks = result.ToList();
                    ebookList.TotalRecords = ebookAll.Count;
                    return ebookList;
                }
                else
                {
                    return null;
                }

            }

        }

        public async Task<Res.EbookListV1?> GetAllV2(Req.GetAllEbookV1 ebook)
        {
            Res.EbookListV1 ebookList = new();
            if (ebook.TopicId != Guid.Empty)
            {
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                        join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                        join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                        join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && s.Id == ebook.SubjectCategoryId && e.TopicId == ebook.TopicId && e.SubjectCategoryId == ebook.SubjectCategoryId
                                        && institute.Id == ebook.InstituteId && e.AuthorName.ToLower() == ebook.WriterName.ToLower() && e.TopicId != Guid.Empty && topic.Id == ebook.TopicId
                                        select new Res.EbookV1
                                        {
                                            Id = e.Id,
                                            EbookPdfUrl = e.EbookPdfUrl,
                                            TopicName = topic.TopicName,
                                            SubjectName = subject.SubjectName,
                                            EbookTitle = e.EbookTitle,
                                            InstituteName = institute.InstituteName,
                                            AuthorName = e.AuthorName,
                                            EbookThumbnail = e.EbookThumbnail,
                                            Price = e.Price,
                                            CreationDateTime = e.CreationDate,
                                            CreatorUserId = e.CreatorUserId,
                                        }).Distinct().ToListAsync();

                if (ebooksList.Count > 0)
                {
                    var result = ebooksList;
                    var resultV1 = result.Page(ebook.PageNumber, ebook.PageSize);
                    ebookList.Ebooks = resultV1.ToList();
                    ebookList.TotalRecords = ebooksList.Count;
                    return ebookList;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                        join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                        join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                        join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || ebook.TopicId == Guid.Empty)
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && s.Id == ebook.SubjectCategoryId && e.SubjectCategoryId == ebook.SubjectCategoryId
                                        && institute.Id == ebook.InstituteId && e.InstituteId == ebook.InstituteId && e.AuthorName.ToLower() == ebook.WriterName.ToLower()
                                        select new Res.EbookV1
                                        {
                                            Id = e.Id,
                                            EbookPdfUrl = e.EbookPdfUrl,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            SubjectName = subject.SubjectName,
                                            EbookTitle = e.EbookTitle,
                                            InstituteName = institute.InstituteName,
                                            AuthorName = e.AuthorName,
                                            EbookThumbnail = e.EbookThumbnail,
                                            Price = e.Price,
                                            CreationDateTime = e.CreationDate,
                                            CreatorUserId = e.CreatorUserId,
                                        }).Distinct().ToListAsync();

                if (ebooksList.Count > 0)
                {
                    var result = ebooksList;
                    var resultV1 = result.Page(ebook.PageNumber, ebook.PageSize);
                    ebookList.Ebooks = resultV1.ToList();
                    ebookList.TotalRecords = ebooksList.Count;
                    return ebookList;
                }
                else
                {
                    return null;
                }

            }

        }

        public async Task<string> GetInstituteName(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == Id && !x.IsDeleted);
            return result != null ? result.InstituteName : string.Empty;
        }
        public async Task<string> GetTopicName(Guid Id)
        {
            var result = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == Id && !x.IsDeleted);
            return result != null ? result.TopicName : string.Empty;
        }
        //public async Task<Res.EbookListV1?> GetAllV3(Req.GetAllEbookV1 ebook)
        //{
        //    Res.EbookListV1 ebookList = new();
        //    List<Res.EbookV1> ebbokListV1 = new();
        //    if (ebook.TopicId != Guid.Empty)
        //    {
        //        var ebookAll = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted && x.InstituteId == ebook.InstituteId && x.SubjectCategoryId == ebook.SubjectCategoryId && x.TopicId == ebook.TopicId && x.AuthorName == ebook.WriterName, orderBy: x => x.OrderByDescending(x => x.CreationDate));
        //        if (ebookAll.Any())
        //        {
        //            var newList = ebookAll.Page(ebook.PageNumber, ebook.PageSize).ToList();
        //            if (ebbokListV1.Any())
        //            {

        //                ebbokListV1 =  newList.Select( o => new Res.EbookV1
        //                {
        //                    Id = o.Id,
        //                    EbookPdfUrl = o.EbookPdfUrl,
        //                    SubjectName =  _subjectRepository.GetSubjectNameBySubjectCategoryId(o.SubjectCategoryId),
        //                    AuthorName = o.AuthorName,
        //                    EbookTitle = o.EbookTitle,
        //                    InstituteName = await GetInstituteName(o.InstituteId),
        //                    TopicName = await GetTopicName(o.TopicId),
        //                    EbookThumbnail = o.EbookThumbnail,
        //                    Price = o.Price,
        //                    CreationDateTime = o.CreationDate,
        //                    CreatorUserId = o.CreatorUserId

        //                }).ToList();
        //                ebookList.Ebooks = result.ToList();
        //                ebookList.TotalRecords = ebookAll.Count;
        //                return ebookList;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }

        //    }
        //    else
        //    {

        //        var ebookAll = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted && x.InstituteId == ebook.InstituteId && x.SubjectCategoryId == ebook.SubjectCategoryId && x.AuthorName == ebook.WriterName, orderBy: x => x.OrderByDescending(x => x.CreationDate));
        //        if (ebookAll.Any())
        //        {
        //            var newList = ebookAll.Page(ebook.PageNumber, ebook.PageSize).ToList();

        //            foreach (var item in newList)
        //            {
        //                var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
        //                var topicName = topicDetail != null ? topicDetail.TopicName : "";
        //                var subjectName = await _subjectRepository.GetSubjectNameBySubjectCategoryId(item.SubjectCategoryId);
        //                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
        //                Res.EbookV1 ebbokList = new();
        //                ebbokList.Id = item.Id;
        //                ebbokList.EbookPdfUrl = item.EbookPdfUrl;
        //                ebbokList.TopicName = item.TopicId != Guid.Empty ? topicName : "";
        //                ebbokList.SubjectName = subjectName;
        //                ebbokList.EbookTitle = item.EbookTitle;
        //                ebbokList.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
        //                ebbokList.AuthorName = item.AuthorName;
        //                ebbokList.EbookThumbnail = item.EbookThumbnail;
        //                ebbokList.Price = item.Price;
        //                ebbokList.CreationDateTime = item.CreationDate;
        //                ebbokList.CreatorUserId = item.CreatorUserId;
        //                ebbokListV1.Add(ebbokList);
        //            }

        //        }

        //        if (ebbokListV1.Any())
        //        {
        //            var result = ebbokListV1;
        //            ebookList.Ebooks = result.ToList();
        //            ebookList.TotalRecords = ebookAll.Count;
        //            return ebookList;
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //    }

        //}
        public async Task<Res.EbookListV1?> GetAll50()
        {
            Res.EbookListV1 ebookList = new();

            var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                    join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                    join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                    join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                    join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                    join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                    join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                    where !e.IsDeleted
                                    select new Res.EbookV1
                                    {
                                        Id = e.Id,
                                        EbookPdfUrl = e.EbookPdfUrl,
                                        TopicName = topic.TopicName,
                                        SubjectName = subject.SubjectName,
                                        EbookTitle = e.EbookTitle,
                                        InstituteName = institute.InstituteName,
                                        AuthorName = e.AuthorName,
                                        EbookThumbnail = e.EbookThumbnail,
                                        Price = e.Price,
                                        CreationDateTime = e.CreationDate,
                                        CreatorUserId = e.CreatorUserId,
                                    }).AsQueryable().Take(50).Distinct().ToListAsync();
            List<Res.EbookV1> ebookV1s = new();
            foreach (var item in ebooksList)
            {
                Res.EbookV1 qu = new()
                {
                    Id = item.Id,
                    EbookPdfUrl = item.EbookPdfUrl,
                    TopicName = item.TopicName,
                    SubjectName = item.SubjectName,
                    EbookTitle = item.EbookTitle,
                    InstituteName = item.InstituteName,
                    AuthorName = item.AuthorName,
                    EbookThumbnail = item.EbookThumbnail,
                    Price = item.Price,
                    CreatorUserId = item.CreatorUserId,
                    CreationDateTime = item.CreationDateTime
                };
                ebookV1s.Add(qu);
            }
            if (ebookV1s.Count > 0)
            {
                //var result = ebookV1s.Page(1,50);
                //var resultV1 = result.Page(e.PageNumber, ebook.PageSize);
                ebookList.Ebooks = ebookV1s.ToList();
                ebookList.TotalRecords = ebooksList.Count;
                return ebookList;
            }
            else
            {
                return null;
            }
        }




        public async Task<Res.AutherList?> GetAllAuthors(Req.GetAllAuthors ebook)
        {
            Res.AutherList ebookList = new();
            if (ebook.TopicId != Guid.Empty)
            {
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                        where !e.IsDeleted && s.Id == ebook.SubjectCategoryId && e.TopicId == ebook.TopicId && e.SubjectCategoryId == ebook.SubjectCategoryId
                                        select new Res.EbookAuthers
                                        {
                                            AutherName = e.AuthorName
                                        }).Distinct().ToListAsync();

                if (ebooksList.Count > 0)
                {
                    ebookList.EbookAuthers = ebooksList;
                    return ebookList;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        where !e.IsDeleted && s.Id == ebook.SubjectCategoryId && e.SubjectCategoryId == ebook.SubjectCategoryId
                                        select new Res.EbookAuthers
                                        {
                                            AutherName = e.AuthorName
                                        }).Distinct().ToListAsync();

                if (ebooksList.Count > 0)
                {
                    ebookList.EbookAuthers = ebooksList;
                    return ebookList;
                }
                else
                {
                    return null;
                }

            }

        }

        public async Task<Res.EbookListV2?> Showallids()
        {
            Res.EbookListV2 ebookListV2 = new();
            var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.Where(x => !x.IsDeleted)
                                    select new Res.Ebooks
                                    {
                                        Id = e.Id,
                                        Name = e.EbookTitle
                                    }).ToListAsync();
            ebookListV2.Ebooks = ebooksList;

            return ebookListV2;

        }
    }
}
