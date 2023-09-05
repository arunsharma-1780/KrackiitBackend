namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IStaffValidation
    {
        public CreateStaffValidator CreateStaffValidator { get; set; }
        public GetAllStaffValidator GetAllStaffValidator { get; set; }
        public GetByIdStaffValidator GetByIdStaffValidator { get; set; }
        public EditStaffValidator EditStaffValidator { get; set; }
    }
}
