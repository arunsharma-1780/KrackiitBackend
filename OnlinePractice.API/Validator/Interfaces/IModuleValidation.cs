namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IModuleValidation
    {
        public CreateModuleValidator CreateModuleValidator { get; set; }
        public EditModuleValidator EditModuleValidator { get; set; }
        public GetModuleValidator GetModuleValidator { get; set; }
    }
}
