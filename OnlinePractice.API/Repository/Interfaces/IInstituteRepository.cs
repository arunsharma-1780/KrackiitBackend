using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IInstituteRepository
    {
        public Task<bool> Create(Req.CreateInstitute createInstitute);
        public Task<string> UploadImage(Req.LogoImage image);
        public Task<Res.InstituteList> GetAllInstitutes(Req.GetAllInstitute institute);
        public Task<Res.InstituteListV1> GetAllInstitutesV1();
        public Task<bool> Edit(Req.EditInstitute editInstitute);
        public Task<Res.Institute?> GetById(Req.InstituteById institute);
        public Task<bool> Delete(Req.InstituteById institute);
        public Task<bool> IsDuplicate(Req.CodeCheck codeCheck);
        public Task<bool> IsEditDuplicate(Req.EditCodeCheck codeCheck);
        public Task<bool> IsInstituteExists(Req.InstituteById institute);
        
    }
}
