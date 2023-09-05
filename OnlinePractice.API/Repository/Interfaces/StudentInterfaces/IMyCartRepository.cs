using Org.BouncyCastle.Ocsp;
using Res = OnlinePractice.API.Models.Response;
using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Repository.Interfaces.StudentInterfaces
{
    public interface IMyCartRepository
    {
        public List<Com.EnumModel> ProductCategoryList();
        public List<Com.EnumModel> ProductCategoryListV1();
        public Task<bool> AddToMyCart(Req.AddtoMyCart myCart);
        public Task<Res.ShowMyCartList?> ShowMyCart(CurrentUser user);
        public Task<bool> RemoveItemFromMyCart(Req.RemoveItemFromMyCart myCart);
        public Task<bool> IsExist(Req.ProductIdCheck product);
        public  Task<Res.MyCartDetails> GetProductCategoryName(ProductCategory productCategory, Guid productId);
    }
}
