namespace Mentorea.Services
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile ImageFile, string SubFolder);
        void DeleteImage(string ImageName, string SubFolder);
        Task<string> SaveFileAsync(IFormFile file);
        bool IsExist(string fileName);
        string GetBaseUrl(IHttpContextAccessor httpContextAccessor);
    }
}
