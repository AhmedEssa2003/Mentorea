using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Mentorea.Services
{
    public class FileService : IFileService
    {
        private readonly string _Path;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _Path = $"{_webHostEnvironment.ContentRootPath}\\wwwroot";
        }

        public async Task<string> SaveImageAsync(IFormFile ImageFile, string SubFolder)
        {
            var path = Path.Combine(_Path, "Images", SubFolder);
            var extension = Path.GetExtension(ImageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(path, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await ImageFile.CopyToAsync(stream);
            return fileName;
        }

        public void DeleteImage(string ImageName, string SubFolder)
        {
            var path = Path.Combine(_Path, "Images", SubFolder, ImageName);
            if (File.Exists(path))
                File.Delete(path);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_Path, "Chat", fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public bool IsExist(string fileName)
        {
            var filePath = Path.Combine(_Path, "Chat", fileName);
            var response = Path.Exists(filePath);
            return response;
        }
        public  string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}/";
        }
            
        
            
        
    }
        
    
}
