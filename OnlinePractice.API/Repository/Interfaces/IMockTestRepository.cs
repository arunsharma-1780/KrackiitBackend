using OnlinePractice.API.Models.Common;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Com = OnlinePractice.API.Models.Common;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IMockTestRepository 
    {
        #region MockTestSetting
        public Task<string> UploadImage(Req.LogoImage image);
        public Task<Res.MockTestInfo?> Create(Req.CreateMockTestSetting mockTestSetting);
        public Task<bool> Edit(Req.EditMockTestSetting mockTestSetting);
        public Task<Res.MockTestSetting?> GetMockTestSettingById(Req.MocktestSettingById mocktestSetting);
        public Task<Res.MockTestSettingList> GetAllMockTestSettings(Req.GetAllMockTest allMockTest);
        public Task<bool> Delete(Req.MocktestSettingById mocktestSettingById);
        public Task<bool> CheckInstitute(Req.CheckInstitute institute);
        public Task<bool> CheckMockTestById(Req.CheckMockTestById checkMockTest);
        public Task<Res.MockTestQuestionsList?> GetQuestionsByFilter(Req.GetAllQuestions question);
        public Task<Res.MockTestSettingListV1?> GetAllMockTestSettingsV1(Req.GetAllMockTestV1 allMockTest);
        public Task<bool> IsDuplicate(Req.MockTestNameCheck mockTestName);
        public Task<bool> IsEditDuplicate(Req.EditMockTestNameCheck mockTestName);
        public bool CheckLanguage(string language);
        public Task<bool> CheckPublishMockTest(Req.MockTestById mockTest);
        public Task<bool> CheckPatternId(Req.GetExamPatternId examPatternId);
        public Task<bool> CheckCurrentMocktest(Req.MocktestSettingById mocktest);
        public Task<Res.UserDetailsById?> GetMocktestUserDetails(Req.GetUserEmail model);
        #endregion

        #region MockTestQuestion
        public Task<bool> CreateMockTestQuestions(Req.CreateMockTestQuestionList mockTestQuestions);
        public Task<bool> UpdateMockTestQuestions(Req.UpdateMockTestQuestionList mockTestQuestions);
        public Task<bool> PublishMockTest(Req.MockTestById mockTest);
        public Task<bool> RemoveMockTestQuestions(Req.MocktestSettingById mockTest);
        public Task<Res.MockTestQuestionListPdf?> GetMocktestQuestionPdf(Req.MockTestById mockTest);
        #endregion

        #region AutomaticMockTestQuestion
        public Task<Res.AutomaticMockTest?> GetAllAutomaticMockTestSettings(Req.AutomaticMockTestQuestion mockTestQuestion);
        public Task<Res.MockTestQuestionList?> GetMocktestQuestionById(Req.MockTestById mockTest);
        public Task<Res.AutoMockTestQuestionList?> GetAutomaticMockTestQuestions(Req.AutomaticMockTestQuestion automaticMockTestQuestion);
        public Task<bool> SaveAutoMaticMocktest(Res.AutoMockTestQuestionList autoMockTestQuestion);
        #endregion


    }
}
