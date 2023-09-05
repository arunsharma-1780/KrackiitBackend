using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Org.BouncyCastle.Ocsp;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using Req = OnlinePractice.API.Models.Request;
using System.Diagnostics;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentPYPRepository : IStudentPYPRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMyPurchasedRespository _myPurchasedRespository;
        public StudentPYPRepository(IUnitOfWork unitOfWork, IMyPurchasedRespository myPurchasedRespository)
        {
            _unitOfWork = unitOfWork;
            _myPurchasedRespository = myPurchasedRespository;
        }

        //Get PreviousYearPaper Data
        public async Task<Res.StudentPYPList> GetPreviousYearPaper(Req.PYPInstitutes pypInstitutes)
        {
            Res.StudentPYPList studentPYPList = new()
            {
                previousYearPapers = new()
            };
            var pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == pypInstitutes.InstituteId
                                                        && x.SubCourseId == pypInstitutes.SubcourseId && !x.IsDeleted);
            studentPYPList.previousYearPapers = pypList
                                .Select(o => new Res.StudentPreviousYearPaperData
                                {
                                    PaperId = o.Id,
                                    PaperTitle = o.PaperTitle,
                                    Price = o.Price,
                                    Language = o.Language,
                                    Year = o.Year
                                }).ToList();
            return studentPYPList;
        }


        //Get PreviousYearPaper Data by Filter
        public async Task<Res.StudentPYPList?> GetPapersDataByFilter(Req.StudentPreviousYearPaperFilter previousYearPaperFilter)
        {
            List<DM.PreviousYearPaper> pypList = new();
            var priceFilter = previousYearPaperFilter.PricingFilter;
            var priceWiseSort = previousYearPaperFilter.PriceWiseSort;
            Res.StudentPYPList studentPYPList = new()
            {
                previousYearPapers = new()
            };
            if (previousYearPaperFilter.PriceWiseSort == PriceWiseSort.All)
            {
                if(previousYearPaperFilter.Year > 0)
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && x.Year == previousYearPaperFilter.Year && !x.IsDeleted);
                }
                else
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && !x.IsDeleted);
                }
                 
                var pyList = pypList
                                    .Select(o => new Res.StudentPreviousYearPaperData
                                    {
                                        PaperId=o.Id,
                                        PaperPdfUrl=o.PaperPdfUrl,
                                        PaperTitle = o.PaperTitle,
                                        Price = o.Price,
                                        Language = o.Language,
                                        Year = o.Year,
                                        CreationDate = o.CreationDate

                                    }).OrderByDescending(x=>x.CreationDate).ToList();
                pyList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.PaperId, ProductCategory.PreviouseYearPaper, previousYearPaperFilter.UserId));

                switch (priceFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        pyList = pyList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        pyList = pyList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (previousYearPaperFilter.LanguageFilter.ToString() != "All")
                {
                    pyList = pyList.Where(x => x.Language == previousYearPaperFilter.LanguageFilter.ToString()).ToList();
                }
                var result = pyList;
                var resultV1 = result.Page(previousYearPaperFilter.PageNumber, previousYearPaperFilter.PageSize);
                studentPYPList.previousYearPapers = resultV1.ToList();
                studentPYPList.TotalRecord = pyList.Count;
                return studentPYPList;
             
            }
            else if (previousYearPaperFilter.PriceWiseSort == PriceWiseSort.LowToHigh)
            {
                if (previousYearPaperFilter.Year > 0)
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && x.Year == previousYearPaperFilter.Year && !x.IsDeleted);
                }
                else
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && !x.IsDeleted);
                }
                var pyList = pypList
                                    .Select(o => new Res.StudentPreviousYearPaperData
                                    {
                                        PaperId = o.Id,
                                        PaperPdfUrl = o.PaperPdfUrl,
                                        PaperTitle = o.PaperTitle,
                                        Price = o.Price,
                                        Language = o.Language,
                                        Year = o.Year,
                                        CreationDate = o.CreationDate
                                        //  IsPurchased = _myPurchasedRespository.PurchasedCheck(o.Id, ProductCategory.PreviouseYearPaper)
                                    }).Distinct().OrderBy(p=>p.Price).ToList();
                pyList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.PaperId, ProductCategory.PreviouseYearPaper, previousYearPaperFilter.UserId));

                switch (priceFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        pyList = pyList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        pyList = pyList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (previousYearPaperFilter.LanguageFilter.ToString() != "All")
                {
                    pyList = pyList.Where(x => x.Language == previousYearPaperFilter.LanguageFilter.ToString()).ToList();
                }

                var result = pyList;
                var resultV1 = result.Page(previousYearPaperFilter.PageNumber, previousYearPaperFilter.PageSize);
                studentPYPList.previousYearPapers = resultV1.ToList();
                studentPYPList.TotalRecord = pyList.Count;
                return studentPYPList;
            }
            else if (previousYearPaperFilter.PriceWiseSort == PriceWiseSort.HighToLow)
            {
                if (previousYearPaperFilter.Year > 0)
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && x.Year == previousYearPaperFilter.Year && !x.IsDeleted);
                }
                else
                {
                    pypList = await _unitOfWork.Repository<DM.PreviousYearPaper>().Get(x => x.InstituteId == previousYearPaperFilter.InstituteId
                                                        && x.SubCourseId == previousYearPaperFilter.SubCourseId && !x.IsDeleted);
                }
                var pyList = pypList
                                    .Select(o => new Res.StudentPreviousYearPaperData
                                    {
                                        PaperId = o.Id,
                                        PaperPdfUrl = o.PaperPdfUrl,
                                        PaperTitle = o.PaperTitle,
                                        Price = o.Price,
                                        Language = o.Language,
                                        Year = o.Year,
                                        CreationDate = o.CreationDate
                                    }).Distinct().OrderByDescending(p=>p.Price).ToList();
                pyList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.PaperId, ProductCategory.PreviouseYearPaper, previousYearPaperFilter.UserId));

                switch (priceFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        pyList = pyList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        pyList = pyList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (previousYearPaperFilter.LanguageFilter.ToString() != "All")
                {
                    pyList = pyList.Where(x => x.Language == previousYearPaperFilter.LanguageFilter.ToString()).ToList();
                }
                var result = pyList;
                var resultV1 = result.Page(previousYearPaperFilter.PageNumber, previousYearPaperFilter.PageSize);
                studentPYPList.previousYearPapers = resultV1.ToList();
                studentPYPList.TotalRecord = pyList.Count;
                return studentPYPList;
            }
            return null;

        }

        public List<Com.EnumModel> GetPriceWiseSort()
        {
            List<Com.EnumModel> enums = ((PriceWiseSort[])Enum.GetValues(typeof(PriceWiseSort))).Select(c => new Com.EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
    }
}
