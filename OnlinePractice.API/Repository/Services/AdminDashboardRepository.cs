using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.AuthDB;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Common;
using Stripe;
using OnlinePractice.API.Models.Request;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using OnlinePractice.API.Models.Response;
using System;

namespace OnlinePractice.API.Repository.Services
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBContext _dbContext;
        public AdminDashboardRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, DBContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<Res.TotalSaleDetails> GetTotalSale(Req.FilterModel model)
        {
            Res.TotalSaleDetails totalSaleDetails = new();
            DateTime fromDate;
            DateTime currentDate = DateTime.UtcNow;
            var transactionInfo = new List<DM.WalletHistory>();
            var transactionDetails = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId));
            totalSaleDetails.TotalSale = transactionDetails.Sum(x => x.DebitAmount);

            switch (model.DurationFilter)
            {
                case DurationFilter.Today:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    transactionInfo = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= fromDate && x.CreationDate <= currentDate);
                    totalSaleDetails.Sale = transactionInfo.Sum(x => x.DebitAmount);
                    break;
                case DurationFilter.Week:
                    int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                    fromDate = DateTime.UtcNow.AddDays(-1 * diff).Date;
                    transactionInfo = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= fromDate && x.CreationDate <= currentDate);
                    totalSaleDetails.Sale = transactionInfo.Sum(x => x.DebitAmount);
                    break;
                case DurationFilter.Month:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    transactionInfo = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= fromDate && x.CreationDate <= currentDate);
                    totalSaleDetails.Sale = transactionInfo.Sum(x => x.DebitAmount);
                    break;
                default:
                    fromDate = DateTime.MinValue;
                    transactionInfo = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= fromDate && x.CreationDate <= currentDate);
                    totalSaleDetails.Sale = transactionInfo.Sum(x => x.DebitAmount);
                    break;
            }
            return totalSaleDetails;
        }
        public async Task<Res.TotalEnrollmentDetails> GetTotalEnrollment(Req.FilterModel model)
        {
            Res.TotalEnrollmentDetails totalSaleDetails = new();
            List<ApplicationUser> applicationUsers = new();
            DateTime fromDate;
            // var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            var aspNetUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted).ToList();
            DateTime currentDate = DateTime.UtcNow;
            totalSaleDetails.TotalEnrollment = aspNetUsers.Count;

            switch (model.DurationFilter)
            {
                case DurationFilter.Today:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                    totalSaleDetails.Enrollment = applicationUsers.Count;
                    break;
                case DurationFilter.Week:
                    int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                    fromDate = DateTime.UtcNow.AddDays(-1 * diff).Date;
                    applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                    totalSaleDetails.Enrollment = applicationUsers.Count;
                    break;
                case DurationFilter.Month:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                    totalSaleDetails.Enrollment = applicationUsers.Count;
                    break;
                default:
                    fromDate = DateTime.MinValue;
                    applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                    totalSaleDetails.Enrollment = applicationUsers.Count;
                    break;
            }
            return totalSaleDetails;
        }
        public async Task<Res.InstituteStudent> GetInstituteStudentCourse(Req.FilterModel model)
        {
            Res.InstituteStudent instituteStudent = new();
            List<Res.InstituteStudentCourse> instituteStudentCourses = new();
            List<ApplicationUser> applicationUsers = new();
            DateTime fromDate;
            // var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var activeInstitute = await _unitOfWork.Repository<DM.Institute>().Get(x => !x.IsDeleted);
            var activeInstituteList = activeInstitute.Select(x => x.Id).ToList();
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            var aspNetUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && activeInstituteList.Contains(x.InstituteId)).ToList();
            DateTime currentDate = DateTime.UtcNow;
           // instituteStudent.TotalActiveStudent = aspNetUsers.Count;
            foreach (var student in aspNetUsers.DistinctBy(x => x.InstituteId))
            {
                Res.InstituteStudentCourse instituteStudentCourse = new();
                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == student.InstituteId && !x.IsDeleted);
                switch (model.DurationFilter)
                {
                    case DurationFilter.Today:
                        fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                        applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && x.InstituteId == student.InstituteId && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                        if (applicationUsers.Any())
                        {
                            instituteStudentCourse.InstituteId = student.InstituteId;
                            instituteStudentCourse.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                            instituteStudentCourse.StudentCount = applicationUsers.Count;
                            instituteStudentCourses.Add(instituteStudentCourse);
                        }
                        break;
                    case DurationFilter.Week:
                        int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                        fromDate = DateTime.UtcNow.AddDays(-1 * diff).Date;
                        applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && x.InstituteId == student.InstituteId && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                        if (applicationUsers.Any())
                        {
                            instituteStudentCourse.InstituteId = student.InstituteId;
                            instituteStudentCourse.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                            instituteStudentCourse.StudentCount = applicationUsers.Count;
                            instituteStudentCourses.Add(instituteStudentCourse);
                        }
                        break;
                    case DurationFilter.Month:
                        fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && x.InstituteId == student.InstituteId && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                        if (applicationUsers.Any())
                        {
                            instituteStudentCourse.InstituteId = student.InstituteId;
                            instituteStudentCourse.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                            instituteStudentCourse.StudentCount = applicationUsers.Count;
                            instituteStudentCourses.Add(instituteStudentCourse);
                        }
                        break;
                    default:
                        fromDate = DateTime.MinValue;
                        applicationUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && x.InstituteId == student.InstituteId && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                        if (applicationUsers.Any())
                        {
                            instituteStudentCourse.InstituteId = student.InstituteId;
                            instituteStudentCourse.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                            instituteStudentCourse.StudentCount = applicationUsers.Count;
                            instituteStudentCourses.Add(instituteStudentCourse);
                        }
                        break;
                }
            }
            instituteStudent.TotalRecords = instituteStudentCourses.Sum(x => x.StudentCount);
            instituteStudent.InstituteStudentCourses = instituteStudentCourses;
            return instituteStudent;
        }
        public async Task<Res.RecentTransactionandTimeLineList?> GetRecentTransactionsandTimeLineList()
        {
            #region Recent transaction
            Res.RecentTransactionandTimeLineList recentTransactionList = new();
            List<Res.RecentTransactions> recentTransactions = new();
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            var aspNetUsers = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted).ToList();
            var userIds = aspNetUsers.Select(x => x.Id).ToList();
            var Guids = userIds.Select(Guid.Parse).ToList();

            var payments = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => Guids.Contains(x.StudentId) && !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId), orderBy: x => x.OrderByDescending(x => x.CreationDate));
            var finalIds = payments.DistinctBy(x => x.TransactionId).Take(5).ToList();
            if (payments.Any())
            {
                foreach (var payment in finalIds)
                {
                    var totalItems = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && x.TransactionId == payment.TransactionId && !string.IsNullOrEmpty(x.TransactionId));
                    if (totalItems.Any())
                    {
                        Res.RecentTransactions recent = new();
                        recent.OrderId = new Guid(payment.TransactionId);
                        recent.Amount = totalItems.Sum(x => x.DebitAmount);
                        recent.Date = payment.CreationDate;
                        var studentId = payment.StudentId;
                        var studentDetails = await _userManager.FindByIdAsync(studentId.ToString());
                        if (studentDetails != null)
                        {
                            recent.StudentName = studentDetails.DisplayName;
                        }
                        recentTransactions.Add(recent);
                    }
                }
                recentTransactionList.RecentTransactions = recentTransactions;
            }

            #endregion
            #region Time Lines
            List<Res.TimeLine> timeLines = new();
            var eBookDetails = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (eBookDetails.Any())
            {
                foreach (var item in eBookDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.EbookTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.eBook.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var pYPDetails = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (pYPDetails.Any())
            {
                foreach (var item in pYPDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.PaperTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add("Paper");
                    timeLines.Add(timeLine);
                }
            }
            var videoDetails = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (videoDetails.Any())
            {
                foreach (var item in videoDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.VideoTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.Video.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (mockTestDetails.Any())
            {
                foreach (var item in mockTestDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.MockTestName;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.MockTest.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var newTimeLine = timeLines.OrderByDescending(x => x.Date).Take(5).ToList();
            recentTransactionList.TimeLines = newTimeLine;
            #endregion
            if (!recentTransactionList.RecentTransactions.Any() && !recentTransactionList.TimeLines.Any())
            {
                return null;
            }
            return recentTransactionList;

        }
        public async Task<Res.TimeLineList?> GetTimeLine()
        {
            Res.TimeLineList timeLineList = new();
            List<Res.TimeLine> timeLines = new();
            var eBookDetails = await _unitOfWork.Repository<DM.Ebook>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (eBookDetails.Any())
            {
                foreach (var item in eBookDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.EbookTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.eBook.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var pYPDetails = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (pYPDetails.Any())
            {
                foreach (var item in pYPDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.PaperTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.PreviouseYearPaper.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var videoDetails = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (videoDetails.Any())
            {
                foreach (var item in videoDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.VideoTitle;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.Video.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().Get(x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.CreationDate), take: 5);
            if (mockTestDetails.Any())
            {
                foreach (var item in mockTestDetails)
                {
                    Res.TimeLine timeLine = new();
                    timeLine.Id = item.Id;
                    timeLine.DocumentName = item.MockTestName;
                    timeLine.Date = item.CreationDate;
                    timeLine.Type.Add(ProductCategory.MockTest.ToString());
                    timeLines.Add(timeLine);
                }
            }
            var newTimeLine = timeLines.OrderByDescending(x => x.Date).Take(5).ToList();
            if (newTimeLine.Any())
            {
                timeLineList.TimeLines = newTimeLine;
                return timeLineList;
            }
            return null;
        }
        public List<Com.EnumModel> GetDurationFilter()
        {
            List<EnumModel> enums = ((DurationFilter[])Enum.GetValues(typeof(DurationFilter))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public async Task<List<KeyValuePair<int, int>>?> GetStudentEnrollmentGraph(Req.FilterModel model)
        {
                DateTime fromDate;
                DateTime currentDate;
                var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
                var studentIds = studentList.Select(student => student.Id).ToList();
                switch (model.DurationFilter)
                {
                    case DurationFilter.Today:
                        fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                        DateTimeOffset utcTime = DateTimeOffset.UtcNow;
                        DateTimeOffset localTime = DateTimeOffset.Now;
                        TimeSpan timeDifference = localTime.Offset - utcTime.Offset;
                        DateTime result = fromDate.Subtract(timeDifference);
                        var studentToday = new List<KeyValuePair<int, int>>();
                        double value = (DateTime.Now.Hour);
                        int roundedValue = (int)Math.Floor(value) +1;
                        for (int i = 0; i < roundedValue; i++)
                        {

                            result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                            if (result == new DateTime(result.Year, result.Month, result.Day, 23, 30, 0))
                            {
                                currentDate = new DateTime(result.Year, result.Month, result.Day+1, 0, 30, 0);
                            }
                            else
                            {
                                currentDate = new DateTime(result.Year, result.Month, result.Day, result.Hour + 1, 30, 0);

                            }
                            var allTests = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= result && x.CreationDate <= currentDate).ToList();
                            studentToday.Add(new KeyValuePair<int, int>(i, allTests.Count));
                            result = currentDate;
                        }
                        return studentToday;
                    case DurationFilter.Week:
                        var studentWeek = new List<KeyValuePair<int, int>>();

                        int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                        fromDate = DateTime.UtcNow.AddDays(-1 * diff).Date;

                        fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, fromDate.Day);
                        for (int i = 0; i <= diff; i++)
                        {
                            //currentDate = DateTime.UtcNow;
                            var newDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day + i, 0, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                            currentDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59);
                            var allTests = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= newDate && x.CreationDate <= currentDate).ToList();
                            studentWeek.Add(new KeyValuePair<int, int>(i, allTests.Count));
                        }
                        return studentWeek;
                    case DurationFilter.Month:
                        fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        var studentMonth = new List<KeyValuePair<int, int>>();
                        for (int i = 1; i <= DateTime.UtcNow.Day; i++)
                        {
                            var newDate = new DateTime(fromDate.Year, fromDate.Month, i, 0, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                            currentDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59);
                            var allTests = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= newDate && x.CreationDate <= currentDate).ToList();
                            studentMonth.Add(new KeyValuePair<int, int>(i, allTests.Count));
                        }
                        return studentMonth;
                    default:
                        return null;

                }
        }
        public async Task<List<KeyValuePair<int, double>>?> GetTotalSalesGraph(Req.FilterModel model)
        {
            DateTime fromDate;
            DateTime currentDate;
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            switch (model.DurationFilter)
            {
                case DurationFilter.Today:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    DateTimeOffset utcTime = DateTimeOffset.UtcNow;
                    DateTimeOffset localTime = DateTimeOffset.Now;
                    TimeSpan timeDifference = localTime.Offset - utcTime.Offset;
                    DateTime result = fromDate.Subtract(timeDifference);
                    var studentToday = new List<KeyValuePair<int, double>>();
                    double value = (DateTime.Now.Hour);
                    int roundedValue = (int)Math.Floor(value) + 1;
                    for (int i = 0; i < roundedValue; i++)
                    {
                        result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                        if (result == new DateTime(result.Year, result.Month, result.Day, 23, 30, 0))
                        {
                            currentDate = new DateTime(result.Year, result.Month, result.Day + 1, 0, 30, 0);
                        }
                        else
                        {
                            currentDate = new DateTime(result.Year, result.Month, result.Day, result.Hour + 1, 30, 0);

                        }
                        var allTests = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= result && x.CreationDate <= currentDate);
                        studentToday.Add(new KeyValuePair<int, double>(i, allTests.Sum(x=>x.DebitAmount)));
                        result = currentDate;
                    }
                    return studentToday;
                case DurationFilter.Week:
                    var studentWeek = new List<KeyValuePair<int, double>>();

                    int diff = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
                    fromDate = DateTime.UtcNow.AddDays(-1 * diff).Date;

                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, fromDate.Day);
                    for (int i = 0; i <= diff; i++)
                    {
                        //currentDate = DateTime.UtcNow;
                        var newDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day + i, 0, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                        currentDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59);
                        var allTests = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= newDate && x.CreationDate <= currentDate);
                        studentWeek.Add(new KeyValuePair<int, double>(i, allTests.Sum(x=>x.DebitAmount)));
                    }
                    return studentWeek;
                case DurationFilter.Month:
                    var studentMonth = new List<KeyValuePair<int, double>>();
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    var s = new List<KeyValuePair<int, double>>();
                    for (int i = 1; i <= DateTime.UtcNow.Day; i++)
                    {
                        var newDate = new DateTime(fromDate.Year, fromDate.Month, i, 0, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                        currentDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59);
                        var allTests = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= newDate && x.CreationDate <= currentDate);
                        studentMonth.Add(new KeyValuePair<int, double>(i, allTests.Sum(x=>x.DebitAmount)));
                    }
                    return studentMonth;
                default:
                    return null;

            }
        }
        public async Task<List<KeyValuePair<DateTime, int>>?> GetStudentEnrollmentGraphV1(Req.FilterModel model)
        {
            DateTime fromDate;
            DateTime currentDate;
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            switch (model.DurationFilter)
            {
                case DurationFilter.Today:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    var studentToday = new List<KeyValuePair<DateTime, int>>();
                    double value = (DateTime.Now.Hour);
                    int roundedValue = (int)Math.Floor(value);
                    for (int i = 0; i < roundedValue; i++)
                    {
                        fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0 + i, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                        currentDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, 59, 59);
                        var allTests = _dbContext.Users.Where(x => studentIds.Contains(x.Id) && !x.IsDeleted && x.CreationDate >= fromDate && x.CreationDate <= currentDate).ToList();
                        studentToday.Add(new KeyValuePair<DateTime, int>(fromDate, allTests.Count));
                    }
                    return studentToday;
                default:
                    return null;

            }
        }
        public async Task<List<KeyValuePair<DateTime, double>>?> GetTotalSalesGraphV2(Req.FilterModel model)
        {
            DateTime fromDate;
            DateTime currentDate;
            var studentList = await _userManager.GetUsersInRoleAsync(UserRoles.Student);
            var studentIds = studentList.Select(student => student.Id).ToList();
            switch (model.DurationFilter)
            {
                case DurationFilter.Today:
                    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    var studentToday = new List<KeyValuePair<DateTime, double>>();
                    double value = (DateTime.Now.Hour);
                    int roundedValue = (int)Math.Floor(value);
                    for (int i = 0; i < roundedValue; i++)
                    {
                        fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0 + i, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                        currentDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, 59, 59);
                        var allTests = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId) && x.CreationDate >= fromDate && x.CreationDate <= currentDate);
                        studentToday.Add(new KeyValuePair<DateTime, double>(fromDate, allTests.Sum(x => x.DebitAmount)));
                    }
                    return studentToday;
                default:
                    return null;

            }
        }
    }
}
