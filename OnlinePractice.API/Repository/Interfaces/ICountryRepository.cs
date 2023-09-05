using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ICountryRepository 
    {
        public Task<bool> Create(CreateCountries createCountries);
        public Task<List<DM.Countries>> GetAllCountries();
        public Task<bool> Edit(EditCountries countries);
        public Task<Res.Countries?> GetById(CountryById countryById);
        public Task<bool> Delete(CountryById country);

    }
}
