using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class AdminDashboardValidation : IAdminDashboardValidation
    {
        public FilterModelValidator FilterModelValidator { get; set; } = new();
    }
}
