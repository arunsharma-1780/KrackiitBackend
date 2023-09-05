using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;


namespace OnlinePractice.API.Repository.Services
{
    public class CountryRepository : ICountryRepository
    {   
        private readonly IUnitOfWork _unitOfWork;
        public CountryRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create country
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        public  async Task<bool> Create(CreateCountries createCountries)
        {
            DM.Countries countries = new()
            {
                CountryName = createCountries.CountryName,
                AliasName = createCountries.AliasName,
                CountryCode = createCountries.CountryCode,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createCountries.UserId,
            };
            int result = await _unitOfWork.Repository<DM.Countries>().Insert(countries);
            return result > 0;
        }


        /// <summary>
        /// GetAllCountries
        /// </summary>
        /// <returns></returns>
        public async Task<List<DM.Countries>> GetAllCountries()
        {
            return await _unitOfWork.Repository<DM.Countries>().Get();
        }


        /// <summary>
        /// EditCountry Method
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        public async Task<bool> Edit(EditCountries countries)
        {
            var country = await _unitOfWork.Repository<DM.Countries>().GetSingle(x => x.Id == countries.id && !x.IsDeleted);
            if (country != null)
            {
                country.CountryName = countries.CountryName;
                country.AliasName = countries.AliasName;
                country.CountryCode = countries.CountryCode;
                country.LastModifierUserId = countries.UserId;
                country.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Countries>().Update(country);
                return result > 0;
            }
            return false;

        }


        /// <summary>
        /// GetCountry By Id
        /// </summary>
        /// <param name="countryById"></param>
        /// <returns></returns>
        public async Task<Res.Countries?> GetById(CountryById countryById)
        {
            var countryData = await _unitOfWork.Repository<DM.Countries>().GetSingle(x => x.Id == countryById.Id && !x.IsDeleted);
            if (countryData != null)
            {
                Res.Countries result = new()
                {
                    Id = countryData.Id,
                    CountryName = countryData.CountryName,
                    AliasName = countryData.AliasName,
                    CountryCode = countryData.CountryCode,
                };
                return result;
            }
            return null;
        }


        /// <summary>
        /// DeleteCountry
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<bool> Delete(CountryById country)
        {
            var countryData = await _unitOfWork.Repository<DM.Countries>().GetSingle(x => x.Id == country.Id && !x.IsDeleted);
            if (countryData != null)
            {
                countryData.IsDeleted = true;
                countryData.DeleterUserId = country.UserId;
                countryData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.Countries>().Update(countryData);
                return result > 0;
            }
            return false;
        }

    }
}
