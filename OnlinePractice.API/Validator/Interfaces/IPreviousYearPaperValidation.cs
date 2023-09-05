namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IPreviousYearPaperValidation
    {
        public PaperFileValidator PaperFileValidator { get; set; }
        public CreatePaperValidator CreatePaperValidator { get; set; }
        public EditPaperValidator EditPaperValidator { get; set; }
        public DeletePaperValidator DeletePaperValidator { get; set; }
        public GetByIdPaperValidator GetByIdPaperValidator { get; set; }
        public GetAllPapersValidator GetAllPapersValidator { get; set; }
        public GetAllFacultiesValidator GetAllFacultiesValidator { get; set; }
    }
}
