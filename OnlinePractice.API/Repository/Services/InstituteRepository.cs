using Req = OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Request;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using FluentValidation.Validators;

namespace OnlinePractice.API.Repository.Services
{
    public class InstituteRepository : IInstituteRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPreviousYearPaperRepository _previousYearPaperRepository;
        private readonly IEbookRepository _ebookRepository;
        private readonly IMockTestRepository _mockTestRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly DBContext _dBContext;

        public InstituteRepository(IUnitOfWork unitOfWork, IFileRepository fileRepository, IHttpContextAccessor baseUrl, UserManager<ApplicationUser> userManager,
            IPreviousYearPaperRepository previousYearPaperRepository, IEbookRepository ebookRepository,
            IMockTestRepository mockTestRepository, IVideoRepository videoRepository, DBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _baseUrl = baseUrl;
            _userManager = userManager;
            _ebookRepository = ebookRepository;
            _mockTestRepository = mockTestRepository;
            _videoRepository = videoRepository;
            _previousYearPaperRepository = previousYearPaperRepository;
            _dBContext = dBContext;
        }

        /// <summary>
        /// Create Institute Data
        /// </summary>
        /// <param name="createInstitute"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateInstitute createInstitute)
        {
            DM.Institute institute = new()
            {
                InstituteName = createInstitute.InstituteName,
                InstituteContactNo = createInstitute.InstituteContactNo,
                InstituteContactPerson = createInstitute.InstituteContactPerson,
                InstituteCode = createInstitute.InstituteCode,
                InstituteEmail = createInstitute.InstituteEmail.Trim(),
                InstituteCity = createInstitute.InstituteCity,
                InstituteAddress = createInstitute.InstituteAddress,
                InstituteLogo = createInstitute.InstituteLogo,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createInstitute.UserId,
            };
            int result = await _unitOfWork.Repository<DM.Institute>().Insert(institute);
            return result > 0;
        }

