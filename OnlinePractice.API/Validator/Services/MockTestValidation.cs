using Microsoft.Extensions.Diagnostics.HealthChecks;
using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class MockTestValidation : IMockTestValidation
    {
        public CreateMockTestSettingValidator CreateMockTestSettingValidator { get; set; } = new();
        public EditMockTestSettingValidator EditMockTestSettingValidator { get; set; } = new();
        public GetMockTestSettingByIdValidator GetMockTestSettingByIdValidator { get; set; } = new();
        public DeleteMockTestSettingValidator DeleteMockTestSettingValidator { get; set; } = new();
        public GetAllQuestionsValidator GetAllQuestionsValidator { get; set; } = new();
        public GetAllQuestionsNewValidator GetAllQuestionsNewValidator { get; set; } = new();
        public MocktTestLogoValidator MocktTestLogoValidator { get; set; } = new();
        public MockTestByIdValidator MockTestByIdValidator { get; set; } = new();
        public CreateMockTestQuestionsValidator CreateMockTestQuestionsValidator { get; set; } = new();
        public UpdateMockTestQuestionListValidator UpdateMockTestQuestionListValidator { get; set; } = new();
        public MockTestQuestionssValidator MockTestQuestionssValidator { get; set; } = new();
        public AutomaticMockTestQuestionValidator AutomaticMockTestQuestionValidator { get; set; } = new();
        public GetAutomaticMockTestQuestionValidator GetAutomaticMockTestQuestionValidator { get; set; } = new();
        public PublishAutoMaticMocktestQuestionValidator PublishAutoMaticMocktestQuestionValidator { get; set; } = new();

    }
}
