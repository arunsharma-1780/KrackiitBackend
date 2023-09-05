using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class StaffValidation : IStaffValidation
    {
        public CreateStaffValidator CreateStaffValidator { get; set; } = new();
        public GetAllStaffValidator GetAllStaffValidator { get; set; } = new();
        public GetByIdStaffValidator GetByIdStaffValidator { get; set; } = new();
        public EditStaffValidator EditStaffValidator { get; set; } = new();
    }
}
