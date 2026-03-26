using MessageService.Contracts;

namespace MessageService.Services;

public class ImageHelper(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IImageHelper
{
    public bool DeleteImage(string relativePath)
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

    public string? GetImageUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute)) return relativePath;

        relativePath = relativePath.Replace("\\", "/").TrimStart('/');
        var request = httpContextAccessor.HttpContext?.Request;
        if (request == null) return "/" + relativePath;

        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        return $"{baseUrl}/{relativePath}";
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile, string subFolder)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("Image file is required.");

        var extension = Path.GetExtension(imageFile.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var folderPath = Path.Combine(webRootPath, "Uploads", "Images", subFolder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return Path.Combine("Uploads", "Images", subFolder, fileName).Replace("\\", "/");
    }
}
