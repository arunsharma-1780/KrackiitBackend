using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Services
{
    public class CityRepository : ICityRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CityRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        /// 
        public async Task<bool> Create(CreateCity createCity)
        {
            DM.City city= new()
            {
                CityName = createCity.CityName,
                AliasName = createCity.AliasName,
                StateId = createCity.StateId,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createCity.UserId,
            };
            int result = await _unitOfWork.Repository<DM.City>().Insert(city);
            return result > 0;
        }


        /// <summary>
        /// GetAll Cities
        /// </summary>
        /// <returns></returns>
        public async Task<List<DM.City>> GetAllCities()
        {
            return await _unitOfWork.Repository<DM.City>().Get();
        }


        /// <summary>
        /// GetAllCities By StateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public async Task<List<DM.City>> GetAllCitiesByStateId(Guid stateId)
        {
            var cityList = await _unitOfWork.Repository<DM.City>().Get(x => x.StateId == stateId && !x.IsDeleted);
            List<DM.City> cities = new();
            cities = cityList
                      .Select(o => new DM.City
                      {
                          Id = o.Id,
                          CityName = o.CityName,
                          AliasName = o.AliasName
                      }).ToList();
            return cities;
        }


        /// <summary>
        /// EditCity 
        /// </summary>
        /// <param name="editCity"></param>
        /// <returns></returns>
        public async Task<bool> Edit(EditCity editCity)
        {
            var city = await _unitOfWork.Repository<DM.City>().GetSingle(x => x.Id == editCity.Id && !x.IsDeleted);
            if (city != null)
            {
                city.CityName = editCity.CityName; 
                city.AliasName = editCity.AliasName;
                city.StateId = editCity.StateId;
                city.LastModifierUserId = editCity.UserId;
                city.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.City>().Update(city);
                return result > 0;
            }
            return false;

        }


        /// <summary>
        /// GetCityData By CityId
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<Models.Response.City?> GetById(CityById city)
        {
            var cityData = await _unitOfWork.Repository<DM.City>().GetSingle(x => x.Id == city.Id && !x.IsDeleted);
            if (cityData != null)
            {
                Models.Response.City result = new()
                {
                    Id= cityData.Id,
                    StateId = cityData.StateId,
                    AliasName = cityData.AliasName,
                    CityName = cityData.CityName,
                    
                };
                return result;
            }
            return null;
        }


        /// <summary>
        /// DeleteCity by CityId Method
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<bool> Delete(CityById city)
        {
            var cityData = await _unitOfWork.Repository<DM.City>().GetSingle(x => x.Id == city.Id && !x.IsDeleted);
            if (cityData != null)
            {
                cityData.IsDeleted = true;
                cityData.DeleterUserId = city.UserId;
                cityData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.City>().Update(cityData);
                return result > 0;
            }
            return false;
        }

    }
}
