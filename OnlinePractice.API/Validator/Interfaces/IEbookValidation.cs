namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IEbookValidation
    {
        public EbookThumbnailValidator EbookThumbnailValidator { get; set; }
        public EbookFileValidator EbookFileValidator { get; set; }
        public CreateEbookValidator CreateEbookValidator { get; set; }
        public EditEbookValidator EditEbookValidator { get; set; }
        public DeleteEbookValidator DeleteEbookValidator { get; set; }
        public GetByIdEbookValidator GetByIdEbookValidator { get; set; }
        public GetAllEbookValidator GetAllEbookValidator { get; set; }
        public GetAllAuthersValidator GetAllAuthersValidator { get; set; }

    }
}
