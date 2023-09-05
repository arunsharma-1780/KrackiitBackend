namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IAccountValidation
    {
        public ProfileImageValidator ProfileImageValidator { get; set; }
        public RegisterModelValidator RegisterModelValidator { get; set; }
        public LoginValidator LoginValidator { get; set; }
        public UpdateAdminValidator UpdateAdminValidator { get; set; }
        public ChangePasswordValidator ChangePasswordValidator { get; set; }
        public ForgetPasswordValidator ForgetPasswordValidator { get; set; }
        public GetUserByIdValidator GetUserByIdValidator { get; set; }
        public RemoveProfileValidator RemoveProfileValidator { get; set; }
    }
}
