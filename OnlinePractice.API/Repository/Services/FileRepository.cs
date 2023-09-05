using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Repository.Interfaces;
using System.Data;

namespace OnlinePractice.API.Repository.Services
{
    public class FileRepository : IFileRepository
    {
        //private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _baseUrl;


        public FileRepository(IConfiguration configuration, IHttpContextAccessor baseUrl)
        {
            //  _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _baseUrl = baseUrl;
        }
        public async Task<string> SaveImage(IFormFile? file, string directoryName)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string partialPath = directoryPath + "/" + fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return partialPath;
        }
        public async Task<string> SavePdfUrl(IFormFile? file, string directoryName)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string partialPath = directoryPath + "/" + fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return partialPath;
        }
        public async Task<string> SaveVideoUrl(IFormFile? file, string directoryName)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string partialPath = directoryPath + "/" + fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return partialPath;
        }
        public async Task<string> SaveExcel(IFormFile? file, string directoryName)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string partialPath = directoryPath + "/" + fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return partialPath;
        }

        public async Task<string> UploadExcel(IFormFile? file)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + "BulkUploadSample";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = "StudentBulkUpload" + Path.GetExtension(file.FileName);
            string partialPath = directoryPath + "/" + fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return partialPath;
        }
        public async Task<bool> RemoveSampleExcel()
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + "BulkUploadSample";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
            // Get all files in the directory
            string[] files = Directory.GetFiles(filePath);
            // Delete each file
            foreach (string file in files)
            {
                File.Delete(file);
            }
            return true;
        }
        public async Task<bool> RemoveStudentExcel()
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + "StudentExcel";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
            // Get all files in the directory
            string[] files = Directory.GetFiles(filePath);
            // Delete each file
            foreach (string file in files)
            {
                File.Delete(file);
            }
            return true;
        }
        
        public string GetSampleExcelUrl()
        {
            var request = _baseUrl?.HttpContext?.Request;
            var domain = $"{request?.Scheme}://{request?.Host}/";
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + "BulkUploadSample";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string partialPath = directoryPath + "/" + "StudentBulkUpload.xlsx";
            return domain + partialPath;
        }
        public bool RemoveExcel(string partialpath, string directoryName)
        {
            string basePath = _configuration.GetValue<string>("AppSetting:FilePath");
            string directoryPath = basePath + directoryName;
            if (Directory.Exists(directoryPath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialpath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return true;
        }
    }

}
