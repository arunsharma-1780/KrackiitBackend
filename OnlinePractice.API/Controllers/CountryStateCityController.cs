using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Filters;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Validator;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;


namespace OnlinePractice.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryStateCityController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CountryStateCityController> _logger;
        private readonly Guid _userId;

        public CountryStateCityController(ILogger<CountryStateCityController> logger, ICountryRepository countryRepository, IStateRepository stateRepository
            , ICityRepository cityRepository)
        {
            _logger = logger;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            _cityRepository = cityRepository;
        }

        #region CountryController
        /// <summary>
        /// Create Country Contoller
        /// </summary>
        /// <param name="createCountries"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("country/createcountry")]
        public async Task<IActionResult> Create(CreateCountries createCountries)
        {
            ErrorResponse? errorResponse;
            try
            {
                createCountries.UserId = _userId;
                var result = await _countryRepository.Create(createCountries);

                if (result)
                {
                    _logger.LogInformation("Country created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Country created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Country not created!");
                    return Ok(ResponseResult<DBNull>.Success("Country not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createcountry", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "createcountry", ex);
                return Ok(ResponseResult<DBNull>.Failure("Country not created!"));
            }
        }



        /// <summary>
        /// get Institute details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("country/getallcountries")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<List<DM.Countries>>))]
        public async Task<IActionResult> GetAllCountries()
        {
            try
            {
                var result = await _countryRepository.GetAllCountries();

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<List<DM.Countries>>.Success("Countries gets successfully", result));
                }
                else
                {
                    _logger.LogInformation("Countries not found!");
                    return Ok(ResponseResult<DBNull>.Success("Countries not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getallcountries", ex);
                return Ok(ResponseResult<DBNull>.Failure("Countries not Found!"));
            }
        }


        /// <summary>
        /// EditCountry
        /// </summary>
        /// <param name="editCountries"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("country/edit")]
        public async Task<IActionResult> Edit(EditCountries editCountries)
        {
            try
            {
                editCountries.UserId = _userId;
                var result = await _countryRepository.Edit(editCountries);
                if (result)
                {
                    _logger.LogInformation("Countries edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Countries edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("Countries not edited!");
                    return Ok(ResponseResult<DBNull>.Success("Countries not edited!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Failure("Country not Found!"));
            }

        }


        /// <summary>
        /// GetCountry ByID
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("country/getbyid")]
        public async Task<IActionResult> GetById(CountryById country)
        {
            try
            {

                var result = await _countryRepository.GetById(country);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.Countries>.Success("Country get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Country not found!");
                    return Ok(ResponseResult<DBNull>.Success("Country not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("Country not Found!"));
            }
        }

        /// <summary>
        /// Delete Country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("country/delete")]
        public async Task<IActionResult> Delete(CountryById country)
        {

            try
            {
                country.UserId = _userId;
                var result = await _countryRepository.Delete(country);
                if (result)
                {
                    _logger.LogInformation("Country deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Country deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("Country not deleted!");
                    return Ok(ResponseResult<DBNull>.Success("Country not deleted!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Failure("Country not Found!"));
            }

        }
        #endregion

        #region StateController

        /// <summary>
        /// CreateState contoller method
        /// </summary>
        /// <param name="createState"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("state/create")]
        public async Task<IActionResult> CreateState(CreateState createState)
        {
            try
            {
                createState.UserId = _userId;
                var result = await _stateRepository.Create(createState);

                if (result)
                {
                    _logger.LogInformation("Record created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("State created successfully!"));
                }
                else
                {
                    _logger.LogCritical("State not created!");
                    return Ok(ResponseResult<DBNull>.Success("State not created!"));
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return Ok(ResponseResult<DBNull>.Failure("State not created!"));
            }
        }


        /// <summary>
        /// GetAllStates 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("state/getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<List<DM.State>>))]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var result = await _stateRepository.GetAllStates();

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<List<DM.State>>.Success("States get successfully", result));
                }
                else
                {
                    _logger.LogInformation("States not found!");
                    return Ok(ResponseResult<DBNull>.Success("States not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall", ex);
                return Ok(ResponseResult<DBNull>.Failure("States not found!"));
            }
        }


        /// <summary>
        /// GetAllStatesByCountryId
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("state/getallstatesbycountryid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<List<DM.State>>))]
        public async Task<IActionResult> GetAllStatesByCountry(Guid countryId)
        {
            try
            {
                var result = await _stateRepository.GetAllStatesByCountryId(countryId);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<List<DM.State>>.Success("States get successfully", result));
                }
                else
                {
                    _logger.LogInformation("States not found!");
                    return Ok(ResponseResult<DBNull>.Success("States not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("States not found!"));
            }
        }


        /// <summary>
        /// EditState
        /// </summary>
        /// <param name="editState"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("state/edit")]
        public async Task<IActionResult> EditState(EditState editState)
        {
            try
            {
                editState.UserId = _userId;
                var result = await _stateRepository.Edit(editState);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("State edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("State not edited!");
                    return Ok(ResponseResult<DBNull>.Success("State not edited!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Failure("States not found!"));
            }

        }




        /// <summary>
        /// GetStatebyId
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("state/getbyid")]
        public async Task<IActionResult> GetByIdState(StateById state)
        {
            try
            {
                var result = await _stateRepository.GetById(state);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.State>.Success("State get successfully", result));
                }
                else
                {
                    _logger.LogInformation("State not found!");
                    return Ok(ResponseResult<DBNull>.Success("State not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("State not found!"));
            }
        }

        /// <summary>
        /// Delete State
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("state/delete")]
        public async Task<IActionResult> DeleteState(StateById state)
        {
            try
            {
                state.UserId = _userId;
                var result = await _stateRepository.Delete(state);
                if (result)
                {
                    _logger.LogInformation("State deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("State deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("State not deleted!");
                    return Ok(ResponseResult<DBNull>.Success("State not deleted!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Failure("State not Found!"));
            }

        }

        #endregion

        #region CityController
        /// <summary>
        /// Create City Master Data
        /// </summary>
        /// <param name="createCity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("city/create")]
        public async Task<IActionResult> CreateCity(CreateCity createCity)
        {
            try
            {


                createCity.UserId = _userId;
                var result = await _cityRepository.Create(createCity);

                if (result)
                {
                    _logger.LogInformation("Record created successfully!");
                    return Ok(ResponseResult<DBNull>.Success("City created successfully!"));
                }
                else
                {
                    _logger.LogCritical("City not created!");
                    return Ok(ResponseResult<DBNull>.Success("City not created!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "create", ex);
                return Ok(ResponseResult<DBNull>.Failure("City not created!"));
            }
        }


        /// <summary>
        /// GetAllCities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("city/getall")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<List<DM.City>>))]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var result = await _cityRepository.GetAllCities();

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<List<DM.City>>.Success("Cities get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Cities not found!");
                    return Ok(ResponseResult<DBNull>.Success("Cities not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getall", ex);
                return Ok(ResponseResult<DBNull>.Failure("Cities not found!"));
            }
        }


        /// <summary>
        /// GetallcitiesbyStateId
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("city/getallcitiesbystateid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseResult<List<DM.City>>))]
        public async Task<IActionResult> GetAllCitiesByState(Guid StateId)
        {
            try
            {
            
                var result = await _cityRepository.GetAllCitiesByStateId(StateId);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<List<DM.City>>.Success("Cities get successfully", result));
                }
                else
                {
                    _logger.LogInformation("Cities not found!");
                    return Ok(ResponseResult<DBNull>.Success("Cities not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getcitiesbystateid", ex);
                return Ok(ResponseResult<DBNull>.Failure("Cities not found!"));
            }
        }


        /// <summary>
        /// EditCity
        /// </summary>
        /// <param name="editCity"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("city/edit")]
        public async Task<IActionResult> EditCity(EditCity editCity)
        {
            try
            {       
                editCity.UserId = _userId;
                var result = await _cityRepository.Edit(editCity);
                if (result)
                {
                    _logger.LogInformation("Record edited successfully!");
                    return Ok(ResponseResult<DBNull>.Success("City edited successfully!"));
                }
                else
                {
                    _logger.LogCritical("City not edited!");
                    return Ok(ResponseResult<DBNull>.Success("City not edited!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "edit", ex);
                return Ok(ResponseResult<DBNull>.Failure("City not found!"));
            }

        }


        /// <summary>
        /// GetCity ByID
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("city/getbyid")]
        public async Task<IActionResult> GetByIdCity(CityById city)
        {
            try
            {
                var result = await _cityRepository.GetById(city);

                if (result != null)
                {
                    _logger.LogInformation("Record get successfully");
                    return Ok(ResponseResult<Res.City>.Success("City get successfully", result));
                }
                else
                {
                    _logger.LogInformation("City not found!");
                    return Ok(ResponseResult<DBNull>.Success("City not found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "getbyid", ex);
                return Ok(ResponseResult<DBNull>.Failure("City not found!"));
            }
        }


        //DeleteCity
        [HttpDelete]
        [Route("city/delete")]
        public async Task<IActionResult> DeleteCity(CityById city)
        {
            try
            {       
                city.UserId = _userId;
                var result = await _cityRepository.Delete(city);
                if (result)
                {
                    _logger.LogInformation("city deleted successfully!");
                    return Ok(ResponseResult<DBNull>.Success("city deleted successfully!"));
                }
                else
                {
                    _logger.LogCritical("city not deleted!");
                    return Ok(ResponseResult<DBNull>.Success("city not deleted!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.UtcNow, "delete", ex);
                return Ok(ResponseResult<DBNull>.Failure("City not Found!"));
            }

        }

        #endregion
    }

}