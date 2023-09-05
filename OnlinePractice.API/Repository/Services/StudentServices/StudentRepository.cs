using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentRepository : IStudentRepository
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService mailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DBContext _dbContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly IFileRepository _fileRepository;


        public StudentRepository(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, DBContext dBConext, IAccountRepository accountRepository, IConfiguration configuration
            , DBContext dBContext, IEmailService mailService, IHttpContextAccessor baseUrl, IFileRepository fileRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _dbContext = dBContext;
            _accountRepository = accountRepository;
            _configuration = configuration;
            this.mailService = mailService;
            _baseUrl = baseUrl;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// Add Student Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public async Task<Com.ResultMessage> AddStudent([FromBody] Req.CreateStudent student)
        {
            Com.ResultMessage resultMessage = new();
            var userExists = await _userManager.FindByEmailAsync(student.Email);
            var mobExists =  _dbContext.Users.Any(x => x.PhoneNumber == student.MobileNumber && !x.IsDeleted);
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
                var userRoles = await _userManager.GetRolesAsync(user);
                string role = userRoles.First();
                var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == user.InstituteId && !x.IsDeleted);
                var instituteName = institute != null ? institute.InstituteName : "";

                var authClaims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(type: "userId", value: user.Id),
                        new Claim(type: "email", value: user.Email),
                        new Claim(type: "mobileNumber", value: user.PhoneNumber),
                        new Claim(type: "name", value: user.DisplayName),
                        new Claim(type: "profileImage", value: user.ProfileImage),
                        new Claim("isVerified", user.IsVerified.ToString(), ClaimValueTypes.Boolean),
                        new Claim(type: "instituteId", value: string.Empty),
                        new Claim("instituteName",value:string.Empty),
                        new Claim("instituteLogo",value:string.Empty),
                        new Claim(type: "subcourseId", value: string.Empty),
                        new Claim("subcourseName",value:string.Empty),
                        new Claim("courseId",value : string.Empty),
                        new Claim("courseName",value : string.Empty),
                        new Claim("balance",value:user.Balance.ToString(),ClaimValueTypes.Double),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        //new Claim("permision", permissionJson, JsonClaimValueTypes.JsonArray),
                        new Claim("role", role),
                    };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);
                #region Insert Token
                var existingToken = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == user.Id && !x.IsDeleted);
                if (existingToken != null)
                {
                    existingToken.Tokens = new JwtSecurityTokenHandler().WriteToken(token);
                    existingToken.LastModifyDate = DateTime.UtcNow;
                    existingToken.LastModifierUserId = Guid.Parse(user.Id);
                    await _unitOfWork.Repository<DM.Token>().Update(existingToken);
                }
                else
                {
                    DM.Token newtoken = new()
                    {
                        Tokens = new JwtSecurityTokenHandler().WriteToken(token),
                        UserId = user.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = Guid.Parse(user.Id)

                    };
                    await _unitOfWork.Repository<DM.Token>().Insert(newtoken);
                }
                #endregion
                return new Com.ResultMessage
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
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
                // userExists.InstituteId = student.InstituteId;
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
            var result1 =   await _unitOfWork.Repository<DM.WalletHistory>().Insert(walletHistory);
                var userRoles = await _userManager.GetRolesAsync(userExists);
                string role = userRoles.First();
                var authClaims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, userExists.UserName),
                        new Claim(type: "userId", value: userExists.Id),
                        new Claim(type: "email", value: userExists.Email),
                        new Claim(type: "mobileNumber", value: userExists.PhoneNumber),
                        new Claim(type: "name", value: userExists.DisplayName),
                        new Claim(type: "profileImage", value: userExists.ProfileImage),
                        new Claim(type: "instituteId", value: string.Empty),
                        new Claim("instituteName",value:string.Empty),
                        new Claim("instituteLogo",value:string.Empty),
                        new Claim(type: "subcourseId", value: string.Empty),
                        new Claim("subcourseName",value : string.Empty),
                        new Claim("courseId",value : string.Empty),
                        new Claim("courseName",value : string.Empty),
                        new Claim("isVerified", userExists.IsVerified.ToString(), ClaimValueTypes.Boolean),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("balance",value:userExists.Balance.ToString(),ClaimValueTypes.Double),
                        new Claim("role", role),
                    };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);
                #region Insert Token
                var existingToken = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == userExists.Id && !x.IsDeleted);
                if (existingToken != null)
                {
                    existingToken.Tokens = new JwtSecurityTokenHandler().WriteToken(token);
                    existingToken.LastModifyDate = DateTime.UtcNow;
                    existingToken.LastModifierUserId = Guid.Parse(userExists.Id);
                    await _unitOfWork.Repository<DM.Token>().Update(existingToken);
                }
                else
                {
                    DM.Token newtoken = new()
                    {
                        Tokens = new JwtSecurityTokenHandler().WriteToken(token),
                        UserId = userExists.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = Guid.Parse(userExists.Id)

                    };
                    await _unitOfWork.Repository<DM.Token>().Insert(newtoken);
                }
                #endregion
                return new Com.ResultMessage
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Result = true,
                    Message = "User Created Successfully"
                };

            }
            resultMessage.Result = false;
            resultMessage.Message = "Email Already Exists";
            return resultMessage;
        }


        /// <summary>
        /// Edit Student Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public async Task<Com.ResultMessage> EditStudent(Req.EditStudent student)
        {
            try
            {
                Com.ResultMessage resultMessage = new();
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
                    var studentId = Guid.Parse(userExists.Id);
                    var checkStudent = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == studentId && !x.IsDeleted);
                    if (checkStudent == null)
                    {
                        StudentCourse subcourse = new()
                        {
                            StudentId = studentId,
                            SubCourseId = student.SubcourseId,
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = student.UserId,
                            InstituteId= student.InstituteId,
                            IsActive = true,
                            IsDeleted = false
                        };
                        var subcourseData = await _unitOfWork.Repository<StudentCourse>().Insert(subcourse);
                    }
                    else
                    {
                        checkStudent.SubCourseId = student.SubcourseId;
                        checkStudent.StudentId = studentId;
                        checkStudent.InstituteId= student.InstituteId;
                        checkStudent.CreationDate = DateTime.UtcNow;
                        checkStudent.CreatorUserId = student.UserId;
                        checkStudent.IsActive = true;
                        checkStudent.IsDeleted = false;
                        await _unitOfWork.Repository<StudentCourse>().Update(checkStudent);
                    }


                    userExists.Email = student.Email;
                    userExists.UserName = student.Email;
                    userExists.PhoneNumber = student.MobileNumber;
                    userExists.DisplayName = student.FullName.Trim();
                    userExists.ProfileImage = student.ProfileImage;
                    userExists.InstituteId = student.InstituteId;
                    userExists.IsVerified = true;
                    userExists.LastModifierUserId = student.UserId;
                    userExists.LastModifyDate = DateTime.UtcNow;
                    var result = await _userManager.UpdateAsync(userExists);

                    var userRoles = await _userManager.GetRolesAsync(userExists);
                    string role = userRoles.First();
                    var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == student.InstituteId && !x.IsDeleted);
                    var instituteId = institute != null ? institute.Id.ToString() : "";
                    var instituteName = institute != null ? institute.InstituteName : "";
                    var instituteLogo = institute != null ? institute.InstituteLogo : "";
                    var subCourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == student.SubcourseId && !x.IsDeleted);
                    var subcourseId = subCourseData != null ? subCourseData.Id.ToString() : "";

                    var subcourseName = subCourseData != null ? subCourseData.SubCourseName : "";
                    var courseId = subCourseData != null ? subCourseData.CourseID.ToString() : "";
                    var checkCourseId = subCourseData != null ? subCourseData.CourseID : Guid.Empty;
                    var cousrseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == checkCourseId && !x.IsDeleted);
                    var courseName = cousrseDetail != null ? cousrseDetail.CourseName : "";

                    var authClaims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, userExists.UserName),
                        new Claim(type: "userId", value: userExists.Id),
                        new Claim(type: "email", value: userExists.Email),
                        new Claim(type: "mobileNumber", value: userExists.PhoneNumber),
                        new Claim(type: "name", value: userExists.DisplayName),
                        new Claim(type: "profileImage", value: userExists.ProfileImage),
                        new Claim("isVerified", userExists.IsVerified.ToString(), ClaimValueTypes.Boolean),
                        new Claim(type: "instituteId", value: instituteId),
                        new Claim("instituteName",value : instituteName),
                        new Claim("instituteLogo",value:instituteLogo),
                        new Claim(type: "subcourseId", value: subcourseId),
                        new Claim("subcourseName",value : subcourseName),
                        new Claim("courseId",value : courseId),
                        new Claim("courseName",value : courseName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("balance",value:userExists.Balance.ToString(),ClaimValueTypes.Double),
                        new Claim("role", role),
                    };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var token = GetToken(authClaims);
                    #region Insert Token
                    var existingToken = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == userExists.Id && !x.IsDeleted);
                    if (existingToken != null)
                    {
                        existingToken.Tokens = new JwtSecurityTokenHandler().WriteToken(token);
                        existingToken.LastModifyDate = DateTime.UtcNow;
                        existingToken.LastModifierUserId = Guid.Parse(userExists.Id);
                        await _unitOfWork.Repository<DM.Token>().Update(existingToken);
                    }
                    else
                    {
                        DM.Token newtoken = new()
                        {
                            Tokens = new JwtSecurityTokenHandler().WriteToken(token),
                            UserId = userExists.Id,
                            IsActive = true,
                            IsDeleted = false,
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = Guid.Parse(userExists.Id)

                        };
                        await _unitOfWork.Repository<DM.Token>().Insert(newtoken);
                    }
                    #endregion
                    return new Com.ResultMessage
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
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


        /// <summary>
        /// Get Token Student Method
        /// </summary>
        /// <param name="authClaims"></param>
        /// <returns></returns>
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                 expires: DateTime.UtcNow.AddYears(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }


        /// <summary>
        /// Get Token By Number Method
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Tokens?> GetTokenByNumber(Req.Tokenlogin login)
        {
            var mobExists = _dbContext.Users.FirstOrDefault(x => x.PhoneNumber == login.MobileNumber && !x.IsDeleted);
            if (mobExists != null)
            {
                var userRoles = await _userManager.GetRolesAsync(mobExists);
                string role = userRoles.First();
                var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == mobExists.InstituteId && !x.IsDeleted);
                var instituteId = institute != null ? institute.Id.ToString() : "";
                var instituteName = institute != null ? institute.InstituteName : "";
                var instituteLogo = institute != null ? institute.InstituteLogo : "";
                var UserId = Guid.Parse(mobExists.Id);
                var studentCourse = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == UserId && !x.IsDeleted);
                var subCourseId = studentCourse != null ? studentCourse.SubCourseId : Guid.Empty;
                var subCourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                var subcourseId = subCourseData != null ? subCourseData.Id.ToString() : "";
                var subcourseName = subCourseData != null ? subCourseData.SubCourseName : "";
                var courseId = subCourseData != null ? subCourseData.CourseID.ToString() : "";
                var checkCourseId = subCourseData != null ? subCourseData.CourseID : Guid.Empty;
                var cousrseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == checkCourseId && !x.IsDeleted);
                var courseName = cousrseDetail != null ? cousrseDetail.CourseName : "";
                var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, mobExists.UserName),
                        new Claim(type: "userId", value: mobExists.Id),
                        new Claim(type: "email", value: mobExists.Email),
                        new Claim(type: "mobileNumber", value: mobExists.PhoneNumber),
                        new Claim("isVerified", mobExists.IsVerified.ToString(), ClaimValueTypes.Boolean),
                        new Claim(type: "name", value: mobExists.DisplayName),
                          new Claim(type: "profileImage", value: mobExists.ProfileImage),
                        new Claim(type: "instituteId", value: instituteId),
                        new Claim("instituteName",value:instituteName),
                        new Claim("instituteLogo",value:instituteLogo),
                        new Claim(type: "subcourseId", value: subcourseId),
                        new Claim("subcourseName",value : subcourseName),
                         new Claim("courseId",value : courseId),
                        new Claim("courseName",value : courseName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim("balance",value:mobExists.Balance.ToString(),ClaimValueTypes.Double),

                        new Claim("role", role),
                    };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);
                #region Insert Token
                var existingToken = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == mobExists.Id && !x.IsDeleted);
                if (existingToken != null)
                {
                    existingToken.Tokens = new JwtSecurityTokenHandler().WriteToken(token);
                    existingToken.LastModifyDate = DateTime.UtcNow;
                    existingToken.LastModifierUserId = Guid.Parse(mobExists.Id);
                    await _unitOfWork.Repository<DM.Token>().Update(existingToken);
                }
                else
                {
                    DM.Token newtoken = new()
                    {
                        Tokens = new JwtSecurityTokenHandler().WriteToken(token),
                        UserId = mobExists.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreationDate = DateTime.UtcNow,
                        CreatorUserId = Guid.Parse(mobExists.Id)

                    };
                    await _unitOfWork.Repository<DM.Token>().Insert(newtoken);
                }
                #endregion
                return new Tokens
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),

                };
            }
            return null;
        }

        /// <summary>
        /// API for send OTP
        /// </summary>
        /// <param name="sendOTP"></param>
        /// <returns></returns>
        public async Task<SendOtp> SendOtp(Req.SendOTP sendOTP)
        {
            string message = "";
            var mobExists = _dbContext.Users.FirstOrDefault(x => x.PhoneNumber == sendOTP.MobileNumber && !x.IsDeleted);

            SendOtp sentotp = new();
            var otp = GenerateRandomNo();
            var uri = new Uri(_configuration.GetValue<string>("PrpSMS:PrpSmsUrl"));
            if (sendOTP.CallingUnit == "Signup")
            {
                if (mobExists == null)
                {
                    sentotp.isOtpSent = false;
                    sentotp.MID = "User is not exist with this mobile number!";
                    return sentotp;
                }
                else
                {
                    //message = "OTP for the Login: " + otp + " PRPOTP";
                    message = otp + " is your Braincord Education (KRACKITT) verification OTP. Do not share this OTP with anyone. Call (help number) for assistance.";
                }

            }
            else if (sendOTP.CallingUnit == "Login")
            {
                if (mobExists == null)
                {
                    sentotp.isOtpSent = false;
                    sentotp.MID = "User is not exist with this mobile number!";
                    return sentotp;
                }
                else
                {
                    message = "Hey, " + otp + " is your One Time Password(OTP) for Login verification on Braincord Education (KRACKITT)";
                }
            }
            else if (sendOTP.CallingUnit == "Forget")
            {
                if (mobExists == null)
                {
                    sentotp.isOtpSent = false;
                    sentotp.MID = "User is not exist with this mobile number!";
                    return sentotp;
                }
                else
                {
                    // message = "OTP for the Login: " + otp + " PRPOTP";
                    message = "Please use the code " + otp + " to reset your Braincord Education (KRACKITT) password. If you did not request your password to be reset, ignore this message.";
                }
            }
            else if (sendOTP.CallingUnit == "Update")
            {
                if (mobExists != null)
                {
                    sentotp.isOtpSent = false;
                    sentotp.MID = "User is already exist with this mobile number!";
                    return sentotp;
                }
                else
                {
                    // message = "OTP for the Login: " + otp + " PRPOTP";
                    message = "Please use the code " + otp + " to update your Braincord Education (KRACKITT) mobile number. If you did not change your mobile number, ignore this message.";
                }
            }

            else
            {
                sentotp.isOtpSent = false;
                sentotp.MID = "Calling unit is not exist!";
                return sentotp;
            }
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("uname", _configuration.GetValue<string>("PrpSMS:Username")),
                new KeyValuePair<string, string>("pass", _configuration.GetValue<string>("PrpSMS:Password")),
                new KeyValuePair<string, string>("send", _configuration.GetValue<string>("PrpSMS:SenderID")),
                new KeyValuePair<string, string>("dest", sendOTP.MobileNumber),
                new KeyValuePair<string, string>("msg", message),
            });
            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.PostAsync(uri, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (apiResponse != null)
                        {
                            string encryptedOtp = Convert.ToBase64String(Encoding.UTF8.GetBytes(otp.ToString()));
                            sentotp.isOtpSent = true;
                            sentotp.MID = apiResponse;
                            sentotp.otp = encryptedOtp;
                            if (sendOTP.CallingUnit == "Forget")
                            {
                                sentotp.IsForget = true;
                            }
                        }
                        else
                        {
                            sentotp.MID = "Otp is not sent!";
                            sentotp.isOtpSent = false;
                        }
                    }
                    else
                    {
                        sentotp.MID = "Otp is not sent!";
                        sentotp.isOtpSent = false;
                    }
                    return sentotp;
                }
            }

        }


        /// <summary>
        /// Check SMS Balance Method
        /// </summary>
        /// <returns></returns>
        public async Task<SMSBalance?> CheckSMSBalance()
        {
            SMSBalance sMSBalance = new();
            var uri = new Uri(_configuration.GetValue<string>("PrpSMS:PrpSmsBalanceUrl"));
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("uname", _configuration.GetValue<string>("PrpSMS:Username")),
                new KeyValuePair<string, string>("pass", _configuration.GetValue<string>("PrpSMS:Password")),
            });
            using (var httpClient = new HttpClient())
            {

                using (var response = httpClient.PostAsync(uri, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        sMSBalance.Balance = Convert.ToInt32(apiResponse);
                        return sMSBalance;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

        }


        /// <summary>
        /// Generate Random Number Method
        /// </summary>
        /// <returns></returns>
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }


        /// <summary>
        /// Student Login Method
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Tokens?> StudentLogin(Req.StudentLogin login)
        {
            try
            {
                ApplicationUser user = new();
                if (!string.IsNullOrEmpty(login.Email))
                {
                    user = await _userManager.FindByEmailAsync(login.Email);
                }
                if (!string.IsNullOrEmpty(login.MobileNumber))
                {
                    user =  _dbContext.Users.FirstOrDefault(x => x.PhoneNumber == login.MobileNumber && !x.IsDeleted);
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password) && !user.IsDeleted)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    string role = userRoles.First();
                    if (role != UserRoles.Student)
                    {
                        return null;
                    }
                    var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == user.InstituteId && !x.IsDeleted);
                    var instituteId = institute != null ? institute.Id.ToString() : "";
                    var instituteName = institute != null ? institute.InstituteName : "";
                    var instituteLogo = institute != null ? institute.InstituteLogo : "";
                    var checkId = Guid.Parse(user.Id);
                    var studentCourse = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == checkId && !x.IsDeleted);
                    var subcourseIdCheck = studentCourse != null && studentCourse.SubCourseId != Guid.Empty ? studentCourse.SubCourseId.ToString() : "";
                    var subCourseId = studentCourse != null ? studentCourse.SubCourseId : Guid.Empty;
                    var subcourseMasterCheck = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    var subcourseName = subcourseMasterCheck != null ? subcourseMasterCheck.SubCourseName : "";
                    var courseId = subcourseMasterCheck != null ? subcourseMasterCheck.CourseID.ToString() : "";
                    var checkCourseId = subcourseMasterCheck != null ? subcourseMasterCheck.CourseID : Guid.Empty;
                    var cousrseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == checkCourseId && !x.IsDeleted);
                    var courseName = cousrseDetail != null ? cousrseDetail.CourseName : "";

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(type: "userId", value: user.Id),
                    new Claim(type: "email", value: user.Email),
                    new Claim(type: "mobileNumber", value: user.PhoneNumber),
                    new Claim(type: "name", value: user.DisplayName),
                    new Claim(type: "profileImage", value: user.ProfileImage),
                    new Claim(type: "instituteId", value: instituteId),
                    new Claim("instituteName",value : instituteName),
                    new Claim("instituteLogo",value : instituteLogo),
                    new Claim(type: "subcourseId", value: subcourseIdCheck),
                    new Claim("subcourseName",value: subcourseName),
                    new Claim("courseId",value : courseId),
                    new Claim("courseName",value : courseName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("balance",value:user.Balance.ToString(),ClaimValueTypes.Double),
                    new Claim("role", role),
                    new Claim("isVerified", user.IsVerified.ToString(), ClaimValueTypes.Boolean)
                };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var token = GetToken(authClaims);
                    #region Insert Token
                    var existingToken = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == user.Id && !x.IsDeleted);
                    if (existingToken != null)
                    {
                        existingToken.Tokens = new JwtSecurityTokenHandler().WriteToken(token);
                        existingToken.LastModifyDate = DateTime.UtcNow;
                        existingToken.LastModifierUserId = Guid.Parse(user.Id);
                        await _unitOfWork.Repository<DM.Token>().Update(existingToken);
                    }
                    else
                    {
                        DM.Token newtoken = new()
                        {
                            Tokens = new JwtSecurityTokenHandler().WriteToken(token),
                            UserId = user.Id,
                            IsActive = true,
                            IsDeleted = false,
                            CreationDate = DateTime.UtcNow,
                            CreatorUserId = Guid.Parse(user.Id)

                        };
                        await _unitOfWork.Repository<DM.Token>().Insert(newtoken);
                    }
                    #endregion

                    return new Tokens
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),

                    };
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Get Student by I
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public async Task<Res.StudentById?> GetStudentById(Req.GetUserById student)
        {
            StudentById user = new();
            var UserData = await _userManager.FindByIdAsync(student.Id.ToString());
            var studentCourse = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == student.Id && !x.IsDeleted);
            if (UserData != null && studentCourse != null)
            {
                var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == UserData.InstituteId && !x.IsDeleted);
                user.Id = new Guid(UserData.Id);
                user.Name = UserData.DisplayName;
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
                user.courseId = subcourseData != null ? subcourseData.CourseID.ToString() : "";
                var courseId = subcourseData != null ? subcourseData.CourseID : Guid.Empty;
                var cousrseDetail = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == courseId && !x.IsDeleted);
                user.courseName = cousrseDetail != null ? cousrseDetail.CourseName : "";
                return user;
            }
            return null;
        }

        public async Task<bool> ForgotPassword(Req.ForgotStudentPassword forgotStudent)
        {
            var mobExists = _dbContext.Users.FirstOrDefault(x => x.PhoneNumber == forgotStudent.MobileNumber && !x.IsDeleted);
            if (mobExists != null)
            {
                string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(forgotStudent.ConfirmPassword));
                string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(encodedPassword));
                var encryptedPassword = _userManager.PasswordHasher.HashPassword(mobExists, decodedPassword);
                mobExists.Password = encodedPassword;
                mobExists.PasswordHash = encryptedPassword;
                mobExists.LastModifyDate = DateTime.UtcNow;
                mobExists.LastModifierUserId = forgotStudent.UserId;
                await _userManager.UpdateAsync(mobExists);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUser(Req.MobNumber number)
        {
            var mobExists = _dbContext.Users.FirstOrDefault(x => x.PhoneNumber == number.MobileNumber && !x.IsDeleted);
            if (mobExists != null)
            {
                mobExists.IsDeleted = true;
                mobExists.IsActive = false;
                mobExists.IsVerified = false;
                mobExists.DeleterUserId = number.UserId;
                mobExists.DeletionDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(mobExists);
                var studentId = Guid.Parse(mobExists.Id);
                var studentCourseCheck = await _unitOfWork.Repository<StudentCourse>().GetSingle(x => x.StudentId == studentId && !x.IsDeleted);
                if (studentCourseCheck != null)
                {
                    studentCourseCheck.IsDeleted = true;
                    studentCourseCheck.IsActive = false;
                    studentCourseCheck.DeleterUserId = number.UserId;
                    studentCourseCheck.DeletionDate = DateTime.UtcNow;
                    await _unitOfWork.Repository<StudentCourse>().Update(studentCourseCheck);
                }
                var token = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == mobExists.Id && !x.IsDeleted);
                if (token != null)
                {
                    await _unitOfWork.Repository<DM.Token>().Delete(token);
                }
                return true;
            }
            return false;
        }

        public async Task<string> GetToken(CurrentUser userId)
        {
            var token = await _unitOfWork.Repository<DM.Token>().GetSingle(x => x.UserId == userId.UserId.ToString() && !x.IsDeleted);
            if (token != null)
            {
                return token.Tokens;
            }
            return string.Empty;
        }

        public async Task<Com.ResultMessage> EditStudentProfile(Req.UpdateStudentProfile studentProfile)
        {
            try
            {

                Com.ResultMessage resultMessage = new();
                var userExists = await _userManager.FindByIdAsync(studentProfile.Id.ToString());
                var studentId = Guid.Parse(userExists.Id);
                if (userExists == null)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "User not Exists";
                    return resultMessage;
                }
                var emailExists = await _userManager.FindByEmailAsync(studentProfile.Email);
                if (emailExists != null && !emailExists.IsDeleted && emailExists.Id != studentProfile.Id.ToString())
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "Email already Exists";
                    return resultMessage;
                }
                var mobExists = _dbContext.Users.Any(x => x.PhoneNumber == studentProfile.MobileNumber && x.Id != studentProfile.Id.ToString() && !x.IsDeleted);
                if (mobExists)
                {
                    resultMessage.Result = false;
                    resultMessage.Message = "Mobile number is already exists.";
                    return resultMessage;
                }
                //var otpSent = SendOtp(studentProfile.MobileNumber.ToString());
                //var mobCheck= await 
                userExists.DisplayName = studentProfile.FullName;
                userExists.Email = studentProfile.Email;
                userExists.PhoneNumber = studentProfile.MobileNumber;
                userExists.LastModifierUserId = studentId;
                userExists.LastModifyDate = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(userExists);
                resultMessage.Result = false;
                resultMessage.Message = "Student Edited Succesflly";
                return resultMessage;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddFeedback(Req.AddFeedback addFeedback)
        {
            var userData = await _userManager.FindByIdAsync(addFeedback.UserId.ToString());
            if (userData != null)
            {
                DM.StudentFeedbacks studentFeedbacks = new()
                {
                    StudentId = addFeedback.UserId,
                    StudentFeedback = addFeedback.StudentFeedback,
                    CreatorUserId = addFeedback.UserId,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false,
                };
                int result = await _unitOfWork.Repository<DM.StudentFeedbacks>().Insert(studentFeedbacks);
                bool emailtoSent = false;
                var adminRole = _userManager.GetUsersInRoleAsync(UserRoles.Admin).Result.FirstOrDefault(x => !x.IsDeleted);
                if (adminRole != null)
                {
                    EmailData feedbackData = new()
                    {
                        ToEmail = adminRole.Email,
                        Subject = "User Feedback",
                        Body = "Hello " + bold(adminRole.DisplayName) + "," + " <br/> Your User " + bold(userData.Email) + " has send the Feedback ." + "<br/> Feedback :<br/>" + bold(addFeedback.StudentFeedback),
                    };
                    emailtoSent = await mailService.SendEmail(feedbackData);

                    return emailtoSent;
                }
                return true;
            }
            return false;
        }

        public async Task<string> UploadImage(Req.ProfileImage profileImage)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(profileImage.Image, "StudentProfileImage");
            return domain + imageUrl;
        }

        private static string bold(string text)
        {
            return $"<b>{text}</b>";
        }

        public bool CheckStudentLanguage(string language)
        {
            if (language == "All" || language == "English" || language == "Gujarati" || language == "Hindi" || language == "Marathi")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Res.StudentInstitueList?> GetStudentInstitute(CurrentUser user)
        {
            Res.StudentInstitueList studentInstitueList = new();

            var studentInstitue = await _userManager.FindByIdAsync(user.UserId.ToString());
            var institueDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == studentInstitue.InstituteId && x.InstituteName != "Other" && !x.IsDeleted);
            var studentInstitutes = new List<Res.StudentInstitute>();
            if (institueDetails != null)
            {
                Res.StudentInstitute studentInstitute = new();
                studentInstitute.InstitueId = institueDetails.Id;
                studentInstitute.InstituteName = institueDetails.InstituteName;
                studentInstitute.InstituteLogo = institueDetails.InstituteLogo;
                studentInstitutes.Add(studentInstitute);
            }
            var otherInstitueDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.InstituteName == "Other" && !x.IsDeleted);
            if (otherInstitueDetails != null)
            {
                Res.StudentInstitute studentInstitute = new();
                studentInstitute.InstitueId = otherInstitueDetails.Id;
                studentInstitute.InstituteName = otherInstitueDetails.InstituteName;
                studentInstitute.InstituteLogo = otherInstitueDetails.InstituteLogo;
                studentInstitutes.Add(studentInstitute);
            }
            studentInstitueList.StudentInstitutes = studentInstitutes;
            studentInstitueList.TotalRecord = studentInstitutes.Count;
            if (studentInstitueList.StudentInstitutes.Any())
            {
                return studentInstitueList;
            }
            else
            {
                return null;
            }          
        }


    }
}



