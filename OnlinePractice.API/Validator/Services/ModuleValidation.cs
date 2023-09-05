using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class ModuleValidation : IModuleValidation
    {
        public CreateModuleValidator CreateModuleValidator { get; set; } = new();
        public EditModuleValidator EditModuleValidator { get; set; } = new();
        public GetModuleValidator GetModuleValidator { get; set; } = new();
    }
}