        /// <summary>
        /// UploadInstituteImage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(Req.LogoImage image)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(image.Image, "Institute");
            return domain + imageUrl;
        }

        /// <summary>
        /// GetAll Institutes
        /// </summary>
        /// <returns></returns>
        public async Task<Res.InstituteList> GetAllInstitutes(Req.GetAllInstitute institute)
        {
            Res.InstituteList instituteList = new()
            {
                Institutes = new()
            };
            if (institute.PageNumber == 0 && institute.PageSize == 0)
            {
                var instituteData = await _unitOfWork.Repository<DM.Institute>().Get(x => !x.IsDeleted);
                foreach (var item in instituteData)
                {
                    Res.InstituteInfo institute1 = new()
                    {
                        Id = item.Id,
                        InstituteCode = item.InstituteCode,
                        InstituteName = item.InstituteName,
                        InstituteContactNo = item.InstituteContactNo,
                        InstituteEmail = item.InstituteEmail,
                        InstituteAddress = item.InstituteAddress,
                        InstituteLogo = item.InstituteLogo,
                        InstituteContactPerson = item.InstituteContactPerson,
                        InstituteCity = item.InstituteCity,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    instituteList.TotalRecords = instituteData.Count;
                    instituteList.Institutes.Add(institute1);
                }
                return instituteList;
            }
            else
            {
                var instituteData = await _unitOfWork.Repository<DM.Institute>().Get(x => !x.IsDeleted);
                foreach (var item in instituteData)
                {

                    Res.InstituteInfo institute1 = new()
                    {
                        Id = item.Id,
                        InstituteCode = item.InstituteCode,
                        InstituteName = item.InstituteName,
                        InstituteContactNo = item.InstituteContactNo,
                        InstituteEmail = item.InstituteEmail,
                        InstituteAddress = item.InstituteAddress,
                        InstituteLogo = item.InstituteLogo,
                        InstituteContactPerson = item.InstituteContactPerson,
                        InstituteCity = item.InstituteCity,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate
                    };
                    instituteList.Institutes.Add(institute1);
                }
                var result = instituteList.Institutes.Page(institute.PageNumber, institute.PageSize);
                instituteList.Institutes = result.ToList();
                instituteList.TotalRecords = instituteData.Count;
                return instituteList;
            }

        }

        /// <summary>
        /// Get Specific Data from institute
        /// </summary>
        /// <returns></returns>
        public async Task<Res.InstituteListV1> GetAllInstitutesV1()
        {
            Res.InstituteListV1 instituteList = new()
            {
                Institutes = new()
            };
            var instituteData = await _unitOfWork.Repository<DM.Institute>().Get(x => !x.IsDeleted);
            foreach (var item in instituteData)
            {

                Res.InstituteV1 institute1 = new()
                {
                    Id = item.Id,
                    InstituteCode = item.InstituteCode,
                    InstituteName = item.InstituteName,
                };
                instituteList.Institutes.Add(institute1);
            }
            var result = instituteList.Institutes;
            instituteList.Institutes = result.ToList();
            instituteList.TotalRecords = instituteData.Count;
            return instituteList;
        }

        /// <summary>
        /// Edit Institute Data
        /// </summary>
        /// <param name="editInstitute"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditInstitute editInstitute)
        {
            var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == editInstitute.Id && !x.IsDeleted);
            if (institute != null)
            {
                institute.InstituteName = editInstitute.InstituteName;
                institute.InstituteContactNo = editInstitute.InstituteContactNo;
                institute.InstituteContactPerson = editInstitute.InstituteContactPerson;
                institute.InstituteCode = editInstitute.InstituteCode;
                institute.InstituteEmail = editInstitute.InstituteEmail.Trim();
                institute.InstituteCity = editInstitute.InstituteCity;
                institute.InstituteAddress = editInstitute.InstituteAddress;
                institute.InstituteLogo = editInstitute.InstituteLogo;
                institute.LastModifierUserId = editInstitute.UserId;
                institute.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Institute>().Update(institute);
                return result > 0;
            }
            return false;

        }

        /// <summary>
        /// Get InstituteData ById
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        public async Task<Res.Institute?> GetById(Req.InstituteById institute)
        {
            var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institute.Id && !x.IsDeleted);
            if(instituteData != null)
            {
                var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
                var studentIds = studentList.Select(student => student.Id).ToList();
                var studentCount = _dBContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.InstituteId == instituteData.Id).ToList();
                var staffList = await _userManager.GetUsersInRoleAsync(UserRoles.Staff);
                var staffIds = staffList.Select(staff => staff.Id).ToList();
                var staffCount = _dBContext.Users.Where(x => staffIds.Contains(x.Id) && !x.IsDeleted && x.InstituteId == instituteData.Id).ToList();
                if (instituteData != null)
                {
                    Res.Institute result = new()
                    {
                        Id = instituteData.Id,
                        InstituteName = instituteData.InstituteName,
                        InstituteContactNo = instituteData.InstituteContactNo,
                        InstituteEmail = instituteData.InstituteEmail,
                        InstituteContactPerson = instituteData.InstituteContactPerson,
                        InstituteAddress = instituteData.InstituteAddress,
                        InstituteCode = instituteData.InstituteCode,
                        InstituteLogo = instituteData.InstituteLogo,
                        InstituteCity = instituteData.InstituteCity,
                        CreationDate = instituteData.CreationDate,
                        LastModifiedDate = instituteData.LastModifyDate,
                        TotalStaff = staffCount.Count,
                        TotalStudent = studentCount.Count,
                    };
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        ///  Deleted requested Institute by id
        /// </summary>
        /// <param name="institute"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Req.InstituteById institute)
        {
            var Institutedata = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institute.Id && !x.IsDeleted);
            if (Institutedata != null)
            {
                var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Staff).Result.Where(x => !x.IsDeleted && x.InstituteId == institute.Id).ToList();
                if (staffList.Count > 0)
                {
                    foreach (var item in staffList)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                        item.DeleterUserId = institute.UserId;
                        item.DeletionDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(item);
                    }

                }
                Institutedata.IsDeleted = true;
                Institutedata.IsActive = false;
                Institutedata.DeleterUserId = institute.UserId;
                Institutedata.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Institute>().Update(Institutedata);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// IsDuplicate Mehtod
        /// </summary>
        /// <param name="codeCheck"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.CodeCheck codeCheck)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.InstituteCode.Trim().ToLower() == codeCheck.InstituteCode.Trim().ToLower() && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }

        /// <summary>
        /// IsEditDuplicate Method
        /// </summary>
        /// <param name="codeCheck"></param>
        /// <returns></returns>
        public async Task<bool> IsEditDuplicate(Req.EditCodeCheck codeCheck)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.InstituteCode.Trim().ToLower() == codeCheck.InstituteCode.Trim().ToLower() && x.Id != codeCheck.Id && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }

        /// <summary>
        /// IsDuplicate Mehtod
        /// </summary>
        /// <param name="codeCheck"></param>
        /// <returns></returns>
        public async Task<bool> IsInstituteExists(Req.InstituteById institute)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institute.Id && !x.IsDeleted);
            if (result != null)
                return true;
            return false;
        }



    }
}



