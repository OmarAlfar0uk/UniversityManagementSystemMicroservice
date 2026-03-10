namespace AcademicService.Contracts
{
    public interface IVideoHelper
    {
        Task<string> SaveVideoAsync(IFormFile videoFile, string subFolder);
        string GetVideoUrl(string relativePath);
        bool DeleteVideo(string relativePath);
        Task<int> GetVideoDurationInMinutesAsync(string filePath);
    }
}
