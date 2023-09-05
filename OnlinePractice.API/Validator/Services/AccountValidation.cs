using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class AccountValidation : IAccountValidation
    { 
        public ProfileImageValidator ProfileImageValidator { get; set; } = new();
        public RegisterModelValidator RegisterModelValidator { get; set; } = new();
        public LoginValidator LoginValidator { get; set; } = new();
        public UpdateAdminValidator UpdateAdminValidator { get; set; } = new();
        public ChangePasswordValidator ChangePasswordValidator { get; set; } = new();
        public ForgetPasswordValidator ForgetPasswordValidator { get; set; } = new();
        public GetUserByIdValidator GetUserByIdValidator { get; set; } = new();
        public RemoveProfileValidator RemoveProfileValidator { get; set; } = new();
    }
}
