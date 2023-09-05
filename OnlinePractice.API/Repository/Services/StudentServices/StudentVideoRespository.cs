using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class StudentVideoRespository : IStudentVideoRespository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMyPurchasedRespository _myPurchasedRespository;
        public StudentVideoRespository(IUnitOfWork unitOfWork, IMyPurchasedRespository myPurchasedRespository)
        {
            _unitOfWork = unitOfWork;
            _myPurchasedRespository = myPurchasedRespository;
        }

        //Get Subjects List
        public async Task<Res.VideosSubjectsList?> GetVideoSubjects(Req.StudentSubjects studentSubjects)
        {
            Res.VideosSubjectsList studentVideosSubjects = new();
            var videosList = await (from v in _unitOfWork.GetContext().Videos.OrderByDescending(x => x.CreationDate)
                                    join s in _unitOfWork.GetContext().SubjectCategories on v.SubjectCategoryId equals s.Id
                                    join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                    join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                    join institute in _unitOfWork.GetContext().Institutes on v.InstituteId equals institute.Id
                                    where !v.IsDeleted && institute.Id == studentSubjects.InstituteId && subCourse.Id == studentSubjects.SubcourseId
                                    select new Res.VideosSubjects
                                    {
                                        SubjectId = subject.Id,
                                        SubjectName = subject.SubjectName
                                    }).Distinct().ToListAsync();

            if (videosList.Count > 0)
            {
                var result = videosList;

                studentVideosSubjects.videoSubjects = result.ToList();
                return studentVideosSubjects;
            }
            else
            {
                return null;
            }
        }
        //Get Videos Data by Filter
        public async Task<Res.StudentVideoList?> GetStudentsVideos(Req.GetStudentVideo studentVideo)
        {
            Res.StudentVideoList studentVideoList = new();
            var priceWiseSort = studentVideo.PriceWiseSort;
            var pricingFilter = studentVideo.PricingFilter;
            if (studentVideo.PriceWiseSort == PriceWiseSort.All)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentVideo.SubcourseId && x.SubjectId == studentVideo.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var videosList = await (from e in _unitOfWork.GetContext().Videos.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentVideo.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentVideoData
                                        {
                                            VideoId = e.Id,
                                            SubjectId = subject.Id,
                                            SubjectName = subject.SubjectName,
                                            VideoThumbnail = e.VideoThumbnail,
                                            VideoURL = e.VideoUrl,
                                            VideoTitle = e.VideoTitle,
                                            TopicName= e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            FacultyName = e.FacultyName,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate= e.CreationDate
                                        }).Distinct().OrderByDescending(x=>x.CreationDate).ToListAsync();
                videosList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.VideoId, ProductCategory.Video,studentVideo.UserId));

                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        videosList = videosList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        videosList = videosList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentVideo.LanguageFilter.ToString() != "All")
                {
                    videosList = videosList.Where(x => x.Language == studentVideo.LanguageFilter.ToString()).ToList();
                }
                if (videosList.Count > 0)
                {
                    var result = videosList;
                    var resultV1 = result.Page(studentVideo.PageNumber, studentVideo.PageSize);
                    studentVideoList.studentVideoDatas = resultV1.ToList();
                    studentVideoList.TotalRecords = videosList.Count;
                    return studentVideoList;
                }


            }
           else if (studentVideo.PriceWiseSort == PriceWiseSort.LowToHigh)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentVideo.SubcourseId && x.SubjectId == studentVideo.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var videosList = await (from e in _unitOfWork.GetContext().Videos.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentVideo.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentVideoData
                                        {
                                            VideoId = e.Id,
                                            SubjectId = subject.Id,
                                            SubjectName = subject.SubjectName,
                                            VideoThumbnail = e.VideoThumbnail,
                                            VideoURL = e.VideoUrl,
                                            VideoTitle = e.VideoTitle,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            FacultyName = e.FacultyName,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate = e.CreationDate
                                            // IsPurchased = _myPurchasedRespository.PurchasedCheck(e.Id, ProductCategory.Video)

                                        }).Distinct().OrderBy(p=>p.Price).ToListAsync();
                videosList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.VideoId, ProductCategory.Video, studentVideo.UserId));

                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        videosList = videosList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        videosList = videosList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentVideo.LanguageFilter.ToString() != "All")
                {
                    videosList = videosList.Where(x => x.Language == studentVideo.LanguageFilter.ToString()).ToList();
                }
                if (videosList.Count > 0)
                {
                    var result = videosList;
                    var resultV1 = result.Page(studentVideo.PageNumber, studentVideo.PageSize);
                    studentVideoList.studentVideoDatas = resultV1.ToList();
                    studentVideoList.TotalRecords = videosList.Count;
                    return studentVideoList;
                }


            }
            else if (studentVideo.PriceWiseSort == PriceWiseSort.HighToLow)
            {
                var subjectCheckCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.SubCourseId == studentVideo.SubcourseId && x.SubjectId == studentVideo.SubjectId && !x.IsDeleted);
                var sujectCatgoryId = subjectCheckCategory != null ? subjectCheckCategory.Id : Guid.Empty;
                var videosList = await (from e in _unitOfWork.GetContext().Videos.OrderByDescending(x => x.CreationDate)
                                        join sc in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals sc.Id
                                        join subject in _unitOfWork.GetContext().Subjects on sc.SubjectId equals subject.Id
                                        join topic in _unitOfWork.GetContext().Topics on sc.Id equals topic.SubjectCategoryId
                                        where (topic.Id == e.TopicId || e.TopicId == Guid.Empty)
                                        join subCourse in _unitOfWork.GetContext().SubCourses on sc.SubCourseId equals subCourse.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && institute.Id == studentVideo.InstituteId && sc.Id == sujectCatgoryId
                                        select new Res.StudentVideoData
                                        {
                                            VideoId = e.Id,
                                            SubjectId = subject.Id,
                                            SubjectName = subject.SubjectName,
                                            VideoThumbnail = e.VideoThumbnail,
                                            VideoURL = e.VideoUrl,
                                            VideoTitle = e.VideoTitle,
                                            TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                            FacultyName = e.FacultyName,
                                            Language = e.Language,
                                            Price = e.Price,
                                            CreationDate = e.CreationDate
                                            // IsPurchased =  _myPurchasedRespository.PurchasedCheck(e.Id,ProductCategory.Video)
                                        }).Distinct().OrderByDescending(x=>x.Price).ToListAsync();
                videosList.ForEach(x => x.IsPurchased = _myPurchasedRespository.PurchasedCheck(x.VideoId, ProductCategory.Video, studentVideo.UserId));

                switch (pricingFilter)
                {
                    case PricingFilter.All:
                        break;
                    case PricingFilter.Free:
                        videosList = videosList.Where(x => x.Price == 0).ToList();
                        break;
                    case PricingFilter.Premium:
                        videosList = videosList.Where(x => x.Price > 0).ToList();
                        break;
                }
                if (studentVideo.LanguageFilter.ToString() != "All")
                {
                    videosList = videosList.Where(x => x.Language == studentVideo.LanguageFilter.ToString()).ToList();
                }
                if (videosList.Count > 0)
                {
                    var result = videosList;
                    var resultV1 = result.Page(studentVideo.PageNumber, studentVideo.PageSize);
                    studentVideoList.studentVideoDatas = resultV1.ToList();
                    studentVideoList.TotalRecords = videosList.Count;
                    return studentVideoList;
                }


            }
            return null;
        }
        //Get Specifiv Video Data by Id
        public async Task<Res.GetVideo?> GetVideos(Req.GetVideoById getVideo)
        {
            var videosCheck = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == getVideo.Id && !x.IsDeleted);
            if (videosCheck != null)
            {
                var topic = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == videosCheck.TopicId);
                var topicName = topic != null ? topic.TopicName : "";
                
                Res.GetVideo video = new();
                video.VideoId = videosCheck.Id;
                video.VideoURL = videosCheck.VideoUrl;
                video.VideoThumbnail = videosCheck.VideoThumbnail;
                video.VideoTitle = videosCheck.VideoTitle;
                video.Price = videosCheck.Price;
                video.FacultyName = videosCheck.FacultyName;
                video.TopicName = topicName;
                video.Language = videosCheck.Language;
                video.Description = videosCheck.Description;
                video.IsPurchased = _myPurchasedRespository.PurchasedCheck(videosCheck.Id, ProductCategory.Video, getVideo.UserId);
                return video;
            }
            return null;
        }


    }
}
