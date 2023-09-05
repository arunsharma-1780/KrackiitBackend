using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class AdminResultValidation : IAdminResultValidation
    {
        public GeAdminResultValidator GeAdminResultValidator { get; set; } = new();
        public GeResultByMockTestIdValidator GeResultByMockTestIdValidator { get; set; } = new();
        public GeResultAnalysisDetailValidator GeResultAnalysisDetailValidator { get; set; } = new();
        public GetMockTestListValidator GetMockTestListValidator { get; set; } = new();
    }
}
