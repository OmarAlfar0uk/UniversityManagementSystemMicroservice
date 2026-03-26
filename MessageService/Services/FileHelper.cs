using MessageService.Contracts;

namespace MessageService.Services;

public class FileHelper(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileHelper
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".doc", ".docx", ".txt" };

    private const long MaxFileSizeBytes = 50L * 1024 * 1024; // 50 MB

    public bool DeleteFile(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return false;

        var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        relativePath = relativePath.Replace("\\", "/").TrimStart('/');
        var fullPath = Path.Combine(webRootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }
        return false;
    }

    public string GetFileUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute)) return relativePath;

        relativePath = relativePath.Replace("\\", "/").TrimStart('/');
        var request = httpContextAccessor.HttpContext?.Request;
        if (request == null) return "/" + relativePath;

        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        return $"{baseUrl}/{relativePath}";
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required.");

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"File format '{extension}' is not allowed.");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException("File exceeds maximum allowed size of 50MB.");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var folderPath = Path.Combine(webRootPath, "Uploads", "Files", subFolder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Path.Combine("Uploads", "Files", subFolder, fileName).Replace("\\", "/");
    }
}
