using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class PreviousYearPaperValidation:IPreviousYearPaperValidation
    {
        public PaperFileValidator PaperFileValidator { get; set; } = new();
        public CreatePaperValidator CreatePaperValidator { get; set; } = new();
        public EditPaperValidator EditPaperValidator { get; set; } = new();
        public DeletePaperValidator DeletePaperValidator { get; set; } = new();
        public GetByIdPaperValidator GetByIdPaperValidator { get; set; } = new();
        public GetAllPapersValidator GetAllPapersValidator { get; set; } = new();
        public GetAllFacultiesValidator GetAllFacultiesValidator { get; set; } = new();

    }
}
