using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class EbookValidation:IEbookValidation
    {
        public EbookThumbnailValidator EbookThumbnailValidator { get; set; } = new();
        public EbookFileValidator EbookFileValidator { get; set; } = new();
        public CreateEbookValidator CreateEbookValidator { get; set; } = new();
        public EditEbookValidator EditEbookValidator { get; set; } = new();
        public DeleteEbookValidator DeleteEbookValidator { get; set; } = new();
        public GetByIdEbookValidator GetByIdEbookValidator { get; set; } = new();
        public GetAllEbookValidator GetAllEbookValidator { get; set; } = new();
        public GetAllAuthersValidator GetAllAuthersValidator { get; set; } = new();

    }
}
