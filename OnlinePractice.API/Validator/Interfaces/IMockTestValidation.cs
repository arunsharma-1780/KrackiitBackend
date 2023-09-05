namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IMockTestValidation
    {
        public CreateMockTestSettingValidator CreateMockTestSettingValidator { get; set; }
        public EditMockTestSettingValidator EditMockTestSettingValidator { get; set; }
        public GetMockTestSettingByIdValidator GetMockTestSettingByIdValidator { get; set; }
        public DeleteMockTestSettingValidator DeleteMockTestSettingValidator { get; set; }
        public GetAllQuestionsValidator GetAllQuestionsValidator { get; set; }
        public GetAllQuestionsNewValidator GetAllQuestionsNewValidator { get; set; }
        public MocktTestLogoValidator MocktTestLogoValidator { get; set; }
        public MockTestByIdValidator MockTestByIdValidator { get; set; }
        public CreateMockTestQuestionsValidator CreateMockTestQuestionsValidator { get; set; }
        public UpdateMockTestQuestionListValidator UpdateMockTestQuestionListValidator { get; set; }
        public MockTestQuestionssValidator MockTestQuestionssValidator { get; set; }
        public AutomaticMockTestQuestionValidator AutomaticMockTestQuestionValidator { get; set; }
        public GetAutomaticMockTestQuestionValidator GetAutomaticMockTestQuestionValidator { get; set; }
        public PublishAutoMaticMocktestQuestionValidator PublishAutoMaticMocktestQuestionValidator { get; set; } 
    }
}
