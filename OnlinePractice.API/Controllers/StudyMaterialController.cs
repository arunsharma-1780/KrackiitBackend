using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using OnlinePractice.API.Validator.Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudyMaterialController : ControllerBase
    {
        private readonly IStudyMaterialRepository _studyMaterialRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly Guid _userId;
        private readonly IStudyMaterialValidation _validation;
        public StudyMaterialController(IStudyMaterialRepository studyMaterialRepository, ILogger<ProductController> logger, IStudyMaterialValidation validation)
        {
            _studyMaterialRepository = studyMaterialRepository;
            _logger = logger;
            _validation = validation;
            _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        }

        /// <summary>
        /// Create StudyMaterial
        /// </summary>
        /// <param name="studyMaterial"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(CreateStudyMaterial studyMaterial)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateStudyMaterialValidator.ValidateAsync(studyMaterial);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                studyMaterial.UserId = _userId;
                var result = await _studyMaterialRepository.Create(studyMaterial);

                if (result)
                {
                    _logger.LogInformation("Study Material created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Study Material created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Study Material not created!");
                    return Ok(ResponseResult<DBNull>.Success("Study Material not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return Ok(ResponseResult<DBNull>.Failure("Study Material not created!"));
            }
        }

        /// <summary>
        /// Edit Study material
        /// </summary>
        /// <param name="editStudyMaterial"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(EditStudyMaterial editStudyMaterial)
        {

            try
            {
                editStudyMaterial.UserId = _userId;
                var result = await _studyMaterialRepository.Edit(editStudyMaterial);
                if (result)
                {
                    _logger.LogInformation("Study material edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Study material edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Study material not edited!");
                    return Ok(ResponseResult<DBNull>.Success("Study material not edited!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Failure("Study material not edited!"));
            }

        }


        /// <summary>
        /// Get Study Material by Id
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbyid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<StudyMaterial>))]
        public async Task<IActionResult> GetById(MaterialById material)
        {
            try
            {
                var result = await _studyMaterialRepository.GetById(material);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<StudyMaterial>.Success("Product get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Record not found!");
                    return Ok(ResponseResult<DBNull>.Success("Study material not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("Study material not found!"));
            }
        }


        /// <summary>
        /// delete product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(MaterialById material)
        {
            try
            {
                material.UserId = _userId;
                var result = await _studyMaterialRepository.Delete(material);
                if (result)
                {
                    _logger.LogInformation("StudyMaterial deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("StudyMaterial deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("StudyMaterial not deleted!");
                    return Ok(ResponseResult<DBNull>.Success("StudyMaterial not deleted!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Failure("StudyMaterial not Found!"));
            }

        }

        /// <summary>
        /// Edit Price Sof Study Material
        /// </summary>
        /// <param name="editPriceStudy"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editprice")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> EditPrice(EditPriceStudyMaterial editPriceStudy)
        {
            try
            {
                editPriceStudy.UserId = _userId;
                var result = await _studyMaterialRepository.EditPrice(editPriceStudy);
                if (result)
                {
                    _logger.LogInformation("Price of Study material edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Price of Study material edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Price of Study material not edited!");
                    return Ok(ResponseResult<DBNull>.Success("Price of Study material not edited!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Failure("Price of Study material not edited!"));
            }

        }

    }
}
