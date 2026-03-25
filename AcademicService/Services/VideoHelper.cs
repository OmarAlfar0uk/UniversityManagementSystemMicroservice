using AcademicService.Contracts;
using TagLib;

namespace AcademicService.Services
{
    public class VideoHelper : IVideoHelper
    {
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".mp4", ".mov", ".avi", ".mkv", ".webm"
            };

        private const long MaxFileSizeBytes = 1024L * 1024 * 1024; // 1 GB

        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoHelper(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool DeleteVideo(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            relativePath = relativePath.Replace("\\", "/").TrimStart('/');
            var fullPath = Path.Combine(webRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public string GetVideoUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
                return relativePath;

            relativePath = relativePath.Replace("\\", "/").TrimStart('/');
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                return "/" + relativePath;

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUrl}/{relativePath}";
        }

        public async Task<string> SaveVideoAsync(IFormFile videoFile, string subFolder)
        {
            if (videoFile == null || videoFile.Length == 0)
                throw new ArgumentException("Video file is required.");

            var extension = Path.GetExtension(videoFile.FileName);
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"Video format '{extension}' is not allowed. Allowed formats: {string.Join(", ", AllowedExtensions)}");

            if (videoFile.Length > MaxFileSizeBytes)
                throw new ArgumentException("Video file exceeds the maximum allowed size of 1GB.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folderPath = Path.Combine(webRootPath, "Uploads", "Videos", subFolder);
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            return Path.Combine("Uploads", "Videos", subFolder, fileName).Replace("\\", "/");
        }

        public Task<int> GetVideoDurationInMinutesAsync(string filePath)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(webRootPath, filePath.Replace("\\", "/").TrimStart('/'));

            try
            {
                using var tagFile = TagLib.File.Create(fullPath);
                var durationMinutes = (int)Math.Ceiling(tagFile.Properties.Duration.TotalMinutes);
                return Task.FromResult(Math.Max(durationMinutes, 1));
            }
            catch
            {
                // If TagLib fails to read metadata, return 0
                return Task.FromResult(0);
            }
        }
    }
}
