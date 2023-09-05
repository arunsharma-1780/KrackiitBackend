using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Org.BouncyCastle.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;
using AutoMapper;


namespace OnlinePractice.API.Repository.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService mailService;
        private readonly IConfiguration _configuration;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IInstituteRepository _instituteRepository;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly DBContext _dbContext;


        public AccountRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IEmailService mailService, IConfiguration configuration, IPermissionRepository permissionRepository,
            IFileRepository fileRepository, IHttpContextAccessor baseUrl, IInstituteRepository instituteRepository, DBContext dbContext)
        {
            _userManager = userManager;
            this.mailService = mailService;
            _roleManager = roleManager;
            _configuration = configuration;
            _permissionRepository = permissionRepository;
            _fileRepository = fileRepository;
            _baseUrl = baseUrl;
            _instituteRepository = instituteRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Tokens?> Login(Req.Login login)
        {

            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(login.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password) && !user.IsDeleted)
                {
                    var permission = await _permissionRepository.GetPermission(user.Id);
                    string permissionJson = JsonConvert.SerializeObject(permission);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    string role = userRoles.First();
                    if (role == UserRoles.Student)
                    {
                        return null;
                    }
                    var institute = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == user.InstituteId && !x.IsDeleted);
                    var instituteId = institute != null ? institute.Id.ToString() : "";
                    var instituteName = institute != null ? institute.InstituteName : "";
                    var instituteLogo = institute != null ? institute.InstituteLogo : "";
                    var checkId = Guid.Parse(user.Id);
                    var studentCourse = await _unitOfWork.Repository<DM.StudentCourse>().GetSingle(x => x.StudentId == checkId && !x.IsDeleted);
                    var subcourseIdCheck = studentCourse != null && studentCourse.SubCourseId != Guid.Empty ? studentCourse.SubCourseId.ToString() : "";
                    var subCourseId = studentCourse != null ? studentCourse.SubCourseId : Guid.Empty;
                    var subcourseMasterCheck = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == subCourseId && !x.IsDeleted);
                    var subcourseName = subcourseMasterCheck != null ? subcourseMasterCheck.SubCourseName : "";

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(type: "userId", value: user.Id),
                    new Claim(type: "email", value: user.Email),
                    new Claim(type: "mobileNumber", value: user.PhoneNumber),
                    new Claim(type: "name", value: user.DisplayName),
                    new Claim(type: "instituteId", value: instituteId),
                    new Claim("instituteName",value : instituteName),
                    new Claim("instituteLogo",value : instituteLogo),
                    new Claim(type: "subcourseId", value: subcourseIdCheck),
                    new Claim("subcourseName",subcourseName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("permision", permissionJson, JsonClaimValueTypes.JsonArray),
                    new Claim("role", role),
                    new Claim("isVerified", user.IsVerified.ToString(), ClaimValueTypes.Boolean)
                };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var token = GetToken(authClaims);
                    return (new Tokens
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),

                    });
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// AddAdmin Method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddAdmin(Req.Register model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            //var MobExists = await _userManager.GetPhoneNumberAsync(userExists);
            if (userExists != null)
                return false;
            string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(model.Password));
            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Password = model.Password,
                DisplayName = model.FullName.Trim(),
                PhoneNumber = model.MobileNumber,
                Location = model.Location,
                ProfileImage = model.ProfileImage,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = Guid.NewGuid(),
                //IsVerified = true,
                IsActive = true,
                IsDeleted = false

            };

            var result = await _userManager.CreateAsync(user, decodedPassword);
            if (!result.Succeeded)
                return false;

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            }
            return true;
        }

        /// <summary>
        /// UploadProfileImage Method
        /// </summary>
        /// <param name="profileImage"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(Req.ProfileImage profileImage)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(profileImage.Image, "ProfileImage");
            return domain + imageUrl;
        }

        /// <summary>
        /// UpdateAdmin Method
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAdmin(Req.UpdateAdmin admin)
        {
            var userExists = await _userManager.FindByIdAsync(admin.Id.ToString());
            if (userExists == null)
                return false;
            userExists.DisplayName = admin.FullName.Trim();
            userExists.PhoneNumber = admin.MobileNumber;
            userExists.ProfileImage = admin.ProfileImage;
            userExists.Location = admin.Location;
            userExists.LastModifyDate = DateTime.UtcNow;
            userExists.LastModifierUserId = admin.UserId;
            await _userManager.UpdateAsync(userExists);
            return true;
        }

        /// <summary>
        /// RemoveProfileImage Method
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public async Task<bool> RemoveImage(Req.RemoveProfile profile)
        {
            var userExists = await _userManager.FindByIdAsync(profile.Id.ToString());
            if (userExists == null)
                return false;
            userExists.ProfileImage = string.Empty;
            userExists.LastModifyDate = DateTime.UtcNow;
            userExists.LastModifierUserId = profile.UserId;
            await _userManager.UpdateAsync(userExists);
            return true;
        }

        /// <summary>
        /// ChangePassword Method
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ChangePassWord(Req.ChangePassword password)
        {
            try
            {


                var userExists = await _userManager.FindByIdAsync(password.Id.ToString());
                if (userExists == null)
                    return false;
                var passwordExists = await _userManager.CheckPasswordAsync(userExists, password.CurrentPassword) && !userExists.IsDeleted;
                if (passwordExists)
                {
                    string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password.ConfirmPassword));
                    string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(encodedPassword));
                    var encryptedPassword = _userManager.PasswordHasher.HashPassword(userExists, decodedPassword);
                    userExists.Password = encodedPassword;
                    userExists.PasswordHash = encryptedPassword;
                    userExists.LastModifyDate = DateTime.UtcNow;
                    userExists.LastModifierUserId = password.UserId;
                    await _userManager.UpdateAsync(userExists);

                    if (decodedPassword != String.Empty)
                    {
                        EmailData emailData = new()
                        {
                            ToEmail = userExists.Email,
                            Subject = "Password Updated",
                            Body = "Your password is " + decodedPassword
                        };
                        await mailService.SendEmail(emailData);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// CheckPassword Method
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> CheckPassword(Req.CurrentPassword password)
        {
            var userExists = await _userManager.FindByIdAsync(password.Id.ToString());
            if (userExists == null)
                return false;
            var passwordExists = await _userManager.CheckPasswordAsync(userExists, password.Password) && !userExists.IsDeleted;
            if (passwordExists)
                return true;
            return false;
        }

        /// <summary>
        /// ForgetPassword Method
        /// </summary>
        /// <param name="forgotPassword"></param>
        /// <returns></returns>
        public async Task<bool> ForgotPassWord(Req.ForgotPassword forgotPassword)
        {
            bool emailtoSent = false;
            string userEmail = string.Concat(HttpUtility.UrlDecode(forgotPassword.Email).Where(c => !char.IsWhiteSpace(c)));
            ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return false;
            var userRoles = await _userManager.GetRolesAsync(user);
            if (user != null && !user.IsDeleted && userRoles.Contains(UserRoles.Admin))
            {
                string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(user.Password));
                if (decodedPassword != String.Empty)
                {
                    EmailData emailData = new()
                    {
                        ToEmail = user.Email,
                        Subject = "Retrieve Password",
                        Body = "Your password is " + decodedPassword
                    };
                    emailtoSent = await mailService.SendEmail(emailData);
                    return emailtoSent;
                }
                return false;
            }
            else if (user != null && !user.IsDeleted && userRoles.Contains(UserRoles.Staff))
            {

                var adminRole = _userManager.GetUsersInRoleAsync(UserRoles.Admin).Result.FirstOrDefault(x => !x.IsDeleted);
                if (adminRole != null)
                {
                    EmailData emailDataS = new()
                    {
                        ToEmail = adminRole.Email,
                        Subject = "Request for Password",
                        Body = "Hello " + adminRole.DisplayName + " Your staff  is requested Password by Email = " + forgotPassword.Email
                    };
                    emailtoSent = await mailService.SendEmail(emailDataS);

                    return emailtoSent;
                }
                return false;

            }
            else
            {
                return emailtoSent;
            }
        }

        /// <summary>
        /// GetUserData by ID Method
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public async Task<Res.UserById?> GetUserById(Req.GetUserById admin)
        {
            Res.UserById user = new();
            var UserData = await _userManager.FindByIdAsync(admin.Id.ToString());

            if (UserData != null)
            {
                user.Id = new Guid(UserData.Id);
                user.Name = UserData.DisplayName;
                user.Location = UserData.Location;
                user.MobileNumber = UserData.PhoneNumber;
                user.Email = UserData.Email;
                user.ProfileImage = UserData.ProfileImage;
                return user;
            }
            return null;
        }


        /// <summary>
        /// CheckOldPassword Method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> CheckOldPassword(string userId, string password)
        {

            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(password))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success)
                    return true;
            }
            return false;
        }

        public string GenerateRandomPassword(PasswordOptions? opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        /// <summary>
        /// GetToken
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
        ///ResendPAssword Method 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ResendPassword(Req.ForgotPassword password)
        {
            bool emailtoSent = false;

            string userEmail = string.Concat(HttpUtility.UrlDecode(password.Email).Where(c => !char.IsWhiteSpace(c)));
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null && !user.IsDeleted)
            {

                string decodedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(user.Password));
                if (!string.IsNullOrEmpty(decodedPassword))
                {
                    EmailData emailData = new()
                    {
                        ToEmail = user.Email,
                        Subject = "Retrieve Password",
                        Body = "Your password is " + decodedPassword
                    };
                    emailtoSent = await mailService.SendEmail(emailData);
                    return emailtoSent;
                }

            }
            return emailtoSent;
        }

    }
}



