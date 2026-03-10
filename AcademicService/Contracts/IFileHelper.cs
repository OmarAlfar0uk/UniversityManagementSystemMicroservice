namespace AcademicService.Contracts
{
    public interface IFileHelper
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder);
        string GetFileUrl(string relativePath);
        bool DeleteFile(string relativePath);
    }
}
