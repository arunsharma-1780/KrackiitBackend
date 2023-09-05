using OnlinePractice.API.Repository.Base;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services.StudentServices
{
    public class MyCartRepository : IMyCartRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DBContext _dbContext;
        public MyCartRepository(IUnitOfWork unitOfWork, DBContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }
        public async Task<bool> AddToMyCart(Req.AddtoMyCart myCart)
        {
            if (myCart == null) return false;
            else
            {
                DM.MyCart addMyCart = new()
                {
                    ProductCategory = myCart.ProductCategory,
                    ProductId = myCart.ProductId,
                    Price = myCart.Price,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = myCart.UserId,
                    StudentId = myCart.UserId,
                    IsPurchased = false

                };
                int result = await _unitOfWork.Repository<DM.MyCart>().Insert(addMyCart);
                return result > 0;
            }
        }
        public async Task<Res.ShowMyCartList?> ShowMyCart(CurrentUser user)
        {
            Res.ShowMyCartList showMyCartList = new();
            var cartDetails = await _unitOfWork.Repository<DM.MyCart>().Get(x => x.StudentId == user.UserId && !x.IsDeleted && !x.IsPurchased);
            if (cartDetails.Any())
            {
                foreach (var cart in cartDetails)
                {
                    var deatils = await GetProductCategoryName(cart.ProductCategory, cart.ProductId);
                    Res.ShowMyCart showMy = new();
                    showMy.Id = cart.Id;
                    showMy.ProductId = cart.ProductId;
                    showMy.Price = cart.Price;
                    showMy.ProductCategory = cart.ProductCategory.ToString();
                    showMy.ProductName = deatils.ProductName;
                    showMy.SubjectName = deatils.SubjectName;
                    showMy.TopicName = deatils.TopicName;
                    showMy.FacultyName = deatils.FacultyName;
                    showMy.Year = deatils.Year;
                    showMy.Thumbnail = deatils.Thumbnail;
                    showMy.SourceUrl = string.Empty;
                    showMyCartList.ShowMyCart.Add(showMy);
                }
                showMyCartList.TotalRecords = cartDetails.Count;
                showMyCartList.CartTotalPrice = cartDetails.Sum(x => x.Price);
            }
            return showMyCartList;

        }
        public async Task<Res.MyCartDetails> GetProductCategoryName(ProductCategory productCategory, Guid productId)
        {
            Res.MyCartDetails myCartDetails = new();
            if (productCategory == ProductCategory.MockTest)
            {
                var mockTestDetails = await _unitOfWork.Repository<DM.MockTestSettings>().GetSingle(x => x.Id == productId && !x.IsDeleted);
                if (mockTestDetails != null)
                {
                    myCartDetails.ProductName = mockTestDetails.MockTestName;
                    var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == mockTestDetails.SubjectId);
                    var subjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                    myCartDetails.SubjectName = subjectName;
                    var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == mockTestDetails.TopicId);
                    var topicName = topicDetails != null ? topicDetails.TopicName : string.Empty;
                    myCartDetails.TopicName = topicName;


                    return myCartDetails;
                }
            }
            else if (productCategory == ProductCategory.Video)
            {
                var videoDetails = await _unitOfWork.Repository<DM.Video>().GetSingle(x => x.Id == productId && !x.IsDeleted);
                if (videoDetails != null)
                {
                    myCartDetails.ProductName = videoDetails.VideoTitle;
                    var subjectCategoryDetails = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == videoDetails.SubjectCategoryId);
                    var subjectId = subjectCategoryDetails != null ? subjectCategoryDetails.SubjectId : Guid.Empty;
                    var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId);
                    var subjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                    myCartDetails.SubjectName = subjectName;
                    var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == videoDetails.TopicId);
                    var topicName = topicDetails != null ? topicDetails.TopicName : string.Empty;
                    myCartDetails.TopicName = topicName;
                    myCartDetails.Thumbnail = videoDetails.VideoThumbnail;
                    myCartDetails.SourceUrl = videoDetails.VideoUrl;
                    myCartDetails.FacultyName = videoDetails.FacultyName;
                    return myCartDetails;
                }
            }
            else if (productCategory == ProductCategory.eBook)
            {
                var ebookDetails = await _unitOfWork.Repository<DM.Ebook>().GetSingle(x => x.Id == productId && !x.IsDeleted);
                if (ebookDetails != null)
                {
                    myCartDetails.ProductName = ebookDetails.EbookTitle;
                    var subjectCategoryDetails = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == ebookDetails.SubjectCategoryId);
                    var subjectId = subjectCategoryDetails != null ? subjectCategoryDetails.SubjectId : Guid.Empty;
                    var subjectDetails = await _unitOfWork.Repository<DM.Subject>().GetSingle(x => x.Id == subjectId);
                    var subjectName = subjectDetails != null ? subjectDetails.SubjectName : "";
                    myCartDetails.SubjectName = subjectName;
                    var topicDetails = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == ebookDetails.TopicId);
                    var topicName = topicDetails != null ? topicDetails.TopicName : string.Empty;
                    myCartDetails.TopicName = topicName;
                    myCartDetails.Thumbnail = ebookDetails.EbookThumbnail;
                    myCartDetails.SourceUrl = ebookDetails.EbookPdfUrl;
                    myCartDetails.FacultyName = ebookDetails.AuthorName;
                    return myCartDetails;
                }
            }
            else if (productCategory == ProductCategory.PreviouseYearPaper)
            {
                var pypDetails = await _unitOfWork.Repository<DM.PreviousYearPaper>().GetSingle(x => x.Id == productId && !x.IsDeleted);
                if (pypDetails != null)
                {
                    myCartDetails.ProductName = pypDetails.PaperTitle;
                    myCartDetails.SourceUrl = pypDetails.PaperPdfUrl;
                    myCartDetails.Year = pypDetails.Year;
                    return myCartDetails;
                }
            }
            else if (productCategory == ProductCategory.Reward)
            {
                myCartDetails.ProductName = "Signup reward!";
            }
            return myCartDetails;
        }
        public async Task<bool> RemoveItemFromMyCart(Req.RemoveItemFromMyCart myCart)
        {
            if (myCart == null) return false;
            else
            {
                var myCartItem = await _unitOfWork.Repository<DM.MyCart>().GetSingle(x => x.Id == myCart.Id && x.ProductId == myCart.ProductId && !x.IsDeleted);
                if (myCartItem != null)
                {
                    myCartItem.IsDeleted = true;
                    myCartItem.DeletionDate = DateTime.Now;
                    myCartItem.DeleterUserId = myCart.UserId;
                    int result = await _unitOfWork.Repository<DM.MyCart>().Update(myCartItem);
                    return result > 0;
                }
                return false;

            }
        }
        public List<Com.EnumModel> ProductCategoryList()
        {
            List<EnumModel> enums = ((ProductCategory[])Enum.GetValues(typeof(ProductCategory))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }
        public List<Com.EnumModel> ProductCategoryListV1()
        {
            List<EnumModel> enumList = new();
            List<EnumModel> enums = ((ProductCategory[])Enum.GetValues(typeof(ProductCategory))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            foreach (EnumModel enumModel in enums)
            {
                if (enumModel.Value == 1)
                {
                    enumModel.Name = "Mock Test";
                }
                else if (enumModel.Value == 2)
                {
                    enumModel.Name = "eBooks";
                }
                else if (enumModel.Value == 3)
                {
                    enumModel.Name = "Videos";
                }
                else if (enumModel.Value == 4)
                {
                    enumModel.Name = "Previous Year Paper";
                }
                enumList.Add(enumModel);
            }
            return enums;
        }
        public async Task<bool> IsExist(Req.ProductIdCheck product)
        {
            var myCart = await _unitOfWork.Repository<DM.MyCart>().GetSingle(x => x.ProductId == product.Id && x.StudentId == product.UserId && !x.IsDeleted);
            if (myCart != null)
                return true;
            return false;
        }
    }
}
