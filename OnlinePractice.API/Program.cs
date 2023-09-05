using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Repository.Services;
using OnlinePractice.API.Repository.Services.StudentServices;
using OnlinePractice.API.Services;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using OnlinePractice.API.Validator.Services;
using OnlinePractice.API.Validator.Services.Student_Services;
using Serilog;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Logging
var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
#endregion
#region DB Connection
builder.Services.AddDbContext<DBContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion
#region Service Registration

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IStudyMaterialRepository, StudyMaterialRepository>();
builder.Services.AddTransient<IPermissionRepository, PermissionRepository>();
builder.Services.AddTransient<IStaffRepository, StaffRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IInstituteRepository, InstituteRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IStateRepository, StateRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<IExamTypeRepository, ExamTypeRepository>();
builder.Services.AddTransient<ICourseRepository, CourseRepository>();
builder.Services.AddTransient<ISubCourseRepository, SubCourseRepository>();
builder.Services.AddTransient<ISubjectRepository, SubjectRepository>();
builder.Services.AddTransient<ITopicRepository, TopicRepository>();
builder.Services.AddTransient<ISubTopicRepository, SubTopicRepository>();
builder.Services.AddTransient<IQuestionBankRepository, QuestionBankRepository>();
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IExamPatternRepository,ExamPatternRepository>();
builder.Services.AddTransient<IMockTestRepository,MockTestRepository>();
builder.Services.AddTransient<IEbookRepository,EbookRepository>();
builder.Services.AddTransient<IVideoRepository, VideoRepository>();
builder.Services.AddTransient<IPreviousYearPaperRepository,PreviousYearPaperRepository>();
builder.Services.AddTransient<IStudentRepository,StudentRepository>();
builder.Services.AddTransient<IStudentEbookRespository,StudentEbookRepository>();
builder.Services.AddTransient<IStudentPYPRepository, StudentPYPRepository>();
builder.Services.AddTransient<IStudentMockTestRepository, StudentMockTestRepository>();
builder.Services.AddTransient<IMyCartRepository, MyCartRepository>();
builder.Services.AddTransient<IStudentVideoRespository, StudentVideoRespository>();
builder.Services.AddTransient<IMyPurchasedRespository, MyPurchasedRespository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<IStudentWalletRepository, StudentWalletRepository>();
builder.Services.AddTransient<IStudentResultRepository, StudentResultRepository>();
builder.Services.AddTransient<IStudentRegistrationRepository, StudentRegistrationRepository>();
builder.Services.AddTransient<IAdminResultRepository, AdminResultRepository>();
builder.Services.AddTransient<IAdminWalletRepository, AdminWalletRepository>();
builder.Services.AddTransient<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddTransient<ICommonRepository, CommonRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDirectoryBrowser();
#endregion
#region Validation Service Registration
builder.Services.AddTransient<IProductValidation, ProductValidation>();
builder.Services.AddTransient<IStudyMaterialValidation, StudyMaterialValidation>();
builder.Services.AddTransient<IAccountValidation, AccountValidation>();
builder.Services.AddTransient<IStaffValidation, StaffValidation>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IInstituteValidation, InstituteValidation>();
builder.Services.AddTransient<IExamTypeValidation, ExamTypeValidation>();
builder.Services.AddTransient<ISubCourseValidation, SubCourseValidation>();
builder.Services.AddTransient<ICourseValidation, CourseValidation>();
builder.Services.AddTransient<ISubjectValidation, SubjectValidation>();
builder.Services.AddTransient<ITopicValidation, TopicValidation>();
builder.Services.AddTransient<ISubTopicValidation, SubTopicValidation>();
builder.Services.AddTransient<IQuestionBankValidation, QuestionBankValidation>();
builder.Services.AddTransient<IExamPatternValidation, ExamPatternValidator>();
builder.Services.AddTransient<IMockTestValidation,MockTestValidation>();
builder.Services.AddTransient<IEbookValidation, EbookValidation>();
builder.Services.AddTransient<IVideoValidation,VideoValidation>();
builder.Services.AddTransient<IPreviousYearPaperValidation, PreviousYearPaperValidation>();
builder.Services.AddTransient<IStudentValidation,StudentValidation>();
builder.Services.AddTransient<IStudentValidation, StudentValidation>();
builder.Services.AddTransient<IStudentDashboardValidation,StudentDashboardValidation>();
builder.Services.AddTransient<IStudentMocktestValidation,StudentMocktestValidation>();
builder.Services.AddTransient<IStudentEbookValidation, StudentEbookValidation>();
builder.Services.AddTransient<IStudentPYPValidation, StudentPYPValidation>();
builder.Services.AddTransient<IMyCartValidation, MyCartValidation>();
builder.Services.AddTransient<IStudentVideoValidation, StudentVideoValidation>();
builder.Services.AddTransient<IStudentWalletValidation, StudentWalletValidation>();
builder.Services.AddTransient<IStudentResultValidation, StudentResultValidation>();
builder.Services.AddTransient<IMyPurchasedValidation, MyPurchasedValidation>();
//builder.Services.AddTransient<IPaymentValidation, PaymentValidation>();
builder.Services.AddTransient<IStudentRegistrationValidation, StudentRegistrationValidation>();
builder.Services.AddTransient<IAdminResultValidation, AdminResultValidation>();
builder.Services.AddTransient<IAdminWalletValidation, AdminWalletValidation>();
builder.Services.AddTransient<IAdminDashboardValidation, AdminDashboardValidation>();
builder.Services.AddTransient<IModuleValidation, ModuleValidation>();


#endregion
ConfigurationManager configuration = builder.Configuration;
// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<DBContext>()
    .AddDefaultTokenProviders();


// Adding Authentication Old
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
    };
});

// Old end

builder.Services.AddAuthorization();
//Email Setting
builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
//

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResponseResult<DBNull>), StatusCodes.Status200OK));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status400BadRequest));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
      builder =>
      {
          builder
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
      });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
 {
     {
           new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             new string[] {}
     }
 });
    option.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
}
    );
builder.Services.AddStripeInfrastructure(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
if (!Directory.Exists(path))
{
    Directory.CreateDirectory(path);
}
var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images"));
var requestPath = "/Images";

// Enable displaying browser links.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "ccavenue_callback",
//        pattern: "receive/{status}/servlet/BankRespReceive",
//        defaults: new { controller = "CCAvenue", action = "Callback" });
//});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
          name: "default",
          pattern: "receive/100/servlet/BankRespReceive",
          defaults: new { controller = "CCAvenue", action = "Callback" });
});
app.Run();