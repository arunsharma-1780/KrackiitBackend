using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using DM = OnlinePractice.API.Models.DBModel;


namespace OnlinePractice.API.Repository.Services
{
    public class StateRepository : IStateRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public StateRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create State
        /// </summary>
        /// <param name="createState"></param>
        /// <returns></returns>
        public async Task<bool> Create(CreateState createState)
        {
            DM.State state= new()
            { 
                StateName= createState.StateName,
                AliasName = createState.AliasName,
                CountryId = createState.CountryId,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = createState.UserId,
            };
            int result = await _unitOfWork.Repository<DM.State>().Insert(state);
            return result > 0;
        }


        /// <summary>
        /// GetAll States
        /// </summary>
        /// <returns></returns>
        public async Task<List<DM.State>> GetAllStates()
        {
            return await _unitOfWork.Repository<DM.State>().Get();

        }


       /// <summary>
       /// GetAllStatesByCountryID
       /// </summary>
       /// <param name="countryId"></param>
       /// <returns></returns>
        public async Task<List<DM.State>> GetAllStatesByCountryId(Guid countryId)
        {
            var StateList = await _unitOfWork.Repository<DM.State>().Get(x => x.CountryId == countryId && !x.IsDeleted);
            List<DM.State> states = StateList
                      .Select(o => new DM.State
                      {
                          Id = o.Id,
                          StateName = o.StateName,
                          AliasName = o.AliasName
                      }).ToList();
            return states;
        }


        /// <summary>
        /// Edit State
        /// </summary>
        /// <param name="editState"></param>
        /// <returns></returns>
        public async Task<bool> Edit(EditState editState)
        {
            var stateData = await _unitOfWork.Repository<DM.State>().GetSingle(x => x.Id == editState.Id && !x.IsDeleted);
            if (stateData != null)
            {
                stateData.StateName=editState.StateName;
                stateData.AliasName=editState.AliasName;
                stateData.CountryId=editState.CountryId;
                stateData.LastModifierUserId = editState.UserId;
                stateData.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.State>().Update(stateData);
                return result > 0;
            }
            return false;

        }



        /// <summary>
        /// GetState By Id Method
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<Models.Response.State?> GetById(StateById state)
        {
            var stateData = await _unitOfWork.Repository<DM.State>().GetSingle(x => x.Id == state.Id && !x.IsDeleted);
            if (stateData != null)
            {
                Models.Response.State result = new()
                {
                   Id = stateData.Id,
                   StateName= stateData.StateName,
                   AliasName=stateData.AliasName,
                   CountryId=stateData.CountryId,
                };
                return result;
            }
            return null;
        }


        /// <summary>
        /// Delete State
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> Delete(StateById state)
        {
            var Statedata = await _unitOfWork.Repository<DM.State>().GetSingle(x => x.Id == state.Id && !x.IsDeleted);
            if (Statedata != null)
            {
                Statedata.IsDeleted = true;
                Statedata.DeleterUserId = state.UserId;
                Statedata.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.State>().Update(Statedata);
                return result > 0;
            }
            return false;
        }


    }
}
