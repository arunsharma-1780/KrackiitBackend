using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.DBModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Repository.Services
{
    public class PreviousYearPaperRepository : IPreviousYearPaperRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PreviousYearPaperRepository(IUnitOfWork unitOfWork, IFileRepository fileRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _baseUrl = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        ///Create Previous year paper 
        /// </summary>
        /// <param name="previousYearPaper"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreatePreviousYearPaper previousYearPaper)
        {
            if (previousYearPaper != null)
            {
                DM.PreviousYearPaper previousYear = new()
                {
                    ExamTypeId = previousYearPaper.ExamTypeId,
                    CourseId = previousYearPaper.CourseId,
                    SubCourseId = previousYearPaper.SubCourseId,
                    InstituteId = previousYearPaper.InstituteId,
                    Year = previousYearPaper.Year,
                    PaperTitle = previousYearPaper.PaperTitle,
                    Description = previousYearPaper.Description,
                    Language = previousYearPaper.Language,
                    PaperPdfUrl = previousYearPaper.PaperPdfUrl,
                    Price = previousYearPaper.Price,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = previousYearPaper.UserId
                };
                int result = await _unitOfWork.Repository<DM.PreviousYearPaper>().Insert(previousYear);
                return (result > 0);
            }
            return false;
        }


        /// <summary>
        /// Upload PaperPdfUrl
        /// </summary>
        /// <param name="pdfUrl"></param>
        /// <returns></returns>
        public async Task<string> UploadPaperPdfUrl(Req.PaperPdfUrl pdfUrl)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SavePdfUrl(pdfUrl.PdfUrl, "PreviousPaper");
            return domain + imageUrl;
        }


        /// <summary>
        /// Edit previousYearPaper
        /// </summary>
        /// <param name="previousYearPaper"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditPreviousYearPaper previousYearPaper)
        {
            var paper = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == previousYearPaper.Id && !x.IsDeleted);
            if (paper != null)
            {
                paper.ExamTypeId = previousYearPaper.ExamTypeId;
                paper.CourseId = previousYearPaper.CourseId;
                paper.SubCourseId = previousYearPaper.SubCourseId;
                paper.InstituteId = previousYearPaper.InstituteId;
                paper.Year = previousYearPaper.Year;
                paper.PaperTitle = previousYearPaper.PaperTitle;
                paper.Description = previousYearPaper.Description;
                paper.Language = previousYearPaper.Language;
                paper.PaperPdfUrl = previousYearPaper.PaperPdfUrl;
                paper.Price = previousYearPaper.Price;
                paper.LastModifyDate = DateTime.UtcNow;
                paper.LastModifierUserId = previousYearPaper.UserId;
                int result = await _unitOfWork.Repository<DM.PreviousYearPaper>().Update(paper);
                return result > 0;
            }
            return false;
        }


        /// <summary>
        /// Delete paperPdf
        /// </summary>
        /// <param name="paperPdf"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.GetPaperPdf paperPdf)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(paperPdf.UserId.ToString());

            var userRoles = await _userManager.GetRolesAsync(user);
            string role = userRoles.First();
            if (role == "Admin")
            {
                var paper = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == paperPdf.Id && !x.IsDeleted);
                if (paper != null)
                {
                    paper.IsActive = false;
                    paper.IsDeleted = true;
                    paper.DeletionDate = DateTime.UtcNow;
                    paper.DeleterUserId = paperPdf.UserId;
                    await _unitOfWork.Repository<DM.PreviousYearPaper>().Update(paper);
                    return true;
                }
            }
            else if (role == "Staff")
            {
                var paper = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == paperPdf.Id && x.CreatorUserId == paperPdf.UserId && !x.IsDeleted);
                if (paper != null)
                {
                    paper.IsActive = false;
                    paper.IsDeleted = true;
                    paper.DeletionDate = DateTime.UtcNow;
                    paper.DeleterUserId = paperPdf.UserId;
                    await _unitOfWork.Repository<DM.PreviousYearPaper>().Update(paper);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Get PaperData by ID
        /// </summary>
        /// <param name="paperPdf"></param>
        /// <returns></returns>
        public async Task<Res.PreviousYearPaper?> GetPaperById(Req.GetPaperPdf paperPdf)
        {
            var paperInfo = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == paperPdf.Id && !x.IsDeleted);
            if (paperInfo != null)
            {
                Res.PreviousYearPaper previousYearPaper = new();
                previousYearPaper.Id = paperInfo.Id;
                previousYearPaper.ExamTypeId = paperInfo.ExamTypeId;
                var examTypeData = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == paperInfo.ExamTypeId && !x.IsDeleted);
                previousYearPaper.ExamTypeName = examTypeData != null ? examTypeData.ExamName : "N/A";
                previousYearPaper.CourseId = paperInfo.CourseId;
                var courseData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == paperInfo.CourseId && !x.IsDeleted);
                previousYearPaper.CourseName = courseData != null ? courseData.CourseName : "N/A";
                previousYearPaper.SubCourseId = paperInfo.SubCourseId;
                var SubcourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == paperInfo.SubCourseId && !x.IsDeleted);
                previousYearPaper.SubCourseName = SubcourseData != null ? SubcourseData.SubCourseName : "N/A";
                previousYearPaper.InstituteId = paperInfo.InstituteId;
                var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == paperInfo.InstituteId && !x.IsDeleted);
                previousYearPaper.InstituteName = instituteData != null ? instituteData.InstituteName : "N/A";
                previousYearPaper.PaperTitle = paperInfo.PaperTitle;
                previousYearPaper.Description = paperInfo.Description;
                previousYearPaper.Year = paperInfo.Year;
                previousYearPaper.Language = paperInfo.Language;
                previousYearPaper.PaperPdfUrl = paperInfo.PaperPdfUrl;
                previousYearPaper.Price = paperInfo.Price;
                previousYearPaper.CreatorUserId = paperInfo.CreatorUserId;
                previousYearPaper.CreationDateTime = paperInfo.CreationDate;
                var totalPurchaseCount = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductId == paperInfo.Id && x.ProductCategory == ProductCategory.PreviouseYearPaper && !x.IsDeleted);
                previousYearPaper.TotalPurchase = totalPurchaseCount != null ? totalPurchaseCount.Count : 0;

                return previousYearPaper;
            }
            return null;
        }

        /// <summary>
        /// GetAll Paper by Filter
        /// </summary>
        /// <param name="papers"></param>
        /// <returns></returns>
        public async Task<Res.PreviousYearPaperList?> GetAllPapers(Req.GetAllPapers papers)
        {
            try
            {
                //ApplicationUser user = await _userManager.FindByIdAsync(papers.UserId.ToString());

                //var userRoles = await _userManager.GetRolesAsync(user);
                //string role = userRoles.First();

                //if (papers.CreationUserId != Guid.Empty || role == "Admin")
                if (papers.CreationUserId != Guid.Empty)
                {
                    Res.PreviousYearPaperList paperList = new();

                    var papersList = await (from pp in _unitOfWork.GetContext().PreviousYearPapers.OrderByDescending(x => x.CreationDate)
                                            join s in _unitOfWork.GetContext().SubCourse on pp.SubCourseId equals s.Id
                                            join institute in _unitOfWork.GetContext().Institutes on pp.InstituteId equals institute.Id
                                            where !pp.IsDeleted && institute.Id == papers.InstituteId && s.Id == papers.SubCourseId && pp.Year == papers.Year
                                            && pp.CreatorUserId == papers.CreationUserId
                                            select new Res.PreviousYearPaperV1
                                            {
                                                Id = pp.Id,
                                                PaperTitle = pp.PaperTitle,
                                                Year = pp.Year,
                                                SubCourseId = pp.SubCourseId,
                                                InstituteName = institute.InstituteName,
                                                CreatorUserId = pp.CreatorUserId,
                                                CreationDateTime = pp.CreationDate,
                                                AddedBy = _userManager.FindByIdAsync(papers.CreationUserId.ToString()).Result.DisplayName
                                            }).Distinct().ToListAsync();


                    if (papersList.Count > 0)
                    {

                        var result = papersList;
                        var resultV1 = result.Page(papers.PageNumber, papers.PageSize);

                        paperList.PreviousYearPapers = resultV1.ToList();
                        foreach (var id in paperList.PreviousYearPapers)
                        {
                            var paperInfo = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == id.Id && !x.IsDeleted);
                            string language = paperInfo != null ? paperInfo.Language : "N/A";
                            id.Language.Add(language);
                        }
                        paperList.TotalRecords = papersList.Count;
                        return paperList;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (papers.CreationUserId == Guid.Empty)
                {
                    Res.PreviousYearPaperList paperList = new();

                    List<Res.PreviousYearPaperV1> quList1 = new();
                    var papersList = await (from pp in _unitOfWork.GetContext().PreviousYearPapers.OrderByDescending(x => x.CreationDate)
                                            join s in _unitOfWork.GetContext().SubCourse on pp.SubCourseId equals s.Id
                                            join institute in _unitOfWork.GetContext().Institutes on pp.InstituteId equals institute.Id
                                            where !pp.IsDeleted && institute.Id == papers.InstituteId && s.Id == papers.SubCourseId && pp.Year == papers.Year
                                            select new Res.PreviousYearPaperV1
                                            {
                                                Id = pp.Id,
                                                PaperTitle = pp.PaperTitle,
                                                Year = pp.Year,
                                                SubCourseId = pp.SubCourseId,
                                                InstituteName = institute.InstituteName,
                                                CreatorUserId = pp.CreatorUserId,
                                                CreationDateTime = pp.CreationDate,
                                            }).Distinct().ToListAsync();
                    foreach (var item in papersList)
                    {
                        var name = _userManager.FindByIdAsync(item.CreatorUserId.ToString()).Result;
                        item.AddedBy = name != null ? name.DisplayName : "N/A";
                        quList1.Add(item);
                    }
                    if (quList1.Count > 0)
                    {

                        var result = quList1;
                        var resultV1 = result.Page(papers.PageNumber, papers.PageSize);

                        paperList.PreviousYearPapers = resultV1.ToList();
                        foreach (var id in paperList.PreviousYearPapers)
                        {
                            var paperInfo = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == id.Id && !x.IsDeleted);
                            string language = paperInfo != null ? paperInfo.Language : "N/A";
                            //string addedBy = _userManager.FindByIdAsync(paperInfo.CreatorUserId.ToString()).Result.DisplayName;
                            id.Language.Add(language);
                            //id.AddedBy.Add(addedBy);
                        }
                        paperList.TotalRecords = papersList.Count;
                        return paperList;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;


        }


        public async Task<Res.PreviousYearPaperList?> GetAll50()
        {
            Res.PreviousYearPaperList previousYearPaper = new();

            var previousYearPapersList = await (from pp in _unitOfWork.GetContext().PreviousYearPapers.OrderByDescending(x => x.CreationDate)
                                                join s in _unitOfWork.GetContext().SubCourse on pp.SubCourseId equals s.Id
                                                join institute in _unitOfWork.GetContext().Institutes on pp.InstituteId equals institute.Id
                                                where !pp.IsDeleted
                                                select new Res.PreviousYearPaperV1
                                                {
                                                    Id = pp.Id,
                                                    PaperTitle = pp.PaperTitle,
                                                    Year = pp.Year,
                                                    SubCourseId = pp.SubCourseId,
                                                    InstituteName = institute.InstituteName,
                                                    CreatorUserId = pp.CreatorUserId,
                                                    CreationDateTime = pp.CreationDate,
                                                }).AsQueryable().Take(50).Distinct().ToListAsync();

            List<Res.PreviousYearPaperV1> previousYearPaperV1s = new();
            foreach (var item in previousYearPapersList)
            {
                //language must also add in the string
                //var paperInfo = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == id.Id && !x.IsDeleted); 
                //var name = _userManager.FindByIdAsync(item.CreatorUserId.ToString()).Result;
                Res.PreviousYearPaperV1 qu = new()
                {
                    Id = item.Id,
                    PaperTitle = item.PaperTitle,
                    Year = item.Year,
                    SubCourseId = item.SubCourseId,
                    InstituteName = item.InstituteName,
                    CreatorUserId = item.CreatorUserId,
                    CreationDateTime = item.CreationDateTime,


                  //  AddedBy= item.AddedBy = name != null ? name.DisplayName : "N/A",
            };
                //var name = _userManager.FindByIdAsync(item.CreatorUserId.ToString()).Result;
                //item.AddedBy = name != null ? name.DisplayName : "N/A";
                previousYearPaperV1s.Add(qu);
            }
            if (previousYearPaperV1s.Count > 0)
            {
                //var result = ebookV1s.Page(1,50);
                //var resultV1 = result.Page(e.PageNumber, ebook.PageSize);
                previousYearPaper.PreviousYearPapers = previousYearPaperV1s.ToList();
                previousYearPaper.TotalRecords = previousYearPapersList.Count;
                return previousYearPaper;
            }
            else
            {
                return null;
            }
        }




        public string? GetName(string Id)
        {
            var name = _userManager.FindByIdAsync(Id).Result.DisplayName;
            return name;
        }

        public async Task<Res.FacultyList?> GetAllFaculties(Req.GetAllFacultyList faculty)
        {
            Res.FacultyList facultyList = new();
            var FacList = await (from pyp in _unitOfWork.GetContext().PreviousYearPapers
                                 join subcourse in _unitOfWork.GetContext().SubCourse on pyp.SubCourseId equals subcourse.Id
                                 join institute in _unitOfWork.GetContext().Institutes on pyp.InstituteId equals institute.Id
                                 where !pyp.IsDeleted && pyp.SubCourseId == faculty.SubCourseId && pyp.CreatorUserId == faculty.AddedBy
                                 select new Res.Faculties
                                 {
                                     FacultyName = _userManager.FindByIdAsync(faculty.AddedBy.ToString()).Result.DisplayName,
                                 }).Distinct().ToListAsync();

            if (FacList.Count > 0)
            {
                facultyList.Faculties = FacList;
                return facultyList;
            }
            else
            {
                return null;
            }
        }

        public async Task<Res.VideoListV2?> Showallids()
        {
            Res.VideoListV2 ebookListV2 = new();
            var videolist = await (from e in _unitOfWork.GetContext().PreviousYearPapers.Where(x => !x.IsDeleted)
                                   select new Res.VideoLists
                                   {
                                       Id = e.Id,
                                       Name = e.PaperTitle
                                   }).ToListAsync();
            ebookListV2.Videos = videolist;

            return ebookListV2;

        }

        public Res.YearList GetYearList()
        {
            Res.YearList yearList = new();
            for (int Year = 1900; Year <= DateTime.Now.Year; Year++)
            {
                yearList.Years.Add(Year);
            }
            yearList.Years.Reverse();
            return yearList;
        }
    }
}
