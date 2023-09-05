using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Base;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface ICityRepository 
    {
        public Task<bool> Create(CreateCity createCity);
        public Task<List<DM.City>> GetAllCities();
        public Task<List<DM.City>> GetAllCitiesByStateId(Guid stateId);
        public Task<bool> Edit(EditCity editCity);
        public Task<Models.Response.City?> GetById( CityById city);
        public Task<bool> Delete(CityById city);

    }
}
