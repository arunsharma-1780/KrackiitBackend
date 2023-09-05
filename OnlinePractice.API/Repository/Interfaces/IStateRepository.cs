using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;


namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IStateRepository 
    {
        public Task<bool> Create(CreateState createState);
        public Task<List<DM.State>> GetAllStates();

        public Task<List<DM.State>> GetAllStatesByCountryId(Guid countryId);
        public Task<bool> Edit(EditState editState);
        public Task<Models.Response.State?> GetById(StateById state);
        public Task<bool> Delete(StateById state);

    }
}
