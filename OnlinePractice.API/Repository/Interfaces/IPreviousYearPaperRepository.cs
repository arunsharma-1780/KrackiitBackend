using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IPreviousYearPaperRepository
    {
        public Task<string> UploadPaperPdfUrl(Req.PaperPdfUrl pdfUrl);
        public Task<bool> Create(Req.CreatePreviousYearPaper previousYearPaper);
        public Task<bool> Edit(Req.EditPreviousYearPaper previousYearPaper);
        public  Task<bool> Delete(Req.GetPaperPdf paperPdf);
        public Task<Res.PreviousYearPaper?> GetPaperById(Req.GetPaperPdf paperPdf);
        public Task<Res.PreviousYearPaperList?> GetAllPapers(Req.GetAllPapers papers);
        public Task<Res.PreviousYearPaperList?> GetAll50();
        public Task<Res.FacultyList?> GetAllFaculties(Req.GetAllFacultyList faculty);
        public Task<Res.VideoListV2?> Showallids();
        public Res.YearList GetYearList();
    }
}
