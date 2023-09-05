using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using System.Text;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using MailKit;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Common;
using System.Web;
using Org.BouncyCastle.Tls;
using System.Collections.Immutable;
using System.Linq;
using MailKit.Search;
using System.Reflection;
using OnlinePractice.API.Validator.Services;
using OnlinePractice.API.Models.Enum;
using SendGrid.Helpers.Mail;
using Com = OnlinePractice.API.Models.Common;


namespace OnlinePractice.API.Repository.Services
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IEbookRepository _ebookRepository;
        private readonly IEmailService mailService;
        private readonly IAccountRepository _accountRepository;
        private readonly DBContext _dbContext;

        public StaffRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPermissionRepository permissionRepository, IEmailService mailService, IEbookRepository ebookRepository, IAccountRepository accountRepository, DBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
            this.mailService = mailService;
            _ebookRepository = ebookRepository;
            _accountRepository = accountRepository;
            _dbContext = dBContext;
        }

        /// <summary>
        /// AddStaff Method
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<Com.ResultMessageAdmin> AddStaff(Req.CreateStaff staff)
        {
            try
            {
                Com.ResultMessageAdmin resultMessage = new();
                var userExists = await _userManager.FindByEmailAsync(staff.Email);
                var mobExists = _dbContext.Users.Any(x => x.PhoneNumber == staff.MobileNumber && !x.IsDeleted);
                if (mobExists)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "Mobile number already exist!";
                    return resultMessage;
                }
                //var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Staff).Result.Where(x => !x.IsDeleted && x.PhoneNumber!=staff.MobileNumber).ToList();
                // var MobExists = await _userManager.GetPhoneNumberAsync(userExists);
                string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(staff.Password));
                if (userExists == null)
                {
                    if (mobExists)
                    {
                        resultMessage.Result = false;
                        resultMessage.Message = "Mobile number already exist!";
                        return resultMessage;
                    }
                    else
                    {
                        ApplicationUser user = new()
                        {
                            Email = staff.Email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = staff.Email,
                            Password = encodedPassword,
                            PhoneNumber = staff.MobileNumber,
                            DisplayName = staff.FullName.Trim(),
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = staff.UserId,
                            IsVerified = true,
                            IsDeleted = false,
                            IsActive = true,
                            InstituteId = staff.InstituteId,
                        };

                        var result = await _userManager.CreateAsync(user, staff.Password);
                        if (!result.Succeeded)
                        {
                            resultMessage.Result = false;
                            resultMessage.Message = "Staff not added!";
                            return resultMessage;
                        }

                        if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
                            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
                        if (await _roleManager.RoleExistsAsync(UserRoles.Staff))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.Staff);
                        }
                        userExists = await _userManager.FindByEmailAsync(staff.Email);
                        staff.Permission.UserId = Guid.Parse(userExists.Id);
                        await _permissionRepository.Create(staff.Permission, staff.UserId);
                        EmailData emailData = new()
                        {
                            ToEmail = staff.Email,
                            Subject = "Retrieve Password",
                            Body = "Your password is " + staff.Password
                        };
                        await mailService.SendEmail(emailData);
                        resultMessage.Result = true;
                        resultMessage.Message = "Staff added successfully!";
                        return resultMessage;
                    }

                }// && !mobExists)


                else if (userExists.IsDeleted)
                {
                    if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                    if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
                    if (await _roleManager.RoleExistsAsync(UserRoles.Student))
                    {
                        await _userManager.RemoveFromRoleAsync(userExists, UserRoles.Student);
                    }
                    if (await _roleManager.RoleExistsAsync(UserRoles.Staff))
                    {
                        await _userManager.AddToRoleAsync(userExists, UserRoles.Staff);
                    }
                    var encryptedPassword = _userManager.PasswordHasher.HashPassword(userExists, staff.Password);
                    userExists.PasswordHash = encryptedPassword;
                    userExists.Password = encodedPassword;
                    userExists.PhoneNumber = staff.MobileNumber;
                    userExists.DisplayName = staff.FullName.Trim();
                    userExists.InstituteId = staff.InstituteId;
                    userExists.CreatorUserId = staff.UserId;
                    userExists.CreationDate = DateTime.UtcNow;
                    userExists.IsDeleted = false;
                    userExists.IsActive = true;
                    var result = await _userManager.UpdateAsync(userExists);
                    if (!result.Succeeded)
                    {
                        resultMessage.Result = false;
                        resultMessage.Message = "Staff not added!";
                        return resultMessage;
                    }
                    staff.Permission.UserId = Guid.Parse(userExists.Id);
                    await _permissionRepository.Create(staff.Permission, staff.UserId);
                    resultMessage.Result = true;
                    resultMessage.Message = "Staff added successfully!";
                    return resultMessage;
                }
                resultMessage.Result = false;
                resultMessage.Message = "Email already exist!";
                return resultMessage;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// UpdateStaff Method
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStaff(Req.EditStaff staff)
        {
            var userExists = await _userManager.FindByIdAsync(staff.Id.ToString());
            var mobExists = _dbContext.Users.Any(x => x.Id != staff.Id.ToString() && x.PhoneNumber == staff.MobileNumber && !x.IsDeleted);
            if (mobExists)
            {
                return false;
            }
            if (userExists == null)
                return false;
            if (userExists.IsDeleted)
                return false;
            userExists.PhoneNumber = staff.MobileNumber;
            userExists.DisplayName = staff.FullName.Trim();
            userExists.InstituteId = staff.InstituteId;
            userExists.LastModifierUserId = staff.UserId;
            userExists.LastModifyDate = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(userExists);
            if (!result.Succeeded)
                return false;
            staff.Permission.UserId = staff.UserId;
            staff.Permission.StaffUserId = staff.Id;
            await _permissionRepository.Update(staff.Permission);
            return true;
        }
        /// <summary>
        /// GetAllStaff Method
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<Res.StaffData> GetAllStaff(Req.GetAllStaff staff)
        {
            Res.StaffData staffDatas = new();
            var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Staff).Result.Where(x => !x.IsDeleted).ToList();
            foreach (var item in staffList)
            {
                var instituteName = await _unitOfWork.GetContext().Institutes.Where(x => x.Id == item.InstituteId && !x.IsDeleted && x.IsActive).FirstOrDefaultAsync();
                if (instituteName != null)
                {
                    Guid staffUserId = new(item.Id);
                    Res.StaffList staffData = new()
                    {
                        Id = staffUserId,
                        Name = item.DisplayName,
                        MobileNumber = item.PhoneNumber,
                        CreationDate = item.CreationDate,
                        LastModifiedDate = item.LastModifyDate,
                        InstituteName = instituteName?.InstituteName
                    };
                    var permissions = await _unitOfWork.GetContext().Permissions.Where(x => x.StaffUserId == staffUserId && !x.IsDeleted).ToListAsync();
                    if (permissions.Any())
                    {
                        staffData.Modules = permissions.Select(o => new Res.ModulesList
                        {
                            ModuleId = o.ModuleId,
                            ModuleName = o.ModuleName,
                        }).ToList();
                        staffDatas.StaffList.Add(staffData);
                    }
                    else
                    {
                        staffData.Modules = null;
                    }
                }
            }
            var final = staffDatas.StaffList.OrderByDescending(x=>x.CreationDate).ToList();
            var result = final.Page(staff.PageNumber, staff.PageSize);
            staffDatas.StaffList = result.ToList();
            staffDatas.TotalRecords = final.Count;
            return staffDatas;
        }

        /// <summary>
        /// GetAllStaffAndAdmin Method
        /// </summary>
        /// <returns></returns>
        public async Task<Res.StaffAdminData> GetAllStaffAndAdmin()
        {
            Res.StaffAdminData staffDatas = new();
            var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Staff).Result.Where(x => !x.IsDeleted).ToList();
            foreach (var item in staffList)
            {
                var instituteName = await _unitOfWork.GetContext().Institutes.Where(x => x.Id == item.InstituteId && !x.IsDeleted && x.IsActive).FirstOrDefaultAsync();
                if (instituteName != null)
                {
                    Guid staffUserId = new(item.Id);
                    Res.StaffAdminList staffData = new()
                    {
                        Id = staffUserId,
                        Name = item.DisplayName,
                    };

                    staffDatas.StaffList.Add(staffData);
                }
            }
            var Admin = _userManager.GetUsersInRoleAsync(UserRoles.Admin).Result.First(x => !x.IsDeleted);
            Guid adminUserId = new(Admin.Id);
            Res.StaffAdminList adminDatas = new()
            {
                Id = adminUserId,
                Name = "Admin",
            };
            staffDatas.StaffList.Add(adminDatas);
            return staffDatas;
        }

        /// <summary>
        /// GetStaffById Method
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<Res.StaffById?> GetStaffById(Req.GetByIdStaff staff)
        {
            Res.StaffById staffs = new();
            ApplicationUser staffData = await _userManager.FindByIdAsync(staff.Id.ToString());

            if (staffData != null && !staffData.IsDeleted)
            {
                //string[] rt = new();
                var instituteName = await _unitOfWork.GetContext().Institutes.Where(x => x.Id == staffData.InstituteId && !x.IsDeleted).FirstOrDefaultAsync();
                Guid staffUserId = new(staffData.Id);
                staffs.Id = staffUserId;
                staffs.Name = staffData.DisplayName;
                staffs.MobileNumber = staffData.PhoneNumber;
                staffs.InstituteId = staffData.InstituteId;
                staffs.InstituteName = instituteName?.InstituteName;
                staffs.Email = staffData.Email;
                var permissions = await _unitOfWork.GetContext().Permissions.Where(x => x.StaffUserId == staffUserId && !x.IsDeleted).ToListAsync();
                staffs.Modules = permissions.Select(o => new Res.ModulesList
                {
                    ModuleId = o.ModuleId,
                    ModuleName = o.ModuleName
                }).ToList();
                foreach (var module in staffs.Modules)
                {
                    if (module.ModuleName == "Ebook")
                    {
                        var ebook = await _unitOfWork.Repository<DM.Ebook>().Get(x => x.CreatorUserId == staff.Id && !x.IsDeleted);
                        module.ModuleData = ebook.Select(r => new ModuleDataList
                        {
                            Id = r.Id,
                            Headings = r.EbookTitle,
                            Description = r.Description,
                            LastModifiedDate = r.LastModifyDate != null ? r.LastModifyDate : r.CreationDate
                        }).OrderByDescending(p => p.LastModifiedDate).Take(3).ToList();
                        foreach (var item in module.ModuleData)
                        {
                            item.ModuleCategory.Add("Ebook");
                        }
                    }
                    if (module.ModuleName == "Videos")
                    {
                        var video = await _unitOfWork.Repository<DM.Video>().Get(x => x.CreatorUserId == staff.Id && !x.IsDeleted);
                        module.ModuleData = video.Select(r => new ModuleDataList
                        {
                            Id = r.Id,
                            Headings = r.VideoTitle,
                            Description = r.Description,
                            LastModifiedDate = r.LastModifyDate != null ? r.LastModifyDate : r.CreationDate
                        }).OrderByDescending(p => p.LastModifiedDate).Take(3).ToList();
                        foreach (var item in module.ModuleData)
                        {
                            item.ModuleCategory.Add("Videos");
                        }
                    }
                    if (module.ModuleName == "Previous Year Paper")
                    {
                        var pYP = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.CreatorUserId == staff.Id && !x.IsDeleted);
                        module.ModuleData = pYP.Select(r => new ModuleDataList
                        {
                            Id = r.Id,
                            Headings = r.PaperTitle,
                            Description = r.Description,
                            LastModifiedDate = r.LastModifyDate != null ? r.LastModifyDate : r.CreationDate
                        }).OrderByDescending(p => p.LastModifiedDate).Take(3).ToList();
                        foreach (var item in module.ModuleData)
                        {
                            item.ModuleCategory.Add("Previous Year Paper");
                        }
                    }
                    if (module.ModuleName == "MockTest")
                    {
                        var mockTest = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => x.CreatorUserId == staff.Id && !x.IsDeleted);
                        module.ModuleData = mockTest.Select(r => new ModuleDataList
                        {
                            Id = r.Id,
                            Headings = r.MockTestName,
                            Description = r.Description,
                            LastModifiedDate = r.LastModifyDate != null ? r.LastModifyDate : r.CreationDate
                        }).OrderByDescending(p => p.LastModifiedDate).Take(3).ToList();
                        foreach (var item in module.ModuleData)
                        {
                            item.ModuleCategory.Add("Mocktest");
                        }
                    }
                }
                return staffs;
            }
            return null;
        }

        /// <summary>
        /// RemoveStaff Method
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<bool> RemoveStaff(Req.GetByIdStaff staff)
        {
            string staffUserId = staff.Id.ToString();
            ApplicationUser user = await _userManager.FindByIdAsync(staffUserId);
            if (user != null && !user.IsDeleted)
            {
                List<DM.Permission> permissions = await _unitOfWork.Repository<DM.Permission>().Get(x => !x.IsDeleted && x.StaffUserId == staff.Id);
                if (permissions.Count > 0)
                {
                    permissions.ForEach(s => s.IsActive = false);
                    permissions.ForEach(s => s.IsDeleted = true);
                    await _unitOfWork.Repository<DM.Permission>().Update(permissions);
                }

                user.IsDeleted = true;
                user.IsActive = false;
                user.DeletionDate = DateTime.UtcNow;
                user.DeleterUserId = staff.UserId;
                await _userManager.UpdateAsync(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// GetModuleList Method
        /// </summary>
        /// <returns></returns>
        public async Task<Res.Module> GetModuleList()
        {
            Res.Module ModuleList = new()
            {
                ModulesList = new()
            };
            var module = await _unitOfWork.Repository<DM.Module>().Get(x => !x.IsDeleted);
            foreach (var item in module)
            {
                Res.ModulesList modules = new()
                {
                    ModuleName = item.ModuleName,
                    ModuleId = item.Id
                };
                ModuleList.ModulesList.Add(modules);

            }
            return ModuleList;
        }

        /// <summary>
        /// IsDuplicate Method
        /// </summary>
        /// <param name="instituteCheck"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Req.InstituteCheck instituteCheck)
        {
            var result = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteCheck.InstituteId && !x.IsDeleted);
            if (result != null)
                return false;
            return true;
        }


    }
}