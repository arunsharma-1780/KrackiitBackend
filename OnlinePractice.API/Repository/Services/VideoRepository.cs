using OnlinePractice.API.Repository.Interfaces;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Base;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.DBModel;
using System;
using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Repository.Services
{
    public class VideoRepository : IVideoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _baseUrl;
        private readonly IFileRepository _fileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public VideoRepository(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor baseUrl,
            IFileRepository fileRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _unitOfWork = unitOfWork;
            _baseUrl = baseUrl;
            _fileRepository = fileRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> UploadImage(Req.VideoThumbnailimage video)
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveImage(video.Image, "MediaContent/VideoThumbnail");
            return domain + imageUrl;
        }
        public async Task<string> UploadVideoUrl(Req.VideoUrl video)
        {

            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string imageUrl = await _fileRepository.SaveVideoUrl(video.VideoLinkUrl, "MediaContent/VideoUrl");
            return domain + imageUrl;
        }

        public async Task<Res.AuthorAndLanguage?> Create(Req.CreateVideo createVideo)
        {
            if (createVideo != null)
            {
                DM.Video video = new()
                {
                    ExamTypeId = createVideo.ExamTypeId,
                    CourseId = createVideo.CourseId,
                    SubCourseId = createVideo.SubCourseId,
                    SubjectCategoryId = createVideo.SubjectCategory,
                    TopicId = createVideo.TopicId,
                    VideoTitle = createVideo.VideoTitle,
                    Description = createVideo.Description,
                    InstituteId = createVideo.InstituteId,
                    FacultyName = createVideo.FacultyName.Trim(),
                    Language = createVideo.Language,
                    VideoUrl = createVideo.VideoUrl,
                    VideoThumbnail = createVideo.VideoThumbnail,
                    Price = createVideo.Price,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = createVideo.UserId,
                };
                int result = await _unitOfWork.Repository<DM.Video>().Insert(video);
                if (result > 0)
                {
                    Res.AuthorAndLanguage authorAndLanguage = new()
                    {
                        authorName = createVideo.FacultyName,
                        Language = createVideo.Language,
                    };
                    return authorAndLanguage;
                }
                return null;
            }
            return null;
        }

        public async Task<Res.AuthorAndLanguage?> Edit(Req.EditVideo editVideo)
        {

            var video = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == editVideo.Id && !x.IsDeleted);
            if (video != null)
            {
                video.ExamTypeId = editVideo.ExamTypeId;
                video.CourseId = editVideo.CourseId;
                video.SubCourseId = editVideo.SubCourseId;
                video.SubjectCategoryId = editVideo.SubjectCategory;
                video.TopicId = editVideo.TopicId;
                video.VideoTitle = editVideo.VideoTitle;
                video.Description = editVideo.Description;
                video.InstituteId = editVideo.InstituteId;
                video.FacultyName = editVideo.FacultyName.Trim();
                video.Language = editVideo.Language;
                video.VideoUrl = editVideo.VideoUrl;
                video.VideoThumbnail = editVideo.VideoThumbnail;
                video.Price = editVideo.Price;
                video.LastModifyDate = DateTime.UtcNow;
                video.LastModifierUserId = editVideo.UserId;
                int result = await _unitOfWork.Repository<DM.Video>().Update(video);
                if (result > 0)
                {
                    Res.AuthorAndLanguage authorAndLanguage = new()
                    {
                        authorName = editVideo.FacultyName,
                        Language = editVideo.Language,
                    };
                    return authorAndLanguage;
                }
                return null;
            }
            return null;
        }

        public async Task<Res.Video?> GetVideoById(Req.VideoById video)
        {
            var videoDetails = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == video.Id && !x.IsDeleted);
            if (videoDetails != null)
            {
                Res.Video videos = new();
                videos.Id = videoDetails.Id;
                videos.ExamTypeId = videoDetails.ExamTypeId;
                var examTypeData = await _unitOfWork.Repository<DM.ExamType>().GetSingle(x => x.Id == videoDetails.ExamTypeId && !x.IsDeleted);
                videos.ExamTypeName = examTypeData != null ? examTypeData.ExamName : "N/A";
                videos.CourseId = videoDetails.CourseId;
                var courseData = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == videoDetails.CourseId && !x.IsDeleted);
                videos.CourseName = courseData != null ? courseData.CourseName : "N/A";
                videos.SubCourseId = videoDetails.SubCourseId;
                var SubcourseData = await _unitOfWork.Repository<DM.SubCourse>().GetSingle(x => x.Id == videoDetails.SubCourseId && !x.IsDeleted);
                videos.SubCourseName = SubcourseData != null ? SubcourseData.SubCourseName : "N/A";
                videos.SubjectCategoryId = videoDetails.SubjectCategoryId;
                var subjectCategoryData = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == videoDetails.SubjectCategoryId && !x.IsDeleted);
                Guid subjectId = subjectCategoryData != null ? subjectCategoryData.SubjectId : Guid.Empty;
                var subjects = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                videos.SubjectName = subjects != null ? subjects.SubjectName : "N/A";
                videos.TopicId = videoDetails.TopicId;
                var topicdata = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == videoDetails.TopicId && !x.IsDeleted);
                videos.TopicName = topicdata != null ? topicdata.TopicName : "N/A";
                videos.VideoTitle = videoDetails.VideoTitle;
                videos.Description = videoDetails.Description;
                videos.InstituteId = videoDetails.InstituteId;
                var instituteData = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == videoDetails.InstituteId && !x.IsDeleted);
                videos.InstituteName = instituteData != null ? instituteData.InstituteName : "N/A";
                videos.FacultyName = videoDetails.FacultyName;
                videos.Language = videoDetails.Language;
                videos.VideoUrl = videoDetails.VideoUrl;
                videos.VideoThumbnail = videoDetails.VideoThumbnail;
                videos.VideoUrl = videos.VideoUrl;
                videos.Price = videoDetails.Price;
                videos.CreationDateTime = videoDetails.CreationDate;
                videos.CreatorUserId = videoDetails.CreatorUserId;
                var totalPurchaseCount = await _unitOfWork.Repository<DM.MyPurchased>().Get(x => x.ProductId == videoDetails.Id && x.ProductCategory == ProductCategory.Video && !x.IsDeleted);
                videos.TotalPurchase = totalPurchaseCount != null ? totalPurchaseCount.Count : 0;
                return videos;
            }
            return null;
        }

        public async Task<bool> Delete(Req.VideoById video)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(video.UserId.ToString());

            var userRoles = await _userManager.GetRolesAsync(user);
            string role = userRoles.First();
            if (role == "Admin")
            {
                var videoDetail = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == video.Id && !x.IsDeleted);
                if (videoDetail != null)
                {
                    videoDetail.IsActive = false;
                    videoDetail.IsDeleted = true;
                    videoDetail.DeletionDate = DateTime.UtcNow;
                    videoDetail.DeleterUserId = video.UserId;
                    await _unitOfWork.Repository<DM.Video>().Update(videoDetail);
                    return true;
                }
            }
            else if (role == "Staff")
            {
                var videoDetail = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == video.Id && !x.IsDeleted && x.CreatorUserId == video.UserId);
                if (videoDetail != null)
                {
                    videoDetail.IsActive = false;
                    videoDetail.IsDeleted = true;
                    videoDetail.DeletionDate = DateTime.UtcNow;
                    videoDetail.DeleterUserId = video.UserId;
                    await _unitOfWork.Repository<DM.Video>().Update(videoDetail);
                    return true;
                }
                return false;
            }
            return false;
        }

        //Get All Video Filter by Staff 
        public async Task<Res.VideoList?> GetAllVideos(Req.GetAllVideos videos)
        {
            Res.VideoList videoList = new();
            List<Res.Videos> videoDetailList = new();
            if (videos.TopicId != Guid.Empty && videos.Language != "All")
            {
                var videoAll = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted && x.InstituteId == videos.InstituteId && x.SubjectCategoryId == videos.SubjectCategoryId && x.TopicId == videos.TopicId && x.FacultyName == videos.FacultyName && x.Language == videos.Language, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (videoAll.Any())
                {
                    var newList = videoAll.Page(videos.PageNumber, videos.PageSize).ToList();

                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == item.SubjectCategoryId && !x.IsDeleted);
                        var subjectId = subjectCategory != null ? subjectCategory.SubjectId : Guid.Empty;
                        var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.Videos videoDetails = new();
                        videoDetails.Id = item.Id;
                        videoDetails.VideoUrl = item.VideoUrl;
                        videoDetails.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        videoDetails.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                        videoDetails.VideoTitle = item.VideoTitle;
                        videoDetails.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        videoDetails.FacultyName = item.FacultyName;
                        videoDetails.VideoThumbnail = item.VideoThumbnail;
                        videoDetails.Price = item.Price;
                        videoDetails.CreationDateTime = item.CreationDate;
                        videoDetails.CreatorUserId = item.CreatorUserId;
                        videoDetails.Language = item.Language;
                        videoDetailList.Add(videoDetails);
                    }

                }

                if (videoDetailList.Any())
                {
                    var result = videoDetailList;
                    videoList.Videos = result.ToList();
                    videoList.TotalRecords = videoAll.Count;
                    return videoList;
                }
                else
                {
                    return null;
                }
            }
            else if (videos.TopicId != Guid.Empty && videos.Language == "All")
            {
                var videoAll = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted && x.InstituteId == videos.InstituteId && x.SubjectCategoryId == videos.SubjectCategoryId && x.TopicId == videos.TopicId && x.FacultyName == videos.FacultyName , orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (videoAll.Any())
                {
                    var newList = videoAll.Page(videos.PageNumber, videos.PageSize).ToList();

                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == item.SubjectCategoryId && !x.IsDeleted);
                        var subjectId = subjectCategory != null ? subjectCategory.SubjectId : Guid.Empty;
                        var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.Videos videoDetails = new();
                        videoDetails.Id = item.Id;
                        videoDetails.VideoUrl = item.VideoUrl;
                        videoDetails.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        videoDetails.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                        videoDetails.VideoTitle = item.VideoTitle;
                        videoDetails.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        videoDetails.FacultyName = item.FacultyName;
                        videoDetails.VideoThumbnail = item.VideoThumbnail;
                        videoDetails.Price = item.Price;
                        videoDetails.CreationDateTime = item.CreationDate;
                        videoDetails.CreatorUserId = item.CreatorUserId;
                        videoDetails.Language = item.Language;
                        videoDetailList.Add(videoDetails);
                    }

                }

                if (videoDetailList.Any())
                {
                    var result = videoDetailList;
                    videoList.Videos = result.ToList();
                    videoList.TotalRecords = videoAll.Count;
                    return videoList;
                }
                else
                {
                    return null;
                }
            }
            else if (videos.TopicId == Guid.Empty && videos.Language != "All")
            {
                var videoAll = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted && x.InstituteId == videos.InstituteId && x.SubjectCategoryId == videos.SubjectCategoryId && x.FacultyName == videos.FacultyName && x.Language == videos.Language, orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (videoAll.Any())
                {
                    var newList = videoAll.Page(videos.PageNumber, videos.PageSize).ToList();
                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == item.SubjectCategoryId && !x.IsDeleted);
                        var subjectId = subjectCategory != null ? subjectCategory.SubjectId : Guid.Empty;
                        var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.Videos videoDetails = new();
                        videoDetails.Id = item.Id;
                        videoDetails.VideoUrl = item.VideoUrl;
                        videoDetails.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        videoDetails.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                        videoDetails.VideoTitle = item.VideoTitle;
                        videoDetails.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        videoDetails.FacultyName = item.FacultyName;
                        videoDetails.VideoThumbnail = item.VideoThumbnail;
                        videoDetails.Price = item.Price;
                        videoDetails.CreationDateTime = item.CreationDate;
                        videoDetails.CreatorUserId = item.CreatorUserId;
                        videoDetails.Language = item.Language;
                        videoDetailList.Add(videoDetails);
                    }

                }
                if (videoDetailList.Count > 0)
                {
                    var result = videoDetailList;
                    videoList.Videos = result.ToList();
                    videoList.TotalRecords = videoAll.Count;
                    return videoList;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                var videoAll = await _unitOfWork.Repository<DM.Video>().Get(x => !x.IsDeleted && x.InstituteId == videos.InstituteId && x.SubjectCategoryId == videos.SubjectCategoryId && x.FacultyName == videos.FacultyName , orderBy: x => x.OrderByDescending(x => x.CreationDate));
                if (videoAll.Any())
                {
                    var newList = videoAll.Page(videos.PageNumber, videos.PageSize).ToList();

                    foreach (var item in newList)
                    {
                        var topicDetail = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == item.TopicId && !x.IsDeleted);
                        var topicName = topicDetail != null ? topicDetail.TopicName : "";
                        var subjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == item.SubjectCategoryId && !x.IsDeleted);
                        var subjectId = subjectCategory != null ? subjectCategory.SubjectId : Guid.Empty;
                        var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId && !x.IsDeleted);
                        var instituteDetails = await _unitOfWork.Repository<DM.Institute>().GetSingle(x => x.Id == item.InstituteId && !x.IsDeleted);
                        Res.Videos videoDetails = new();
                        videoDetails.Id = item.Id;
                        videoDetails.VideoUrl = item.VideoUrl;
                        videoDetails.TopicName = item.TopicId != Guid.Empty ? topicName : "";
                        videoDetails.SubjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                        videoDetails.VideoTitle = item.VideoTitle;
                        videoDetails.InstituteName = instituteDetails != null ? instituteDetails.InstituteName : "";
                        videoDetails.FacultyName = item.FacultyName;
                        videoDetails.VideoThumbnail = item.VideoThumbnail;
                        videoDetails.Price = item.Price;
                        videoDetails.CreationDateTime = item.CreationDate;
                        videoDetails.CreatorUserId = item.CreatorUserId;
                        videoDetails.Language = item.Language;
                        videoDetailList.Add(videoDetails);
                    }

                }

                if (videoDetailList.Any())
                {
                    var result = videoDetailList;
                    videoList.Videos = result.ToList();
                    videoList.TotalRecords = videoAll.Count;
                    return videoList;
                }
                else
                {
                    return null;
                }

            }
        }
        public async Task<Res.VideoList?> GetAll50()
        {
            Res.VideoList videoList = new();

            var videosList = await(from e in _unitOfWork.GetContext().Videos
                                   join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                   join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                   join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                   join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                   join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                   join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                   where !e.IsDeleted
                                   select new Res.Videos
                                   {
                                       Id = e.Id,
                                       TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
                                       SubjectName = subject.SubjectName,
                                       VideoTitle = e.VideoTitle,
                                       InstituteName = institute.InstituteName,
                                       FacultyName = e.FacultyName,
                                       VideoThumbnail = e.VideoThumbnail,
                                       Price = e.Price,
                                       CreatorUserId = e.CreatorUserId,
                                       CreationDateTime = e.CreationDate,
                                       VideoUrl = e.VideoUrl,
                                       Language = e.Language
                                   }).AsQueryable().Take(50).Distinct().ToListAsync();
            List<Res.Videos> videosV1s = new();
            foreach (var item in videosList)
            {
                Res.Videos qu = new()
                {
                    Id = item.Id,
                    TopicName = item.TopicName,
                    SubjectName=item.SubjectName,
                    VideoTitle=item.VideoTitle,
                    InstituteName = item.InstituteName,
                    FacultyName=item.FacultyName,
                    VideoThumbnail= item.VideoThumbnail,                    
                    Price = item.Price,
                    CreatorUserId = item.CreatorUserId,
                    CreationDateTime = item.CreationDateTime,
                    VideoUrl= item.VideoUrl,
                    Language= item.Language
                };
                videosV1s.Add(qu);
            }
            if (videosV1s.Count > 0)
            {
                //var result = ebookV1s.Page(1,50);
                //var resultV1 = result.Page(e.PageNumber, ebook.PageSize);
                videoList.Videos = videosV1s.ToList();
                videoList.TotalRecords = videosV1s.Count;
                return videoList;
            }
            else
            {
                return null;
            }
        }

        //Get All Data with staff Filter
        //public async Task<Res.VideoList?> GetAllStaffVideos(Req.GetAllVideos videos)
        //{
        //    Res.VideoList videoList = new();
        //    if (videos.TopicId != Guid.Empty && videos.Language != "All")
        //    {
        //        var videosList = await(from e in _unitOfWork.GetContext().Videos
        //                               join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
        //                               join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
        //                               join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
        //                               join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
        //                               join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
        //                               join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
        //                               where !e.IsDeleted && s.Id == videos.SubjectCategoryId && topic.Id == videos.TopicId && e.SubjectCategoryId == videos.SubjectCategoryId
        //                               && institute.Id == videos.InstituteId && e.FacultyName.ToLower() == videos.FacultyName.ToLower() && e.TopicId == videos.TopicId && e.Language.ToLower() == videos.Language.ToLower()
        //                               select new Res.Videos
        //                               {
        //                                   Id = e.Id,
        //                                   TopicName = topic.TopicName,
        //                                   SubjectName = subject.SubjectName,
        //                                   VideoTitle = e.VideoTitle,
        //                                   InstituteName = institute.InstituteName,
        //                                   FacultyName = e.FacultyName,
        //                                   VideoThumbnail = e.VideoThumbnail,
        //                                   Price = e.Price,
        //                                   VideoUrl = e.VideoUrl,
        //                                   CreatorUserId = e.CreatorUserId,
        //                                   CreationDateTime = e.CreationDate,
        //                                   Language = e.Language,
        //                               }).Distinct().ToListAsync();

        //        if (videosList.Count > 0)
        //        {
        //            var result = videosList.OrderByDescending(x => x.Id);
        //            var resultV1 = result.Page(videos.PageNumber, videos.PageSize);
        //            videoList.Videos = resultV1.ToList();
        //            videoList.TotalRecords = videosList.Count;
        //            return videoList;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else if (videos.TopicId != Guid.Empty && videos.Language == "All")
        //    {
        //        var videosList = await(from e in _unitOfWork.GetContext().Videos
        //                               join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
        //                               join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
        //                               join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
        //                               join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
        //                               join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
        //                               join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
        //                               where !e.IsDeleted && s.Id == videos.SubjectCategoryId && topic.Id == videos.TopicId && e.SubjectCategoryId == videos.SubjectCategoryId
        //                               && institute.Id == videos.InstituteId && e.FacultyName.ToLower() == videos.FacultyName.ToLower() && e.TopicId == videos.TopicId
        //                               select new Res.Videos
        //                               {
        //                                   Id = e.Id,
        //                                   TopicName = topic.TopicName,
        //                                   SubjectName = subject.SubjectName,
        //                                   VideoTitle = e.VideoTitle,
        //                                   InstituteName = institute.InstituteName,
        //                                   FacultyName = e.FacultyName,
        //                                   VideoThumbnail = e.VideoThumbnail,
        //                                   Price = e.Price,
        //                                   VideoUrl = e.VideoUrl,
        //                                   CreatorUserId = e.CreatorUserId,
        //                                   CreationDateTime = e.CreationDate,
        //                                   Language = e.Language,
        //                               }).Distinct().ToListAsync();

        //        if (videosList.Count > 0)
        //        {
        //            var result = videosList.OrderByDescending(x => x.Id);
        //            var resultV1 = result.Page(videos.PageNumber, videos.PageSize);
        //            videoList.Videos = resultV1.ToList();
        //            videoList.TotalRecords = videosList.Count;
        //            return videoList;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else if (videos.TopicId == Guid.Empty && videos.Language != "All")
        //    {
        //        var videosList = await(from e in _unitOfWork.GetContext().Videos
        //                               join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
        //                               join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
        //                               join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
        //                               join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
        //                               join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
        //                               where e.TopicId == Guid.Empty
        //                               join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
        //                               where !e.IsDeleted && s.Id == videos.SubjectCategoryId && e.SubjectCategoryId == videos.SubjectCategoryId
        //                               && institute.Id == videos.InstituteId && e.FacultyName.ToLower() == videos.FacultyName.ToLower() && e.Language.ToLower() == videos.Language.ToLower()
        //                               select new Res.Videos
        //                               {
        //                                   Id = e.Id,
        //                                   TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
        //                                   SubjectName = subject.SubjectName,
        //                                   VideoTitle = e.VideoTitle,
        //                                   InstituteName = institute.InstituteName,
        //                                   FacultyName = e.FacultyName,
        //                                   VideoThumbnail = e.VideoThumbnail,
        //                                   Price = e.Price,
        //                                   CreatorUserId = e.CreatorUserId,
        //                                   CreationDateTime = e.CreationDate,
        //                                   VideoUrl = e.VideoUrl,
        //                                   Language = e.Language,
        //                               }).Distinct().ToListAsync();
        //        if (videosList.Count > 0)
        //        {
        //            var result = videosList.OrderByDescending(x => x.Id);
        //            var resultV1 = result.Page(videos.PageNumber, videos.PageSize);
        //            videoList.Videos = resultV1.ToList();
        //            videoList.TotalRecords = videosList.Count;
        //            return videoList;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        var videosList = await(from e in _unitOfWork.GetContext().Videos
        //                               join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
        //                               join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
        //                               join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
        //                               join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
        //                               join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
        //                               where e.TopicId == Guid.Empty
        //                               join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
        //                               where !e.IsDeleted && s.Id == videos.SubjectCategoryId && e.SubjectCategoryId == videos.SubjectCategoryId
        //                               && institute.Id == videos.InstituteId && e.InstituteId == videos.InstituteId && e.FacultyName.ToLower() == videos.FacultyName.ToLower()
        //                               select new Res.Videos
        //                               {
        //                                   Id = e.Id,
        //                                   TopicName = e.TopicId != Guid.Empty ? topic.TopicName : "",
        //                                   SubjectName = subject.SubjectName,
        //                                   VideoTitle = e.VideoTitle,
        //                                   InstituteName = institute.InstituteName,
        //                                   FacultyName = e.FacultyName,
        //                                   VideoThumbnail = e.VideoThumbnail,
        //                                   Price = e.Price,
        //                                   CreatorUserId = e.CreatorUserId,
        //                                   CreationDateTime = e.CreationDate,
        //                                   VideoUrl = e.VideoUrl,
        //                                   Language = e.Language
        //                               }).Distinct().ToListAsync();

        //        if (videosList.Count > 0)
        //        {
        //            var result = videosList.OrderByDescending(x => x.Id);
        //            var resultV1 = result.Page(videos.PageNumber, videos.PageSize);
        //            videoList.Videos = resultV1.ToList();
        //            videoList.TotalRecords = videosList.Count;
        //            return videoList;
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //    }
        //}

        public async Task<Res.VideoAuthorList?> GetAllAuthors(Req.GetAllVideoAuthors video)
        {
            Res.VideoAuthorList videoList = new();
            if (video.TopicId != Guid.Empty)
            {
                var videosList = await (from e in _unitOfWork.GetContext().Videos
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && s.Id == video.SubjectCategoryId && topic.Id == video.TopicId && e.SubjectCategoryId == video.SubjectCategoryId
                                        && e.InstituteId == video.InstituteId
                                        select new Res.VideoAuthors
                                        {
                                            FacultyName = e.FacultyName
                                        }).Distinct().ToListAsync();

                if (videosList.Count > 0)
                {
                    videoList.VideoAuthors = videosList;
                    return videoList;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var videosList = await (from e in _unitOfWork.GetContext().Videos
                                        join s in _unitOfWork.GetContext().SubjectCategories on e.SubjectCategoryId equals s.Id
                                        join institute in _unitOfWork.GetContext().Institutes on e.InstituteId equals institute.Id
                                        where !e.IsDeleted && e.SubjectCategoryId == video.SubjectCategoryId
                                        && e.InstituteId == video.InstituteId && institute.Id == video.InstituteId && s.Id == video.SubjectCategoryId
                                        select new Res.VideoAuthors
                                        {
                                            FacultyName = e.FacultyName
                                        }).Distinct().ToListAsync();

                if (videosList.Count > 0)
                {
                    videoList.VideoAuthors = videosList;
                    return videoList;
                }
                else
                {
                    return null;
                }

            }

        }

        public async Task<Res.VideoListV2?> Showallids()
        {
            Res.VideoListV2 ebookListV2 = new();
            var videolist = await (from e in _unitOfWork.GetContext().Videos.Where(x => !x.IsDeleted)
                                   select new Res.VideoLists
                                   {
                                       Id = e.Id,
                                       Name = e.VideoTitle
                                   }).ToListAsync();
            ebookListV2.Videos = videolist;

            return ebookListV2;

        }
    }
}
