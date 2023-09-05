using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Repository.Interfaces.StudentInterfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using OnlinePractice.API.Repository.Services.StudentServices;

namespace OnlinePractice.API.Controllers.StudentController
{
   // [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class MyCartController : BaseController
    {
        private readonly ILogger<MyCartController> _logger;
        public IStudentRepository _studentRepository;
        public IMyCartRepository _myCartRepository;
        public readonly IMyCartValidation _validation;


        public MyCartController(ILogger<MyCartController> logger, IStudentRepository studentRepository, IMyCartRepository myCartRepository, IMyCartValidation validation)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _myCartRepository = myCartRepository;
            _validation = validation;
        }
        /// <summary>
        /// API for get list of
        /// product category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
       // [ApiExplorerSettings(IgnoreApi = true)]
        [Route("getproductcategorylist")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult GetProductCategoryList()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = _myCartRepository.ProductCategoryList();
                if (result != null)
                {
                    _logger.LogInformation("Get product category list successfully!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Success("Get product category list successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Product category not found!");
                    return Ok(ResponseResult<List<Com.EnumModel>>.Failure("Product category not found!", new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getproductcategorylist", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getproductcategorylist", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }
        }


        /// <summary>
        /// API for add to cart items
        /// </summary>
        /// <param name="myCart"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addtocart")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddToCart(Req.AddtoMyCart myCart)
        {
            #region Check Login Token
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            CurrentUser user = new()
            {
                UserId = this.UserId,
            };
            var oldToken = await _studentRepository.GetToken(user);
            if (token != oldToken)
            {
                return Unauthorized();
            }
            #endregion
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.AddToCartValidator.ValidateAsync(myCart);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                #region ProductIdCheck
                Req.ProductIdCheck productIdCheck = new()
                {
                    Id = myCart.ProductId,
                    UserId= this.UserId,
                };
                var isProductExist = await _myCartRepository.IsExist(productIdCheck);
                if (isProductExist)
                {
                    _logger.LogInformation("Product Id already added!");
                    return Ok(ResponseResult<DBNull>.Failure("Product Id already added!"));
                }
                #endregion
                myCart.UserId = this.UserId;
                var result = await _myCartRepository.AddToMyCart(myCart);
                if (result)
                {
                    _logger.LogInformation("Added to cart successfully!");
                    return Ok(ResponseResult<bool>.Success("Added to cart successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Not added to cart!");
                    return Ok(ResponseResult<bool>.Failure("Not added to cart!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addtocart", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "addtocart", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// API for  get all cart items 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("showmycart")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ShowMyCart()
        {
            #region Check Login Token
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            CurrentUser user = new()
            {
                UserId = this.UserId,
            };
            var oldToken = await _studentRepository.GetToken(user);
            if (token != oldToken)
            {
                return Unauthorized();
            }
            #endregion
            ErrorResponse? errorResponse;
            try
            {                
                var result = await _myCartRepository.ShowMyCart(user);
                if (result != null && result.ShowMyCart.Any())
                {
                    _logger.LogInformation("Get all item cart list successfully!");
                    return Ok(ResponseResult<Res.ShowMyCartList>.Success("Get all item cart list successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Cart not found!");
                    return Ok(ResponseResult<Res.ShowMyCartList>.Failure("Cart not found!",new()));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "showmycart", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "showmycart", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }
        /// <summary>
        /// API for removing item
        /// from the cart.
        /// </summary>
        /// <param name="myCart"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("removeitemfrommycart")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveItemFromMyCart(Req.RemoveItemFromMyCart myCart)
        {
            #region Check Login Token
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            CurrentUser user = new()
            {
                UserId = this.UserId,
            };
            var oldToken = await _studentRepository.GetToken(user);
            if (token != oldToken)
            {
                return Unauthorized();
            }
            #endregion
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.RemoveItemFromMyCartValidator.ValidateAsync(myCart);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion   
                myCart.UserId = this.UserId;
                var result = await _myCartRepository.RemoveItemFromMyCart(myCart);
                if (result)
                {
                    _logger.LogInformation("Item removed from cart successfully!");
                    return Ok(ResponseResult<bool>.Success("Item removed from cart successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Cart item not found!");
                    return Ok(ResponseResult<bool>.Failure("Cart item not found!", result));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removeitemfrommycart", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "removeitemfrommycart", ex);
                return Ok(ResponseResult<DBNull>.Exception("Something went wrong!"));
            }

        }

    }
}
