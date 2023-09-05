using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IQuestionBankValidation
    {
        public CreateQuestionBankValidator  CreateQuestionBankValidator{ get; set; }
        public EditQuestionBankValidator EditQuestionBankValidator { get; set; }
        public GetByReferenceIdQuestionBankValidator GetByReferenceIdQuestionBankValidator { get; set; }
        public DeleteQuestionBankValidator DeleteQuestionBankValidator { get; set; }
        public GetAllQuestionBankValidator GetAllQuestionBankValidator { get; set; }
    }
}
