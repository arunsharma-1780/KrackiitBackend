namespace OnlinePractice.API.Repository.Interfaces
{
    public interface IFileRepository
    {
        public Task<string> SaveImage(IFormFile? file, string directoryName);
        public Task<string> SavePdfUrl(IFormFile? file, string directoryName);
        public Task<string> SaveVideoUrl(IFormFile? file, string directoryName);
        public Task<string> SaveExcel(IFormFile? file, string directoryName);
        public bool RemoveExcel(string partialPath, string directoryName);
        public  string GetSampleExcelUrl();
        public Task<string> UploadExcel(IFormFile? file);
        public Task<bool> RemoveSampleExcel();
        public Task<bool> RemoveStudentExcel();
    }
}
