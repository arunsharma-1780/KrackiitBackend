using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlinePractice.API.Models;
using Microsoft.IdentityModel.Tokens;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using SendGrid.Helpers.Mail;
using System.Data;
using System.IO.Compression;
using IronXL;
using System.IO;
using OfficeOpenXml;
using OnlinePractice.API.Validator.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using SixLabors.ImageSharp;
using Org.BouncyCastle.Asn1.Ocsp;

namespace OnlinePractice.API.Repository.Services
{
    public class StudentRegistrationRepository : IStudentRegistrationRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly IFileRepository _fileRepository;
        private readonly IStudentRegistrationValidation _validation;
        private readonly IMyCartRepository _myCartRepository;



        public StudentRegistrationRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, DBContext dbContext, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IHttpContextAccessor baseUrl, IFileRepository fileRepository, IStudentRegistrationValidation validation, IMyCartRepository myCartRepository) { _unitOfWork = unitOfWork; _userManager = userManager; _dbContext = dbContext; _roleManager = roleManager; _configuration = configuration; _baseUrl = baseUrl; _fileRepository = fileRepository; _validation = validation; _myCartRepository = myCartRepository; }

        public async Task<Res.StudentList> GetAllStudent(Req.GetAllStudent allStudent)
        {
            if (allStudent.SubCourseId != Guid.Empty && allStudent.InstituteId != Guid.Empty)
            {
                Res.StudentList studentList = new();
                List<Res.StudentDetails> studentDetailsList = new();
                var studentDetails = await _unitOfWork.Repository<DM.StudentCourse>().Get(x => x.SubCourseId == allStudent.SubCourseId && x.InstituteId == allStudent.InstituteId && !x.IsDeleted);
                foreach (var student in studentDetails)
                {
                    Res.StudentDetails details = new();
                    details.StudentId = student.StudentId;
                    var studentInfo = await _userManager.FindByIdAsync(student.StudentId.ToString());
                    details.StudentName = studentInfo != null ? studentInfo.DisplayName : string.Empty;
                    var subCourseId = student.SubCourseId;
                    var instituteId = student.InstituteId;
                    var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    details.SubCourseName = subCourseDetail != null ? subCourseDetail.SubCourseName : string.Empty;
                    var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteId);
                    details.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                    var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == student.StudentId && !x.IsDeleted);
                    Guid courseId = subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                    var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                    details.CourseName = courseDetail != null ? courseDetail.CourseName : string.Empty;
                    details.EnrollmentDate = studentInfo != null ? studentInfo.CreationDate : null;
                    studentDetailsList.Add(details);
                }
                studentList.TotalRecords = studentDetailsList.Count;
                var final = studentDetailsList.OrderByDescending(x => x.EnrollmentDate);
                var result = final.Page(allStudent.PageNumber, allStudent.PageSize);
                studentList.StudentDetails = result.ToList();
                return studentList;
            }
            else if (allStudent.SubCourseId != Guid.Empty && allStudent.InstituteId == Guid.Empty)
            {
                Res.StudentList studentList = new();
                List<Res.StudentDetails> studentDetailsList = new();
                var studentDetails = await _unitOfWork.Repository<DM.StudentCourse>().Get(x => x.SubCourseId == allStudent.SubCourseId && !x.IsDeleted);
                foreach (var student in studentDetails)
                {
                    Res.StudentDetails details = new();
                    details.StudentId = student.StudentId;
                    var studentInfo = await _userManager.FindByIdAsync(student.StudentId.ToString());
                    details.StudentName = studentInfo != null ? studentInfo.DisplayName : string.Empty;
                    var subCourseId = student.SubCourseId;
                    var instituteId = student.InstituteId;
                    var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    details.SubCourseName = subCourseDetail != null ? subCourseDetail.SubCourseName : string.Empty;
                    var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteId);
                    details.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                    var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == student.StudentId && !x.IsDeleted);
                    Guid courseId = subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                    var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                    details.CourseName = courseDetail != null ? courseDetail.CourseName : string.Empty;
                    details.EnrollmentDate = studentInfo != null ? studentInfo.CreationDate : null;
                    studentDetailsList.Add(details);
                }
                studentList.TotalRecords = studentDetailsList.Count;
                var final = studentDetailsList.OrderByDescending(x => x.EnrollmentDate);
                var result = final.Page(allStudent.PageNumber, allStudent.PageSize);
                studentList.StudentDetails = result.ToList();
                return studentList;
            }
            else if (allStudent.SubCourseId == Guid.Empty && allStudent.InstituteId != Guid.Empty)
            {
                Res.StudentList studentList = new();
                List<Res.StudentDetails> studentDetailsList = new();
                var studentDetails = await _unitOfWork.Repository<DM.StudentCourse>().Get(x => x.InstituteId == allStudent.InstituteId && !x.IsDeleted);
                foreach (var student in studentDetails)
                {
                    Res.StudentDetails details = new();
                    details.StudentId = student.StudentId;
                    var studentInfo = await _userManager.FindByIdAsync(student.StudentId.ToString());
                    details.StudentName = studentInfo != null ? studentInfo.DisplayName : string.Empty;
                    var subCourseId = student.SubCourseId;
                    var instituteId = student.InstituteId;
                    var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    details.SubCourseName = subCourseDetail != null ? subCourseDetail.SubCourseName : string.Empty;
                    var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteId);
                    details.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                    var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == student.StudentId && !x.IsDeleted);
                    Guid courseId = subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                    var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                    details.CourseName = courseDetail != null ? courseDetail.CourseName : string.Empty;
                    details.EnrollmentDate = studentInfo != null ? studentInfo.CreationDate : null;
                    studentDetailsList.Add(details);
                }
                studentList.TotalRecords = studentDetailsList.Count;
                var final = studentDetailsList.OrderByDescending(x => x.EnrollmentDate);
                var result = final.Page(allStudent.PageNumber, allStudent.PageSize);
                studentList.StudentDetails = result.ToList();
                return studentList;
            }
            else
            {
                Res.StudentList studentList = new();
                List<Res.StudentDetails> studentDetailsList = new();
                var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Student).Result.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
                var studentDetails = await _dbContext.Users.Where(x => staffList.Contains(x.Id)).ToArrayAsync();

                // var studentDetails = await _dbContext.Users.Where(x => !x.IsDeleted).ToArrayAsync();
                foreach (var student in studentDetails)
                {
                    var studentId = Guid.Parse(student.Id);
                    var studentSubcoursDetails = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == studentId && !x.IsDeleted);
                    Res.StudentDetails details = new();
                    details.StudentId = Guid.Parse(student.Id);
                    var studentInfo = await _userManager.FindByIdAsync(student.Id);
                    details.StudentName = studentInfo != null ? studentInfo.DisplayName : string.Empty;
                    var subCourseId = studentSubcoursDetails != null ? studentSubcoursDetails.SubCourseId : Guid.Empty;
                    var instituteId = studentInfo != null ? studentInfo.InstituteId : Guid.Empty;
                    var subCourseDetail = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    details.SubCourseName = subCourseDetail != null ? subCourseDetail.SubCourseName : string.Empty;
                    var instituteDetail = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == instituteId);
                    details.InstituteName = instituteDetail != null ? instituteDetail.InstituteName : string.Empty;
                    var studentsubCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == studentId && !x.IsDeleted);
                    Guid courseId = subCourseDetail != null ? subCourseDetail.CourseID : Guid.Empty;
                    var courseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                    details.CourseName = courseDetail != null ? courseDetail.CourseName : string.Empty;
                    details.EnrollmentDate = studentInfo != null ? studentInfo.CreationDate : null;
                    studentDetailsList.Add(details);
                }
                studentList.TotalRecords = studentDetailsList.Count;
                var final = studentDetailsList.OrderByDescending(x => x.EnrollmentDate);
                var result = final.Page(allStudent.PageNumber, allStudent.PageSize);
                studentList.StudentDetails = result.ToList();
                return studentList;
            }
        }
        public async Task<Com.ResultMessageAdmin> AddStudent([FromBody] Req.AddStudent student)
        {
            Com.ResultMessageAdmin resultMessage = new();

            var userExists = await _userManager.FindByEmailAsync(student.Email);
            var mobExists = _dbContext.Users.Any(x => x.PhoneNumber == student.MobileNumber && !x.IsDeleted);
            if (mobExists)
            {
                resultMessage.Result = false;
                resultMessage.Message = "Mobile number is already exists.";
                return resultMessage;
            }
            string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(student.Password));
            if (userExists == null)
            {
                ApplicationUser user = new()
                {
                    Email = student.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = student.Email,
                    Password = encodedPassword,
                    PhoneNumber = student.MobileNumber,
                    DisplayName = student.FullName.Trim(),
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = student.UserId,
                    IsVerified = false,
                    IsDeleted = false,
                    IsActive = true,
                    Balance = 100
                };

                var result = await _userManager.CreateAsync(user, student.Password);
                DM.WalletHistory walletHistory = new()
                {
                    ProductId = Guid.Empty,
                    ProductCategory = ProductCategory.Reward,
                    StudentId = Guid.Parse(user.Id),
                    CreditAmount = 100,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = Guid.Parse(user.Id)
                };
                var result1 = await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                if (!result.Succeeded)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User not created.";
                    return resultMessage;
                }

                if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));

                if (await _roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Student);
                }
                userExists = await _userManager.FindByEmailAsync(student.Email);
                return new Com.ResultMessageAdmin
                {
                    Result = true,
                    Message = "User Created Successfully"
                };

            }
            else if (userExists.IsDeleted)
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
                if (await _roleManager.RoleExistsAsync(UserRoles.Staff))
                {
                    await _userManager.RemoveFromRoleAsync(userExists, UserRoles.Staff);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await _userManager.AddToRoleAsync(userExists, UserRoles.Student);
                }
                var encryptedPassword = _userManager.PasswordHasher.HashPassword(userExists, student.Password);
                userExists.PasswordHash = encryptedPassword;
                userExists.Password = encodedPassword;
                userExists.PhoneNumber = student.MobileNumber;
                userExists.DisplayName = student.FullName.Trim();
                userExists.Balance = 100;
                userExists.CreatorUserId = student.UserId;
                userExists.CreationDate = DateTime.UtcNow;
                userExists.IsDeleted = false;
                userExists.IsActive = true;
                var result = await _userManager.UpdateAsync(userExists);
                if (!result.Succeeded)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User not created.";
                    return resultMessage;
                }
                DM.WalletHistory walletHistory = new()
                {
                    ProductId = Guid.Empty,
                    ProductCategory = ProductCategory.Reward,
                    StudentId = student.UserId,
                    CreditAmount = 100,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = student.UserId
                };
                var result1 = await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);

                return new Com.ResultMessageAdmin
                {
                    Result = true,
                    Message = "User Created Successfully"
                };

            }
            resultMessage.Result = false;
            resultMessage.Message = "Email Already Exists";
            return resultMessage;
        }
        public async Task<Com.ResultMessageAdmin> AddBulkUploadStudent([FromBody] Req.AddBulkUploadStudent student)
        {
            Com.ResultMessageAdmin resultMessage = new();
            var amount = Convert.ToDouble(student.Amount);

            if (student.IsBulkUpload)
            {
                ErrorResponse? errorResponse;
                #region Validate Request Model
                var validation = await _validation.AddBulkUploadStudentValidator.ValidateAsync(student);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = errorResponse.Message;
                    return resultMessage;
                }
                #endregion
            }

            var userExists = await _userManager.FindByEmailAsync(student.Email);
            var mobExists = _dbContext.Users.Any(x => x.PhoneNumber == student.MobileNumber && !x.IsDeleted);
            if (mobExists)
            {
                resultMessage.Result = false;
                resultMessage.Message = "Mobile number is already exists.";
                return resultMessage;
            }
            string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(student.Password));
            if (userExists == null)
            {
                ApplicationUser user = new()
                {
                    Email = student.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = student.Email,
                    Password = encodedPassword,
                    PhoneNumber = student.MobileNumber,
                    DisplayName = student.FullName.Trim(),
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = student.UserId,
                    IsVerified = false,
                    IsDeleted = false,
                    IsActive = true,
                    Balance = amount
                };

                var result = await _userManager.CreateAsync(user, student.Password);
                DM.WalletHistory walletHistory = new()
                {
                    ProductId = Guid.Empty,
                    ProductCategory = ProductCategory.Reward,
                    StudentId = Guid.Parse(user.Id),
                    CreditAmount = amount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = Guid.Parse(user.Id)
                };
                var result1 = await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                if (!result.Succeeded)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User not created.";
                    return resultMessage;
                }

                if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));

                if (await _roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Student);
                }
                userExists = await _userManager.FindByEmailAsync(student.Email);
                return new Com.ResultMessageAdmin
                {
                    Result = true,
                    Message = "User Created Successfully"
                };

            }
            else if (userExists.IsDeleted)
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
                if (await _roleManager.RoleExistsAsync(UserRoles.Staff))
                {
                    await _userManager.RemoveFromRoleAsync(userExists, UserRoles.Staff);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await _userManager.AddToRoleAsync(userExists, UserRoles.Student);
                }
                var encryptedPassword = _userManager.PasswordHasher.HashPassword(userExists, student.Password);
                userExists.PasswordHash = encryptedPassword;
                userExists.Password = encodedPassword;
                userExists.PhoneNumber = student.MobileNumber;
                userExists.DisplayName = student.FullName.Trim();
                userExists.Balance = amount;
                userExists.CreatorUserId = student.UserId;
                userExists.CreationDate = DateTime.UtcNow;
                userExists.IsDeleted = false;
                userExists.IsActive = true;
                var result = await _userManager.UpdateAsync(userExists);
                if (!result.Succeeded)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User not created.";
                    return resultMessage;
                }
                DM.WalletHistory walletHistory = new()
                {
                    ProductId = Guid.Empty,
                    ProductCategory = ProductCategory.Reward,
                    StudentId = student.UserId,
                    CreditAmount = amount,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = student.UserId
                };
                var result1 = await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);

                return new Com.ResultMessageAdmin
                {
                    Result = true,
                    Message = "User Created Successfully"
                };

            }
            resultMessage.Result = false;
            resultMessage.Message = "Email Already Exists";
            return resultMessage;
        }
        public async Task<Com.ResultMessageAdmin> EditStudent(Req.UpdateStudent student)
        {
            try
            {
                Com.ResultMessageAdmin resultMessage = new();
                var userExists = await _userManager.FindByIdAsync(student.Id.ToString());
                if (userExists == null)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User is not exists !!!";
                    return resultMessage;
                }
                else
                {
                    var emailExists = await _userManager.FindByEmailAsync(student.Email);
                    if (emailExists != null && emailExists.Id != student.Id.ToString() && !emailExists.IsDeleted)
                    {
                        resultMessage.Result = false;
                        resultMessage.Message = "Email already Exists";
                        return resultMessage;
                    }
                    var mobExists = _dbContext.Users.Any(x => x.PhoneNumber == student.MobileNumber && x.Id != student.Id.ToString() && !x.IsDeleted);
                    if (mobExists)
                    {
                        resultMessage.Result = false;
                        resultMessage.Message = "Mobile number is already exists.";
                        return resultMessage;
                    }
                    string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(student.Password));
                    var studentId = Guid.Parse(userExists.Id);
                    var encryptedPassword = _userManager.PasswordHasher.HashPassword(userExists, student.Password);
                    userExists.PasswordHash = encryptedPassword;
                    userExists.Password = encodedPassword;
                    userExists.Email = student.Email;
                    userExists.UserName = student.Email;
                    userExists.PhoneNumber = student.MobileNumber;
                    userExists.DisplayName = student.FullName.Trim();
                    userExists.IsVerified = false;
                    userExists.LastModifierUserId = student.UserId;
                    userExists.LastModifyDate = DateTime.UtcNow;
                    var result = await _userManager.UpdateAsync(userExists);
                    return new Com.ResultMessageAdmin
                    {
                        Result = true,
                        Message = "User Updated Successfully"
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Res.StudentByIdInfo?> GetStudentById(Req.GetStudentById student)
        {
            StudentByIdInfo user = new();
            List<Res.PurchaseItems> purchaseItems = new();
            List<Res.WalletTransaction> walletTransactions = new();
            var UserData = await _userManager.FindByIdAsync(student.Id.ToString());
            var studentCourse = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == student.Id && !x.IsDeleted);
            if (UserData != null)
            {
                string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(UserData.Password));

                var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == UserData.InstituteId && !x.IsDeleted);
                user.Id = new Guid(UserData.Id);
                user.Name = UserData.DisplayName;
                user.Password = decodedPassword;
                user.MobileNumber = UserData.PhoneNumber;
                user.Email = UserData.Email;
                user.ProfileImage = UserData.ProfileImage;
                user.IsVerified = UserData.IsVerified;
                user.Balance = UserData.Balance;
                user.InstituteId = instituteData != null ? instituteData.Id.ToString() : "";
                user.InstituteName = instituteData != null ? instituteData.InstituteName : "";
                user.InstituteLogo = instituteData != null ? instituteData.InstituteLogo : "";
                var subCourseId = studentCourse != null ? studentCourse.SubCourseId : Guid.Empty;
                var subcourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                user.SubcourseName = subcourseData != null ? subcourseData.SubCourseName : "";
                user.SubcourseId = subcourseData != null ? subcourseData.Id.ToString() : "";
                user.CourseId = subcourseData != null ? subcourseData.CourseID.ToString() : "";
                var courseId = subcourseData != null ? subcourseData.CourseID : Guid.Empty;
                var cousrseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                user.CourseName = cousrseDetail != null ? cousrseDetail.CourseName : "";
                var purchaseItemsAllDetails = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.StudentId == user.Id && !x.IsDeleted);
                var purchaseItemsDetails = purchaseItemsAllDetails.OrderByDescending(x => x.CreationDate).Take(5).ToList();
                if (purchaseItemsDetails.Any())
                {
                    foreach (var purchase in purchaseItemsDetails)
                    {
                        Res.PurchaseItems purchaseInfo = new();
                        var itemDetails = await _myCartRepository.GetProductCategoryName(purchase.ProductCategory, purchase.ProductId);
                        purchaseInfo.ItemName = itemDetails.ProductName;
                        purchaseInfo.Amount = purchase.Price;
                        purchaseInfo.Type.Add(purchase.ProductCategory.ToString());
                        purchaseInfo.Date = purchase.CreationDate;
                        purchaseItems.Add(purchaseInfo);
                    }
                    user.TotalItems = purchaseItemsAllDetails.Count;
                    user.PurchaseItems = purchaseItems;
                }

                var walletHistories = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => x.StudentId == user.Id && !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
                //   var walletHistories = allWalletHistories.OrderByDescending(x=>x.CreationDate).Take(5).ToList();
                if (walletHistories.Any())
                {
                    foreach (var walletHistory in walletHistories)
                    {
                        Res.WalletTransaction walletTransaction = new();
                        walletTransaction.Amount = walletHistory.CreditAmount > 0 ? walletHistory.CreditAmount : walletHistory.DebitAmount;
                        var walletTransactionType = walletHistory.CreditAmount > 0 ? "Deposit" : "Withdraw";
                        walletTransaction.Type.Add(walletTransactionType);
                        walletTransaction.Date = walletHistory.CreationDate;
                        walletTransactions.Add(walletTransaction);
                    }
                    user.WalletTransactions = walletTransactions;
                }
                return user;
            }
            return null;
        }
        public async Task<bool> RemoveStudent(Req.GetStudentById student)
        {
            string studentUserId = student.Id.ToString();
            ApplicationUser user = await _userManager.FindByIdAsync(studentUserId);
            if (user != null && !user.IsDeleted)
            {
                user.IsDeleted = true;
                user.IsActive = false;
                user.IsVerified = false;
                user.DeleterUserId = student.UserId;
                user.DeletionDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                var studentId = Guid.Parse(user.Id);
                var studentCourseCheck = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == studentId && !x.IsDeleted);
                if (studentCourseCheck != null)
                {
                    studentCourseCheck.IsDeleted = true;
                    studentCourseCheck.IsActive = false;
                    studentCourseCheck.DeleterUserId = student.UserId;
                    studentCourseCheck.DeletionDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<StudentCourse>().Update(studentCourseCheck);
                }
                var token = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == user.Id && !x.IsDeleted);
                if (token != null)
                {
                    await _unitOfWork.Repository<DM.Token>().Delete(token);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //public async Task<bool> BulkUpload([FromForm] Req.BulkUpload bulkUpload)
        //{
        //    string excelUrl = await _fileRepository.SaveExcel(bulkUpload.file, "StudentExcel", bulkUpload.UserId);
        //    if (!string.IsNullOrEmpty(excelUrl))
        //    {
        //        var rowCount = await ReadExcelFile(excelUrl, bulkUpload.UserId);
        //        var rowCount = await ReadExcelFile(bulkUpload.file, "StudentExcel", bulkUpload.UserId);
        //    return rowCount;
        //    }
        //    return false;

        //}
        public async Task<bool> BulkUpload([FromForm] Req.BulkUpload bulkUpload)
        {

            var rowCount = await ReadExcelFile(bulkUpload.file, bulkUpload.UserId);
            return rowCount;

        }
        public async Task<bool> ReadExcelFile(IFormFile? file, Guid userId)
        {
            try
            {
                // create a file path for the Excel file
                Stream fileStream = GetUploadedFileStream(file); // Replace with your own implementation
                using (ExcelPackage package = new ExcelPackage(fileStream))
                {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    if (worksheet != null)
                        {
                            var rows = worksheet.Dimension.Rows;
                            var columns = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rows; row++)
                            {
                                var data = new List<string[]>();
                                var rowData = new List<string>();
                                for (int col = 1; col <= columns; col++)
                                {
                                    var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
                                    rowData.Add(cellValue);
                                }
                                data.Add(rowData.ToArray());

                                for (int i = 0; i < data.Count; i++)
                                {
                                    var student = new Req.AddBulkUploadStudent
                                    {
                                        FullName = data[i][0].ToString() ?? string.Empty,
                                        Email = data[i][1].ToString() ?? string.Empty,
                                        MobileNumber = data[i][2].ToString() ?? string.Empty,
                                        Password = data[i][3].ToString() ?? string.Empty,
                                        Amount = data[i][4].ToString() ?? string.Empty,
                                        UserId = userId,
                                        IsBulkUpload = true
                                    };
                                    await AddBulkUploadStudent(student);
                                }
                            }

                            return true;
                        }
                        return false;
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Stream GetUploadedFileStream(IFormFile? file)
        {           
            Stream fileStream = file.OpenReadStream();
            return fileStream;
        }
        //public async Task<bool> ReadExcelFile(string fileName, Guid userId)
        //{
        //    try
        //    {
        //        // create a file path for the Excel file
        //        string directoryPath = fileName;
        //        string partialPath = directoryPath;
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
        //        using (var package = new ExcelPackage(new FileInfo(filePath)))
        //        {
        //            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //            if (worksheet != null)
        //            {
        //                var rows = worksheet.Dimension.Rows;
        //                var columns = worksheet.Dimension.Columns;

        //                for (int row = 2; row <= rows; row++)
        //                {
        //                    var data = new List<string[]>();
        //                    var rowData = new List<string>();
        //                    for (int col = 1; col <= columns; col++)
        //                    {
        //                        var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
        //                        rowData.Add(cellValue);
        //                    }
        //                    data.Add(rowData.ToArray());

        //                    for (int i = 0; i < data.Count; i++)
        //                    {
        //                        var student = new Req.AddBulkUploadStudent
        //                        {
        //                            FullName = data[i][0].ToString() ?? string.Empty,
        //                            Email = data[i][1].ToString() ?? string.Empty,
        //                            MobileNumber = data[i][2].ToString() ?? string.Empty,
        //                            Password = data[i][3].ToString() ?? string.Empty,
        //                            Amount = data[i][4].ToString() ?? string.Empty,
        //                            UserId = userId,
        //                            IsBulkUpload = true
        //                        };
        //                        await AddBulkUploadStudent(student);
        //                    }
        //                }

        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}       
        /// <summary>
        /// Get Excel Data
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="baseMerchant"></param>
        /// <returns></returns>
        public async Task<bool> GetExcelData(string? filePaths)
        {
            WorkBook workbook = WorkBook.Load(filePaths);
            WorkSheet sheet = workbook.DefaultWorkSheet;
            DataTable csvFilereader = sheet.ToDataTable();
            DataRow row = csvFilereader.Rows[0];
            csvFilereader.Rows.Remove(row);
            sheet.Rows[0].RemoveRow();
            List<Req.AddStudent> students = new();
            Res.CustomerBulkUplpad customerBulkUplpad = new()
            {
                TotalCustomer = csvFilereader.Rows.Count
            };

            for (int i = 0; i < csvFilereader.Rows.Count; i++)
            {
                customerBulkUplpad.TotalAdded += 1;
                var student = new Req.AddStudent
                {
                    FullName = csvFilereader.Rows[i][0].ToString() ?? String.Empty,
                    Email = csvFilereader.Rows[i][1].ToString() ?? String.Empty,
                    MobileNumber = csvFilereader.Rows[i][2].ToString() ?? String.Empty,
                    Password = csvFilereader.Rows[i][3].ToString() ?? String.Empty,
                };
                await AddStudent(student);
            }
            return true;
        }
        public FileStreamResult? GetSample()
        {
            DataTable dt = new DataTable("SampleDataTable");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Full Name"), new DataColumn("Email"), new DataColumn("Mobile Number"), new DataColumn("Password"), new DataColumn("Amount") });
            // create an Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("SampleWorksheet");
            worksheet.Cells.LoadFromDataTable(dt, true);
            // convert the Excel package to a byte array
            var fileBytes = package.GetAsByteArray();
            // set the content type and file name
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "SampleExcelFile.xlsx";
            // create a file stream result
            var stream = new MemoryStream(fileBytes);
            var fileStreamResult = new FileStreamResult(stream, contentType);
            // set the file download name
            fileStreamResult.FileDownloadName = fileName;
            return fileStreamResult;
        }
        public async Task<bool> UpdateRecords()
        {
            var staffList = _userManager.GetUsersInRoleAsync(UserRoles.Student).Result.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
            var studentDetails = await _dbContext.Users.Where(x => staffList.Contains(x.Id) && x.InstituteId != Guid.Empty).ToListAsync();
            foreach (var student in studentDetails)
            {
                var studentsubcours = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == Guid.Parse(student.Id) && x.InstituteId == Guid.Empty);
                if (studentsubcours != null)
                {
                    studentsubcours.InstituteId = student.InstituteId;
                    await _unitOfWork.Repository<DM.StudentCourse>().Update(studentsubcours);
                }
            }
            return true;
        }

    }
}
