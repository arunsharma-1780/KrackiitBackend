using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace OnlinePractice.API.Repository.Services
{
    public class AdminWalletRepository : IAdminWalletRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMyCartRepository _myCartRepository;
        private readonly DBContext _dbContext;
        public AdminWalletRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,IMyCartRepository myCartRepository, DBContext dbContext)
        { _unitOfWork = unitOfWork; _userManager = userManager; _myCartRepository = myCartRepository; _dbContext = dbContext; }

        public async Task<Res.AdminWalletList?> GetAllTransctions(Req.GetAdminWallet adminWallet)
        {
            Res.AdminWalletList walletList = new();
            List<Res.AdminWallet> walletListing = new();
            var studentList =  _userManager.GetUsersInRoleAsync(UserRoles.Student).Result.Where(x => !x.IsDeleted).Select(x=>x.Id).ToList();
            var aspNetUsers = await _dbContext.Users.Where(x=> studentList.Contains(x.Id) && x.InstituteId == adminWallet.InstituteId && !x.IsDeleted).ToListAsync();
            var userIds = aspNetUsers.Select(x=>x.Id).ToList();
           var Guids = userIds.Select(Guid.Parse).ToList();


            var payments = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => Guids.Contains(x.StudentId) && !x.IsDeleted && x.DebitAmount > 0 && !string.IsNullOrEmpty(x.TransactionId), orderBy:x=>x.OrderByDescending(x=>x.CreationDate));
            if (payments.Any())
            {
                foreach (var payment in payments.DistinctBy(x => x.TransactionId))
                {
                    var totalItems = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && x.TransactionId == payment.TransactionId && !string.IsNullOrEmpty(x.TransactionId));
                    if (totalItems.Any())
                    {
                        Res.AdminWallet wallet = new();
                        wallet.OrderId = Guid.Parse(payment.TransactionId);
                        wallet.OrderDate = payment.CreationDate;
                        wallet.Amount = totalItems.Sum(x => x.DebitAmount);
                        wallet.TotalItem = totalItems.Count;
                        var userInfo = await _userManager.FindByIdAsync(payment.StudentId.ToString());
                        wallet.StudentName = userInfo != null ? userInfo.DisplayName : string.Empty;
                        var institueId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institueId && !x.IsDeleted);
                        wallet.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                        walletListing.Add(wallet);
                    }
                }
                walletList.TotalRecords = walletListing.Count;
                walletList.TotalAmount = walletListing.Sum(x => x.Amount);
                var result = walletListing.Page(adminWallet.PageNumber, adminWallet.PageSize).ToList();
                walletList.AdminWallets = result;
                return walletList;
            }
            return null;
        }
        public async Task<Res.TransactionDetails?> GetTransactionDetails(Req.GetTransactionDetails getTransaction)
        {
            Res.TransactionDetails transactionDetails = new();

            var transactionList = await _unitOfWork.Repository<DM.WalletHistory>().Get(x => !x.IsDeleted && x.DebitAmount > 0 && x.TransactionId == getTransaction.OrderId.ToString() && !string.IsNullOrEmpty(x.TransactionId));
            if (transactionList.Any())
            {
                transactionDetails.OrderId = getTransaction.OrderId;
                transactionDetails.OrderDate = transactionList.First().CreationDate;
                transactionDetails.TotalAmount = transactionList.Sum(x => x.DebitAmount);
                transactionDetails.TotalItem = transactionList.Count;
                var studentId = transactionList.First().StudentId;
                var userInfo = await _userManager.FindByIdAsync(studentId.ToString());
                transactionDetails.StudentName = userInfo != null ? userInfo.DisplayName : string.Empty;
                var institueId = userInfo != null ? userInfo.InstituteId : Guid.Empty;
                var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == institueId && !x.IsDeleted);
                transactionDetails.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : string.Empty;
                List<Res.OrderSummary> orderSummaries = new();
                foreach (var item in transactionList)
                {
                    Res.OrderSummary orderSummary = new();
                    var productName = await _myCartRepository.GetProductCategoryName(item.ProductCategory, item.ProductId);
                    orderSummary.ItemName = productName.ProductName;
                    orderSummary.Amount = item.DebitAmount;
                    orderSummaries.Add(orderSummary);
                }
                transactionDetails.OrderSummaries = orderSummaries;
                return transactionDetails;
            }
            return null;
        }
    }
}
