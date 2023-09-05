using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IEbookRepository
    {
        public Task<string> UploadImage(Req.EbookThumbnailimage ebookThumbnail);
        public Task<string> UploadEbookPdfUrl(Req.EbookPdfUrl ebookPdfUrl);
        public Task<Res.AuthorAndLanguage2?> Create(Req.CreateEbook createEbook );
        public Task<Res.AuthorAndLanguage2?> Edit(Req.EditEbook editEbook);
        public Task<bool> Delete(Req.DeleteEbook deleteEbook );
        public Task<Res.Ebook?> GetEbookById(Req.EbookById ebook);
        public Task<Res.EbookListV1?> GetAll(Req.GetAllEbookV1 ebook);
        public Task<Res.EbookListV2?> Showallids();
        public Task<Res.AutherList?> GetAllAuthors(Req.GetAllAuthors ebook);
        public Task<Res.EbookListV1?> GetAll50();
        public Task<Res.EbookListV1?> GetAllV2(Req.GetAllEbookV1 ebook);

    }
}
