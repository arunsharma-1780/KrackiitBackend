namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IAdminResultValidation
    {
        public GeAdminResultValidator GeAdminResultValidator { get; set; }
        public GeResultByMockTestIdValidator GeResultByMockTestIdValidator { get; set; }
        public GeResultAnalysisDetailValidator GeResultAnalysisDetailValidator { get; set; }
        public GetMockTestListValidator GetMockTestListValidator { get; set; }
    }
}
