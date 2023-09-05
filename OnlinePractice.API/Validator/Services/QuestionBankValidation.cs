using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class QuestionBankValidation : IQuestionBankValidation

    {
        public CreateQuestionBankValidator CreateQuestionBankValidator { get; set; }= new ();
        public EditQuestionBankValidator EditQuestionBankValidator { get; set; } = new();
        public GetByReferenceIdQuestionBankValidator GetByReferenceIdQuestionBankValidator { get; set; } = new();
        public DeleteQuestionBankValidator DeleteQuestionBankValidator { get; set; } = new();
        public GetAllQuestionBankValidator GetAllQuestionBankValidator { get; set; } = new();
    }
}
