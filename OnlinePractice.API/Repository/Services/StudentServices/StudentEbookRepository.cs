using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Base;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;


namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentEbookRepository : IStudentEbookRespository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMyPurchasedRespository _myPurchasedRespository;
        private readonly DBContext _dbContext;
        public StudentEbookRepository(IUnitOfWork unitOfWork, IMyPurchasedRespository myPurchasedRespository, DBContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _myPurchasedRespository = myPurchasedRespository;
            _dbContext = dbContext;
        }

        //Get Ebooks list method
        public async Task<Res.StudentEbooksList?> GetStudentsEbook(Req.GetStudentEbook studentEbook)
        {
            Res.StudentEbooksList studentEbooksList = new();
            var priceWiseSort = studentEbook.PriceWiseSort;
            var pricingFilter = studentEbook.PriceFilter;
            if (studentEbook.PriceWiseSort == PriceWiseSort.All)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentEbook
                                        {
                                            EbookId = e.Id,
                                            SubjectId = subject.Id,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            SubjectName = subject.SubjectName,
                                            EbookThumbnail = e.EbookThumbnail,
                                            EbookPdfURL = e.EbookPdfUrl,
                                            EbookTitle = e.EbookTitle,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate= e.CreationDate
                                        }).Distinct().OrderByDescending(x=>x.CreationDate).ToListAsync();
                ebooksList.ForEach(x => x.IsPurchased =  _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentEbook.LanguageFilter.ToString() != "All")
                {
                    ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                }
                if (ebooksList.Count > 0)
                {
                    var result = ebooksList;
                    var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                    studentEbooksList.StudentEbookList = resultV1.ToList();
                    studentEbooksList.TotalRecord = ebooksList.Count;
                    return studentEbooksList;
                }
                else
                {
                    return null;
                }

            }
            else if (studentEbook.PriceWiseSort == PriceWiseSort.LowToHigh)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentEbook
                                        {
                                            EbookId = e.Id,
                                            SubjectId = subject.Id,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            SubjectName = subject.SubjectName,
                                            EbookThumbnail = e.EbookThumbnail,
                                            EbookPdfURL = e.EbookPdfUrl,
                                            EbookTitle = e.EbookTitle,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate= e.CreationDate,
                                        }).Distinct().OrderBy(p => p.Price).ToListAsync();
                ebooksList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentEbook.LanguageFilter.ToString() != "All")
                {
                    ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                }
                if (ebooksList.Count > 0)
                {
                    var result = ebooksList;
                    var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                    studentEbooksList.StudentEbookList = resultV1.ToList();
                    studentEbooksList.TotalRecord = ebooksList.Count;
                    return studentEbooksList;
                }
                else
                {
                    return null;
                }
            }
            else if (studentEbook.PriceWiseSort == PriceWiseSort.HighToLow)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentEbook
                                        {
                                            EbookId = e.Id,
                                            SubjectId = subject.Id,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            SubjectName = subject.SubjectName,
                                            EbookThumbnail = e.EbookThumbnail,
                                            EbookPdfURL = e.EbookPdfUrl,
                                            EbookTitle = e.EbookTitle,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate = e.CreationDate
                                        }).Distinct().OrderByDescending(p => p.Price).ToListAsync();
                ebooksList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentEbook.LanguageFilter.ToString() != "All")
                {
                    ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                }
                if (ebooksList.Count > 0)
                {
                    var result = ebooksList;
                    var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                    studentEbooksList.StudentEbookList = resultV1.ToList();
                    studentEbooksList.TotalRecord = ebooksList.Count;
                    return studentEbooksList;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public async Task<Res.StudentEbooksList?> GetStudentsEbook2(Req.GetStudentEbook studentEbook)
        {
            Res.StudentEbooksList studentEbooksList = new();
            var priceWiseSort = studentEbook.PriceWiseSort;
            var pricingFilter = studentEbook.PriceFilter;
            switch (studentEbook.PriceWiseSort)
            {
                case PriceWiseSort.All:
                    var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                    var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                    //var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                    //                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                    //                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                    //                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                    //                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                    //                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                    //                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                    //                        where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                    //                        select new Res.StudentEbook
                    //                        {
                    //                            EbookId = e.Id,
                    //                            SubjectId = subject.Id,
                    //                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                    //                            SubjectName = subject.SubjectName,
                    //                            EbookThumbnail = e.EbookThumbnail,
                    //                            EbookPdfURL = e.EbookPdfUrl,
                    //                            EbookTitle = e.EbookTitle,
                    //                            Language = e.Language,
                    //                            Price = e.Price,
                    //                            CreationDate = e.CreationDate
                    //                        }).Distinct().OrderByDescending(x => x.CreationDate).ToListAsync();
                    var ebooks= _dbContext.Set<Res.StudentEbook>().FromSqlRaw("EXEC dbo.GetEbooksByCriteria @SubCourseId,@SubjectId,@InstituteId",studentEbook.SubcourseId,studentEbook.SubjectId,studentEbook.InstituteId);

                    List<Res.StudentEbook> ebooksList = ebooks.ToList();
                    ebooksList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                    switch (pricingFilter)
                    {
                        case PricingFilter.All:
                            break;
                        case PricingFilter.Free:
                            ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                            break;
                        case PricingFilter.Premium:
                            ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                            break;
                    }
                    if (studentEbook.LanguageFilter.ToString() != "All")
                    {
                        ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                    }
                    if (ebooksList.Count > 0)
                    {
                        var result = ebooksList;
                        var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                        studentEbooksList.StudentEbookList = resultV1.ToList();
                        studentEbooksList.TotalRecord = ebooksList.Count;
                        return studentEbooksList;
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case PriceWiseSort.HighToLow:
                     subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                     sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                     ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                            join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                            join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                            where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                            join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                            join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                            join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                            where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                                            select new Res.StudentEbook
                                            {
                                                EbookId = e.Id,
                                                SubjectId = subject.Id,
                                                TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                                SubjectName = subject.SubjectName,
                                                EbookThumbnail = e.EbookThumbnail,
                                                EbookPdfURL = e.EbookPdfUrl,
                                                EbookTitle = e.EbookTitle,
                                                Language = e.Language,
                                                Price = e.Price,
                                                CreationDate = e.CreationDate
                                            }).Distinct().OrderByDescending(p => p.Price).ToListAsync();
                    ebooksList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                    switch (pricingFilter)
                    {
                        case PricingFilter.All:
                            break;
                        case PricingFilter.Free:
                            ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                            break;
                        case PricingFilter.Premium:
                            ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                            break;
                    }
                    if (studentEbook.LanguageFilter.ToString() != "All")
                    {
                        ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                    }
                    if (ebooksList.Count > 0)
                    {
                        var result = ebooksList;
                        var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                        studentEbooksList.StudentEbookList = resultV1.ToList();
                        studentEbooksList.TotalRecord = ebooksList.Count;
                        return studentEbooksList;
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case PriceWiseSort.LowToHigh:
                     subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentEbook.SubcourseId && x.SubjectId == studentEbook.SubjectId && !x.IsDeleted);
                     sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                     ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                            join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                            join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                            where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                            join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                            join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                            join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                            where !e.IsDeleted && institute.Id == studentEbook.InstituteId && sc.Id == sujectCatgoryId
                                            select new Res.StudentEbook
                                            {
                                                EbookId = e.Id,
                                                SubjectId = subject.Id,
                                                TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                                SubjectName = subject.SubjectName,
                                                EbookThumbnail = e.EbookThumbnail,
                                                EbookPdfURL = e.EbookPdfUrl,
                                                EbookTitle = e.EbookTitle,
                                                Language = e.Language,
                                                Price = e.Price,
                                                CreationDate = e.CreationDate,
                                            }).Distinct().OrderBy(p => p.Price).ToListAsync();
                    ebooksList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.EbookId, ProductCategory.eBook, studentEbook.UserId));
                    switch (pricingFilter)
                    {
                        case PricingFilter.All:
                            break;
                        case PricingFilter.Free:
                            ebooksList = ebooksList.Where(x => x.Price == 0).ToList();
                            break;
                        case PricingFilter.Premium:
                            ebooksList = ebooksList.Where(x => x.Price > 0).ToList();
                            break;
                    }
                    if (studentEbook.LanguageFilter.ToString() != "All")
                    {
                        ebooksList = ebooksList.Where(x => x.Language == studentEbook.LanguageFilter.ToString()).ToList();
                    }
                    if (ebooksList.Count > 0)
                    {
                        var result = ebooksList;
                        var resultV1 = result.Page(studentEbook.PageNumber, studentEbook.PageSize);
                        studentEbooksList.StudentEbookList = resultV1.ToList();
                        studentEbooksList.TotalRecord = ebooksList.Count;
                        return studentEbooksList;
                    }
                    else
                    {
                        return null;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        //Get Subject list method
        public async Task<Res.EbookSubjectsList?> GetEbookSubjects(Req.EbooksSubjects ebooksSubjects)
        {
            Res.EbookSubjectsList subjectsList = new();
            var ebooksList = await (from e in _unitOfWork.GetContext().Ebooks.OrderByDescending(x => x.CreationDate)
                                    join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                    join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                    join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                    join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                    where !e.IsDeleted && institute.Id == ebooksSubjects.InstituteId && subCourse.Id == ebooksSubjects.SubcourseId
                                    select new Res.EbookSubjects
                                    {
                                        SubjectId = subject.Id,
                                        SubjectName = subject.SubjectName
                                    }).Distinct().ToListAsync();

            if (ebooksList.Count > 0)
            {
                var result = ebooksList;
                subjectsList.ebookSubjects = result.ToList();
                return subjectsList;
            }
            else
            {
                return null;
            }
        }
        //Get Ebook Data by Id Method
        public async Task<Res.GetEbook?> GetEbook(Req.GetEbookById getEbook)
        {
            var eBookCheck = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == getEbook.Id && !x.IsDeleted);
            if (eBookCheck != null)
            {
                var topic = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == eBookCheck.TopicId);
                var topicName = topic != null ? topic.TopicName : "";
                Res.GetEbook eBook = new();
                eBook.EbookId = eBookCheck.Id;
                eBook.EbookThumbnail = eBookCheck.EbookThumbnail;
                eBook.EbookPdfURL = eBookCheck.EbookPdfUrl;
                eBook.TopicName = topicName;
                eBook.EbookTitle = eBookCheck.EbookTitle;
                eBook.Price = eBookCheck.Price;
                eBook.AuthorName = eBookCheck.AuthorName;
                eBook.Language = eBookCheck.Language;
                eBook.Description = eBookCheck.Description;
                eBook.IsPurchased = _myPurchasedRespository.PurchasedCheck(eBookCheck.Id, ProductCategory.eBook, getEbook.UserId);
                return eBook;
            }
            return null;
        }
    }

}

