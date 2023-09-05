using OnlinePractice.API.Models.Request;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;


namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IStudyMaterialRepository
    {
        public Task<bool> Create(CreateStudyMaterial studyMaterial);
        public Task<bool> Edit(EditStudyMaterial studyMaterial);
        public Task<Models.Response.StudyMaterial?> GetById(MaterialById material);
        public Task<bool> Delete(MaterialById material);
        public Task<bool> EditPrice(EditPriceStudyMaterial studyMaterial);

    }
}
