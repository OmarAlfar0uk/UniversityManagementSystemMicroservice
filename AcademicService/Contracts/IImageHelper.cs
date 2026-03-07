using Microsoft.AspNetCore.Http;

namespace AcademicService.Contracts
{
    public interface IImageHelper
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string subFolder);
        string? GetImageUrl(string relativePath);
        bool DeleteImage(string relativePath);
    }
}
